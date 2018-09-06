using System;
using System.Threading.Tasks;

namespace RabbitMQRebusException
{
    public class Subscriber
    {
        private int _seq;

        public async Task Subscribe(RebusRabbitMQClient client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            await client.Bus.Subscribe<Message>();

            client.Adapter.Handle<Message>(m =>
            {
                Console.WriteLine($"Received: {m.Payload}");

                if (int.TryParse(m.Payload?.ToString(), out var value))
                {
                    if (value - _seq > 1)
                        WriteError($"Message loss detected. Expected: ${_seq + 1}, received: {value}, lost: {value - _seq - 1}");

                    _seq = value;
                }
                else
                {
                    WriteError("Could not parse counter");
                }

                return Task.CompletedTask;
            });
        }

        private static void WriteError(string message)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = consoleColor;
        }
    }
}