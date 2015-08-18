using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WindowsIoTCore.Drivers.Hdc100X.Exceptions
{
    public class Hdc100XMeasurementException : Exception
    {
        public Hdc100XMeasurementException()
        {
        }

        public Hdc100XMeasurementException(string message) : base(message)
        {
        }

        public Hdc100XMeasurementException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
