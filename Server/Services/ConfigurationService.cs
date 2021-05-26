using System;
using System.Text;

namespace Server.Services
{
    //TODO: Do dokończenia
    class ConfigurationService : IServiceModule
    {
        public delegate void AddListenerD(IListener listener);
        public delegate void AddServiceD(string name, IServiceModule service);

        private AddListenerD _addListener;
        private AddServiceD _addService;
        public ConfigurationService(AddListenerD AddListenerD, AddServiceD AddServiceD)
        {
            _addListener = AddListenerD;
            _addService = AddServiceD;
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
                    case "addservice": return AddService(attributes);
                    case "help": return Help();
                    default: return "Command is incorrect!" + Environment.NewLine;
                }
            }
            return "Command is incorrect!" + Environment.NewLine;
        }

        //TODO: Dokończyć robienie helpu
        private string Help()
        {
            return "This is conf help\n" +
                "conf addlistener name listenertype config" +
                "examples:" +
                "conf addlistener name tcp address port" +
                "conf addlistener name udp address port" +
                "conf addlistener name rs232 port [boundrate]" +
                "conf addlistener name netremoting tcpchannel" +
                "";
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

        private string AddService(string[] command)
        {
            switch(command[3])
            {
                case "ftp":
                    try { _addService(command[4], new FTPService(command[5])); }
                    catch(Exception ex) { return ex.Message; }
                    return "Succesfull added service!" + Environment.NewLine;
                case "chat":
                    try { _addService(command[4], new ChatService()); }
                    catch(Exception ex) { return ex.Message; }
                    return "Succesfull added service!" + Environment.NewLine;
                default: return "Command is incorrect!" + Environment.NewLine;

            }
        }

        //!!!!
        //Tutaj nazwa jest zbędna
        private string AddListener(string[] command)
        {
            //command[2] - to nazwa
            //Zróbmy tutaj rozdzielenie tego stringa, żeby przekazywać tylko string conf do każdego listenera - a nie męczyć się tak jak teraz
            StringBuilder sb = new StringBuilder();
            for(int i = 4; i < command.Length; i++)
            {
                sb.Append(command[i] + " ");
            }

            Console.WriteLine("[conf] " + sb.ToString());
            switch(command[3])
            {
                //Trzeba posprawdzać te długości i wszystkie listenery, czy działają prawidłowo
                case "tcp": 
                    if(command.Length > 5)
                    {
                        try { _addListener(new TCPListener(sb.ToString())); }
                        catch(Exception ex) { return ex.Message; }
                        return "Successfull created listener!" + Environment.NewLine;
                    }
                    break;
                case "udp":
                    if(command.Length > 5)
                    {
                        try { _addListener(new UDPListener(sb.ToString())); }
                        catch(Exception ex) { return ex.Message; }
                        return "Successfull created listener!" + Environment.NewLine;
                    }
                    break;
                case "netremoting":
                    if(command.Length > 5)
                    {
                        try { _addListener(new NETRemotingListener(sb.ToString())); }
                        catch(Exception ex) { return ex.Message; }
                        return "Succesfull created listener!" + Environment.NewLine;
                    }
                    break;
                case "rs232":
                    if(command.Length > 5)
                    {
                        try { _addListener(new RS232Listener(sb.ToString())); }
                        catch(Exception ex) { return ex.Message; }
                        return "Succesfull created listener!" + Environment.NewLine;
                    }
                    break;
                default: return "Command is incorrect!" + Environment.NewLine;
            }
            return "Successfull created listener!" + Environment.NewLine;
        }
    }
}
