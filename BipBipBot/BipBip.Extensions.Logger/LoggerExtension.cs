using System;
using System.Threading.Tasks;
using BipBip.Extensions.Abstractions;
using Microsoft.Extensions.Logging;

namespace BipBip.Extensions.Logger
{
    public class LoggerExtension : IBipExtension
    {
        private ILogger<LoggerExtension> _logger;

        public LoggerExtension(ILogger<LoggerExtension> logger)
        {
            _logger = logger;
        }

        public Task OnChannelJoinedAsync(string channel, string userName)
        {
            return Task.CompletedTask;
        }

        public Task OnChannelPartedAsync(string channel, string userName)
        {
            return Task.CompletedTask;
        }

        public Task OnMessageReceivedAsync(string channel, string user, string message)
        {
            _logger.Log(LogLevel.Information, $"[{DateTime.UtcNow:t}] {user}➡️{channel}: {message}");
            return Task.CompletedTask;
        }

        public Task OnChannelModdedAsync(string channel, ChannelChange change)
        {
            return Task.CompletedTask;
        }

        public Task OnConnectedAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnDisconnectedAsync()
        {
            return Task.CompletedTask;
        }
    }
}