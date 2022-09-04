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
            UberRedisConfigurator uberRedisConfigurator = new UberRedisConfigurator(services);

            // This seems wrong ? I'm not sure how to get a concrete list of all my streamer wrapper objects so I can preemptively
            // Connect it to a consumer
            services.AddHostedService(x =>
            {
                List<Type> consumersTypes = uberRedisConfigurator.ConsumerStreamerClasses;

                foreach (var type in consumersTypes)
                {
                    Type constructedRedisWrapperGenericType = typeof(IRedisConsumerStreamWrapper<>).MakeGenericType(type);

                    // Request the wrapper object
                    var service = (IRedisConsumerStreamerWrapper)x.GetRequiredService(constructedRedisWrapperGenericType);

                    // Connect the consumer to the stream
                    _ = service.Connect();
                }

                // Construct and then return the new hosted service
                return new RedisJobBackgroundService(x.GetRequiredService<IRedisQueueService>(), x.GetRequiredService<RedisJobServiceConfig>());
            });

            services.AddSingleton<IRedisQueueService, RedisQueueService>();
            services.AddSingleton<IRedisRouter, RedisRouter>();
            services.AddSingleton(typeof(IRedisConsumerWrapper<>), typeof(RedisConsumerWrapper<>));
            services.AddSingleton(typeof(IRedisConsumerStreamWrapper<>), typeof(RedisConsumerStreamWrapper<>));

            var preActions = services.GetPreConfigureActions<RedisJobServiceConfig>();
            var config = preActions.Configure();
            preActions.Configure(config);
            configuration?.Invoke(config, uberRedisConfigurator);
            services.AddSingleton(config);

            return services;
        }
    }
}
