using UberQueue.Core.Consumers;

namespace UberQueue.Core.Queue.Interfaces
{
    public interface IRedisConsumerWrapper<T>
    {
        IRedisConsumer<T> Consumer { get; }
    }
}