using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using StackExchange.Redis;
using UberQueue.Core.Jobs;
using UberQueue.Core.Queue.Interfaces;

namespace UberQueue.Core.Queue
{
    public class RedisJobBackgroundService : BackgroundService
    {
        private readonly IRedisQueueService _redisQueueService;
        private readonly IRedisRouter _redisRouter;
        private readonly RedisJobServiceConfig _config;

        public RedisJobBackgroundService(IRedisQueueService queueService, RedisJobServiceConfig config, IRedisRouter redisRouter)
        {
            _redisQueueService = queueService;
            _config = config;
            _redisRouter = redisRouter;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var results = await _redisQueueService.Dequeue(_config.Key, _config.BatchSize);

                if (results?.Length > 0)
                {
                    await ProcessResults(results);
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        }

        private async Task ProcessResults(RedisValue[]? results)
        {
            _ = Parallel.ForEach(results!, async result =>
            {
                JobObject? jobData = JsonConvert.DeserializeObject<JobObject>(result!);

                if (jobData != null)
                {
                    if (jobData is JobObject)
                    {
                        var jobObject = jobData;

                        var jobObjectPayload = jobObject.Payload;

                        await _redisRouter.Route(jobObjectPayload);
                    }
                    else if (jobData is JobObject)
                    {
                        var jobObject = jobData;

                        var jobObjectPayload = jobObject.Payload;

                        await _redisRouter.Route(jobObjectPayload);
                    }
                }
            });
        }
    }
}
