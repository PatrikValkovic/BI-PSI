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
        public static CommunicationPacket Receive(Socket socket)
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

            Logger.WriteLine($"RECV from={connectionNumber:X} seq={serialNumber} conf={confirmationNumber} flags={Convert.ToString(flags,2)} data({data.Length})={getDataInString(data)}",ConsoleColor.Red);
            return new CommunicationPacket(connectionNumber,serialNumber,confirmationNumber,flags,data);
        }

        private static byte[] outBuffer = new byte[(int)Sizes.PACKET_MAX];
        public static void Send(Socket socket, CommunicationPacket p)
        {
            if (p.Data.Length > 255)
                throw new ArgumentException("Data have more then 255 bytes");

            Logger.WriteLine($"SEND from={p.ConnectionNumber:X} seq={p.SerialNumber} conf={p.ConfirmationNumber} flags={Convert.ToString(p.Flags, 2)} data({p.Data.Length})={getDataInString(p.Data)}",ConsoleColor.Green);

            //Fucking rotate it, because that fucking image send data as fucking big endian
            UInt32 connectionNumber = p.ConnectionNumber;
            UInt16 serialNumber = p.SerialNumber;
            UInt16 confirmationNumber = p.ConfirmationNumber;
            if (BitConverter.IsLittleEndian)
            {
                connectionNumber = BitConverter.ToUInt32(BitConverter.GetBytes(connectionNumber).Reverse().ToArray(), 0);
                serialNumber = BitConverter.ToUInt16(BitConverter.GetBytes(serialNumber).Reverse().ToArray(), 0);
                confirmationNumber = BitConverter.ToUInt16(BitConverter.GetBytes(confirmationNumber).Reverse().ToArray(), 0);
            }

            BitConverter.GetBytes(connectionNumber).CopyTo(outBuffer,0);
            BitConverter.GetBytes(serialNumber).CopyTo(outBuffer,4);
            BitConverter.GetBytes(confirmationNumber).CopyTo(outBuffer, 6);
            outBuffer[8] = p.Flags;
            p.Data.CopyTo(outBuffer, 9);

            socket.Send(outBuffer.Take(p.Data.Length + 9).ToArray());
        }

        public static UInt32 InitConnection(Socket socket, Command action)
        {
            int i = 0;
            socket.ReceiveTimeout = 100;
            
            while (i < 20)
            {
                CommunicationFacade.Send(socket, new CommunicationPacket(0,0,0,(byte)Flag.SYN, new byte[] { (byte)action }));
                try
                {
                    while (true)
                    {
                        CommunicationPacket recived = CommunicationFacade.Receive(socket);
                        if (recived.Flags == (byte)Flag.SYN && recived.Data[0] == (byte)action && recived.SerialNumber==0 && recived.ConfirmationNumber==0)
                        {
                            Logger.WriteLine($"Connection established - communication {recived.ConnectionNumber:X}");
                            socket.ReceiveTimeout = 0;
                            return recived.ConnectionNumber;
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
