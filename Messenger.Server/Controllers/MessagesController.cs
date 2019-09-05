namespace Messenger.Server.Controllers
{
    using System;
    using System.Collections.Generic;

    using Messenger.DAL.Models;
    using Messenger.DAL.Services;

    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/messages")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessagesService _messagesService;

        public MessagesController(IMessagesService messagesService)
        {
            _messagesService = messagesService;
        }

        [HttpGet]
        [Route("getByDateRange/{startDate}/{endDate}")]
        public ActionResult<IEnumerable<Message>> GetMessagesByDateRange(DateTime startDate, DateTime endDate)
        {
            var messages = _messagesService.GetMessagesByDateRange(startDate, endDate);
            return new ActionResult<IEnumerable<Message>>(messages);
        }

        [HttpPost]
        [Route("send")]
        public void SendMessage([FromBody] Message message)
        {
            _messagesService.SendMessage(message);
        }
    }
}