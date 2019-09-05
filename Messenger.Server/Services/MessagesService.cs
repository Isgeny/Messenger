namespace Messenger.Server.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Messenger.DAL.Models;
    using Messenger.DAL.Services;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using Npgsql;

    public class MessagesService : IMessagesService
    {
        private const string GET_MESSAGES_SQL =
            "SELECT number, senddate, text FROM public.\"messages\" WHERE @StartDate <= senddate AND senddate <= @EndDate";

        private const string SEND_MESSAGE_SQL =
            "INSERT INTO public.\"messages\" (number, senddate, text) VALUES (@Number, @SendDate, @Text)";

        private readonly string _connectionString;

        private readonly ILogger<MessagesService> _logger;

        private readonly WebSocketService _socketService;

        public MessagesService(IConfiguration configuration, ILogger<MessagesService> logger,
            WebSocketService socketService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
            _socketService = socketService;
        }

        public IEnumerable<Message> GetMessagesByDateRange(DateTime startDate, DateTime endDate)
        {
            var messages = new List<Message>();

            _logger.LogInformation($"Отправка SQL-запроса на получение сообщений от {startDate} до {endDate}");

            using (var connection = new NpgsqlConnection(_connectionString))
            using (var command = new NpgsqlCommand())
            {
                connection.Open();

                command.Connection = connection;
                command.CommandText = GET_MESSAGES_SQL;
                command.Parameters.AddWithValue("StartDate", startDate);
                command.Parameters.AddWithValue("EndDate", endDate);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var message = new Message
                        {
                            Number = reader.GetInt32(0),
                            SendDate = reader.GetDateTime(1),
                            Text = reader.GetString(2)
                        };

                        messages.Add(message);
                    }
                }
            }

            _logger.LogInformation($"От {startDate} до {endDate} найдено сообщений: {messages.Count}");

            return messages;
        }

        public void SendMessage(Message message)
        {
            if (!IsMessageValid(message))
                return;

            message.SendDate = DateTime.Now;

            _logger.LogInformation($"Отправка SQL-запроса на добавление сообщения с номером {message.Number}");

            using (var connection = new NpgsqlConnection(_connectionString))
            using (var command = new NpgsqlCommand())
            {
                connection.Open();

                command.Connection = connection;
                command.CommandText = SEND_MESSAGE_SQL;
                command.Parameters.AddWithValue("Number", message.Number);
                command.Parameters.AddWithValue("SendDate", message.SendDate);
                command.Parameters.AddWithValue("Text", message.Text);

                command.ExecuteNonQuery();
            }

            _logger.LogInformation($"Сообщение с номером {message.Number} успешно добавлено в БД");

            var messageString = JsonConvert.SerializeObject(message);
            Task.Run(() => _socketService.SendMessageAsync(messageString));
        }

        private static bool IsMessageValid(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("Сообщение не может иметь значение null.");

            if (message.Text.Length > 128)
                throw new ArgumentException("Сообщение не может иметь длину более 128 символов.");

            return true;
        }
    }
}