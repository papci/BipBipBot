using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;


namespace IrcNetLib.Core.Ping
{
    public class PingEngine
    {
        public string Hostname { get; protected set; }
        public int HostPort { get; protected set; }
        private int interval;
        private Task workingTask;
        private CancellationTokenSource taskCancellationToken;
        public PingEngine(string host, int port, int pingInterval)
        {
            this.interval = pingInterval;
            taskCancellationToken = new CancellationTokenSource();
            this.Hostname = host;
            this.HostPort = port;
        }

        public void Start()
        {
            if (taskCancellationToken.IsCancellationRequested)
                Stop();
            workingTask = new Task(async () =>
            {
      
                
                while ((!taskCancellationToken.IsCancellationRequested))
                {
                    try
                    {
                        var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                        {
                            Blocking = true
                        };

                        var stopwatch = new Stopwatch();

                        // Measure the Connect call only
                        stopwatch.Start();
                        sock.Connect(new DnsEndPoint(Hostname, HostPort));
                        stopwatch.Stop();

                        double t = stopwatch.Elapsed.TotalMilliseconds;
                        Debug.WriteLine("{0:0.00}ms", t);


                        sock.Dispose();
                        OnPingDone(new PingEngineArgs() {RoundTripTime = Convert.ToInt64(t)});
                        await Task.Delay(TimeSpan.FromSeconds(30));
                    }
                    catch (Exception)
                    {

                        await Task.Delay(TimeSpan.FromSeconds(30));
                    }

                }
               
            });

            workingTask.Start();
        }

        public void Stop()
        {
            if (taskCancellationToken.Token.CanBeCanceled)
            {
               taskCancellationToken.Cancel();
            }
        }

        public event PingEventHandler PingDone;

        protected virtual void OnPingDone(PingEngineArgs args)
        {
            PingDone?.Invoke(this, args);
        }
    }

    public delegate void PingEventHandler(object sender, PingEngineArgs args);

    public class PingEngineArgs : EventArgs
    {
        public long RoundTripTime { get; set; }
    }
}
