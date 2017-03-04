using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace second
{
    class Program
    {
        static void Main(string[] args)
        {
            string ip = "192.168.56.101";
            int port = 4000;
            string file = "";

            if (args.Length == 0 || args.Length > 2)
            {
                Console.WriteLine("Usage: second <ip> [<file>]");
                Console.WriteLine($"Default action will be use: download form ip {ip}");
                args = new string[] { ip };
            }

            //ip validation
            ip = args[0];
            IPAddress address;
            if (!IPAddress.TryParse(ip, out address))
            {
                Console.WriteLine($"Invalid IP address {ip}");
                Console.WriteLine("Program will be terminated");
                return;
            };
            using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                Console.WriteLine($"Connecting to {address}");
                try { s.Connect(address, port); }
                catch (SocketException e)
                {
                    Console.WriteLine("Connection was not established");
                    Console.WriteLine("Program will be terminated");
                    return;
                }

                if (args.Length == 1)
                {
                    Console.WriteLine("Download action will be use");
                    //TODO download
                }
                else if (args.Length == 2)
                {
                    Console.WriteLine("Firmware upload action will be use");
                    //file validation
                    if (!File.Exists(args[1]))
                    {
                        Console.WriteLine($"Invalid file {args[1]}");
                        Console.WriteLine("Program will be terminated");
                        return;
                    }
                    //TODO upload
                }
            }
            byte[] buffer = new byte[10];
            using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                Console.WriteLine("Connecting");
                s.Connect("192.168.56.101", 4000);
                //s.Connect("147.32.232.173", 4000);
                Console.WriteLine("Pripojeno");
                s.Send(new byte[] { 0,0,0,0, //cislo spojeni
                    0,0, //sekvencni cislo
                    0,0, //cislo potvrzeni
                    4, //SYN
                    1 /*download fotografie*/});
                Console.WriteLine("Odeslano");
                s.ReceiveTimeout = 10000;
                try
                {
                    byte priznaky = 0;
                    UInt32 cisloSpojeni = 0;
                    UInt16 sekvencniCislo = 0;
                    UInt16 CisloPotvrzeni = 0;
                    byte prikaz = 0;

                    while (priznaky != 4)
                    {
                        s.Receive(buffer);

                        foreach (byte b in buffer)
                        {
                            Console.Write($"{b:X00}");
                        }
                        Console.WriteLine();

                        cisloSpojeni = BitConverter.ToUInt32(buffer, 0);
                        sekvencniCislo = BitConverter.ToUInt16(buffer, 4);
                        CisloPotvrzeni = BitConverter.ToUInt16(buffer, 6);
                        priznaky = buffer[8];
                        prikaz = buffer[9];
                        Console.WriteLine($"Cislo spojeni {cisloSpojeni:X} sekvencni {sekvencniCislo} potvrzeni {CisloPotvrzeni} priznaky {Convert.ToString(priznaky, 2)} prikaz {prikaz}");
                    }
                    Console.WriteLine("Prikaz 4 prisel");

                }
                catch (SocketException e)
                {
                    Console.WriteLine("Timeouted");
                    Console.ReadKey();
                    return;
                }
            }

            Console.WriteLine("Konec");
            Console.ReadKey();
        }
    }
}
