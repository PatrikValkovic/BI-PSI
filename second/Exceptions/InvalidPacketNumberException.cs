using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace second.Exceptions
{
    class InvalidPacketNumberException : InvalidPacketException
    {
        public InvalidPacketNumberException()
        {
        }

        public InvalidPacketNumberException(string message) : base(message)
        {
        }

        public InvalidPacketNumberException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidPacketNumberException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
