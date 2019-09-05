namespace Messenger.Server.Services
{
    using System;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class WebSocketService
    {
        private WebSocket _currentSocket;

        public void OnConnected(WebSocket socket)
        {
            if (_currentSocket != null)
                return;

            _currentSocket = socket;
        }

        public async Task OnDisconnected(WebSocket socket)
        {
            await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            _currentSocket = null;
        }

        public async Task ReceiveAsync(WebSocketReceiveResult result, byte[] buffer)
        {
            var message = Encoding.ASCII.GetString(buffer, 0, result.Count);
            await SendMessageAsync(message);
        }

        public async Task SendMessageAsync(string message)
        {
            if (_currentSocket.State != WebSocketState.Open)
                return;

            var bytes = new ArraySegment<byte>(Encoding.ASCII.GetBytes(message), 0, message.Length);
            await _currentSocket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}