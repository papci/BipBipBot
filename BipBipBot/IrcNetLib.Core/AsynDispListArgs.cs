using System;

namespace IrcNetLib.Core
{
    public class AsyncDispListArgs : EventArgs
    {
        public string destination;
        public string msg;

        public AsyncDispListArgs(string dest, string m)
        {
            destination = dest;
            msg = m;

        }
    }
}
