using System;

namespace IrcNetLib.Core.DCC
{
    public class EventProgressArgs : EventArgs
    {
        public readonly float Percent;
        public readonly float BytePerSec;
        public EventProgressArgs(float _percent,float _bp)
        {
            Percent = _percent;
            BytePerSec = _bp;
        }
        public override string ToString()
        {
            return Percent.ToString() + "%";
        }
    }
}
