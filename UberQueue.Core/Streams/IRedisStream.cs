namespace UberQueue.Core.Streams
{
    public interface IRedisStream<T>
    {
        Task Consume(Func<T, Task> consumeFunc, CancellationToken cancellation);
        Task Publish(T[] message);
        Task Publish(T message);
    }
}