using System.Text;

namespace IrcNetLib.Core
{
    class IrcBuffer 
    {

        public byte[] BytesBuff;
        private int bsize;
        public int BytesRead;

        public IrcBuffer(byte[] _bytetab)
        {
            bsize = 512;
            BytesBuff = new byte[bsize];
            int inboucle;

            if (_bytetab.Length < bsize)
                inboucle = _bytetab.Length;
            else
                inboucle = bsize;

            for (int i = 0; i < inboucle; i++)
                BytesBuff[i] = _bytetab[i];
        }
        public IrcBuffer(string _strbuff)
        {
            bsize = 512;
            BytesBuff = new byte[bsize];
            StrBuff = _strbuff;
        }

        public IrcBuffer()
        {
            bsize = 512;
            BytesBuff = new byte[bsize];
        }
        public IrcBuffer(int _size)
        {
            bsize = _size;
            BytesBuff = new byte[bsize];
        }
        public int Size => BytesBuff.Length;

        public string StrBuff
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (BytesRead == 0)
                {
                    int i = 0;
                    bool found = false;
                    while (!(found))
                    {
                        if (BytesBuff[i] == '\0')
                            found = true;
                        i += 1;
                    }
                    BytesRead = i-1;

                }
                sb.Append(Encoding.GetEncoding("iso-8859-1").GetString(BytesBuff,0,BytesRead));
                return sb.ToString();
            
            }
            set
            {
                char[] chartab = new char[value.Length];
                chartab = value.ToCharArray();
                int maxsize;
                if (chartab.Length > bsize)
                    maxsize = bsize;
                else
                    maxsize = chartab.Length;

                byte[] tbytemp = new byte[bsize];
                for (int i = 0; i < maxsize; i++)
                    tbytemp[i] = (byte)chartab[i];
                BytesBuff = tbytemp;
            }
        }

        public override string ToString()
        {
            return StrBuff;
        }

        public int GetByteArraySize()
        {
            return BytesBuff.Length;
        }



    }
}
