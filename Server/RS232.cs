using System;
using System.IO.Ports;
using System.Threading;

namespace Server
{
    class RS232Listener : IListener
    {
        private SerialPort _serialPort;

        public RS232Listener(SerialPort serialPort) => _serialPort = serialPort;

        public void Start(CommunicatorD onConnect)
        {
            Console.WriteLine("[RS232] Incoming data:");
            _serialPort.Open();
            while(true)
            {
                Console.WriteLine(_serialPort.ReadExisting());
                Thread.Sleep(3000);
            }
        }

        public void Stop() => _serialPort.Close();
    }

    class RS232Communicator : ICommunicator
    {
        private CommandD _onCommand;
        private CommunicatorD _onDisconnect;
        private SerialPort _serialPort;
        private SerialPort _serialPort1;
        public bool Running => throw new NotImplementedException();

        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            //TODO: Później zostanie to prawidłowo zaimplementowane - na razie do testów
            _serialPort = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
            _serialPort1 = new SerialPort("COM2", 9600, Parity.None, 8, StopBits.One);
            _onCommand = onCommand;
            _onDisconnect = onDisconnect;
        }

        public void AnswerTask()
        {
            while(_serialPort.IsOpen)
            {
                string message = _serialPort1.ReadExisting();
                string answer = _onCommand(message);
                _serialPort.Write(answer);
            }
        }

        public void Stop()
        {
            _serialPort.Close();
            _serialPort1.Close();
        }
    }
}