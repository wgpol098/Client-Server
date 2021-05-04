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
        void Start(CommandD onCommand, CommunicatorD onDisconnect);
        void Stop();
    }

    interface IListener
    {
        void Start(CommunicatorD onConnect);
        void Stop();
    }
}
