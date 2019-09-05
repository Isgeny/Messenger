namespace Messenger.Server.Middlewares
{
    using System;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;

    using Messenger.Server.Services;

    using Microsoft.AspNetCore.Http;

    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly WebSocketService _webSocketService;

        public WebSocketMiddleware(RequestDelegate next, WebSocketService webSocketService)
        {
            _next = next;
            _webSocketService = webSocketService;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                return;

            var socket = await context.WebSockets.AcceptWebSocketAsync();

            _webSocketService.OnConnected(socket);

            await Receive(socket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    await _webSocketService.ReceiveAsync(result, buffer);
                    return;
                }

                if (result.MessageType == WebSocketMessageType.Close)
                    await _webSocketService.OnDisconnected(socket);
            });

            await _next.Invoke(context);
        }

        private static async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                handleMessage(result, buffer);
            }
        }
    }
}