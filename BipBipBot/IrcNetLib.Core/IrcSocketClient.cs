
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace IrcNetLib.Core
{

    /// <summary>
    /// This Class represent an IRC Connector.
    /// </summary>
    public sealed class IrcSocketClient 
    {

        /*
         *  Public Members */

        public string Hostname;
        private int Port;
        private string Receivedlast;
        private string Sendlast;
        private ConnectionState State;
        private SemaphoreSlim _semaphoreSlim;
        public bool Connected
        {
            get => (State == ConnectionState.Connected);
            private set
            {
                if (value)
                    State = ConnectionState.Connected;
            }

        }

        /*------------------------------------------*/
        /*   private members  */

        private static string _crlf = "\r\n";
        private string _localcharset;
        /// <summary>
        /// Return Basic informations
        /// </summary>
        /// <returns>string  hostname:port</returns>
        public override string ToString()
        {
            return Hostname + ":" + Port.ToString();
        }

        /// <summary>
        /// get or set the used Charset.
        /// </summary>
        public string GlobalCharset
        {
            get => _localcharset;
            set
            {
                switch (value)
                {
                    case "iso-8859-1":
                    case "UTF-8":
                    case "iso-8859-2":
                    case "iso-8859-3":
                    case "iso-8859-4":
                    case "iso-8859-5":
                    case "iso-8859-6":
                    case "iso-8859-7":
                    case "iso-8859-8":
                    case "iso-8859-9":
                    case "iso-8859-10":
                        _localcharset = value;
                        break;
                    default:
                        _localcharset = "iso-8859-1";
                        break;
                }

            }
        }
        private TcpClient _tcpClient;
        private NetworkStream _stream;


        /// <summary>
        /// Defaut constructor
        /// </summary>
        /// <param name="host">hostname, IP format</param>
        /// <param name="port">port used</param>
        public IrcSocketClient(string host, int port)
        {
            Hostname = host;
            Port = port;
            this._tcpClient = new TcpClient();
            Connected = false;
            _localcharset = "iso-8859-1";
            this._semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        /// <summary>
        /// return hostname
        /// </summary>
        /// <returns></returns>
        public string GetLocalAdress()
        {
            //TODO
            return "localhost";
        }

        /// <summary>
        /// Connect to irc server
        /// </summary>
        /// <returns>the connexion was successful or not</returns>
        public async Task ConnectAsync(string password = "")
        {
            try
            {
                await ThreadedConnect(password);

            }

            catch (SocketException)
            {


                throw new IRCexeption("Socket connection Error");
            }
            catch (ArgumentException)
            {

            }
        }
        private async Task ThreadedConnect(string password = "")
        {
            SocketsArgs e = new SocketsArgs("connecting");
            OnConnecting(e);
            await _tcpClient.ConnectAsync(Hostname, Port);

            try
            {
                ConnectedArgs f = new ConnectedArgs("ClientConnected", password);
                _stream = this._tcpClient.GetStream();
                Connected = true;
                OnClientConnected(f);



            }
            catch (Exception ex)
            {
                SocketsArgs fArgs = new SocketsArgs("Socket Error");
                Connected = false;
                OnClientUnableConnect(fArgs);

            }
        }



        public async Task<bool> DisconnectAsync()
        {
            if (!Connected) return false;
            try
            {
                await SendIrcCmdAsync("QUIT", "", "YaP!");
                _tcpClient.Dispose();
                ClientDisconnected?.Invoke(this, new SocketsArgs("Client Requested"));
                Connected = false;
                return true;
            }
            catch (SocketException)
            {
                return false;
            }


            return false;
        }


        public async Task SendIrcCmdAsync(string cmd, string dest, string msg)
        {
            await this._semaphoreSlim.WaitAsync();
        
            if (Connected)
            {
                string tosend = " ";
                dest = dest.TrimStart((char)37);
                dest = dest.TrimStart((char)33);
                dest = dest.TrimStart((char)64);
                dest = dest.TrimStart((char)43);
                switch (cmd)
                {


                    case "LIST":
                        tosend = cmd + _crlf;
                        break;
                    case "TOPIC":
                        tosend = cmd + " " + dest + " :" + msg + _crlf;
                        break;
                    case "KICK":
                        string[] args = msg.Split("\t".ToCharArray());
                        tosend = cmd + " " + dest + " " + args[0] + " " + args[1] + _crlf;
                        break;
                    case "PART":
                        tosend = cmd + " " + msg + _crlf;
                        break;
                    case "NOTICE":
                        tosend = cmd + " " + dest + " :" + msg + _crlf;
                        break;
                    case "PONG":
                        tosend = cmd + " " + msg + _crlf;
                        break;
                    case "QUIT":
                        //special case here, we must send the quit command immediatly

                        tosend = cmd + " :" + msg + _crlf;
                        byte[] aenvoyer = System.Text.Encoding.GetEncoding("iso-8859-1").GetBytes(tosend);
                        this.Sendlast = tosend;
                        string[] cutted = tosend.Split((char)32);
                        if ((_tcpClient.Connected) && (_stream.CanWrite))
                            _stream.Write(aenvoyer, 0, aenvoyer.Length);
                        tosend = string.Empty;
                        DataSend?.Invoke(this, new EventSendArgs(cmd, string.Empty, msg));
                        break;
                    case "NICK":
                        tosend = cmd + " " + msg + _crlf;
                        break;
                    case "MODE":
                        tosend = cmd + " " + dest + " " + msg + _crlf;
                        break;
                    case "USER":
                        tosend = cmd + " " + msg + _crlf;
                        break;
                    case "PRIVMSG":
                        tosend = cmd + " " + dest + " :" + msg + _crlf;
                        break;
                    case "JOIN":
                        string tojoin;
                        if (!(msg.StartsWith("#")))
                            tojoin = "#" + msg;
                        else
                            tojoin = msg;
                        tosend = cmd + " " + tojoin + _crlf;
                        break;
                    default:
                        tosend = cmd + " " + msg;
                        break;
                }

                if (tosend != string.Empty)
                {
                    await SendRawStringAsync(tosend);
                    OnDataSend(new EventSendArgs(cmd, dest, msg));
                }
            }

            _semaphoreSlim.Release();

        }

        public async Task SendRawStringAsync(string textTosend)
        {
            if (!(textTosend.EndsWith(_crlf)))
                textTosend += _crlf;
            try
            {

                byte[] aenvoyer = System.Text.Encoding.GetEncoding("iso-8859-1").GetBytes(textTosend);
                this.Sendlast = textTosend;
                string[] cutted = textTosend.Split((char)32);
                if (Connected)
                {
                    //_stream.BeginWrite(aenvoyer, 0, aenvoyer.Length, new AsyncCallback(DebutEnvoyer), null);
                    await _stream.WriteAsync(aenvoyer, 0, aenvoyer.Length);


                    this._stream = this._tcpClient.GetStream();

                }
                EventArgs f = new EventArgs();
            }
            catch (System.IndexOutOfRangeException ex)
            { }
        }

        /// <summary>
        /// This Method intiate the read loop
        /// </summary>
        public async Task ReceiveTextAsync()
        {
            try
            {
                if ((_stream.CanRead) && (_tcpClient.Connected))
                {
                    this._stream = this._tcpClient.GetStream();
                    IrcBuffer mBuff = new IrcBuffer();
                    int nbRead = await _stream.ReadAsync(mBuff.BytesBuff, 0, mBuff.BytesBuff.Length);
                    mBuff.BytesRead = nbRead;
                    if (nbRead > 0)
                    {
                       await SignalReceiveAsync(mBuff);
                    }
                    else
                    {
                        await CloseReceiveAsync();
                    }

                }
            }
            catch (NullReferenceException)
            { }
        }


        private async Task SignalReceiveAsync(IrcBuffer buffer)
        {
         

            // while (buffer.StrBuff.EndsWith("\0"))
            //    buffer.StrBuff = buffer.StrBuff.TrimEnd("\0".ToCharArray());

            
            string message;

            if (buffer.StrBuff.EndsWith("\r\n"))
            {
                message = Receivedlast + buffer.StrBuff;
                Receivedlast = string.Empty;
                SocketsArgs e = new SocketsArgs(message);
                OnReceived?.Invoke(this, e);


            }
            else
            {
                Receivedlast += buffer.StrBuff;

            }
            await ReceiveTextAsync();
        }

        private async Task CloseReceiveAsync()
        {
            try
            {
                this._tcpClient.Dispose();
                this.Receivedlast = "Fermeture socket distante";
                OnClientDisconnected(new SocketsArgs("Fermeture socket distante"));
            }
            catch (SocketException)
            {
                this.Receivedlast = "Socket Error!";
            }
        }




       #region Events
        public delegate void SocketReceivedHandler(object sender, SocketsArgs e);
        public event SocketReceivedHandler OnReceived;

        public delegate void SocketSendHandler(object sender, EventSendArgs e);
        public event SocketSendHandler DataSend;

        public delegate void SocketUnableConnectHandler(object sender, SocketsArgs e);
        public event SocketUnableConnectHandler ClientUnableConnect;

        public delegate void SocketConnectingHandler(object sender, SocketsArgs e);
        public event SocketConnectingHandler ClientConnecting;

        public delegate void SocketConnectedHandler(object sender, ConnectedArgs e);
        public event SocketConnectedHandler ClientConnected;

        public delegate void SocketDisconnectingHandler(object sender, SocketsArgs e);
        public event SocketDisconnectingHandler OnDisconnecting;

        public delegate void SocketDisconnectedHandler(object sender, SocketsArgs e);
        public event SocketDisconnectedHandler ClientDisconnected;

        public delegate void SocketExGeneratedHandler(object sender, SocketsArgs e);
        public event SocketExGeneratedHandler OnExSocket;

        public delegate void OnJoinHandler(object sender, EventJoinArgs e);
        public event OnJoinHandler OnJoin;


        private void OnOnDisconnecting(SocketsArgs e)
        {
            OnDisconnecting?.Invoke(this, e);
        }

        private void OnConnecting(SocketsArgs e)
        {
            ClientConnecting?.Invoke(this, e);
        }

        private void OnClientConnected(ConnectedArgs e)
        {
            ClientConnected?.Invoke(this, e);
        }

        private void OnClientUnableConnect(SocketsArgs e)
        {
            ClientUnableConnect?.Invoke(this, e);
        }

        private void OnDataSend(EventSendArgs e)
        {
            DataSend?.Invoke(this, e);
        }

        private void OnClientDisconnected(SocketsArgs e)
        {
            ClientDisconnected?.Invoke(this, e);
        }
        
         #endregion 
    }
}
