using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utf8Json;

namespace P2PTester.Client
{
    public class Client : IDisposable
    {
        public CancellationTokenSource tokenSource;
        public CancellationToken token;
        private TcpClient _tcpClient;

        public Client()
        {
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
        }

        public async Task ConnectAsync(IPEndPoint endPoint)
        {
            _tcpClient = new TcpClient(AddressFamily.InterNetwork);
            try
            {
                await _tcpClient.ConnectAsync(endPoint.Address, endPoint.Port);
            }
            catch (SocketException)
            {
                var _ = Task.Delay(TimeSpan.FromSeconds(10), token).ContinueWith(_ => ConnectAsync(endPoint), token);
                return;
            } 
        }

        public async Task SendMessageAsync(Message message)
        {
            if(_tcpClient == null) return;
            try
            {
                await JsonSerializer.SerializeAsync(_tcpClient.GetStream(), message);
            }
            finally{}

        }

        public void Dispose()
        {
            _tcpClient?.GetStream().Close();
            _tcpClient?.Close();
        }
    }
}
