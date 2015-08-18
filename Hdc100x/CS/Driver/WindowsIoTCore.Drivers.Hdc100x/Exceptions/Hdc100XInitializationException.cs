using System;

namespace WindowsIoTCore.Drivers.Hdc100X.Exceptions
{
    public class Hdc100XInitializationException : Exception
    {
        public Hdc100XInitializationException()
        {
        }

        public Hdc100XInitializationException(string message) : base(message)
        {
        }

        public Hdc100XInitializationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}