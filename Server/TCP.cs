using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class TCPListener : IListener
    {
        TcpListener server;
        List<TcpClient> tcpClients = new List<TcpClient>();
        public void Start(CommunicatorD onConnect)
        {
            server = new TcpListener(IPAddress.Any, 12345);
            server.Start();
            byte[] bytes = new byte[256];

            //nasłuchuj cały czas
            //Tutaj powinno się wyświetlać to co nasłuchuje
            //To chyba będzie dobrze działać dla kilku klientów -- ale trzeba przetestować
            while(true)
            {
                //to już na tym poziomie powinno być tworzenie odpowadacza
                TcpClient client = server.AcceptTcpClient();


                //tmp.Start(command, onConnect);

                //Pytanie co się stanie jak będzie więcej klientów.
                //Czytanie odbieranych danych
                NetworkStream stream = client.GetStream();
                string data = null;
                int len, nl;
                while ((len = stream.Read(bytes, 0, bytes.Length)) > 0)
                {
                    data += Encoding.ASCII.GetString(bytes, 0, len);

                    Console.WriteLine(data);
                    //Jeśli przyszły jakieś dane to tworzę odpowiadacza, który odpowiada

                    ICommunicator tmp = new CommunicatorTCP(client);

                    //  ICommunicator tmp = new CommunicatorTCP(client.Client.Add);
                    CommandD command = new CommandD(Ping.Pong);
                    tmp.Start(command, onConnect);


                    //  ICommunicator tmp = new CommunicatorTCP();
                    //  CommandD command = new CommandD(Ping.Pong);

                    //  tmp.Start(command, onConnect);
                    // onConnect(tmp);
                    data = null;
                }
            }
            

            /*            while (true)
                        {
                            TcpClient client = server.AcceptTcpClient();
                            string data = null;
                            int len, nl;
                            NetworkStream stream = client.GetStream();
                            while ((len = stream.Read(bytes, 0, bytes.Length)) > 0)
                            {
                                data += Encoding.ASCII.GetString(bytes, 0, len);

                                Console.WriteLine(data);

                              //  ICommunicator tmp = new CommunicatorTCP();
                              //  CommandD command = new CommandD(Ping.Pong);

                              //  tmp.Start(command, onConnect);
                               // onConnect(tmp);
                                data = null;
                            }
                        }*/
        }

        public void Stop() => server.Stop();
    }

    class CommunicatorTCP : ICommunicator
    {
        TcpClient client;
        public CommunicatorTCP(TcpClient tcpClient)
        {
            client = tcpClient;
        }
        //Na razie nie ważne w onCommand
        //Pierwszy argument mówi jak odpowiadacz ma wygenerować argument
        //Drugi mówi jak ma się odmeldowywać odpowiadacz. Kiedy odpowiadacz uzna, że koniec komunikacji, to odmelduje się przez ten delegat
        //Wywoływana kiedy odpowiadacz zakończy swoje działanie.
        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            NetworkStream stream = client.GetStream();


                    string message = onCommand("pong 102 asd");
                    byte[] dat1a = Encoding.ASCII.GetBytes(message);

                    stream.Write(dat1a, 0, dat1a.Length);
                    Console.Write("Wysłane: {0}", message);

                    //  ICommunicator tmp = new CommunicatorTCP();
                    //  CommandD command = new CommandD(Ping.Pong);

                    //  tmp.Start(command, onConnect);
                    // onConnect(tmp);
 


          //  stream.Close();
          //  client.Close();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
