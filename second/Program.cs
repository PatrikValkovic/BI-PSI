﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace second
{
    class Program
    {
        static void Main(string[] args)
        {
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
                        
                        foreach(byte b in buffer)
                        {
                            Console.Write($"{b:X00}");
                        }
                        Console.WriteLine();

                        cisloSpojeni = BitConverter.ToUInt32(buffer, 0);
                        sekvencniCislo = BitConverter.ToUInt16(buffer, 4);
                        CisloPotvrzeni = BitConverter.ToUInt16(buffer, 6);
                        priznaky = buffer[8];
                        prikaz = buffer[9];
                        Console.WriteLine($"Cislo spojeni {cisloSpojeni:X} sekvencni {sekvencniCislo} potvrzeni {CisloPotvrzeni} priznaky {Convert.ToString(priznaky,2)} prikaz {prikaz}");
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