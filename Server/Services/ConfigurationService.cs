using System;
using System.Linq;
using System.Text;

namespace Server.Services
{
    //TODO: Przetestować działanie tej usługi
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

            string[] attributes = command.Trim().Split();
            if(attributes.Length > 2)
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
                "conf addlistener name listenertype config\n" +
                "conf addservice servicetype name [foldername]\n" +
                "conf removelistener servicetype config\n" +
                "conf removeservice name\n" +
                "examples:\n" +
                "conf addlistener tcp address port\n" +
                "conf addlistener udp address port\n" +
                "conf addlistener rs232 port [boundrate]\n" +
                "conf addlistener netremoting tcpchannel\n" +
                "conf addservice ftp name foldername\n" +
                "conf removeservice name\n" +
                "conf removelistener tcplistener address port\n";
        }

        private string RemoveService(string[] command)
        {
            try 
            { 
                _removeServiceModule(command[2]);
                return $"Successfull removed {command[2]} service!";
            }
            catch(Exception ex) { return ex.Message.ToString(); }
        }

        private string RemoveListener(string[] command)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 3; i < command.Length; i++) sb.Append(command[i] + " ");
            try
            {
                Type t = Type.GetType($"Server.{command[2]}", false, true);
                if (t == null) return "Listener is incorrect!";
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

        //TODO: Do przetesowania usługa FTP
        private string AddService(string[] command)
        {
            try
            {
                Type t = Type.GetType($"Server.Services.{command[2]}", false, true);
                if (t == null) return "Service is incorrect!";
                if(t.GetInterfaces().Contains(typeof(IServiceModule)))
                {
                    object ob;
                    if (command.Length > 4)
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int i = 4; i < command.Length; i++) sb.Append($"{command[i]} ");
                        ob = Activator.CreateInstance(t, sb.ToString().Trim());
                    }
                    else ob = Activator.CreateInstance(t);
                    _addService(command[3], (IServiceModule)ob);
                    return $"Succesfull added {command[3]} service!";
                }
                return "Service is incorrect!";
            }
            catch(Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        private string AddListener(string[] command)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 3; i < command.Length; i++) sb.Append(command[i] + " ");

            Console.WriteLine("[conf] " + sb.ToString());
            try
            {
                Type t = Type.GetType("Server." + command[2], false, true);
                if (t == null) return "Listener is incorrect!";
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
