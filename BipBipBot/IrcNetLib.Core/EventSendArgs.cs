using System;

namespace IrcNetLib.Core
{
    public class EventSendArgs :EventArgs
    {
        private string cmd;
        private string msg;
        private string dest;

        public EventSendArgs(string _cm, string _des, string _ms)
        {
            cmd = _cm;
            dest = _des;
            msg = _ms;
        }

        public string Command
        {
            get
            {
                return cmd;
            }
        }
        public string Message
        {
            get
            {
                return msg;
            }
        }
        public string Destination
        {
            get
            {
                return dest;
            }
        }
    }
}
