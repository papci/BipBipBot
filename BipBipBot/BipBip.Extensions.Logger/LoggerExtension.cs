using System;
using System.Threading.Tasks;
using BipBip.Extensions.Abstractions;
using Microsoft.Extensions.Logging;

namespace BipBip.Extensions.Logger
{
    public class LoggerExtension : IBipExtension
    {
        private ILogger<LoggerExtension> _logger;
        
        public LoggerExtension()
        {
            throw new NotImplementedException();
        }
        public Task OnChannelJoinedAsync(string channel, string userName)
        {
            throw new NotImplementedException();
        }

        public Task OnChannelPartedAsync(string channel, string userName)
        {
            throw new NotImplementedException();
        }

        public Task OnMessageReceivedAsync(string channel, string user, string message)
        {
            throw new NotImplementedException();
        }

        public Task OnChannelModdedAsync(string channel, ChannelChange change)
        {
            throw new NotImplementedException();
        }
    }
}