using System;

namespace IrcNetLib.Core
{
    public class SocketsArgs : EventArgs
    {
        public SocketsArgs(string theEventText)
        {
            EventText = theEventText ?? "\0";
        }

        public string EventText { get; } = null;
    }

    public class ConnectedArgs : SocketsArgs
    {
        public ConnectedArgs(string theEventText, string password) : base(theEventText)
        {
            ServerPassword = password;
        }

        public string ServerPassword { get; protected set; }
    }
}
