using System;
using System.IO.Ports;

namespace Server
{
    class RS232Listener : IListener
    {
        private SerialPort _serialPort;

        public RS232Listener(SerialPort serialPort) => _serialPort = serialPort;

        //Nasłuchiwacz odpowiada jedynie za utworzenie odpowiadacza
        public void Start(CommunicatorD onConnect) => onConnect(new RS232Communicator(_serialPort));

        public void Stop() => _serialPort.Close();
    }

    //Trzeba użyć jeszcze onDisconnect
    class RS232Communicator : ICommunicator
    {
        private CommandD _onCommand;
        private CommunicatorD _onDisconnect;
        private SerialPort _serialPort;

        public RS232Communicator(SerialPort serialPort)
        {
            _serialPort = serialPort;
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(Answer);
        }

        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            _onCommand = onCommand;
            _onDisconnect = onDisconnect;
            _serialPort.Open();
        }

        private void Answer(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                string command = sp.ReadExisting();
                Console.WriteLine("[RS232] " + command);
                sp.Write(_onCommand(command));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Stop() => _serialPort.Close();
    }
}
