using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ConsoleAppFramework;
using Microsoft.Extensions.Hosting;
using P2PTester.Client;

namespace P2PTester
{
    class Program : ConsoleAppBase
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder().RunConsoleAppFrameworkAsync<Program>(args);
        }

        [Command("client")]
        public async Task ClientRunAsync([Option("i")]string endPoint)
        {
            if (string.IsNullOrEmpty(endPoint) || IPEndPoint.TryParse(endPoint, out var ipEndPoint))
            {
                Console.WriteLine("終了");
                return;
            }

            using var client = new Client.Client();
            await client.ConnectAsync(ipEndPoint);
            await client.SendMessageAsync(new Message() {Name = "ABC", Amount = 123});

            Console.ReadLine();
        }

        [Command("client")]
        public async Task ClientRunAsync([Option("a")] string address, [Option("p")] string port) =>
            await ClientRunAsync($"{address}:{port}");

        [Command("server")]
        public async Task ServerRunAsync()
        {
            using var server = new Server.Server();
            await server.StartAsync();

            Console.ReadLine();
        }
    }
}
