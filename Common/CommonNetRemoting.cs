using System;

namespace Common
{
    public class CommonNetRemoting : MarshalByRefObject
    {
        public delegate string CommandD(string command);
        private CommandD _onCommand;
        public CommonNetRemoting(CommandD onCommand) => _onCommand = onCommand;
        public string Command(string command)
        {
            if (_onCommand != null) return _onCommand(command);
            return "Error!";
        }
    }
}
