using Common;
using System;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Client
    {
        //Klient powinien umieć czytać komendy, na początek zrobiłbym czas pingu i jeśli wpisze się ping to jest wysyłany pakiet i jest on mierzony.
        static void Main(string[] args)
        {
            Start();

           // Test0TCP();
        }

        static void Start()
        {
            bool exitFlag = false;
            while (!exitFlag)
            {
                Console.WriteLine("Wybierz protokół komunikacji:");
                Console.WriteLine("1. TCP");
                Console.WriteLine("2. UDP");
                Console.WriteLine("3. RS232");
                Console.WriteLine("4. FTP");
                Console.WriteLine("0. Exit");

                int protocol;
                while(!int.TryParse(Console.ReadLine(), out protocol)) Console.WriteLine("Podałeś błędne wartości, spróbuj ponownie!");
                
                switch (protocol)
                {
                    case 1: StartTCP();
                        break;
                    case 2: StartUDP();
                        break;
                    case 3: StartRS232();
                        break;
                    case 4: StartFTP();
                        break;
                    case 0: exitFlag = true;
                        break;
                    default: Console.WriteLine("Wybrano błędną opcję!");
                        break;
                }
            }
        }

        static void StartFTP()
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://hostname.com/");
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

                request.Credentials = new NetworkCredential("maruthi", "******");
                request.KeepAlive = false;
                request.UseBinary = true;
                request.UsePassive = true;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                Console.WriteLine(reader.ReadToEnd());

                Console.WriteLine("Directory List Complete status {0}", response.StatusDescription);

                reader.Close();
                response.Close();
            
            
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message.ToString());
            }
        }

        static void StartRS232()
        {
            bool exitFlag = false;
            SerialPort sp = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
            sp.Open();
            while (!exitFlag)
            {
                string command;
                Console.WriteLine("Podaj komende:");
                command = Console.ReadLine();
                sp.WriteLine(command);
            }
        }

        static UdpClient receiver;

        static void Receive(IAsyncResult ar)
        {
            string server = "127.0.0.1";
            IPEndPoint ipe = new IPEndPoint(IPAddress.Any, 0);
            receiver.Connect(ipe);
            byte[] data = receiver.EndReceive(ar, ref ipe);
            string msg = Encoding.Unicode.GetString(data);
            Console.WriteLine(msg);
            receiver.BeginReceive(Receive, null);
        }

        //Wysyłanie danych przez UDP
        static void StartUDP()
        {
            //Tworzenie nasłuchiwacza udp
          //  receiver = new UdpClient(1234);
           // receiver.BeginReceive(Receive, null);

            bool exitFlag = false;
            string server = "127.0.0.1";
            UdpClient client = new UdpClient();
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(server), 12346);
            try
            {
                client.Connect(ip);
                Console.WriteLine(ip);
                while (!exitFlag)
                {
                    string command;
                    Console.WriteLine("Podaj komende:");
                    command = Console.ReadLine();

                    string[] tmp = command.Split();
                    switch(tmp[0])
                    {
                        case "ping":
                            //while(true)
                            {
                                string message = Ping.Query(int.Parse(tmp[1]), int.Parse(tmp[2]));
                                byte[] data = Encoding.ASCII.GetBytes(message);
                                client.Send(data, data.Length);
                                
                            }
              /*              byte[] send = client.Receive(ref ip);
                            string receiveString = Encoding.ASCII.GetString(send);
                            Console.WriteLine(receiveString);*/
                            break;
                        case "chat":
                            byte[] data1 = Encoding.ASCII.GetBytes(command);
                            client.Send(data1, data1.Length);
                            byte[] send1 = client.Receive(ref ip);
                            string receiveString1 = Encoding.ASCII.GetString(send1);
                            Console.WriteLine(receiveString1);
                            break;
                        case "logout":
                            exitFlag = true;
                            break;
                        default: Console.WriteLine("Wpisano nieprawidłową komende!");
                            break;
                    }
                }
                client.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void SendTCP(NetworkStream stream, string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);

            var watch = System.Diagnostics.Stopwatch.StartNew();
            stream.Write(data, 0, data.Length);
            //Console.Write("Wysłane: {0}", message);

            byte[] response = new byte[256];
            string responseStr = string.Empty;
            //czytanie dopóki response Str nie będzie miało większego 
            int bytes;
            do
            {
                bytes = stream.Read(response, 0, response.Length);
                responseStr += Encoding.ASCII.GetString(response, 0, bytes);
            }
            while ( stream.DataAvailable) /*responseStr.Length < int.Parse(tmp[2])*/;
            watch.Stop();
            Console.WriteLine(responseStr);
            Console.WriteLine("Czas wykonywania komendy: " + watch.ElapsedMilliseconds);
        }

        static void StartTCP()
        {
            try
            {
                bool exitFlag = false;
                //TODO: Trzeba ogarnać co się stanie jak nie ma połączenia
                string server = "localhost";
                TcpClient client = new TcpClient(server, 12345);
                NetworkStream stream = client.GetStream();

                while (!exitFlag)
                {
                    string command;
                    Console.WriteLine("Podaj komendę");
                    //Czytanie komend z linii
                    command = Console.ReadLine();

                    //Splitowanie, żeby ogarnąć jaka komenda została wpisana
                    string[] tmp = command.Split();
                    switch (tmp[0])
                    {
                        case "ping":
                            try
                            {
                                if (tmp.Length != 3)
                                {
                                    Console.WriteLine("Nieprawidłowe użycie komendy ping!");
                                    break;
                                }
                                string message = Ping.Query(int.Parse(tmp[1]), int.Parse(tmp[2]));
                                SendTCP(stream,message);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                Console.WriteLine("Połączenie zostało nieoczekiwnie przerwane!");
                                exitFlag = true;
                            }
                            break;                      
                        case "logout":
                            exitFlag = true;
                            break;
                        default:
                            try
                            {
                                SendTCP(stream, command);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                Console.WriteLine("Połączenie zostało nieoczekiwnie przerwane!");
                                exitFlag = true;
                            }
                            break;
                    }
                }
                stream.Close();
                client.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}