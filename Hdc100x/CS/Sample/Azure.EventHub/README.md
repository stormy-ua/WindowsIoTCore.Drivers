### Description

This sample shows how HDC100x driver can be used with Raspberri Pi 2 in order to continuosly post measurement data to [Azure EventHub](http://azure.microsoft.com/en-us/services/event-hubs/).
The application itself is a UWP background task application. Data is being posted to EventHub using the [ConnectTheDots sample application](https://github.com/MSOpenTech/connectthedots/tree/master/Devices/DirectlyConnectedDevices/WindowsIoTCorePi2).

### Hardware/Software Requirements

- [Raspberry Pi 2 Model B](https://www.raspberrypi.org/products/raspberry-pi-2-model-b/)
- [Windows 10 IoT](https://ms-iot.github.io/content/en-US/GetStarted.htm)
- [HDC100x temperature and humidity sensor breakout](https://learn.adafruit.com/adafruit-hdc1008-temperature-and-humidity-sensor-breakout/overview)

![Hardware](https://raw.githubusercontent.com/stormy-ua/WindowsIoTCore.Drivers/master/Hdc100x/Images/Hardware.png)

### Azure Setup

[ConnectTheDots](https://github.com/MSOpenTech/connectthedots) already has infrastructure for setting up Azure EventHub. Just follow instructions.

### Sample Setup

Go to [StartupTask.cs](https://github.com/stormy-ua/WindowsIoTCore.Drivers/blob/master/Hdc100x/CS/Sample/Azure.EventHub/StartupTask.cs) and enter your Azure/EventHub settings:

```csharp
var connectTheDotsHelper = new ConnectTheDotsHelper(serviceBusNamespace: "[your service bus namespace]",
    eventHubName: "[yor event hub name]",
    keyName: "[your shared access policy key name]",
    key: "[your shared access policy key]",
    displayName: "[any display name]",
    organization: "[your organization]",
    location: "[your location]",
    sensorList: sensors);
```

### Real-time Results Observation

[ConnectTheDots](https://github.com/MSOpenTech/connectthedots) provides web site which allows to observe temperature and humidity measurements in a real-time.
Just deploy their Web application and you will be all set.

Here is the screenshot of my measurements harvested from the EventHub by the web application. Spikes on graphs correspond to the moment when I blew on the sensor:

![Dashboard](https://raw.githubusercontent.com/stormy-ua/WindowsIoTCore.Drivers/master/Hdc100x/Images/ConnectTheDotsWebAppDashboard.png)

