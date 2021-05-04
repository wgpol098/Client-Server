using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    //Tak można zrobić TCPLISTENERA
    class FTPListener : IListener
    {
        private TcpListener _listener;
        public void Start(CommunicatorD onConnect)
        {
            _listener = new TcpListener(IPAddress.Any, 12345);
            _listener.Start();
            _listener.BeginAcceptTcpClient(HandleAcceptTcpClient, _listener);
        }

        public void Stop()
        {
            if (_listener != null) _listener.Stop();
        }

        private void HandleAcceptTcpClient(IAsyncResult result)
        {
            TcpClient client = _listener.EndAcceptTcpClient(result);
            _listener.BeginAcceptTcpClient(HandleAcceptTcpClient, _listener);

            NetworkStream stream = client.GetStream();

            using (StreamWriter writer = new StreamWriter(stream, Encoding.ASCII))
            {
                using(StreamReader reader = new StreamReader(stream, Encoding.ASCII))
                {
                    writer.WriteLine("YOU CONNECT TO ME");
                    writer.Flush();
                    writer.WriteLine("I will repeat after you. Send a blank line to quit.");
                    writer.Flush();

                    string line = string.Empty;

                    while(!string.IsNullOrEmpty(line = reader.ReadLine()))
                    {
                        writer.WriteLine("Echoing back: {0}", line);
                        writer.Flush();
                    }
                }
            }
        }
    }

    class FTPCommunicator : ICommunicator
    {
        public bool Running => throw new NotImplementedException();

        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
