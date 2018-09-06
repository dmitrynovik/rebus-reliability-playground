using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using NLog;

namespace RabbitMQRebusException.ReliabilityExtensions
{
    public class ReliablePublisher : IDisposable
    {
        private readonly Func<Message, Task> _publisher;
        private readonly ConcurrentDictionary<Guid, Message> _messageQueue = new ConcurrentDictionary<Guid, Message>();
        private readonly Timer _timer;
        private bool _isResending;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
         
        public ReliablePublisher(Func<Message, Task> publisher, double interval = 5000)
        {
            _publisher = publisher;
            _timer = new Timer(interval) { AutoReset = true, Enabled = false };

            _timer.Elapsed += (sender, args) =>
            {
                if (_isResending)
                    return;

                _isResending = true;
                try
                {
                    PublishQueued();

                    if (_messageQueue.IsEmpty)
                        _timer.Stop();
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
                finally
                {
                    _isResending = false;
                }
            };
        }

        public void PublishQueued()
        {
            foreach (var id in _messageQueue.Keys.ToArray())
            {
                if (_messageQueue.TryRemove(id, out var message))
                    Publish(message, true);
            }
        }

        public Task Publish(Message message, bool retry = false)
        {
            try
            {
                if (retry)
                    Logger.Warn($"Re-sending {message.Id}");

                return _publisher(message);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                _messageQueue[message.Id] = message;
                _timer.Start();
                return Task.CompletedTask;
            }
        }

        public virtual void Dispose()
        {
            _timer?.Dispose();
            GC.SuppressFinalize(this);
        }

        ~ReliablePublisher()
        {
            Dispose();
        }
    }
}
