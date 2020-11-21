using System;

namespace IrcNetLib.Core
{
    public class IRCexeption : Exception
    {
        public const string stLib = "IRC Engine has thrown a new exception: ";
        public string pLib;
        public IRCexeption(string lib)
        {
            pLib = lib;
        }
        public override string ToString()
        {
            return stLib + pLib;
        }
        
    }
}
