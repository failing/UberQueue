namespace UberQueue.Core.Consumers
{
    /// <summary>
    /// Redis consumer interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRedisConsumer<T> : IConsumer
    {
        Task Consume(T message);
    }
}
