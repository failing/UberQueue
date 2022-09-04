namespace UberQueue.Core.Queue
{
    public interface IRedisWrapperManager
    {
        Task StartListening();
    }
}