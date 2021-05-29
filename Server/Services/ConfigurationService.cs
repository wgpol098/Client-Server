using System;
using System.Text;

namespace Server.Services
{
    //TODO: Przetestować działanie tej usługi
    //Wydaje mi się, że mam tutaj błędne indeksy w czytaniu informacji 
    class ConfigurationService : IServiceModule
    {
        public delegate void AddListenerD(IListener listener);
        public delegate void RemoveListenerD(IListener listener);
        public delegate void AddServiceD(string name, IServiceModule service);
        public delegate void RemoveServiceModuleD(string name);

        private AddListenerD _addListener;
        private AddServiceD _addService;
        private RemoveServiceModuleD _removeServiceModule;
        private RemoveListenerD _removeListener;

        public ConfigurationService(AddListenerD AddListenerD, AddServiceD AddServiceD, RemoveServiceModuleD RemoveServiceModuleD, RemoveListenerD RemoveListenerD)
        {
            _addListener = AddListenerD;
            _addService = AddServiceD;
            _removeServiceModule = RemoveServiceModuleD;
            _removeListener = RemoveListenerD;
        }

        public string AnswerCommand(string command)
        {
            //Przykład 
            //conf addlistener nazwa protokol

            string[] attributes = command.Split();
            if(attributes.Length > 3)
            {
                switch(attributes[1])
                {
                    case "addlistener": return AddListener(attributes);
                    case "removelistener": return RemoveListener(attributes);
                    case "addservice": return AddService(attributes);
                    case "removeservice": return RemoveService(attributes);
                    case "help": return Help();
                    default: return "Command is incorrect!" + Environment.NewLine;
                }
            }
            return "Command is incorrect!" + Environment.NewLine;
        }

        private string Help()
        {
            return "This is conf help\n" +
                "conf addlistener name listenertype config" +
                "conf addservice servicetype name [foldername]" +
                "conf removelistener servicetype config" +
                "conf removeservice servicetype name" +
                "examples:" +
                "conf addlistener name tcp address port" +
                "conf addlistener name udp address port" +
                "conf addlistener name rs232 port [boundrate]" +
                "conf addlistener name netremoting tcpchannel" +
                "conf addservice ftp name foldername" +
                "conf removeservice name" +
                "conf removelistener tcp address port";
        }

        private string RemoveService(string[] command)
        {
            if (command.Length > 3)
            {
                try { _removeServiceModule(command[3]); }
                catch(Exception ex) { return ex.Message; }
            }
            return "Command is incorrect!" + Environment.NewLine; 
        }

        //TODO: Trzeba zrobić jakieś porównanie, żeby to normalnie móc usunąć
        private string RemoveListener(string[] command)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 4; i < command.Length; i++) sb.Append(command[i] + " ");

            switch (command[3])
            {
                case "tcp":
                    try { _removeListener(new TCPListener(sb.ToString())); }
                    catch(Exception ex) { return ex.Message; }
                    return "Successfull removedd listener!" + Environment.NewLine;
                case "udp":
                    try { _removeListener(new UDPListener(sb.ToString())); }
                    catch(Exception ex) { return ex.Message; }
                    return "Successfull removedd listener!" + Environment.NewLine;
                case "netremoting":
                    try { _removeListener(new NETRemotingListener(sb.ToString())); }
                    catch(Exception ex) { return ex.Message; }
                    return "Successfull removedd listener!" + Environment.NewLine;
                case "rs232":
                    try { _removeListener(new RS232Listener(sb.ToString())); }
                    catch(Exception ex) { return ex.Message; }
                    return "Successfull removedd listener!" + Environment.NewLine;
                default: return "Listener is incorrect" + Environment.NewLine;
            }
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
                case "ping":
                    try { _addService(command[4], new PingService()); }
                    catch(Exception ex) { return ex.Message; }
                    return "Succesfull added service!" + Environment.NewLine;
                default: return "Service is incorrect!" + Environment.NewLine;
            }
        }

        //!!!!
        //Tutaj nazwa jest zbędna
        private string AddListener(string[] command)
        {
            //command[2] - to nazwa
            StringBuilder sb = new StringBuilder();
            for(int i = 4; i < command.Length; i++) sb.Append(command[i] + " ");

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
