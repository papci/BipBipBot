using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IrcNetLib.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BipBipBot
{
    public class Startup
    {
        protected BotConfiguration _botConfiguration;
        protected ConcurrentDictionary<ServerConfiguration, IrcClient> Clients;
        protected ILogger<Startup> _logger;
        protected CancellationTokenSource _cancellationTokenSource;
        public Startup(IConfigurationRoot configuration, IServiceCollection serviceProvider)
        {
            this.Clients = new ConcurrentDictionary<ServerConfiguration, IrcClient>();
            this._cancellationTokenSource = new CancellationTokenSource();
            _botConfiguration = configuration.Get<BotConfiguration>();
      
        }

        public async Task RunAsync()
        {
            foreach (ServerConfiguration serverConfiguration in _botConfiguration.ServerConfigurations)
            {
                var settings = new IrcClientSettings()
                {
                    Nickname = serverConfiguration.BotName,
                    AltNickname = serverConfiguration.AltName,
                    HostName = serverConfiguration.Host,
                    Port = serverConfiguration.Port
                };
                var client =new IrcClient(settings);
                
                await client.ConnectAsync();
                client.SocketClient.OnReceived += SocketClientOnOnReceived;
                foreach (ChannelConfiguration channelConfiguration in serverConfiguration.ChannelConfigurations)
                {
                    await client.JoinChannelAsync(channelConfiguration.ChannelName);
                }
                Clients.TryAdd(serverConfiguration, client);
            }

            while (!_cancellationTokenSource.IsCancellationRequested)
            {

                TryCancel();
                await Task.Delay(1000);
            }
        }

        private void SocketClientOnOnReceived(object sender, SocketsArgs e)
        {
            _logger.Log(LogLevel.Trace, e.EventText);
        }

        private void TryCancel()
        {
            bool shouldCancel = Clients.All(pair => !pair.Value.Connected);
            if (shouldCancel)
            {
                _cancellationTokenSource.Cancel();
            }
        }
    }
}