### Adafruit HDC100X

[![Nuget](https://img.shields.io/nuget/v/WindowsIoTCore.Drivers.Hdc100X.svg)](https://www.nuget.org/packages/WindowsIoTCore.Drivers.Hdc100X/) 
[![Nuget](https://img.shields.io/nuget/dt/WindowsIoTCore.Drivers.Hdc100X.svg)](https://www.nuget.org/packages/WindowsIoTCore.Drivers.Hdc100X/)

Windows 10 IoT driver for [Adafruit HDC100X](https://learn.adafruit.com/adafruit-hdc1008-temperature-and-humidity-sensor-breakout/overview)

![HDC1008 image](https://raw.githubusercontent.com/stormy-ua/WindowsIoTCore.Drivers/master/Hdc100x/Images/Hdc1008.jpg)

- [Datasheet](http://www.adafruit.com/datasheets/hdc1008.pdf)
- [Pinouts](https://learn.adafruit.com/adafruit-hdc1008-temperature-and-humidity-sensor-breakout/pinouts)

#### Sample Code

```csharp
using (var sensor = await Hdc100X.Get(BusAddress.Address40))
{
    // infinite loop for temperature and humidity measurement once per 500 ms
    for (;;)
    {
        try
        {
            var measurment = await sensor.MeasureAsync();
            Debug.WriteLine("{0} : {1}", measurment.Temperature, measurment.Humidity);
        }
        catch (Exception ex)
        {
            //Ooops...something bad happened
        }

        await Task.Delay(500);
    }
}
```
