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
    public class Server
    {
        private TcpListener _tcpListener;
        public CancellationTokenSource tokenSource;
        public CancellationToken token;

        public async Task StartAsync()
        {
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
            var endPoint = IPEndPoint.Parse("0.0.0.0:42151");
            _tcpListener = new TcpListener(endPoint);
            _tcpListener.Start();
            var ig = Task.Run(async () => await ConnectionWaitAsync(), token);
        }

        async Task ConnectionWaitAsync()
        {
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
    }
}
