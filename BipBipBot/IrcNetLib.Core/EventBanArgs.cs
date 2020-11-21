using System;

namespace IrcNetLib.Core
{
    public class EventBanArgs : EventArgs
    {
        public string BanMask;
        public string Channel;

        public EventBanArgs(string _bm, string _ch)
        {
            BanMask = _bm;
            Channel = _ch;
            
        }
    }
}
