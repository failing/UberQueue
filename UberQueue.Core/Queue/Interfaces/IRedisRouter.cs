namespace UberQueue.Core.Queue.Interfaces
{
    public interface IRedisRouter
    {
        Task Route<T>(T message);
    }
}