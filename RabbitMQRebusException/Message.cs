using System;
using System.Collections.Generic;
using MassTransit;

namespace RabbitMQRebusException
{
    public class Message
    {
        public Message(object payload, Dictionary<string, string> headers = null) : this(NewId.NextGuid(), payload, headers) {  }

        public Message(Guid id, object payload, Dictionary<string, string> headers = null)
        {
            Id = id;
            Payload = payload;
            Headers = headers;
        }

        public Guid Id { get; }
        public object Payload { get;  }
        public Dictionary<string, string> Headers { get; }
    }
}