using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace anko2unity
{
    class TCPSender
    {
        TcpListener listener;
        List<TcpClient> tcpClients;
        Encoding encoding;
        public bool IsRunning { get; private set; }
        TaskFactory taskFactory = new TaskFactory();
        private Task currentTask;
        private CancellationTokenSource currentCancellationTokenSource;
        public int Port { get; private set; }

        public TCPSender()
        {
            tcpClients = new List<TcpClient>();
            encoding = System.Text.Encoding.UTF8;
        }


        public CancellationTokenSource Start(int port)
        {
            Disconnect();
            Port = port;
            this.listener = new TcpListener(System.Net.IPAddress.Parse("127.0.0.1"), port);
            this.listener.Start();
            var tokenSource = new CancellationTokenSource();
            currentCancellationTokenSource = tokenSource;
            currentTask = taskFactory.StartNew(async () =>
            {
                while (true)
                {
                    tokenSource.Token.ThrowIfCancellationRequested();
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    client.NoDelay = true;
                    tcpClients.Add(client);
                    Debug.Print("接続{0}", ((IPEndPoint)(client.Client.RemoteEndPoint)).Address);
                }
            }, tokenSource.Token);
            IsRunning = true;
            return currentCancellationTokenSource;
        }

        public void Stop()
        {
            Disconnect();
        }

        private void Disconnect()
        {
            IsRunning = false;
            listener?.Stop();
            currentCancellationTokenSource?.Cancel();
            currentTask?.Wait();
        }

        /// <summary>
        /// 全てのクライアントにMessageをブロードキャストする
        /// </summary>
        /// <param name="client_data"></param>
        /// <param name="message"></param>
        public void SendToAll(string message)
        {
            //接続が切れているクライアントは除去する
            var closedClients = tcpClients.Where(x => !x.Connected).ToList();
            closedClients.ForEach(x => tcpClients.Remove(x));

            foreach (var client in tcpClients)
            {
                //接続が切れていないか再確認
                if (!client.Connected) { continue; }
                var ns = client.GetStream();
                byte[] message_byte = encoding.GetBytes(message);
                try
                {
                    do
                    {
                        ns.WriteAsync(message_byte, 0, message_byte.Length);

                    } while (ns.DataAvailable);
                }
                catch (Exception e)
                {
                    Debug.Print(e.Message);
                    if (!client.Connected)
                    {
                        client.Close();
                    }
                }
            }
        }
    }
}
