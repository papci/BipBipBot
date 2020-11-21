using System.Net.Sockets;

namespace IrcNetLib.Core.DCC
{
    class PacketState
    {
        public readonly int PacketSize;
        public readonly NetworkStream BoundedStream;
        public byte[] Buffer;
        public bool SendNextTime = true;
        public long LastOffset;

        public PacketState(int _ps, NetworkStream _stream, long _ofs)
        {
            PacketSize = _ps;
            BoundedStream = _stream;
            LastOffset = _ofs;
        }
    }
}
