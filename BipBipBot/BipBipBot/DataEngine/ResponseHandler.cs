using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IrcDotNet;

namespace BipBipBot.DataEngine
{
    public static class ResponseHandler
    {
        public static async Task HandleAsync(string eventText, ExtendedIrcClient ircClient)
        {
            var split = eventText.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
          
            if (split.Skip(1).FirstOrDefault() == "PRIVMSG")
            {
                await HandlePrivmsgAsync(eventText, ircClient);
            }

          
         
        }

        private static async Task HandlePrivmsgAsync(string data, ExtendedIrcClient socketClient)
        {
            PrivateMessageEvent privateMessageEvent = new PrivateMessageEvent(data);
            socketClient.OnIrcEvent.OnNext(privateMessageEvent);
            socketClient.OnPrivateMessage.OnNext(privateMessageEvent);
        }

    

        private static async Task HandleVersionAsync()
        {
        }

        private static Regex PingRegex => new Regex("PING\\s(.*?)");
    }
}