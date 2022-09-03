using Microsoft.Extensions.DependencyInjection;
using UberQueue.Core.Queue;
using UberQueue.Core.Queue.Interfaces;

namespace UberQueue.AspNetCore.Configuration
{
    public static class DependancyInjectionExtensions
    {
        public static IServiceCollection AddUberRedisQueue(this IServiceCollection services, Action<RedisJobServiceConfig> configuration)
        {
            return services.AddUberRedisQueue((a, b) => configuration(a));
        }

        public static IServiceCollection AddUberRedisQueue(this IServiceCollection services, Action<RedisJobServiceConfig, IUberRedisConfigurator>? configuration)
        {
            services.AddHostedService<RedisJobBackgroundService>();
            services.AddSingleton<IRedisQueueService, RedisQueueService>();
            services.AddSingleton<IRedisRouter, RedisRouter>();

            RedisJobServiceConfig config = new RedisJobServiceConfig();

            var uberRedisConfigurator = new UberRedisConfigurator(services);

            configuration?.Invoke(config, uberRedisConfigurator);
            services.AddSingleton(config);

            return services;
        }
    }
}
