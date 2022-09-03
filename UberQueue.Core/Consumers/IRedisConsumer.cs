namespace UberQueue.Core.Consumers
{
    public interface IRedisConsumer<T> : IConsumer
    {
        Task Consume(T message);
    }
}
