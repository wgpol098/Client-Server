using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
    class ConfigurationService : IServiceModule
    {
        public string AnswerCommand(string command)
        {
            return "ConfigurationService";
        }
    }
}
