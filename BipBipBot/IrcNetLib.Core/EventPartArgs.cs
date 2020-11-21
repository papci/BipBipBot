using System;

namespace IrcNetLib.Core
{
    /// <summary>
    /// Provide Extra Argument for Part Event
    /// </summary>
    public class EventPartArgs : EventArgs
    {
        public string Channel;
        public string Mask;
        public string Message;


        /// <summary>
        /// Default contructor
        /// </summary>
        /// <param name="_ch">Current channel</param>
        /// <param name="_ma">User Mask</param>
        /// <param name="_msg">User Message</param>
        public EventPartArgs(string _ch, string _ma, string _msg)
        {
            Channel = _ch;
            Mask = _ma;
            Message = _msg;
        }
    }
}
