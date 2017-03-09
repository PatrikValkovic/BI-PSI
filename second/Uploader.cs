using second.Packets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace second
{
    class Uploader
    {
        private BinaryReader inFile;
        private Socket socket;

        private UInt32 connectionNumber;
        private UInt64 windowBegin;

        public Uploader(Socket s, BinaryReader reader)
        {
            this.inFile = reader;
            this.socket = s;
        }

        public void InitConnection()
        {
            this.connectionNumber = CommunicationFacade.InitConnection(this.socket, Command.UPLOAD);
        }

        public UInt64 Sended { get { return this.windowBegin; } }

        private class SharedObject
        {
            public SharedObject(Socket s)
            {
                this.Socket = s;
            }

            public object CountersLocker = new object();
            public UInt64 Confirmed = 0;
            public ushort ThisConfirmedTimes = 0;
            public Socket Socket;
            public Queue<CommunicationPacket> ArriveQueue = new Queue<CommunicationPacket>();
        }

        static private void TimeoutCheckerThread(object Param)
        {
            Logger.WriteLine("TimeoutChecker thread started");
        }

        static private void ProccessDataThread(object Param)
        {
            Logger.WriteLine("ProccessData thread started");
        }

        static private void ReceiveThread(object param)
        {
            SharedObject data = (SharedObject)param;
            while (true)
            {
                CommunicationPacket p = CommunicationFacade.Receive(data.Socket);
                //TODO validation?
                lock (data.ArriveQueue)
                {
                    data.ArriveQueue.Enqueue(p);
                }
            }
        }

        public async void SendFile()
        {
            SharedObject shared = new SharedObject(this.socket);

            Task timeoutChecker = new Task(TimeoutCheckerThread, shared);
            Task receive = new Task(ReceiveThread, shared);
            Task dataProccess = new Task(ProccessDataThread, shared);
            Task[] tasks = new Task[] { timeoutChecker, receive, dataProccess };
            try
            {
                foreach (Task t in tasks)
                    t.Start();

                Task<Task> waiter = Task.WhenAny(timeoutChecker, receive, dataProccess);
                waiter.Wait();

                Task whoEndIt = waiter.Result;

                foreach (Task t in tasks)
                    t.Wait();
            }
            finally
            {
                foreach (Task t in tasks)
                    t.Dispose();
            }
        }
    }
}
