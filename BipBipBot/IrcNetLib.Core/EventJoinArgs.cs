using System;

namespace IrcNetLib.Core
{
    public class EventJoinArgs : EventArgs
    {
        public string Channel;
        public string Mask;

        public EventJoinArgs(string _ch, string _ma)
        {
            Channel = _ch;
            Mask = _ma;

        }
    }
}
