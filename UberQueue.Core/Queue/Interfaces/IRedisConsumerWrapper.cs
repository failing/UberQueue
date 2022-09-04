using UberQueue.Core.Consumers;

namespace UberQueue.Core.Queue.Interfaces
{
    public interface IRedisConsumerWrapper<T> : IRedisWrapper
    {
        IRedisConsumer<T> Consumer { get; }
    }
}