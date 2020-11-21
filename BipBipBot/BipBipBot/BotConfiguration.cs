using System.Collections.Generic;

namespace BipBipBot
{
    public class BotConfiguration
    {
        public List<ServerConfiguration> ChannelConfigurations { get; set; }
    }

    public class ServerConfiguration
    {
        public string BotName { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public List<ChannelConfiguration> ChannelConfigurations { get; set; }
    }

    public class ChannelConfiguration
    {
        public string ChannelName { get; set; }
        public List<string> ScriptsName { get; set; }
    }
}