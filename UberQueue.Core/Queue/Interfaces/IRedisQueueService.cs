using StackExchange.Redis;
using UberQueue.Core.Jobs;

namespace UberQueue.Core.Queue.Interfaces
{
    public interface IRedisQueueService
    {
        Task<RedisValue[]?> Dequeue(string sortedSetKey, int batchSize = 500);
        Task Enqueue(string sortedSetKey, JobData data, DateTimeOffset timeToExecute);
    }
}