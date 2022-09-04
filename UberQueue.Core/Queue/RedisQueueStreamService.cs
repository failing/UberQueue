using Newtonsoft.Json;
using StackExchange.Redis;
using UberQueue.Core.Jobs;
using UberQueue.Core.Queue.Interfaces;

namespace UberQueue.Core.Queue
{
    public class RedisQueueStreamService : IRedisQueueService
    {
        private readonly LuaScript _luaScript = LuaScript.Prepare(@"
                            local setValues = redis.call(""ZRANGE"", @sortedSetKey, ""-inf"", @utcMsTimestamp, ""BYSCORE"", ""LIMIT"", 0, @batch)
	                        if (next(setValues) ~= nil)
                                then
                                    redis.call(""ZREM"", @sortedSetKey, unpack(setValues))
                                    for v in unpack(setValues) do
                                        redis.call(""XADD"", @streamName, *, ""object"", @value)
                                    end
                                end
	                        return setValues");

        private readonly IDatabase _redisDatabase;

        public RedisQueueStreamService(IDatabase redisDatabase)
        {
            _redisDatabase = redisDatabase;
        }

        public async Task Enqueue(string sortedSetKey, JobData data, DateTimeOffset timeToExecute)
        {
            string serializedJob = JsonConvert.SerializeObject(data);

            await _redisDatabase.SortedSetAddAsync(sortedSetKey, serializedJob, timeToExecute.ToUnixTimeMilliseconds());
        }

        public async Task<RedisValue[]?> Dequeue(string sortedSetKey, int batchSize = 500)
        {
            double utcLimitTS = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            object redisParams = new { sortedSetKey = new RedisKey(sortedSetKey), utcMsTimestamp = utcLimitTS, batch = batchSize };

            RedisValue[]? sortedSetValues = (RedisValue[]?)await _redisDatabase.ScriptEvaluateAsync(_luaScript, redisParams);

            return sortedSetValues;
        }

        public Task Process(RedisValue[]? values)
        {
            return Task.CompletedTask;
        }
    }
}
