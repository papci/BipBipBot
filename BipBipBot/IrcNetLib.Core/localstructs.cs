namespace IrcNetLib.Core
{
    public enum MsgType
    {
        Ping,
        Auth,
        ChTopic,
        LstUsers,
        Motd,
        WelServ,
        NickInUse,
        Notice,
        Mode,
        Join,
        Privmsg,
        Quit,
        Kick,
        Part,
        Nick
    }
    public struct unmsg
    {
        public string dest;
        public string from;
        public MsgType type;
        public string msg;
    }

    public struct achan
    {
        public string nom;
        public string[] users;
        public string[] opers;
        public string[] voiced;
        //public System.Windows.Forms.Form fenetre;
        //public System.Windows.Forms.ToolStripItem bouton;
    }
}
