using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
    class ChatService : IServiceModule
    {
        //<Odbiorca wiadomości, <Nadawca wiadomości, <Treść wiadomości>>>
        private Dictionary<string, Dictionary<string, List<string>>> _messages = new Dictionary<string, Dictionary<string, List<string>>>(); 

        public string AnswerCommand(string command)
        {
            //Przykład komendy odpowiedzialnej za wysyłanie 
            //usługa, komenda, od kogo, do kogo, wiadomość
            //chat send adam andrzej co tam u Ciebie?

            //usługa, komenda, kogo wiadomosci
            //chat get adam
            if(command.Split().Length > 0)
            {
                switch (command.Split()[1])
                {
                    case "send":
                        if(AddMessage(command)) return "Successfully send message!\n";
                        else return "Command is incorrect!\n";
                    case "get": return GetMessage(command);
                    case "help": return Help();
                    default:
                        return "Command is incorrect!\n";
                }
            }
            return "Command is incorrect!\n";
        }

        private string Help()
        {
            return
                "This is chat help\n" +
                "chat send -user -user - send message\n" +
                "chat get -user - get user messages\n";
        }

        private string GetMessage(string command)
        {
            if(command.Split().Length > 2)
            {
                string odbiorca = command.Split()[2];

                if (_messages.ContainsKey(odbiorca))
                {
                    StringBuilder sr = new StringBuilder();

                    Dictionary<string, List<string>> item = _messages[odbiorca];

                    foreach (var i in item)
                    {
                        sr.Append("Sender: " + i.Key + "\n");
                        for (int j = 0; j < i.Value.Count; j++) sr.Append(i.Value[j] + "\n");
                    }
                    return sr.ToString();
                }
                return "No messages for " + odbiorca + "\n";
            }
            return "Command is incorrect!\n";
        }

        private bool AddMessage(string command)
        {
            string[] stringArray = command.Split();

            if (stringArray.Length > 3)
            {
                string nadawca = stringArray[2];
                string odbiorca = stringArray[3];

                string wiadomosc = string.Empty;
                for (int i = 4; i < stringArray.Length; i++) wiadomosc += stringArray[i] + " ";

                //Dodawanie do słownika 
                if (_messages.ContainsKey(odbiorca))
                {
                    if (_messages[odbiorca].ContainsKey(nadawca))
                    {
                        if (_messages[odbiorca][nadawca] != null) _messages[odbiorca][nadawca].Add(wiadomosc);
                        else _messages[odbiorca][nadawca] = new List<string>() { wiadomosc };
                    }
                    else _messages[odbiorca].Add(nadawca, new List<string>() { wiadomosc });
                }
                else _messages.Add(odbiorca, new Dictionary<string, List<string>>() { { nadawca, new List<string>() { wiadomosc } } });
                return true;
            }
            return false;
        }
    }
}