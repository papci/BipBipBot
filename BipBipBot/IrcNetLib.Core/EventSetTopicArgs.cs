using System;

namespace IrcNetLib.Core
{
    /// <summary>
    /// Provide extra arguments for "On Set Topic" event.
    /// </summary>
    public class EventSetTopicArgs : EventArgs
    {
        public string Channel;
        public string Topic;
        public string Mask;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="_ch">Current Channel</param>
        /// <param name="_to">New Topic</param>
        /// <param name="_ma">User Mask</param>
        public EventSetTopicArgs(string _ch, string _to, string _ma)
        {
            Channel = _ch;
            Topic = _to;
        }
    }
}
