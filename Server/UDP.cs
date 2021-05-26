using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class UDPListener : IListener
    {
        private UdpClient _udpClient;

        private IPEndPoint _ipEndPoint;
        public UDPListener(IPEndPoint iPEnd) => _ipEndPoint = iPEnd;

        public UDPListener(string config)
        {
            if(config != null)
            {
                string[] tmp = config.Split();
                if (tmp.Length == 2) _ipEndPoint = new IPEndPoint(IPAddress.Parse(tmp[0]), int.Parse(tmp[1]));
            }
        }

        public void Start(CommunicatorD onConnect)
        {
            _udpClient = new UdpClient(_ipEndPoint);
            onConnect(new UDPCommunicator(_udpClient, _ipEndPoint));
        }

        public void Stop() => _udpClient.Close();
    }

    class UDPCommunicator : ICommunicator
    {
        private IPEndPoint _ipEndPoint;
        private UdpClient _client;
        private CommandD _onCommand;
        private CommunicatorD _onDisconnect;

        public UDPCommunicator(UdpClient client) => _client = client;

        public UDPCommunicator(UdpClient client, IPEndPoint udpClient)
        {
            _ipEndPoint = udpClient;
            _client = client;
        }

        private void HandleConnection(IAsyncResult ar)
        {
            byte[] receiveBytes = _client.EndReceive(ar, ref _ipEndPoint);
            string receiveString = Encoding.ASCII.GetString(receiveBytes);
            string receive = _onCommand(receiveString);
            byte[] sendBytes = Encoding.ASCII.GetBytes(receive);
            Console.WriteLine(receive);
            _client.Send(sendBytes, sendBytes.Length, _ipEndPoint);
            _client.BeginReceive(new AsyncCallback(HandleConnection), _client);
        }
        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            _onCommand = onCommand;
            _onDisconnect = onDisconnect;
            _client.BeginReceive(new AsyncCallback(HandleConnection), _client);
        }

        public void Stop() => Console.WriteLine("[UDP] Stop Connector");
    }
}
