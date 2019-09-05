namespace Messenger.Client2
{
    using System;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Messenger.DAL;
    using Messenger.DAL.Models;

    using Newtonsoft.Json;

    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var webSocket = new ClientWebSocket();

            while (true)
            {
                try
                {
                    await webSocket.ConnectAsync(new Uri(Connections.SOCKETS), CancellationToken.None);
                    if (webSocket.State == WebSocketState.Open)
                        break;
                }
                catch (Exception)
                {
                }
            }

            while (true)
            {
                try
                {
                    var buffer = new ArraySegment<byte>(new byte[1024 * 4]);
                    var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                    var response = Encoding.ASCII.GetString(buffer.Array, 0, result.Count);
                    var message = JsonConvert.DeserializeObject<Message>(response);
                    Console.WriteLine($"{message.Number} {message.SendDate} {message.Text}");
                }
                catch (Exception)
                {
                }
            }
        }
    }
}