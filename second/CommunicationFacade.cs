using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace second
{
    class CommunicationFacade
    {
        private static readonly int MAXIMUM_LENGTH_OF_MESSAGE = 264;

        private static byte[] buffer = new byte[MAXIMUM_LENGTH_OF_MESSAGE];
        public static void receive(Socket socket, out UInt32 ConnectionNumber, out UInt16 SerialNumber, out UInt16 ConfirmationNumber, out byte Flags, out byte[] Data)
        {
            socket.Receive(buffer);
            ConnectionNumber = BitConverter.ToUInt32(buffer,0);
            SerialNumber = BitConverter.ToUInt16(buffer, 4);
            ConfirmationNumber = BitConverter.ToUInt16(buffer, 6);
            Flags = buffer[8];
            Data = buffer.Skip(8).ToArray();
        }
    }
}
