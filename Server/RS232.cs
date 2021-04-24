using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class RS232Listener : IListener
    {
        public void Start(CommunicatorD onConnect)
        {
            SerialPort serialPort = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
            Console.WriteLine("Incoming data:");
            serialPort.Open();
            while(true)
            {
                Console.WriteLine(serialPort.ReadExisting());
            }
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }

    class RS232Communicator : ICommunicator
    {
        public string ServiceName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool Running => throw new NotImplementedException();

        public Dictionary<string, IServiceModule> services { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}