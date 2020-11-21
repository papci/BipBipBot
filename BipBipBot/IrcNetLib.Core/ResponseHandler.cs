using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IrcNetLib.Core
{
    public static class ResponseHandler
    {
        public static async Task HandleAsync(SocketsArgs args, IrcClient socketClient)
        {
            var split = args.EventText.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            if (split.FirstOrDefault() == "PING")
            {
                await HandlePingAsync(split.Skip(1).FirstOrDefault(), socketClient);
            }

            if (split.FirstOrDefault() == "PRIVMSG")
            {
                await HandlePrivmsgAsync(split.Skip(1), socketClient);
            }
            
        }

        private static async Task HandlePrivmsgAsync(IEnumerable<string> data, IrcClient socketClient)
        {
            if (data.LastOrDefault() == ":VERSION")
            {
                
            }
        }

        private static async Task HandlePingAsync(string pingData, IrcClient socketClient)
        {
            await socketClient.SendPongAsync(pingData);
        }

        private static async Task HandleVersionAsync()
        {
            
        }

        private static Regex PingRegex => new Regex("PING\\s(.*?)");
    }
}