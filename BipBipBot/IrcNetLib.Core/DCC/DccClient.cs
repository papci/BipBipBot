using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace IrcNetLib.Core.DCC
{

    public class DccClient
    {
        #region Properties
        //**** Usable properties ****//
        private TcpClient DccSocket;
        private int Port;
        private string IP;
        private long HandShakeSize;
        private string filenameField;
        private FileStream File;
        private uint ReceivedSize;

        private IrcBuffer WorkingBuffer;
        private const int MAX_BUFFER_SIZE = 4194304; //huge buffer
        private const int MAX_WAIT = 30;
        private uint BlockSize = 0;
        private uint[] BlockSizePool = new uint[3];
        private bool BlockSizeKnown = false;
        private int loopcount = 0;
#if DEBUG

        private int shortreadcount;
        private List<string> DbgList;
#endif
        /*****************************/
        private string FileName
        {
            get
            {
                return filenameField;
            }
            set
            {
                if (value.Contains(" "))
                {
                    value.Replace(" ", "_");
                }
                filenameField = value;
            }
        }
        private ManualResetEvent TimeOut;
        private bool ExitByRead;
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_filename"></param>
        /// <param name="_addcomp"></param>
        /// <param name="_portascii"></param>
        /// <param name="_handshakesize"></param>
        public DccClient(string _filename, string _addcomp, string _portascii, string _handshakesize)
        {
            //Buffer Init
            WorkingBuffer = new IrcBuffer(MAX_BUFFER_SIZE);
            //Checking File
            string df = string.Empty;
            int comp = 0;
            FileName = _filename;
            while (FileExists(FileName))
            {
                comp += 1;
                FileName = comp.ToString() + "-" + FileName;
            }
            File = new FileStream(FileName, FileMode.CreateNew);
            IrcBuffer FileBuffer = new IrcBuffer();

            long byteip;
            long.TryParse(_addcomp, out byteip);

            IPAddress cIP = new IPAddress(byteip);
            IP = StrIpReverse(cIP.ToString());

            //file size conversion
            long.TryParse(_handshakesize, out HandShakeSize);

            //and for port, too ... ;)
            int.TryParse(_portascii, out Port);

            this.TimeOut = new ManualResetEvent(false);
            this.ExitByRead = false;
#if DEBUG
            DbgList = new List<string>();
#endif


        }
        #endregion

        #region Private Methods

        #region Async Methods / Deprecated
        private async Task PacketReceived(IrcBuffer buffer, uint readLen)
        {




#if DEBUG
            DbgList.Add("Packet Received: " + readLen.ToString());
            DbgList.Add("Packet ASCII content: " + buffer.StrBuff);
#endif
            if (readLen > 0)
            {
                File.Write(buffer.BytesBuff, 0, (int)readLen);
                if (BlockSize == 0)
                {
                    //Max Delay is xxx ms, so each time we have to wait 105 ms, to check short read
                    int ShortRead = 0;
                    const int MaxDelay = 230;
                    ShortReadStart:
                    int currDelay = 0;

                    while (currDelay < MaxDelay)
                    {
                        if (DccSocket.GetStream().DataAvailable)
                        {
                            IrcBuffer localBuffer = new IrcBuffer(MAX_BUFFER_SIZE);
                            int ShorReadLen = DccSocket.GetStream().Read(localBuffer.BytesBuff, 0, localBuffer.Size);
                            readLen += (uint)ShorReadLen;
                            File.Write(localBuffer.BytesBuff, 0, (int)ShorReadLen);
                            ShortRead++;
#if DEBUG
                            DbgList.Add("Short Read found Len: " + ShorReadLen.ToString());
                            DbgList.Add("Short Read ASCII Content: " + localBuffer.StrBuff);
#endif

                        }
                        TimeOut.WaitOne(MAX_WAIT);
                        currDelay += MAX_WAIT;
                    }


#if DEBUG
                    DbgList.Add("Total Short Read: " + ShortRead.ToString());
                    if (DccSocket.GetStream().DataAvailable)
                        DbgList.Add("Datas still available O_o");
                    DbgList.Add("Total Loop Read: " + readLen.ToString());
#endif
                    if (DccSocket.GetStream().DataAvailable)
                        goto ShortReadStart;
                }
                else
                {
                    while (readLen < BlockSize)
                    {
                        if (DccSocket.GetStream().DataAvailable)
                        {
                            IrcBuffer localBuffer = new IrcBuffer(MAX_BUFFER_SIZE);
                            int ShorReadLen = DccSocket.GetStream().Read(localBuffer.BytesBuff, 0, localBuffer.Size);
                            readLen += (uint)ShorReadLen;
                            File.Write(localBuffer.BytesBuff, 0, (int)ShorReadLen);
                        }
                        TimeOut.WaitOne(5);
                    }
                }

                //Total Received or just for this packet ? O_o
                ReceivedSize += readLen;
                byte[] reply = BitConverter.GetBytes(readLen);

                DccSocket.GetStream().Write(reply, 0,
                                            reply.Length);
                if (BlockSize == 0)
                {
                    if (loopcount < 2)
                    {
                        BlockSizePool[loopcount] = readLen;
                        loopcount++;
                    }
                    else if (loopcount == 2)
                    {
                        BlockSizePool[loopcount] = readLen;
                        //polling BlockSizePool to know the block size;
                        if ((BlockSizePool[0] == BlockSizePool[1]) && (BlockSizePool[0] == BlockSizePool[2]))
                            BlockSize = BlockSizePool[0];
                        else
                        {
                            loopcount = 0;
                        }
                    }
                }
                else
                {
                    loopcount++;
                }




#if DEBUG
                DbgList.Add("reply sent: " + ReceivedSize + " with " + reply.Length + " bytes");

                if (DccSocket.GetStream().DataAvailable)
                    DbgList.Add("Datas still available O_o");
                DbgList.Add("End of loop n°: " + loopcount.ToString());
#endif

                float percent = (ReceivedSize * 100) / HandShakeSize;
                OnProgress(this, new EventProgressArgs(percent, 0));
                WorkingBuffer = new IrcBuffer(MAX_BUFFER_SIZE);

                //this.DccSocket.GetStream().BeginRead(WorkingBuffer.BytesBuff, 0, WorkingBuffer.Size,
                //                                     new AsyncCallback(PacketReceived), WorkingBuffer);
                var rLen = await DccSocket.GetStream().ReadAsync(WorkingBuffer.BytesBuff, 0, WorkingBuffer.Size);
                if (rLen > 0)
                {
                    await PacketReceived(WorkingBuffer, (uint)rLen);
                }

            }
            else
            {
                OnFinished(this, null);
            }


        }

        #endregion
        private string StrIpReverse(string strIp)
        {
            string[] split = strIp.Split(".".ToCharArray());
            Array.Reverse(split);
            string val = string.Empty;
            for (int i = 0; i < split.Length; i++)
                val += (split[i] + ".");
            val = val.TrimEnd(".".ToCharArray());
            return val;

        }

        private bool FileExists(string _filename)
        {
            return System.IO.File.Exists(_filename);



        }
        #endregion

        #region Public Methods
        public async Task StartReceive()
        {
            try
            {
                this.DccSocket = new TcpClient();
                DccSocket.ReceiveBufferSize = MAX_BUFFER_SIZE;
                //DccSocket.SendBufferSize = 4096;
                // this.DccSocket.BeginConnect(IPAddress.Parse(IP), Port, new AsyncCallback(ConnectCallBack), null);
                await this.DccSocket.ConnectAsync(IPAddress.Parse(IP), Port);


                //  DccSocket.EndConnect(ar);
                OnStartReceive();
                bool Complete = false;
                TimeOut.Reset();

                DateTime TimeBegin = DateTime.Now;
                DateTime TimeTick = DateTime.Now;

                if (!(DccSocket.Connected))
                    OnError(this, new EventArgs());
                WorkingBuffer = new IrcBuffer(MAX_BUFFER_SIZE);

                //this.DccSocket.GetStream().BeginRead(WorkingBuffer.BytesBuff, 0, WorkingBuffer.Size,
                //                                                        new AsyncCallback(PacketReceived), WorkingBuffer);

                var stream = DccSocket.GetStream();
                var readResult = await stream.ReadAsync(WorkingBuffer.BytesBuff, 0, WorkingBuffer.Size);
                await PacketReceived(WorkingBuffer, (uint)readResult);
            }
            catch (SocketException)
            {

            }
        }
        #endregion

        #region Delegates
        //For Events
        public delegate void OnStartReceiveHandler(object sender, EventArgs e);
        public delegate void OnProgressHandler(object sender, EventProgressArgs e);
        public delegate void OnFinishedHandler(object sender, EventArgs e);
        public delegate void OnConnectErrHandler(object sender, EventArgs e);
        public delegate void OnFileExistHandler(object sender, EventArgs e);
        #endregion

        #region Events
        public event OnStartReceiveHandler ReceiveStarted;
        public event OnProgressHandler OnProgress;
        public event OnFinishedHandler OnFinished;
        public event OnConnectErrHandler OnError;
        public event OnFileExistHandler OnFileExists;
        #endregion

        protected virtual void OnStartReceive()
        {
            ReceiveStarted?.Invoke(this, EventArgs.Empty);
        }
    }
}
