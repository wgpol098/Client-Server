using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    struct UdpState
    {
        public UdpClient udpClient;
        public IPEndPoint endPoint;
    }

    //Obsługa tego nasłuchiwacza jest łatwieszja niż TCP
    //TODO: Refaktoryzacja
    class UDPListener : IListener
    {
        private UdpClient _udpClient;
        private CommunicatorD _onConnect;

        private IPEndPoint _ipEndPoint;
        public UDPListener(IPEndPoint iPEnd)
        {
            _ipEndPoint = iPEnd;
        }

        //To wszystko i tak będzie w odpowiadaczu
        //Tutaj jest coś nie tak - trzeba to naprawić
        private void HandleConnection(IAsyncResult ar)
        {
            UdpClient udpClient = ((UdpState)ar.AsyncState).udpClient;
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);
            
            byte[] receiveBytes = udpClient.EndReceive(ar, ref iPEndPoint);
            string receiveString = Encoding.ASCII.GetString(receiveBytes);

            ICommunicator communicator = new UDPCommunicator(udpClient, iPEndPoint, receiveString);
            _onConnect(communicator);

            UdpState n = new UdpState
            {
                endPoint = iPEndPoint,
                udpClient = udpClient
            }; 
            udpClient.BeginReceive(new AsyncCallback(HandleConnection), n);
        }

        //Odpalenie nasłuchiwacza UDP
        public void Start(CommunicatorD onConnect)
        {
            _onConnect = onConnect;
            _udpClient = new UdpClient(_ipEndPoint);
            UdpState udpState = new UdpState
            {
                endPoint = _ipEndPoint,
                udpClient = _udpClient
            };
            Console.WriteLine("[UDP] Waiting for clients!");
            _udpClient.BeginReceive(new AsyncCallback(HandleConnection), udpState);
        }

        public void Stop()
        {
            _udpClient.Close();
        }
    }

    class UDPCommunicator : ICommunicator
    {
        private IPEndPoint _ipEndPoint;
        private string _receivedString = string.Empty;
        UdpClient _client;

        public UDPCommunicator(UdpClient client, IPEndPoint udpClient, string receivedString)
        {
            _ipEndPoint = udpClient;
            _receivedString = receivedString;
            _client = client;
           // GetServiceName(receivedString);
            //_client = new UdpClient();
        }
        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            //_ipEndPoint.Port = 12346;
            //_client.Connect(_ipEndPoint);
            //Console.WriteLine(_ipEndPoint);
           // Console.WriteLine(_client.Client.RemoteEndPoint);
            byte[] send = Encoding.ASCII.GetBytes(onCommand(_receivedString));
           // _client.Send(send, send.Length);
            //Stop();

            UdpClient udp = new UdpClient(_ipEndPoint);
            //udp.Connect(_ipEndPoint);
           // Console.WriteLine(udp.Client.RemoteEndPoint);
            udp.Send(send, send.Length, _ipEndPoint);
            //udp.Send(send, send.Length);
            udp.Close();

            onDisconnect(this);
            Stop();
        }

        public void Stop()
        {
            Console.WriteLine("[UDP] Stop Connector");
            //_client.Close();
        }
    }
}