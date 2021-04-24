using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    delegate string CommandD(string command);
    delegate void CommunicatorD(ICommunicator commander);
    interface IServiceModule
    {
        string AnswerCommand(string command);
    }

    interface ICommunicator
    {
        bool Running { get; }
        void Start(CommandD onCommand, CommunicatorD onDisconnect);
        void Stop();
    }

    interface IListener
    {
        void Start(CommunicatorD onConnect);
        void Stop();
    }
}