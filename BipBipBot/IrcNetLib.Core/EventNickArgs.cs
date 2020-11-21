using System;

namespace IrcNetLib.Core
{
    
    /// <summary>
    /// Provide Extra Argument for Nick Event
    /// </summary>
    public class EventNickArgs : EventArgs
    {
        public string OldMask;
        public string NewMask;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="_om">Old Mask</param>
        /// <param name="_nm">New Mask</param>
        public EventNickArgs(string _om, string _nm)
        {
            OldMask = _om;
            NewMask = _nm;
        }
        
    }
}
