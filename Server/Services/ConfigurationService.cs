using System;
using System.Linq;
using System.Text;

namespace Server.Services
{
    //TODO: Przetestować działanie tej usługi
    //TODO: Wydaje mi się, że mam tutaj błędne indeksy w czytaniu informacji 
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

        //TODO: Do uzupełnienia
        private string Help()
        {
            return "This is conf help\n" +
                "conf addlistener name listenertype config" +
                "conf addservice servicetype name [foldername]" +
                "conf removelistener servicetype config" +
                "conf removeservice servicetype name" +
                "examples:" +
                "conf addlistener tcp address port" +
                "conf addlistener udp address port" +
                "conf addlistener rs232 port [boundrate]" +
                "conf addlistener netremoting tcpchannel" +
                "conf addservice ftp name foldername" +
                "conf removeservice name" +
                "conf removelistener tcp address port";
        }

        private string RemoveService(string[] command)
        {
            if (command.Length > 3)
            {
                try { _removeServiceModule(command[3]); }
                catch(Exception ex) { return ex.Message.ToString(); }
            }
            return "Command is incorrect!" + Environment.NewLine; 
        }

        private string RemoveListener(string[] command)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 3; i < command.Length; i++) sb.Append(command[i] + " ");
            try
            {
                Type t = Type.GetType("Server." + command[2], false, true);
                if (t.GetInterfaces().Contains(typeof(IListener)))
                {
                    object ob = Activator.CreateInstance(t, sb.ToString());
                    _removeListener((IListener)ob);
                }
                else return "Listener is incorrect!";
            }
            catch(Exception ex) {  return ex.Message.ToString();}
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
                case "ping":
                    try { _addService(command[4], new PingService()); }
                    catch(Exception ex) { return ex.Message; }
                    return "Succesfull added service!" + Environment.NewLine;
                default: return "Service is incorrect!" + Environment.NewLine;
            }
        }

        //TODO: Przetestowanie dodawania błędnych danych w stringu do _addListener
        private string AddListener(string[] command)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 3; i < command.Length; i++) sb.Append(command[i] + " ");

            Console.WriteLine("[conf] " + sb.ToString());
            try
            {
                Type t = Type.GetType("Server." + command[2], false, true);
                if (t.GetInterfaces().Contains(typeof(IListener)))
                {
                    object ob = Activator.CreateInstance(t, sb.ToString());
                    _addListener((IListener)ob);
                }
                else return "Listener is incorrect!";
            }
            catch (Exception ex) {  return ex.Message.ToString(); }
            return "Successfull created listener!";
        }
    }
}
