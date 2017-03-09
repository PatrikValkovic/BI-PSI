using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace second.Exceptions
{
    class InvalidPacketException : Exception
    {
        public InvalidPacketException()
        {
        }

        public InvalidPacketException(string message) : base(message)
        {
        }

        public InvalidPacketException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidPacketException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
