using System;

namespace IrcNetLib.Core
{
    public class EventQuitArgs : EventArgs
    {
        public string Mask;
        public string Message;
        public EventQuitArgs(string _ma, string _ms)
        {
            Mask = _ma;
            Message = _ms;
        }
    }
}
