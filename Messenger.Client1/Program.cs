namespace Messenger.Client1
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Messenger.DAL;
    using Messenger.DAL.Models;

    using Newtonsoft.Json;

    internal class Program
    {
        private  static readonly Random Random = new Random(DateTime.Now.Millisecond);

        private static void Main(string[] args)
        {
            var counter = 0;
            
            while (true)
            {
                try
                {
                    Thread.Sleep(5000);

                    var message = new Message
                    {
                        Number = ++counter,
                        Text = GetRandomString(128)
                    };

                    Task.Run(() => SendMessage(message));
                }
                catch (Exception)
                {
                }
            }
        }

        public static string GetRandomString(int maxLength)
        {
            const string chars = "abcdefghijklmnoprstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, Random.Next(maxLength + 1))
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        private static async Task SendMessage(Message message)
        {
            using (var httpClient = new HttpClient())
            {
                var request = $"{Connections.API}messages/send";
                var messageString = JsonConvert.SerializeObject(message);
                var jsonContent = new StringContent(messageString, Encoding.UTF8, "application/json");
                await httpClient.PostAsync(request, jsonContent);
            }
        }
    }
}