using System;
using System.Net;

namespace Server.Services
{
    //TODO: Do dokończenia
    class ConfigurationService : IServiceModule
    {
        public delegate void AddListenerD(IListener listener);

        private AddListenerD _addListener;
        public ConfigurationService(AddListenerD AddListenerD)
        {
            _addListener = AddListenerD;
        }
        public string AnswerCommand(string command)
        {
            //Przykład 
            //conf addlistener nazwa protokol

            //musi być przynajmniej conf nazwa co zrobić oraz nazwa i protokół
            string[] attributes = command.Split();
            if(attributes.Length > 3)
            {
                switch(attributes[1])
                {
                    case "addlistener": return AddListener(attributes);
                    case "removelistener": return RemoveListener(attributes);
                    default: return "Command is incorrect!" + Environment.NewLine;
                }
            }
            return "Command is incorrect!" + Environment.NewLine;
        }

        //Trzeba zrobić jakieś porównanie, żeby to normalnie móc usunąć
        private string RemoveListener(string[] command)
        {
            switch(command[3])
            {
                case "tcp":

                    break;
            }
            return "Successfull removed listener!";
        }

        //Tutaj nazwa jest zbędna
        private string AddListener(string[] command)
        {
            //command[2] - to nazwa 
            switch(command[3])
            {
                //niech [4] - adresip port
                case "tcp": 
                    if(command.Length > 5)
                    {
                        try
                        {
                            _addListener(new TCPListener(new IPEndPoint(IPAddress.Parse(command[4]), int.Parse(command[5]))));
                        }
                        catch(Exception ex)
                        {
                            return ex.Message;
                        }
                        return "Successfull created listener!" + Environment.NewLine;
                    }
                    break;
                case "udp":
                    if(command.Length > 5)
                    {
                        try
                        {
                            _addListener(new UDPListener(new IPEndPoint(IPAddress.Parse(command[4]), int.Parse(command[5]))));
                        }
                        catch(Exception ex)
                        {
                            return ex.Message;
                        }
                        return "Successfull created listener!" + Environment.NewLine;
                    }
                    break;
                default: return "Command is incorrect!" + Environment.NewLine;
            }
            return "Successfull created listener!" + Environment.NewLine;
        }
    }
}
