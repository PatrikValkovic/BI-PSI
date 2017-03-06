using second.Packets;
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
        public static RecvPacket Receive(Socket socket)
        {      
            int recived = socket.Receive(inputBuffer);
            UInt32 connectionNumber = BitConverter.ToUInt32(inputBuffer,0);
            UInt16 serialNumber = BitConverter.ToUInt16(inputBuffer, 4);
            UInt16 confirmationNumber = BitConverter.ToUInt16(inputBuffer, 6);
            byte flags = inputBuffer[8];
            byte[] data = inputBuffer.Skip(9).Take(recived-(int)Sizes.HEADER_SIZE).ToArray();

            //Fucking rotate it, because that fucking image send data as fucking big endian
            if (BitConverter.IsLittleEndian)
            {
                connectionNumber = BitConverter.ToUInt32(BitConverter.GetBytes(connectionNumber).Reverse().ToArray(),0);
                serialNumber = BitConverter.ToUInt16(BitConverter.GetBytes(serialNumber).Reverse().ToArray(),0);
                confirmationNumber = BitConverter.ToUInt16(BitConverter.GetBytes(confirmationNumber).Reverse().ToArray(),0);
            }

            Logger.WriteLine($"RECV from={connectionNumber:X} seq={serialNumber} conf={confirmationNumber} flags={Convert.ToString(flags,2)} data({data.Length})={getDataInString(data)}");
            return new RecvPacket(connectionNumber,serialNumber,confirmationNumber,flags,data);
        }

        private static byte[] outBuffer = new byte[(int)Sizes.PACKET_MAX];
        public static void Send(Socket socket, UInt32 ConnectionNumber, UInt16 SerialNumber, UInt16 ConfirmationNumber, byte Flags, byte[] Data)
        {
            if (Data.Length > 255)
                throw new ArgumentException("Data have more then 255 bytes");

            Logger.WriteLine($"SEND from={ConnectionNumber:X} seq={SerialNumber} conf={ConfirmationNumber} flags={Convert.ToString(Flags, 2)} data({Data.Length})={getDataInString(Data)}");

            //Fucking rotate it, because that fucking image send data as fucking big endian
            if (BitConverter.IsLittleEndian)
            {
                ConnectionNumber = BitConverter.ToUInt32(BitConverter.GetBytes(ConnectionNumber).Reverse().ToArray(), 0);
                SerialNumber = BitConverter.ToUInt16(BitConverter.GetBytes(SerialNumber).Reverse().ToArray(), 0);
                ConfirmationNumber = BitConverter.ToUInt16(BitConverter.GetBytes(ConfirmationNumber).Reverse().ToArray(), 0);
            }

            BitConverter.GetBytes(ConnectionNumber).CopyTo(outBuffer,0);
            BitConverter.GetBytes(SerialNumber).CopyTo(outBuffer,4);
            BitConverter.GetBytes(ConfirmationNumber).CopyTo(outBuffer, 6);
            outBuffer[8] = Flags;
            Data.CopyTo(outBuffer, 9);

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
