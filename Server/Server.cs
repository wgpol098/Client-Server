using Server.Services;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net;
using System.Threading;

namespace Server
{
    public enum ServerStatus
    {
        Created = 1,
        Stop = 2,
        Running = 3
    }
    class Server
    {
        public ServerStatus Status = ServerStatus.Created;

        List<IListener> listeners = new List<IListener>();
        List<ICommunicator> communicators = new List<ICommunicator>();
        Dictionary<string, IServiceModule> services = new Dictionary<string, IServiceModule>();

        //blokada dla communicators
        private object _lock = new object();
        //blokada dla listeners
        private object _listenersLock = new object();
        public Server() { }

        void AddServiceModule(string name, IServiceModule service) => services.Add(name, service);

        void AddCommunicator(ICommunicator communicator)
        {
            lock(_lock)
            {
                communicators.Add(communicator);
                if (Status == ServerStatus.Running) communicator.Start(new CommandD(Answer), RemoveCommunicator);
            }
        }

        void AddListener(IListener listener)
        {
            lock(_listenersLock)
            {
                listeners.Add(listener);
                if (Status == ServerStatus.Running) listener.Start(new CommunicatorD(AddCommunicator));
            }
            Console.WriteLine("[SRV] Added listener!");
        }

        void RemoveServiceModule(string name) => services.Remove(name);

        void RemoveCommunicator(ICommunicator communicator) 
        {
            var cm = communicators.Find(x => x.Equals(communicator));
            cm.Stop();
            lock(_lock) { communicators.Remove(cm); }
            Console.WriteLine("[SRV] Removed communicator!");
        }

        void RemoveListener(IListener listener) 
        {
            lock(_listenersLock)
            {
                var lr = listeners.Find(x => x.Equals(listener));
                if (lr != null) lr.Stop();
                listeners.Remove(lr);
            }
            Console.WriteLine("[SRV] Removed listener!");
        }

        void Start()
        {
            AddServiceModule("conf", new ConfigurationService
                (
                    new ConfigurationService.AddListenerD(AddListener), 
                    new ConfigurationService.AddServiceD(AddServiceModule),
                    new ConfigurationService.RemoveServiceModuleD(RemoveServiceModule),
                    new ConfigurationService.RemoveListenerD(RemoveListener)
                ));

            for (int i = 0; i < listeners.Count; i++) listeners[i].Start(new CommunicatorD(AddCommunicator));
            Status = ServerStatus.Running;
        }

        private string Answer(string command)
        {
            if( command != null)
            {
                string serviceName = command.Split()[0];
                if (services.ContainsKey(serviceName)) return services[serviceName].AnswerCommand(command);
                return "Services is unavailable.";
            }
            return "Command was null!";
        }

        void WaitForStop()
        {
            while (communicators.Count != 0) Thread.Sleep(1000);
            while (services.Count != 0) Thread.Sleep(1000);
        }

        void Stop()
        {
            for (int i = 0; i < listeners.Count; i++) listeners[i].Stop();
            listeners.Clear();
            for (int i = 0; i < communicators.Count; i++) communicators[i].Stop();
            communicators.Clear();
            services.Clear();
            Status = ServerStatus.Stop;
        }

        static void Main()
        {
            var srv = new Server();
            srv.AddServiceModule("ping", new PingService());
            srv.AddServiceModule("chat", new ChatService());
            srv.AddServiceModule("ftp", new FTPService("FTP"));
            
            srv.Start();
            srv.AddListener(new TCPListener(new IPEndPoint(IPAddress.Any, 12345)));
            srv.AddListener(new UDPListener(new IPEndPoint(IPAddress.Any, 12346)));
            srv.AddListener(new RS232Listener(new SerialPort("COM2", 9600, Parity.None, 8, StopBits.One)));
            srv.AddListener(new FilesListener("FilesListener"));
            srv.AddListener(new NETRemotingListener("65432"));
            srv.WaitForStop();
            srv.Stop();
        }
    }
}