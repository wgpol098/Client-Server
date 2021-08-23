using Common;
using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Server
{
    class NETRemotingListener : IListener
    {
        private TcpChannel _tcpChannel;

        public NETRemotingListener(TcpChannel tcpChannel) => _tcpChannel = tcpChannel;
        public NETRemotingListener(string port)
        {
            _tcpChannel = new TcpChannel(int.Parse(port));
        }

        public void Start(CommunicatorD onConnect)
        {
            onConnect(new NETRemotingCommunicator(_tcpChannel));
            Console.WriteLine("[.net remoting] Waiting for clients!");
        }

        public void Stop() { return; }
    }

    class NETRemotingCommunicator : ICommunicator
    {
        private TcpChannel _tcpChannel;
        
        public NETRemotingCommunicator(TcpChannel tcpChannel) => _tcpChannel = tcpChannel;

        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            ChannelServices.RegisterChannel(_tcpChannel,false);
            CommonNetRemoting common = new CommonNetRemoting(new CommonNetRemoting.CommandD(onCommand));
            RemotingServices.Marshal(common, "command");
        }

        public void Stop()
        {
            ChannelServices.UnregisterChannel(_tcpChannel);
        }
    }
}
