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
            try
            {
                while (true)
                {
                    command = GetCommand();
                    if (command.Equals("logout")) break;
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    CommonNetRemoting obj = (CommonNetRemoting)Activator.GetObject(typeof(CommonNetRemoting), "tcp://localhost:65432/command");
                    if (command.Contains("ftp get ")) FTP.StringToFile(obj.Command(command), command.Split()[2]);
                    else Console.WriteLine(obj.Command(command));
                    Console.WriteLine("CommandTime: " + watch.Elapsed);
                    Console.WriteLine("------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        //TODO: Do zrobienia 
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

        //Wszystkie usługi są obsługiwane
        //TODO: server list albo trzeba obsłużyć albo prerobić
        static void StartRS232()
        {
            try
            {
                using (SerialPort sp = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One))
                {
                    sp.Open();
                    string command = string.Empty;
                    string response = string.Empty;
                    while (true)
                    {
                        command = GetCommand();
                        if (command.Equals("logout")) break;
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        sp.WriteLine(command);
                        response = sp.ReadLine();
                        if (command.Contains("ftp get ")) FTP.StringToFile(response, command.Split()[2]);
                        else Console.WriteLine(command);
                        Console.WriteLine("CommandTime: " + watch.Elapsed);
                        Console.WriteLine("------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());  
            }
        }

        //Wysyłanie danych przez UDP
        //TODO: Trzeba ogarnąć bufor wysyłania i odbierania plików w tym protokole
        static void StartUDP()
        {
            string server = "127.0.0.1";
            int port = 12346;
            UdpClient client = new UdpClient();
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(server), port);
            try
            {
                client.Connect(ip);
                string command = string.Empty;
                string response = string.Empty;
                while (true)
                {
                    command = GetCommand();
                    if (command.Equals("logout")) break;

                    byte[] data = Encoding.ASCII.GetBytes(command + Environment.NewLine);
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    client.Send(data, data.Length);

                    byte[] received = client.Receive(ref ip);
                    response = Encoding.ASCII.GetString(received);

                    if (command.Contains("ftp get ")) FTP.StringToFile(response, command.Split()[2]);
                    else Console.WriteLine(command);
                    Console.WriteLine("CommandTime: " + watch.Elapsed);
                    Console.WriteLine("------------------");
                }
                client.Close();
            }
            catch(Exception e) { Console.WriteLine(e.ToString()); }
        }

        static string GetCommand()
        {
            Console.WriteLine("Give the command:");
            return CreateCommand(Console.ReadLine());
        }

        static string CreateCommand(string command)
        {
            if (command.Contains(" send ")) return command + " " + FTP.FileToString(command.Split()[2]);
            if (command.Contains("ping ")) return Ping.Query(int.Parse(command.Split()[1]), int.Parse(command.Split()[2]));
            return command;
        }

        static void StartTCP()
        {
            try
            {
                string server = "localhost";
                
                using(TcpClient client = new TcpClient(server, 12345))
                {
                    NetworkStream stream = client.GetStream();
                    string command = string.Empty;
                    StringBuilder responseStr = new StringBuilder();
                    while (true)
                    {
                        command = GetCommand();
                        if (command.Equals("logout")) break;

                        byte[] data = Encoding.ASCII.GetBytes(command + Environment.NewLine);
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        stream.Write(data, 0, data.Length);
                        byte[] response = new byte[256];
                        responseStr = new StringBuilder();
                        int bytes;
                        do
                        {
                            bytes = stream.Read(response, 0, response.Length);
                            responseStr.Append(Encoding.ASCII.GetString(response, 0, bytes));
                        }
                        while (stream.DataAvailable);

                        if (command.Contains("ftp get ")) FTP.StringToFile(responseStr.ToString(), command.Split()[2]);
                        else Console.WriteLine(responseStr.ToString());
                        Console.WriteLine("CommandTime: " + watch.Elapsed);
                        Console.WriteLine("------------------");

                    }
                    stream.Close();
                }

            }
            catch(Exception e) { Console.WriteLine(e.Message); }
        }
    }
}
