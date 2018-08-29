using System;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Routing.TypeBased;

namespace RabbitMQRebusException
{
    public class RebusRabbitMQClient : IDisposable
    {
        private readonly TimeSpan _timeout;
        private readonly string _connectionString;
        private readonly string _queueName;

        public BuiltinHandlerActivator Adapter { get; set; }
        public IBus Bus { get; set; }

        public RebusRabbitMQClient(TimeSpan timeout, string connectionString = "amqp://guest:guest@localhost", string queueName = "test-rebus")
        {
            _timeout = timeout;
            _connectionString = connectionString;
            _queueName = queueName;
        }

        public void Start()
        {
            Adapter = new BuiltinHandlerActivator();

            var configuration = Configure.With(Adapter)
                .Transport(t => t.UseRabbitMq(_connectionString, _queueName)
                    .CustomizeConnectionFactory(cf =>
                    {
                        cf.RequestedHeartbeat = (ushort)_timeout.TotalSeconds;
                        return cf;
                    }))
                .Routing(r => r.TypeBased());

            Bus = configuration.Start();

        }

        public void Dispose()
        {
            Bus?.Dispose();
            Adapter?.Dispose();
        }
    }
}
