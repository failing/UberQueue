using UberQueue.Core.Consumers;

namespace UberQueue.Core.Queue.Wrappers
{
    public interface IRedisConsumerWrapper<T> : IRedisWrapper
    {
        IRedisConsumer<T> Consumer { get; }
    }
}