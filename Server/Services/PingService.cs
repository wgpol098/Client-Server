using Common;

namespace Server.Services
{
    class PingService : IServiceModule
    {
        public string AnswerCommand(string command) => Ping.Pong(command);
    }
}
