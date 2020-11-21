using System;

namespace IrcNetLib.Core
{
    public class SendArgs : EventArgs
    {
        public string mytext;
        public string destination;
        public SendArgs(string dest, string msg)
        {
            mytext = msg;
            destination = dest;
        }
    }
}
