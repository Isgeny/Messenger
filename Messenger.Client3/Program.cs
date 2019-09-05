namespace Messenger.Client3
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Messenger.DAL;
    using Messenger.DAL.Models;

    using Newtonsoft.Json;

    internal class Program
    {
        private static async Task Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(5000);
                    Console.Clear();
                    await ShowLastTenMinutesMessages();
                }
                catch(Exception)
                {
                }
            }
        }

        private static async Task<IEnumerable<Message>> GetMessages()
        {
            using (var httpClient = new HttpClient())
            {
                var request = $"{Connections.API}messages/getByDateRange/" +
                              $"{DateTime.Now.AddMinutes(-10):MM/dd/yyyy HH:mm:ss}/" +
                              $"{DateTime.Now:MM/dd/yyyy HH:mm:ss}";

                var response = await httpClient.GetAsync(request);
                var stringResult = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<Message>>(stringResult);
            }
        }

        private static async Task ShowLastTenMinutesMessages()
        {
            var messages = await GetMessages();

            foreach (var message in messages)
            {
                Console.WriteLine($"{message.Number} {message.SendDate} {message.Text}");
            }
        }
    }
}