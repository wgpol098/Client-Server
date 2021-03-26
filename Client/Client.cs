using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Client
    {
        //Klient powinien umieć czytać komendy, na początek zrobiłbym czas pingu i jeśli wpisze się ping to jest wysyłany pakiet i jest on mierzony.
        static void Main(string[] args)
        {
            Test0TCP();
        }


        //Dużo czasu zajmuje łączenie się z serwerem
        static void Test0TCP()
        {
            while(true)
            {
                string server = "localhost";
                TcpClient client = new TcpClient(server, 12345);
                NetworkStream stream = client.GetStream();

                //zawalanie pingiem
                while(true)
                {
                    string message = Ping.Query(102, 102);
                    byte[] data = Encoding.ASCII.GetBytes(message);

                    stream.Write(data, 0, data.Length);
                    Console.Write("Wysłane: {0}", message);

                    byte[] response = new byte[256];
                    string responseStr = string.Empty;
                    int bytes;
                    do
                    {
                        bytes = stream.Read(response, 0, response.Length);
                        responseStr += Encoding.ASCII.GetString(response, 0, bytes);
                    }
                    while (stream.DataAvailable);

                    Console.WriteLine("Pobrane: {0}", responseStr);
                }


                client.Close();
            }

            
        }
    }
}