# Azure IoT Samples
Azure IoT Samples are samples created by our engineers to help you get started with Azure IoT Certificate Authentication. To get started, we recommend reading our [IoT security best practices whitepaper](https://docs.keytos.io/azure-pki/azure-iot-hub/security-best-practices/). This Repository contains two different projects to help you get started.

## Simulate IoT Hub Device
This project guides you through how to authenticate and connect IoT devices with Azure IoT hub using X509 Certificates. This is great to manually connect a handful devices to Azure to do some testing or a proof of concept. To get started or get more information read [our documentation for connecting Azure IoT Devices to the cloud](https://docs.keytos.io/azure-pki/azure-iot-hub/certificate-based-authentication/).

## Simulate Device Provisioning
While the "Simulate IoT Hub Device" project is great for getting you started and having a few devices up and running, it does not scale to create thousands or millions of devices. This project takes it to the next step by also simulating the device provisioning device that you would be running in the factory to provision the devices and connect them to Azure (learn more about how this is done in our [how to create an IoT Identity best practices whitepaper](https://docs.keytos.io/azure-pki/azure-iot-hub/security-best-practices/#creating-the-identity)). To get started on this project and for a detailed explanation of the code, read [our documentation for automatically provisioning devices in the cloud](https://docs.keytos.io/azure-pki/azure-iot-hub/how-to-create-azure-device-provisioning-service/)

## EZCA Shared Library
The projects in this repository use X509 Certificates to authenticate the IoT devices in Azure. To automatically issue certificates for the IoT devices we used [EZCA](https://www.keytos.io/azure-pki.html). EZCA Library contains the code to register new domains, and create new certificates in EZCA. To automatically rotate certificates using your existing certificates, please see the code in our [Certificate Renewal Client Repository](https://github.com/markeytos/Certificate-Renewal-Client)