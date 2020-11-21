using System;

namespace IrcNetLib.Core
{
    public class EventModeArgs : EventArgs 
    {
        public string Channel;
        public string Mask;

        public EventModeArgs(string _ch, string _ma)
        {
            Channel = _ch;
            Mask = _ma;
        }
    }
}
