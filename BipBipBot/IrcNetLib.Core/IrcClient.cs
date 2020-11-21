using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace IrcNetLib.Core
{
    public class IrcClient
    {
        public IrcSocketClient SocketClient { get; protected set; }
        public bool Connected { get;  protected set; }

        public IrcClientSettings ClientSettings { get; protected set; }

        private Task ReadTask;
        public IrcClient(IrcClientSettings settings)
        {
            ClientSettings = settings;
            SocketClient = new IrcSocketClient(settings.HostName, settings.Port);
            SocketClient.ClientConnected += SocketClientOnClientConnected;
            SocketClient.ClientDisconnected += SocketClientOnClientDisconnected;
            SocketClient.OnReceived += SocketClientOnOnReceived;
        }

        private void  SocketClientOnOnReceived(object sender, SocketsArgs e)
        {
            Debug.Write(e.EventText);
             ResponseHandler.HandleAsync(e, this).GetAwaiter().GetResult();

        }

        private void SocketClientOnClientDisconnected(object sender, SocketsArgs e)
        {
            Connected = false;
        }

        private void SocketClientOnClientConnected(object sender, ConnectedArgs e)
        {
        
            Connected = true;
        }
        
        

        public async Task ConnectAsync()
        {
            await SocketClient.ConnectAsync(ClientSettings.ServerPassword);
            ReadTask = Task.Run(async () => await SocketClient.ReceiveTextAsync());
     
            await SocketClient.SendIrcCmdAsync("NICK", "", " " + ClientSettings.Nickname);
            await Task.Delay(1000);
            await SocketClient.SendIrcCmdAsync("USER", "",
                "localname " + SocketClient.GetLocalAdress() + " " + SocketClient.Hostname +
                " :YaP! IRC ");
            
            await Task.Delay(1000);
        }

        public async Task DisconnectAsync()
        {
            AssertConnected();
            await SocketClient.DisconnectAsync();
        }

        public async Task JoinChannelAsync(string channelName)
        {
            AssertConnected();
            await SocketClient.SendIrcCmdAsync("JOIN", "", channelName);
        }

        public async Task LeaveChannelAsync(string channelName)
        {
            AssertConnected();
            await SocketClient.SendIrcCmdAsync("PART", "", channelName);
        }

        public async Task SendMessageToAsync(string destination, string message)
        {
            AssertConnected();
            await SocketClient.SendIrcCmdAsync("PRIVMSG", destination, message);
            
        }
        public async Task SendPongAsync(string pingData)
        {
            AssertConnected();
            await SocketClient.SendIrcCmdAsync("PONG", "", pingData);
        }
        private void AssertConnected()
        {
            if (!Connected)
                throw new IRCexeption("Not Connected");
        }


        
    }
}