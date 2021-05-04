using Server.Services;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

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

        Task _checkerTask;
        //blokada dla communicators
        private object _lock = new object();
        //blokada dla listeners
        private object _listenersLock = new object();
        public Server() { }

        void AddServiceModule(string name, IServiceModule service) => services.Add(name, service);

        //Dodawanie odpowiadacza
        void AddCommunicator(ICommunicator communicator)
        {
            lock(_lock)
            {
                communicators.Add(communicator);
            }
        }

        //Dodawanie nasłuchiwacza
        void AddListener(IListener listener)
        {
            lock(_listenersLock)
            {
                listeners.Add(listener);
                if (Status == ServerStatus.Running) listener.Start(new CommunicatorD(AddCommunicator));
            }
        }

        void RemoveServiceModule(string name) => services.Remove(name);

        void RemoveCommunicator(ICommunicator communicator) 
        {
            var cm = communicators.Find(x => x.Equals(communicator));
            lock(_lock)
            {
                communicators.Remove(cm);
                Console.WriteLine("[SRV] Removed communicator");
            }
        }

        void RemoveListener(IListener listener) 
        {
            //lr.Stop();
            lock(_listenersLock)
            {
                var lr = listeners.Find(x => x.Equals(listener));
                listeners.Remove(lr);
            }
        }

        void Start()
        {
            for(int i = 0; i < listeners.Count; i++) listeners[i].Start(new CommunicatorD(AddCommunicator));
            _checkerTask = new Task(() => Checker());
            _checkerTask.Start();
            Status = ServerStatus.Running;
        }

        //Metoda odpowiedzialna za odpowiadanie 
        private string Answer(string command)
        {
            string serviceName = command.Split()[0];
            if (services.ContainsKey(serviceName)) return services[serviceName].AnswerCommand(command);
            return "Services is unavailable.\n";
        }


        //Trzebaby dodać wątek obserwujący stan usług, odpowiadaczy i nasłuchiwaczy i odpalający - wyłączający je, bądź usuwający
        private void Checker()
        {
            Console.WriteLine("[SRV] Checker task = start");
            while(true)
            {
                //jeśli zmieni się lista odpowiadaczy, bądź nasłuchiwaczy
                //Na razie zrobię obsługę odpowiadaczy
                //Może warto to rozdzielić na kilka wątków, jak będzie dużo odpowiadaczy to będą się tu działy dziwne rzeczy
                //Jest tylko jeden odpowiadacz dla danej usługi

                lock (_lock)
                {
                    for (int i = 0; i < communicators.Count; i++)
                    {
                        if (!communicators[i].Running)
                        {
                            //Może zaimplementować IEquals dla Communicatora?
                            //Wyrzucany jest wyjątek, bo czasem zostanie usunięty
                            communicators[i].Start(new CommandD(Answer), RemoveCommunicator);
                        }
                    }
                }
                //Thread.Sleep(3000);
                //Console.WriteLine("[SRV] Checker running. No communicators: " + communicators.Count);
            }
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
            //dodanie usług serwera
            srv.AddServiceModule("ping", new PingService());
            srv.AddServiceModule("chat", new ChatService());
            srv.AddServiceModule("ftp", new FTPService("FTP"));
            srv.AddServiceModule("conf", new ConfigurationService());
            srv.Start();
            //dodaje i włącza nasłuchiwacza
            srv.AddListener(new TCPListener(new IPEndPoint(IPAddress.Any, 12345)));
            srv.AddListener(new UDPListener(new IPEndPoint(IPAddress.Any, 12346)));
            srv.AddListener(new RS232Listener(new SerialPort("COM2", 9600, Parity.None, 8, StopBits.One)));
            //srv.AddListener(new FTPListener());
            srv.WaitForStop();
            srv.Stop();
        }
    }
}