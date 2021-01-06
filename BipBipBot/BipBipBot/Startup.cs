using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BipBipBot.DataEngine;
using IrcDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BipBipBot
{
    public class Startup
    {
        protected BotConfiguration _botConfiguration;
        protected ConcurrentDictionary<ServerConfiguration, IrcClient> Clients;
        protected CancellationTokenSource _cancellationTokenSource;
        protected ServiceProvider ServiceProvider;

        public Startup(IConfigurationRoot configuration, IServiceCollection serviceProvider)
        {
            this.Clients = new ConcurrentDictionary<ServerConfiguration, IrcClient>();
            this._cancellationTokenSource = new CancellationTokenSource();
            _botConfiguration = configuration.Get<BotConfiguration>();
            ServiceProvider = serviceProvider.BuildServiceProvider();
        }

        public async Task RunAsync()
        {
            foreach (ServerConfiguration serverConfiguration in _botConfiguration.ServerConfigurations)
            {
                var client = new ExtendedIrcClient(serverConfiguration);

                client.Connected += ClientOnConnected;
                client.Registered += ClientOnRegistered;
                client.RawMessageReceived += ClientOnRawMessageReceived;
              
                client.OnIrcEvent.Subscribe(OnNextIrcEvent);
                client.OnPrivateMessage.Subscribe(OnNextPrivateMessageEvent);
                client.ConnectAsync();

                Clients.TryAdd(serverConfiguration, client);
            }

            await Task.Delay(1000);

            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                await KeepChannelsAsync();
                TryCancel();

                await Task.Delay(1000);
            }
        }

        private void OnNextPrivateMessageEvent(PrivateMessageEvent obj)
        {
           Log(Json(obj), LogLevel.Information );
           
        }

        private string Json(object privateMessageEvent)
        {
            return JsonSerializer.Serialize(privateMessageEvent, privateMessageEvent.GetType());
        }


        private void OnNextIrcEvent(IrcEvent obj)
        {
            Log(Json(obj), LogLevel.Information );
        }

        private void ClientOnRawMessageReceived(object? sender, IrcRawMessageEventArgs e)
        {
  
        }

        private void ClientOnRegistered(object? sender, EventArgs e)
        {
            var client = sender as ExtendedIrcClient;
            client?.JoinChannels();
            Debug.WriteLine("Client registered");
        }

        private void ClientOnConnected(object? sender, EventArgs e)
        {
            Debug.WriteLine("Client connected");
        }

        private async Task KeepChannelsAsync()
        {
        }


        private void TryCancel()
        {
            bool shouldCancel = Clients.All(pair => !pair.Value.IsConnected);
            if (shouldCancel)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        private void Log(string message, LogLevel level)
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var logger = scope.ServiceProvider.GetService<ILogger<Startup>>();
                logger.Log(level, message);
            }
        }
    }
}