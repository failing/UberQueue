using UberQueue.Core.Consumers;
using UberQueue.Core.Queue.Interfaces;

namespace UberQueue.Core.Queue
{
    public class RedisConsumerWrapper<T> : IRedisConsumerWrapper<T>
    {
        public IRedisConsumer<T> Consumer { get; private set; }

        public RedisConsumerWrapper(IRedisConsumer<T> consumer)
        {
            Consumer = consumer;
        }
    }
}
