namespace IrcNetLib.Core
{
    public record IrcClientSettings
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string ServerPassword { get; set; }
        public string Nickname { get; set; }
        public string AltNickname { get; set; }
    }
}