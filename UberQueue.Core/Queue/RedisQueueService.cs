using Newtonsoft.Json;
using StackExchange.Redis;
using UberQueue.Core.Jobs;

namespace UberQueue.Core.Queue
{
    public class RedisDirectQueueService : IRedisDirectQueue
    {
        private readonly LuaScript _luaScript = LuaScript.Prepare(@"
                            local setValues = redis.call(""ZRANGE"", @sortedSetKey, ""-inf"", @utcMsTimestamp, ""BYSCORE"", ""LIMIT"", 0, @batch)
	                        if (next(setValues) ~= nil) then redis.call(""ZREM"", @sortedSetKey, unpack(setValues)) end
	                        return setValues");

        private readonly IDatabase _redisDatabase;
        private readonly IRedisRouter _redisRouter;

        public RedisDirectQueueService(IDatabase redisDatabase, IRedisRouter redisRouter)
        {
            _redisDatabase = redisDatabase;
            _redisRouter = redisRouter;
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

        public async Task Process(RedisValue[]? values)
        {
            _ = Parallel.ForEach(values!, async result =>
            {
                JobObject? jobData = JsonConvert.DeserializeObject<JobObject>(result!);

                if (jobData != null)
                {
                    var jobObjectPayload = jobData.Payload;

                    await _redisRouter.Route(jobObjectPayload);
                }
            });
        }
    }
}
