using Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Server
    {
        List<IListener> listeners = new List<IListener>();
        List<ICommunicator> communicators = new List<ICommunicator>();
        Dictionary<string, IServiceModule> services = new Dictionary<string, IServiceModule>();

        public Server()
        {
            //AddListener(new TCPListener());
        }

        void AddServiceModule(string name, IServiceModule service)
        {

        }

        //Dodawanie odpowiadacza
        void AddCommunicator(ICommunicator communicator)
        {
            communicators.Add(communicator);
        }

        //Dodawanie nasłuchiwacza
        void AddListener(IListener listener)
        {
            listeners.Add(listener);
            listener.Start(new CommunicatorD(AddCommunicator));
        }

        void RemoveServiceModule(string name, IServiceModule service)
        {

        }

        void RemoveCommunicator(ICommunicator communicator)
        {

        }

        void RemoveListener(IListener listener)
        {

        }

        static void Main()
        {
            var ser = new Server();
            ser.AddListener(new TCPListener());
        }

        static void Test0PingTCP()
        {
            TcpListener server = new TcpListener(IPAddress.Any, 12345);
            server.Start();
            byte[] bytes = new byte[256];
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                string data = null;
                int len, nl;
                NetworkStream stream = client.GetStream();
                while ((len = stream.Read(bytes, 0, bytes.Length)) > 0)
                {
                    data += Encoding.ASCII.GetString(bytes, 0, len);
                    while ((nl = data.IndexOf('\n')) != -1)
                    {
                        string line = data.Substring(0, nl + 1);
                        data = data.Substring(nl + 1);
                        byte[] msg = Encoding.ASCII.GetBytes(Ping.Pong(line));
                        stream.Write(msg, 0, msg.Length);
                    }
                }
                client.Close();
                server.Stop();
            }
        }
    }
}