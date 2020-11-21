using System;

namespace IrcNetLib.Core
{
    public  class EventInfoServArgs : EventArgs
    {
        public string Message;
        public EventInfoServArgs(string _me)
        {
            Message = _me;
        }

    }
}
