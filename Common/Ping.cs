using System;

namespace Common
{
    public class Ping
    {
        public static string Query(int sizeRequest, int sizeResponse)
        {
            string query = $"ping {sizeResponse} ";
            return $"{query}{Trash(sizeRequest - query.Length - 1)}\n";
        }

        private static string Trash(int count)
        {
            Random r = new Random();

            char[] tab = new char[count];
            for (int i = 0; i < count; i++) tab[i] = (char)r.Next(48, 122);
            return new string(tab);
        }

        public static string Pong(string line)
        {
            string[] tab = line.Split();
            string response = "pong ";
            return $"{response}{Trash(int.Parse(tab[1]) - response.Length - 2)}\n";
        }
    }
}
