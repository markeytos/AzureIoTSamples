using Azure.Core;
using Azure.Identity;
using SimulateIoTHubDevice.Managers;
using SimulateIoTHubDevice.Models;
using SimulateIoTHubDevice.Services;

HttpService httpService = new (new HttpClient());
EZCAManager ezMananger = new(httpService);

// Get Available CAs 
Console.WriteLine("Getting Available CAs..");
AvailableCAModel[]? availableCAs = await ezMananger.GetAvailableCAsAsync();
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
bool success = await ezMananger.RegisterDomainAsync(selectedCA, deviceID);
if(!success)
{
    Console.WriteLine("Could not register new device in EZCA");
    return;
}

// get cert from EZCA 
Console.WriteLine("Getting Device Certificate..");
var test = await ezMananger.RequestCertificateAsync(selectedCA, deviceID);



// register device in Azure
Console.WriteLine("Registering Device In Azure..");
