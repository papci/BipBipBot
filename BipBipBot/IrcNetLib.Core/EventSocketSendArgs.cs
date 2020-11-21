namespace IrcNetLib.Core
{
    class EventSocketSendArgs
    {
        public string Command;
        public string Dest;
        public string Message;

        public EventSocketSendArgs(string _co, string _de, string _msg)
        {
            Command = _co;
            Dest = _de;
            Message = _msg;
        }
    }
}
