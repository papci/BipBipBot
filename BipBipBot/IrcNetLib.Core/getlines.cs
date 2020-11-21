namespace IrcNetLib.Core
{
    public class getlines
    {
        public string[] meslignes;

        public getlines(string msg)
        {
            meslignes = new string[256];
            this.Fillit(msg);
        }

        private void Fillit(string msg)
        {
            if (msg.Contains("\r\n"))
            {
                this.meslignes = msg.Split((char)13);
                for (int i = 0; i < meslignes.Length; i++)
                {
                    while (meslignes[i].StartsWith("\0"))
                        meslignes[i] = meslignes[i].TrimStart("\0".ToCharArray());


                    while (meslignes[i].StartsWith("\r"))
                        meslignes[i] = meslignes[i].TrimStart("\r".ToCharArray());

                    while (meslignes[i].StartsWith("\n"))
                        meslignes[i] = meslignes[i].TrimStart("\n".ToCharArray());

                    meslignes[i] = meslignes[i].TrimStart((char)10);
                }
            }
        }
    }
}
