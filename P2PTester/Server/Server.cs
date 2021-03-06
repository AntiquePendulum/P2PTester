﻿using System;
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
            Console.CancelKeyPress += (obj, args) => Dispose();
        }

        public async Task StartAsync()
        {
            var endPoint = IPEndPoint.Parse("0.0.0.0:42151");
            _tcpListener = new TcpListener(endPoint);
            _tcpListener.Start();
            await ConnectionWaitAsync();
        }

        async Task ConnectionWaitAsync()
        {
            Console.WriteLine("ConnectionWaitAsync");
            if (_tcpListener == null) return;
                var tcs = new TaskCompletionSource<int>();
                await using (token.Register(tcs.SetCanceled))
                {
                    while (!token.IsCancellationRequested)
                    {
                        var tcpTask = _tcpListener.AcceptTcpClientAsync();
                        if ((await Task.WhenAny(tcpTask, tcs.Task)).IsCanceled) break;

                        try
                        {
                            using var client = tcpTask.Result;
                            var message = await JsonSerializer.DeserializeAsync<Message>(client.GetStream());
                            Console.WriteLine($"Name : {message.Name} / Amount : {message.Amount}");
                        }
                        catch (SocketException)
                        {
                            
                        }
                    }
                }
            _tcpListener.Stop();
        }
    }
}