using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utf8Json;

namespace P2PTester.Server
{
    public class Server : IDisposable
    {
        private TcpListener _tcpListener;
        public CancellationTokenSource tokenSource;
        public CancellationToken token;

        public Server()
        {
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
        }

        public async Task StartAsync()
        {
            var endPoint = IPEndPoint.Parse("0.0.0.0:42151");
            _tcpListener = new TcpListener(endPoint);
            _tcpListener.Start();
            await Task.Run(async () => await ConnectionWaitAsync(), token);
        }

        async Task ConnectionWaitAsync()
        {
            Console.WriteLine("ConnectionWaitAsync");
            if(_tcpListener == null) return;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    using var client = await _tcpListener.AcceptTcpClientAsync();
                    var message = await JsonSerializer.DeserializeAsync<Message>(client.GetStream());
                    Console.WriteLine($"Name : {message.Name} / Amount : {message.Amount}");
                }
            }
            finally{_tcpListener.Stop();}
        }

        public void Dispose()
        {
            if (tokenSource == null) return;
            tokenSource.Cancel();
            tokenSource.Dispose();
            tokenSource = null;
        }
    }
}
