using System;

namespace IrcNetLib.Core
{
    public class EventPingArgs : EventArgs 
    {
        public string PingMessage;
        public EventPingArgs(string _pm)
        {
            PingMessage = _pm;
        }
    }
}
