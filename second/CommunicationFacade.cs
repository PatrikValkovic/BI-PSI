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
        private static StringBuilder log = new StringBuilder();
        private static string getDataInString(byte[] data)
        {
            log.Clear();
            if (data.Length > 16)
            {
                foreach (byte b in data.Take(8).ToArray())
                    log.Append($"{b:X00} ");
                log.Append("... ");
                foreach (byte b in data.Skip(data.Length - 8).ToArray())
                    log.Append($"{b:X00} ");
            }
            else
            {
                foreach (byte b in data)
                    log.Append($"{b:X00} ");
            }
            return log.ToString();
        }



        private static byte[] inputBuffer = new byte[(int) Sizes.PACKET_MAX];
        public static void Receive(Socket socket, out UInt32 ConnectionNumber, out UInt16 SerialNumber, out UInt16 ConfirmationNumber, out byte Flags, out byte[] Data)
        {
            int recived = socket.Receive(inputBuffer);
            ConnectionNumber = BitConverter.ToUInt32(inputBuffer,0);
            SerialNumber = BitConverter.ToUInt16(inputBuffer, 4);
            ConfirmationNumber = BitConverter.ToUInt16(inputBuffer, 6);
            Flags = inputBuffer[8];
            Data = inputBuffer.Skip(9).Take(recived-(int)Sizes.HEADER_SIZE).ToArray();

            Logger.WriteLine($"RECV from={ConnectionNumber:X} seq={SerialNumber} conf={ConfirmationNumber} flags={Convert.ToString(Flags,2)} Data={getDataInString(Data)}");
        }

        private static byte[] outBuffer = new byte[(int)Sizes.PACKET_MAX];
        public static void Send(Socket socket, UInt32 ConnectionNumber, UInt16 SerialNumber, UInt16 ConfirmationNumber, byte Flags, byte[] Data)
        {
            if (Data.Length > 255)
                throw new ArgumentException("Data have more then 255 bytes");

            BitConverter.GetBytes(ConnectionNumber).CopyTo(outBuffer,0);
            BitConverter.GetBytes(SerialNumber).CopyTo(outBuffer,4);
            BitConverter.GetBytes(ConnectionNumber).CopyTo(outBuffer,6);
            outBuffer[8] = Flags;
            Data.CopyTo(outBuffer, 9);

            Logger.WriteLine($"SEND from={ConnectionNumber:X} seq={SerialNumber} conf={ConfirmationNumber} flags={Convert.ToString(Flags, 2)}Data={getDataInString(Data)}");

            socket.Send(outBuffer.Take(Data.Length + 9).ToArray());
        }

        public static void InitConnection(Socket socket,out UInt32 ConnectionNumber, Command action)
        {
            int i = 0;
            socket.ReceiveTimeout = 100;

            byte flags;
            byte[] data = new byte[(int)Sizes.PACKET_MAX];
            UInt16 serialNumber;
            UInt16 confirmationNumber;
            while (i < 20)
            {
                CommunicationFacade.Send(socket, 0, 0, 0, (byte)Flag.SYN, new byte[] { (byte)action });
                try
                {
                    while (true)
                    {
                        CommunicationFacade.Receive(socket, out ConnectionNumber, out serialNumber, out confirmationNumber, out flags, out data);
                        if (flags == (byte)Flag.SYN && data[0] == (byte)action && serialNumber==0 && confirmationNumber==0)
                        {
                            Logger.WriteLine($"Connection established - communication {ConnectionNumber:X}");
                            socket.ReceiveTimeout = 0;
                            return;
                        }
                        else
                            Console.WriteLine("Data obtained before connection packet received, ignoring");
                    }
                }
                catch (SocketException e) when (e.SocketErrorCode == SocketError.TimedOut)
                {
                    Logger.WriteLine($"Connection timeouted, attemp number {i + 1}");
                    i++;
                }
            }
            throw new Exceptions.MaximumAttempException();
        }
    }
}
