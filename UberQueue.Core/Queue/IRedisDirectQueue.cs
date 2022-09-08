using StackExchange.Redis;

namespace UberQueue.Core.Queue
{
    public interface IRedisDirectQueue : IRedisQueue
    {
        Task<RedisValue[]?> Dequeue(string sortedSetKey, int batchSize = 500);
        Task Process(RedisValue[]? values);
    }
}

