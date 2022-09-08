using UberQueue.Core.Jobs;

namespace UberQueue.Core.Queue
{
    public interface IRedisQueueManager
    {
        Task Dequeue(int batchSize = 500);
        Task Enqueue(JobData data, DateTimeOffset timeToExecute);
    }
}