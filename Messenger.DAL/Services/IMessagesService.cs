namespace Messenger.DAL.Services
{
    using System;
    using System.Collections.Generic;

    using Messenger.DAL.Models;

    public interface IMessagesService
    {
        IEnumerable<Message> GetMessagesByDateRange(DateTime startDate, DateTime endDate);

        void SendMessage(Message message);
    }
}