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

        private static byte[] inputBuffer = new byte[MAXIMUM_LENGTH_OF_MESSAGE];
        public static void receive(Socket socket, out UInt32 ConnectionNumber, out UInt16 SerialNumber, out UInt16 ConfirmationNumber, out byte Flags, out byte[] Data)
        {
            socket.Receive(inputBuffer);
            ConnectionNumber = BitConverter.ToUInt32(inputBuffer,0);
            SerialNumber = BitConverter.ToUInt16(inputBuffer, 4);
            ConfirmationNumber = BitConverter.ToUInt16(inputBuffer, 6);
            Flags = inputBuffer[8];
            Data = inputBuffer.Skip(8).ToArray();
        }

        private static byte[] outBuffer = new byte[MAXIMUM_LENGTH_OF_MESSAGE];
        public static void send(Socket socket, UInt32 ConnectionNumber, UInt16 SerialNumber, UInt16 ConfirmationNumber, byte Flags, byte[] Data)
        {
            if (Data.Length > 255)
                throw new ArgumentException("Data have more then 255 bytes");

            BitConverter.GetBytes(ConnectionNumber).CopyTo(outBuffer,0);
            BitConverter.GetBytes(SerialNumber).CopyTo(outBuffer,4);
            BitConverter.GetBytes(ConnectionNumber).CopyTo(outBuffer,6);
            outBuffer[8] = Flags;
            Data.CopyTo(outBuffer, 9);
            socket.Send(outBuffer.Take(Data.Length + 9).ToArray());
        }
    }
}
