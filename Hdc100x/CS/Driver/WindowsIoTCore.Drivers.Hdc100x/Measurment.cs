namespace WindowsIoTCore.Drivers.Hdc100X
{
    public class Measurment
    {
        public double Temperature { get; private set; }
        public double Humidity { get; private set; }

        public Measurment(double temperature, double humidity)
        {
            Temperature = temperature;
            Humidity = humidity;
        }
    }
}