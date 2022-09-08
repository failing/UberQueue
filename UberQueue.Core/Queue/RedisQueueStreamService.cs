using Newtonsoft.Json;
using StackExchange.Redis;
using UberQueue.Core.Jobs;

namespace UberQueue.Core.Queue
{
    public class RedisQueueStreamService : IRedisQueueStreamService
    {
        private readonly LuaScript _luaScript = LuaScript.Prepare(@"
                            local setValues = redis.call(""ZRANGE"", @sortedSetKey, ""-inf"", @utcMsTimestamp, ""BYSCORE"", ""LIMIT"", 0, @batch)
                            if (next(setValues) ~= nil)
                                then
                                    redis.call(""ZREM"", @sortedSetKey, unpack(setValues))
                                    for i = 1, #setValues do
                                        local a = cjson.decode(setValues[i]);
                                        redis.call(""XADD"", a.payload_type_short, ""*"", ""object"", setValues[i])
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

        public async Task Dequeue(string sortedSetKey, int batchSize = 500)
        {
            double utcLimitTS = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            object redisParams = new { sortedSetKey = new RedisKey(sortedSetKey), utcMsTimestamp = utcLimitTS, batch = batchSize };

            await _redisDatabase.ScriptEvaluateAsync(_luaScript, redisParams);
        }
    }
}
