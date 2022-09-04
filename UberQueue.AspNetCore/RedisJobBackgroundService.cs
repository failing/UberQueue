using Microsoft.Extensions.Hosting;
using UberQueue.Core.Queue.Interfaces;

namespace UberQueue.Core.Queue
{
    public class RedisJobBackgroundService : BackgroundService
    {
        private readonly IRedisQueueService _redisQueueService;
        private readonly RedisJobServiceConfig _config;

        public RedisJobBackgroundService(IRedisQueueService queueService, RedisJobServiceConfig config)
        {
            _redisQueueService = queueService;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var results = await _redisQueueService.Dequeue(_config.Key, _config.BatchSize);

                if (results?.Length > 0)
                {
                    await _redisQueueService.Process(results);
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(25));
                }
            }
        }
    }
}
