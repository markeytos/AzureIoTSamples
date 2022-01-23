using Microsoft.Azure.Devices.Client;
using EZCASharedLibrary.Managers;
using EZCASharedLibrary.Models;
using EZCASharedLibrary.Services;
using System.Security.Cryptography.X509Certificates;
using System.Text;

//Variables Change this to match your IoT Hub
string _iotHubEndpoint = "ezcaiothubtest.azure-devices.net";


HttpService httpService = new (new HttpClient());
EZCAManager ezManager = new(httpService);

// Get Available CAs 
Console.WriteLine("Getting Available CAs..");
AvailableCAModel[]? availableCAs = await ezManager.GetAvailableCAsAsync();
if(availableCAs == null || availableCAs.Any() == false)
{
    Console.WriteLine("Could not find any available CAs in EZCA");
    return;
}
AvailableCAModel selectedCA = InputService.SelectCA(availableCAs);


// Register New Domain
//Generate Random Guid to simulate new Device ID
Console.WriteLine("Registering Device in EZCA..");
string deviceID = Guid.NewGuid().ToString();
Console.WriteLine($"Please register your device in Azure. Device ID: {deviceID}");
Console.WriteLine("Press Enter to continue..");
Console.ReadLine();
bool success = await ezManager.RegisterDomainAsync(selectedCA, deviceID);
if (!success)
{
   Console.WriteLine("Could not register new device in EZCA");
   return;
}

// get cert from EZCA 
Console.WriteLine("Getting Device Certificate..");
X509Certificate2? deviceCertificate = await ezManager.RequestCertificateAsync(selectedCA, deviceID);
if (deviceCertificate == null)
{
    Console.WriteLine("Could not create device certificate");
    return;
}


// Send device information to Azure
Console.WriteLine("Authenticating In Azure..");
// Create an authentication object using your X.509 certificate. 
DeviceAuthenticationWithX509Certificate auth = new (deviceID, deviceCertificate);
var deviceClient = DeviceClient.Create(_iotHubEndpoint, auth, TransportType.Mqtt);

if (deviceClient == null)
{
    Console.WriteLine("Failed to create DeviceClient!");
}
else
{
    Console.WriteLine("Successfully created DeviceClient!");
    await SendEventAsync(deviceClient, deviceID);
}
return;


static async Task SendEventAsync(DeviceClient deviceClient, string deviceId)
{
    //ref https://docs.microsoft.com/en-us/azure/iot-hub/tutorial-x509-test-certificate
    string dataBuffer;
    int MESSAGE_COUNT = 5;
    Random rnd = new Random();
    float temperature;
    float humidity;
    int TEMPERATURE_THRESHOLD = 30;
    Console.WriteLine("Device sending {0} messages to IoTHub...\n", MESSAGE_COUNT);
    // Iterate MESSAGE_COUNT times to set random temperature and humidity values.
    for (int count = 0; count < MESSAGE_COUNT; count++)
    {
        // Set random values for temperature and humidity.
        temperature = rnd.Next(20, 35);
        humidity = rnd.Next(60, 80);
        dataBuffer = string.Format("{{\"deviceId\":\"{0}\",\"messageId\":{1},\"temperature\":{2},\"humidity\":{3}}}",
            deviceId, count, temperature, humidity);
        Message eventMessage = new Message(Encoding.UTF8.GetBytes(dataBuffer));
        eventMessage.Properties.Add("temperatureAlert",
            (temperature > TEMPERATURE_THRESHOLD) ? "true" : "false");
        Console.WriteLine("\t{0}> Sending message: {1}, Data: [{2}]",
            DateTime.Now.ToLocalTime(), count, dataBuffer);

        // Send to IoT Hub.
        await deviceClient.SendEventAsync(eventMessage);
    }
}