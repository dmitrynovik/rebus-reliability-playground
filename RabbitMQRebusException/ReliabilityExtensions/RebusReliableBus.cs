using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using Rebus.Bus;
using Rebus.Bus.Advanced;

namespace RabbitMQRebusException.ReliabilityExtensions
{
    public class RebusReliableBus : ReliablePublisher, IBus
    {
        private readonly IBus _bus;

        public RebusReliableBus(IBus bus, double interval = 5000)
            : base(message => message.Topic == null ? 
                bus.Publish(message.Payload, message.Headers): 
                bus.Advanced.Topics.Publish(message.Topic, message.Payload, message.Headers), interval)
        {
            _bus = bus;
        }

        public override void Dispose()
        {
            base.Dispose();
            _bus.Dispose();
        }

        public Task SendLocal(object commandMessage, Dictionary<string, string> optionalHeaders = null) => _bus.SendLocal(commandMessage, optionalHeaders);

        public Task Send(object commandMessage, Dictionary<string, string> optionalHeaders = null) => _bus.Send(commandMessage, optionalHeaders);

        public Task DeferLocal(TimeSpan delay, object message, Dictionary<string, string> optionalHeaders = null) => _bus.DeferLocal(delay, message, optionalHeaders);

        public Task Defer(TimeSpan delay, object message, Dictionary<string, string> optionalHeaders = null) => _bus.Defer(delay, message, optionalHeaders);

        public Task Reply(object replyMessage, Dictionary<string, string> optionalHeaders = null) => _bus.Reply(replyMessage, optionalHeaders);

        public Task Subscribe<TEvent>() => _bus.Subscribe<TEvent>();

        public Task Subscribe(Type eventType) => _bus.Subscribe(eventType);

        public Task Unsubscribe<TEvent>() => _bus.Unsubscribe<TEvent>();

        public Task Unsubscribe(Type eventType) => _bus.Unsubscribe(eventType);

        public Task Publish(object payload, Dictionary<string, string> optionalHeaders = null) => base.Publish(new Message(NewId.NextGuid(), payload, null, optionalHeaders));

        public IAdvancedApi Advanced => _bus.Advanced;
    }
}
