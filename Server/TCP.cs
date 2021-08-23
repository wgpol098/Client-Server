using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class TCPListener : IListener
    {
        private TcpListener _listener;
        private CommunicatorD _onConnect;

        private IPEndPoint _ipEndPoint;

        public TCPListener(IPEndPoint iPEndPoint) => _ipEndPoint = iPEndPoint;

        public TCPListener(string config)
        {
            if(config != null)
            {
                string[] tmp = config.Split();
                if (tmp.Length >= 2)
                {
                    _ipEndPoint = new IPEndPoint(IPAddress.Parse(tmp[0]), int.Parse(tmp[1]));
                }
            }
        }

        private void HandleAcceptTcpClient(IAsyncResult result)
        {
            TcpClient tcp = _listener.EndAcceptTcpClient(result);
            _listener.BeginAcceptTcpClient(HandleAcceptTcpClient, _listener);
            ICommunicator tmp = new TCPCommunicator(tcp);
            _onConnect(tmp); 
            Console.WriteLine("[TCP] Connected with: " + tcp.Client.RemoteEndPoint);
        }

        public void Start(CommunicatorD onConnect)
        {
            _onConnect = onConnect;
            _listener = new TcpListener(_ipEndPoint);
            _listener.Start();
            _listener.BeginAcceptTcpClient(HandleAcceptTcpClient, _listener);
        }

        public void Stop() => _listener.Stop();
    }

    class TCPCommunicator : ICommunicator
    {
        private TcpClient _client;
        public TCPCommunicator(TcpClient tcpClient) => _client = tcpClient;

        Task task;

        private CommandD _onCommand;
        private CommunicatorD _onDisconnect;

        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            _onCommand = onCommand;
            _onDisconnect = onDisconnect;
            task = new Task(() => AnswerTask());
            task.Start();
        }

        //Background Method
        private void AnswerTask()
        {
            Console.WriteLine("[TCP] Communicator start");
            NetworkStream networkStream = _client.GetStream();
            byte[] bytes = new byte[256];

            string data = string.Empty;
            while (_client.Connected)
            {
                if (networkStream.DataAvailable)
                {
                    int len = networkStream.Read(bytes, 0, bytes.Length);
                    data += Encoding.ASCII.GetString(bytes, 0, len);
                }
                else if (data != string.Empty)
                {
                    string message = _onCommand(data);
                    bytes = Encoding.ASCII.GetBytes(message);
                    networkStream.Write(bytes, 0, bytes.Length);
                    Console.Write("[TCP] Send: {0}", message);
                    data = string.Empty;
                }
                if(_client.Client.Poll(0, SelectMode.SelectRead))
                {
                    if (_client.Client.Receive(new byte[1], SocketFlags.Peek) == 0) _onDisconnect(this);
                }
            }
            networkStream.Close();
            Stop();
        }

        public void Stop()
        {
            _client.Close();
            Console.WriteLine("[TCP] Communicator stopped");
        }
    }
}
