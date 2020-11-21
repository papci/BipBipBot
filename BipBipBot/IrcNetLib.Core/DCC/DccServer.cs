using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace IrcNetLib.Core.DCC
{
    class DccServer
    {
        #region private properties
        private string FilePath;
        private string Nick;
        private long Size;
        private string MyAddress;
        private int Port;
        private TcpListener DccTcpServer;
        private FileStream SendFile;
        private int TotalSended;
        private const int MAX_BUFFER_LEN = 8192;

        #endregion

        #region Constructor
        public DccServer(string _FilePath, string _Nick)
        {
            FilePath = _FilePath;
            Nick = _Nick;
            FileInfo Fi = new FileInfo(FilePath);
            SendFile = new FileStream(FilePath, FileMode.Open);
            Size = Fi.Length;
            Port = 3636;
        }


        #endregion
        public async Task StartAsync()
        {

            DccTcpServer = new TcpListener(new IPEndPoint(IPAddress.Any, Port));

            TcpClient client = await DccTcpServer.AcceptTcpClientAsync();
            await HandleClient(client);
        }

        private async Task HandleClient(TcpClient tc)
        {

            NetworkStream ns = tc.GetStream();
            byte[] fBuff = new byte[MAX_BUFFER_LEN];
            int MaxReadSize;
            MaxReadSize = Size > 2048 ? 2048 : System.Convert.ToInt32(Size);

            SendFile.Read(fBuff, 0, MaxReadSize);
            SendFile.Seek(MaxReadSize, SeekOrigin.Current);
            PacketState State = new PacketState(MaxReadSize, ns, MaxReadSize);
            await ns.WriteAsync(fBuff, 0, MaxReadSize);
            await HandleWrite(State);
        
        }
        private async Task HandleWrite(PacketState packetState)
        {
            PacketState State = packetState;
        
            State.Buffer = null;
            if (State.SendNextTime)
            {
              //  State.BoundedStream.BeginRead(State.Buffer, 0, 4, new AsyncCallback(Reader), State);
               var rt =  State.BoundedStream.ReadAsync(State.Buffer, 0, 4);
                TotalSended += State.Buffer.Length;
                await rt;
                await HandleRead(packetState);

            }
            else
                State.BoundedStream.Dispose();
        }
        private async Task HandleRead(PacketState packetState)
        {
            PacketState State = packetState;
           
            byte[] fBuff = new byte[4096];
            FileInfo fi = new FileInfo(FilePath);
            //TODO: Control sended size
            if (fi.Length > TotalSended + 2048)
            {
                SendFile.Read(fBuff, 0, 2048);
                State.SendNextTime = false;
            }
            else
            {
                SendFile.Read(fBuff, 0, System.Convert.ToInt32(fi.Length - TotalSended));
                State.SendNextTime = true;
            }
          //  State.BoundedStream.BeginWrite(fBuff, 0, fBuff.Length, new AsyncCallback(Writer), State);
            await State.BoundedStream.WriteAsync(fBuff, 0, fBuff.Length);
            await HandleWrite(State);

        }
        #region Delegates
        //private delegate void EventReadyHandler(object sender, Event
        #endregion

    }
}
