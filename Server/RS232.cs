﻿using System;
using System.IO.Ports;

namespace Server
{
    class RS232Listener : IListener
    {
        private SerialPort _serialPort;

        public RS232Listener(SerialPort serialPort) => _serialPort = serialPort;

        //TODO: Tutaj można dodać jeszcze więcej opcji
        public RS232Listener(string config)
        {
            if(config != null)
            {
                string[] tmp = config.Split();
                if(tmp.Length == 1) _serialPort = new SerialPort(tmp[0]);
                else _serialPort = new SerialPort(tmp[0], int.Parse(tmp[1]));
            }
        }

        public void Start(CommunicatorD onConnect)
        {
            if(_serialPort != null) onConnect(new RS232Communicator(_serialPort));
        }

        public void Stop()
        {
            if(_serialPort != null) _serialPort.Close();
        }
    }

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
                string command = sp.ReadLine();
                Console.WriteLine("[RS232] " + command);
                sp.Write(_onCommand(command));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                _onDisconnect(this);
            }
        }

        public void Stop() => _serialPort.Close();
    }
}
