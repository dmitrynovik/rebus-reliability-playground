using System;
using System.Collections.Generic;
using MassTransit;

namespace RabbitMQRebusException
{
    public class Message
    {
        public Message() {  }

        public Message(object payload, string topic = null, Dictionary<string, string> headers = null) : this(NewId.NextGuid(), payload, topic, headers) {  }

        public Message(Guid id, object payload, string topic = null, Dictionary<string, string> headers = null)
        {
            Id = id;
            Payload = payload;
            Headers = headers;
            Topic = topic;
        }

        public Guid Id { get; set; }
        public object Payload { get; set; }
        public string Topic { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}