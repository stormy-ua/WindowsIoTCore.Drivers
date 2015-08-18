using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using WindowsIoTCore.Drivers.Hdc100X.Exceptions;

namespace WindowsIoTCore.Drivers.Hdc100X
{
    public class Hdc100X : IDisposable
    {
        private const int MaxAttemptsCount = 3;
        private static readonly ConcurrentDictionary<BusAddress, Lazy<Hdc100X>> _sensors = 
            new ConcurrentDictionary<BusAddress, Lazy<Hdc100X>>();
        private readonly TaskCompletionSource<bool> _initalizationCompletion = new TaskCompletionSource<bool> ();
        private readonly BusAddress _busAddress;
        private I2cDevice _sensorDevice;
        private int _isInitialized = 0;

        private Hdc100X(BusAddress busAddress)
        {
            _busAddress = busAddress;
        }

        private async Task Initialize()
        {
            try
            {
                if (Interlocked.CompareExchange(ref _isInitialized, 1, 0) == 1)
                {
                    return;
                }

                // Get a selector string that will return all I2C controllers on the system
                string deviceSelector = I2cDevice.GetDeviceSelector();
                // Find the I2C bus controller device with our selector string
                var dis = await DeviceInformation.FindAllAsync(deviceSelector);
                if (dis.Count == 0)
                {
                    throw new Hdc100XInitializationException("No I2C controllers were found on the system");
                }

                var settings = new I2cConnectionSettings((int)_busAddress)
                {
                    BusSpeed = I2cBusSpeed.FastMode
                };
                _sensorDevice = await I2cDevice.FromIdAsync(dis[0].Id, settings);
                if (_sensorDevice == null)
                {
                    throw new Hdc100XInitializationException(string.Format(
                        "Slave address {0} on I2C Controller {1} is currently in use by " +
                        "another application. Please ensure that no other applications are using I2C.",
                        settings.SlaveAddress,
                        dis[0].Id));
                }

                // Configure sensor:
                // - measure with 14bit precision
                // - measure both temperature and humidity
                _sensorDevice.Write(new byte[] { 0x02, 0x10, 0x00 });

                _initalizationCompletion.SetResult(true);
            }
            catch (Hdc100XInitializationException)
            {
                _initalizationCompletion.SetResult(false);
                throw;
            }
            catch (Exception exc)
            {
                _initalizationCompletion.SetResult(false);
                throw new Hdc100XInitializationException("Unexpected error during initialization", exc);
            }
        }

        public static async Task<Hdc100X> Get(BusAddress busAddress)
        {
            var sensor = _sensors.GetOrAdd(busAddress, new Lazy<Hdc100X>(() => new Hdc100X(busAddress))).Value;
            await sensor.Initialize();

            return sensor;
        }

        public async Task<Measurment> MeasureAsync()
        {
            if (!await _initalizationCompletion.Task)
            {
                throw new Hdc100XMeasurementException("Can't start measurement on not initialized sensor");
            }

            try
            {
                // Trigger measurement
                _sensorDevice.Write(new byte[] { 0x00 });

                // Conversion time for 14bit resolution is 6.5 ms so 10ms should be enough to wait for the measurement
                await Task.Delay(10);

                for (int attempt = 1; ; ++attempt)
                {
                    byte[] buf = new byte[4];

                    try
                    {
                        // Read result
                        _sensorDevice.Read(buf);

                        double temp = ((buf[0] << 8) + buf[1]) * 1.0 / (1 << 16) * 165 - 40;
                        double humidity = ((buf[2] << 8) + buf[3]) * 1.0 / (1 << 16) * 100;

                        return new Measurment(temp, humidity);
                    }
                    catch (Exception)
                    {
                        if (attempt == MaxAttemptsCount)
                        {
                            throw;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                throw new Hdc100XMeasurementException("Measurement failed", exc);
            }
        }

        public void Dispose()
        {
            if (_sensorDevice != null)
            {
                _sensorDevice.Dispose();
            }
        }
    }
}
