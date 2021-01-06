using System.Collections.Generic;
using System.Net;

namespace BipBipBot
{
    public class BotConfiguration
    {
        public List<ServerConfiguration> ServerConfigurations { get; set; }
  
    }

    public class ServerConfiguration
    {
        public string BotName { get; set; }
        public string AltName { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public List<ChannelConfiguration> ChannelConfigurations { get; set; }
        public List<string> Extensions { get; set; }
        public EndPoint GetServerEndpoint()
        {
            return new DnsEndPoint(Host, Port);
        }
    }

    public class ChannelConfiguration
    {
        public string ChannelName { get; set; }

    }
}