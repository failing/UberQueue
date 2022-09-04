namespace UberQueue.Core.Queue.Interfaces
{
    public interface IRedisConsumerStreamWrapper<T> : IRedisConsumerWrapper<T>, IRedisConsumerStreamerWrapper
    {
    }
}
