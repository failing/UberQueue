using UberQueue.Core.Consumers;
using UberQueue.Core.Queue.Interfaces;
using UberQueue.Core.Streams;

namespace UberQueue.Core.Queue
{
    public class RedisConsumerStreamWrapper<T> : IRedisConsumerStreamWrapper<T>, IDisposable
    {
        public IRedisConsumer<T> Consumer { get; private set; }

        private readonly IRedisStream<T> _stream;
        private CancellationTokenSource cts = new CancellationTokenSource();

        public RedisConsumerStreamWrapper(IRedisConsumer<T> consumer, IRedisStream<T> stream)
        {
            Consumer = consumer;
            _stream = stream;
        }

        public void Dispose()
        {
            cts.Cancel();
        }

        public async Task Connect()
        {
            await _stream.Consume(Consumer.Consume, cts.Token);
        }
    }
}
