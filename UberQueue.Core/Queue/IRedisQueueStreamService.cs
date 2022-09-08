namespace UberQueue.Core.Queue
{
    public interface IRedisQueueStreamService : IRedisQueue
    {
        Task Dequeue(string sortedSetKey, int batchSize = 500);
    }
}
