namespace UberQueue.Core.Queue
{
    public interface IRedisRouter
    {
        Task Route<T>(T message);
    }
}