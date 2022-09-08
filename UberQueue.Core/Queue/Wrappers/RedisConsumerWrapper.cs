using UberQueue.Core.Consumers;

namespace UberQueue.Core.Queue.Wrappers
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
