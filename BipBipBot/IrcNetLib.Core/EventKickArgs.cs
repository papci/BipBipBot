using System;

namespace IrcNetLib.Core
{
    
    /// <summary>
    /// Provide Extra Arguments for Kick Event
    /// </summary>
    public class EventKickArgs : EventArgs
    {
        //public string _channel, string _nick, string _msg
        public string Channel;
        public string Nick;
        public string Message;

        /// <summary>
        /// Primary constructor
        /// </summary>
        /// <param name="_ch">Channel</param>
        /// <param name="_ni">Nick</param>
        /// <param name="_msg">Kick Message</param>
        public EventKickArgs(string _ch, string _ni, string _msg)
        {
            Channel = _ch;
            Nick = _ni;
            Message = _msg;
        }
    }
}
