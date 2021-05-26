using Common;
using System;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    //TODO: Przesyłanie plików przez każdy protokół
    class Client
    {
        static void Main(string[] args)
        {
            Start();
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
                Console.WriteLine("5. .Net remoting");
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
                    case 5: StartNetRemoting();
                        break;
                    case 0: exitFlag = true;
                        break;
                    default: Console.WriteLine("Wybrano błędną opcję!");
                        break;
                }
            }
        }

        static void StartNetRemoting()
        {
            string command = string.Empty;
            while(!command.Equals("logout"))
            {
                Console.WriteLine("Podaj komende:");
                command = Console.ReadLine();
                CommonNetRemoting obj = (CommonNetRemoting)Activator.GetObject(typeof(CommonNetRemoting), "tcp://localhost:65432/command");
                Console.WriteLine(obj.Command(command));
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
            SerialPort sp = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
            sp.Open();
            string command = string.Empty;
            while (!command.Equals("logout"))
            {
                Console.WriteLine("Podaj komende:");
                command = Console.ReadLine();
                sp.Write(command + Environment.NewLine);
                command = sp.ReadLine();
                Console.WriteLine(command);
            }
        }

        //Wysyłanie danych przez UDP
        static void StartUDP()
        {
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

                    byte[] data;
                    byte[] send;
                    string[] tmp = command.Split();
                    string received = string.Empty;
                    switch(tmp[0])
                    {
                        case "ping":
                            string message = Ping.Query(int.Parse(tmp[1]), int.Parse(tmp[2]));
                            data = Encoding.ASCII.GetBytes(message);
                            client.Send(data, data.Length);
                            send = client.Receive(ref ip);
                            received = Encoding.ASCII.GetString(send);
                            Console.WriteLine(received);
                            break;
                        default:
                            data = Encoding.ASCII.GetBytes(command);
                            client.Send(data, data.Length);
                            send = client.Receive(ref ip);
                            received = Encoding.ASCII.GetString(send);
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

        //Na razie zrobię odbieranie plików
        static void SendTCPFTP(NetworkStream stream, string command)
        {
            if (command.Split()[1].Equals("send"))
            {
                command += " " + FTP.FileToString(command.Split()[2]);
                Console.WriteLine(command.Split()[3].Length);
            }

            byte[] data = Encoding.ASCII.GetBytes(command);

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
            while (stream.DataAvailable) /*responseStr.Length < int.Parse(tmp[2])*/;
            watch.Stop();

            if (command.Split()[1].Equals("get")) FTP.StringToFile(responseStr, command.Split()[2]);
            else Console.WriteLine(responseStr);
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
                        case "ftp":
                            try
                            {
                                SendTCPFTP(stream, command);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                exitFlag = true;
                            }
                            break;
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
