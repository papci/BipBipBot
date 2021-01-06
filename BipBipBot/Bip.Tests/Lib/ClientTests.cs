using System.Collections.Generic;
using System.Threading.Tasks;
using BipBipBot;
using Xunit;

namespace Bip.Tests.Lib
{
    public class ClientTests
    {
        [Fact]
        public async Task TestConnection()
        {
            ServerConfiguration settings = new ServerConfiguration()
            {
                BotName = "YapTestBot",
                AltName = "TestBotYap",
                Host = "librenet.europnet.org",
                Port = 6667,
                ChannelConfigurations = new List<ChannelConfiguration>()
                {
                    new ChannelConfiguration() {ChannelName = "#lataix"}
                }
            };

            ExtendedIrcClient ircClient = new ExtendedIrcClient(settings);
            await ircClient.ConnectAsync();
            Assert.True(ircClient.IsConnected);

            ircClient.JoinChannels();
            await ircClient.SendMessageAsync("#lataix", "Bip Bip !");
     
            await ircClient.DisconnectAsync();
            Assert.True(!ircClient.IsConnected);
        }
    }
}