using System;
using System.Collections.Generic;
using System.Threading;
using Rebus.Bus;

namespace RabbitMQRebusException
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Running from {Environment.CurrentDirectory}");
            Console.WriteLine("Starting Rebus RabbitMQ client ...");
            try
            {
                using (var client = new RebusRabbitMQClient(ParseRabbitMQTimeout(args)))
                {
                    client.Start();
                    
                    var sub = new Subscriber();
                    Console.WriteLine("Subscribing to Rebus RabbitMQ client ...");
                    sub.Subscribe(client).Wait();

                    //Console.WriteLine("Connected. Publishing test message");
                    var cancellationTokenSource = new CancellationTokenSource();
                    StartPublishing(client.Bus, cancellationTokenSource.Token);

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

        private static TimeSpan ParseRabbitMQTimeout(IReadOnlyList<string> args) => args.Count > 0 && int.TryParse(args[0], out var seconds) ? 
            TimeSpan.FromSeconds(seconds) : TimeSpan.FromSeconds(10);

        private static void StartPublishing(IBus bus, CancellationToken cancellation)
        {
            ulong counter = 0;
            while (!cancellation.IsCancellationRequested)
            {
                bus.Publish(new Message(++counter));
                Thread.Sleep(1000);
            }
        }
    }
}
