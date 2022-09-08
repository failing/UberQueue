using UberQueue.Core.Jobs;

namespace UberQueue.Core.Queue
{
    public class RedisQueueManager : IRedisQueueManager
    {
        private readonly IRedisDirectQueue _directRedisQueue;
        private readonly IRedisQueueStreamService _redisStreamQueue;
        private readonly IRedisTypeCache _redisTypeCache;
        private readonly IServiceProvider _serviceProvider;
        private static string _streamKey = "stream_jobs";
        private static string _directKey = "jobs";

        public RedisQueueManager(IRedisQueueStreamService redisStreamQueue, IRedisDirectQueue redisDirectQueue, IRedisTypeCache typeCache, IServiceProvider serviceProvider)
        {
            _redisStreamQueue = redisStreamQueue;
            _directRedisQueue = redisDirectQueue;
            _redisTypeCache = typeCache;
            _serviceProvider = serviceProvider;
        }

        public async Task Dequeue(int batchSize = 500)
        {
            await _redisStreamQueue.Dequeue(_streamKey, batchSize);
            var results = await _directRedisQueue.Dequeue(_directKey, batchSize);
            await _directRedisQueue.Process(results);
        }

        public async Task Enqueue(JobData data, DateTimeOffset timeToExecute)
        {
            if (data is JobObject obj)
            {
                Type serviceType = _redisTypeCache.GetServiceForType(obj.PayloadType);
                IRedisQueue redisQueue = (IRedisQueue)_serviceProvider.GetService(serviceType);
                Type queueType = redisQueue.GetType();

                if (typeof(RedisQueueStreamService) == queueType)
                {
                    await redisQueue!.Enqueue(_streamKey, data, timeToExecute);
                }
                else
                {
                    await redisQueue!.Enqueue(_directKey, data, timeToExecute);
                }
            }
        }
    }
}
