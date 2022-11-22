using System;

namespace Network.Model
{
    class Message
    {
        public Guid GameId { get; private set; }
        public int SenderId { get; private set; }
        public DateTime Date { get; private set; }
        public string Text { get; private set; }

        public Message(Guid gameId, int senderId, DateTime date, string text)
        {
            GameId = gameId;
            SenderId = senderId;
            Date = date;
            Text = text;
        }

        public bool IsSeverMessage()
        {
            return SenderId == -1;
        }
    }
}
