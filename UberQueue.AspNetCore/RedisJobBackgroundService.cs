using Microsoft.Extensions.Hosting;
using UberQueue.Core.Queue;

namespace UberQueue.AspNetCore
{
    public class RedisJobBackgroundService : BackgroundService
    {
        private readonly IRedisQueueManager _redisQueueService;

        public RedisJobBackgroundService(IRedisQueueManager queueService)
        {
            _redisQueueService = queueService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _redisQueueService.Dequeue();
                await Task.Delay(2000);
            }
        }
    }
}
