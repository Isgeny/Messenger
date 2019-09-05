namespace Messenger.DAL.Models
{
    using System;

    public class Message
    {
        public int Number { get; set; }

        public DateTime SendDate { get; set; }

        public string Text { get; set; }
    }
}