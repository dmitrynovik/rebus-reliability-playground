using System;
using System.Threading;
using Rebus.Bus;

namespace RabbitMQRebusException
{
    class Program
    {
        static void Main(string[] args)
        {
            TimeSpan heartbeat;
            if (args.Length > 0 && int.TryParse(args[0], out var seconds))
            {
                heartbeat = TimeSpan.FromSeconds(seconds);
            }
            else
            {
                heartbeat = TimeSpan.FromSeconds(10);
            }

            Console.WriteLine($"Running from {Environment.CurrentDirectory}");
            Console.WriteLine("Starting Rebus RabbitMQ client ...");

            try
            {
                using (var client = new RebusRabbitMQClient(heartbeat))
                {
                    client.Start();

                    //Console.WriteLine("Connected. Publishing test message");
                    var cancellationTokenSource = new CancellationTokenSource();
                    StartPublishing(client.Bus, "test-message", cancellationTokenSource.Token);

                    Console.WriteLine("Press any key to exit");
                    Console.Read();
                    cancellationTokenSource.Cancel();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void StartPublishing(IBus bus, object message, CancellationToken cancellation)
        {
            var publisher = new RebusReliableBus(bus);
            while (!cancellation.IsCancellationRequested)
            {
                publisher.Publish(new Message(message));
                Thread.Sleep(1000);
            }
        }
    }
}
