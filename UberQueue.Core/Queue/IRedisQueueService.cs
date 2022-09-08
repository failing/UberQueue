using StackExchange.Redis;
using UberQueue.Core.Jobs;

namespace UberQueue.Core.Queue
{
    public interface IRedisQueue
    {
        Task Enqueue(string sortedSetKey, JobData data, DateTimeOffset timeToExecute);
    }
}