using System.Threading.Tasks;
using IrcNetLib.Core;
using Xunit;

namespace Bip.Tests.Lib
{
    public class ClientTests
    {
        [Fact]
        public async Task TestConnection()
        {

            IrcClientSettings settings = new IrcClientSettings()
            {
                Nickname = "YapTestBot",
                AltNickname = "TestBotYap",
                HostName = "librenet.europnet.org",
                Port = 6667,
                ServerPassword = string.Empty

            };

            IrcClient ircClient = new IrcClient(settings);
            await ircClient.ConnectAsync();
            Assert.True(ircClient.Connected);
        
            await ircClient.JoinChannelAsync("#lataix");
            await ircClient.SendMessageToAsync("#lataix", "Bip Bip !");
            await ircClient.LeaveChannelAsync("#lataix");
            await ircClient.DisconnectAsync();
            Assert.True(!ircClient.Connected);
        
        }
    }
}