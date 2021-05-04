using System;
using System.Diagnostics;
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
        private TcpClient client;
        public TCPCommunicator(TcpClient tcpClient) => client = tcpClient;

        Task task;
        private bool _running = false;
        public bool Running { get { return _running; } }

        private CommandD _onCommand;
        private CommunicatorD _onDisconnect;

        private int _time = 10000;

        //Start powinien s
        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            //Tutaj możliwe jest ustawianie timeoutu połączenia
            client.ReceiveTimeout = _time;
            client.SendTimeout = _time;
            _onCommand = onCommand;
            _onDisconnect = onDisconnect;
            task = new Task(() => AnswerTask());
            task.Start();
            _running = true;
        }

        //client Connecter pokazuje stan na poprzednią operację, trzeba pingować klienta, żeby okazało się czy wszystko jest dobrze
        //Może trzeba zrobić jakiś service, który będzie odpowiedzialny za kończenie połączenia?
        //Można zrobić jakiś ping co dwie sekundy podtrzymujący połączenie

        //Klient musi dodawać jakiś znak końca linii i trzeba to czytać dopóki nie otrzymamy tego znaku


        //Ta metoda działa w tle
        private void AnswerTask()
        {
            Console.WriteLine("[TCP] Communicator start");
            NetworkStream networkStream = client.GetStream();
            byte[] bytes = new byte[256];
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //To musi być tutaj
            string data = string.Empty;
            while (client.Connected /*&& stopwatch.ElapsedMilliseconds <= _time*/)
            {
                if (/*!data.Contains('3'.ToString()) && */ networkStream.DataAvailable)
                {
                    int len = networkStream.Read(bytes, 0, bytes.Length);
                    data += Encoding.ASCII.GetString(bytes, 0, len);
                }
                else if (data != string.Empty)
                {
                    string message = _onCommand(data);
                    bytes = Encoding.ASCII.GetBytes(message);
                    networkStream.Write(bytes, 0, bytes.Length);
                    Console.Write("[TCP] Wysłane: {0}", message);
                    stopwatch.Restart();
                    data = string.Empty;
                }
            }
            networkStream.Close();
            Stop();
        }

        public void Stop()
        {
            _onDisconnect(this);
            Console.WriteLine("[TCP] Communicator stopped");
            client.Close();
            _running = false;
        }
    }
}