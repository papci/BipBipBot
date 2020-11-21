using System;

namespace IrcNetLib.Core
{
    public class EventPrivmsgArgs : EventArgs
    {
        public string From;
        public string Dest;
        public string Message;

        public EventPrivmsgArgs(string _fr, string _de, string _me)
        {
            From = _fr;
            Dest = _de;
            Message = _me;
        }
    }
}
