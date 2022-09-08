namespace UberQueue.Core.Queue.Wrappers
{
    public interface IRedisConsumerStreamWrapper<T> : IRedisConsumerWrapper<T>, IRedisConsumerStreamerWrapper
    {
    }
}
