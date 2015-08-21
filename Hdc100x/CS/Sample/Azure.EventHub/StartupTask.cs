using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using WindowsIoTCore.Drivers.Hdc100X;

namespace Hdc100x.Azure.EventHub
{
    public sealed class StartupTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            var temperatureSensor = new ConnectTheDotsSensor("2298a348-e2f9-4438-ab23-82a3930662ab", "Temperature", "C");
            var humiditySensor = new ConnectTheDotsSensor("ADD6358D-1434-41EC-B156-E9096A37CB6C", "Humidity", "%");
            var sensors = new List<ConnectTheDotsSensor> { temperatureSensor, humiditySensor };

            var connectTheDotsHelper = new ConnectTheDotsHelper(serviceBusNamespace: "[your service bus namespace]",
                eventHubName: "[yor event hub name]",
                keyName: "[your shared access policy key name]",
                key: "[your shared access policy key]",
                displayName: "[any display name]",
                organization: "[your organization]",
                location: "[your location]",
                sensorList: sensors);

            // infinite loop for measurment temperature & humidity once per second
            using (var sensor = await Hdc100X.Get(BusAddress.Address40))
            {
                for (;;)
                {
                    try
                    {
                        // ask sensor to measure and get result
                        var measurement = await sensor.MeasureAsync();
                        Debug.WriteLine("{0} : {1}", measurement.Temperature, measurement.Humidity);

                        // post measurements to EventHub
                        temperatureSensor.value = measurement.Temperature;
                        humiditySensor.value = measurement.Humidity;
                        connectTheDotsHelper.SendSensorData(temperatureSensor);
                        connectTheDotsHelper.SendSensorData(humiditySensor);
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine("Ooops! Exception:{0}", exc);
                    }

                    await Task.Delay(1000);
                }
            }
        }
    }
}
