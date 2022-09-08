using Microsoft.Extensions.DependencyInjection;
using UberQueue.Core.Queue;
using UberQueue.Core.Queue.Wrappers;

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
            List<Type> consumersTypes = uberRedisConfigurator.ConsumerStreamerClasses;

            // This seems wrong ? I'm not sure how to get a concrete list of all my streamer wrapper objects so I can preemptively connect it to a consumer
            services.AddHostedService(x =>
            {
                foreach (var type in consumersTypes)
                {
                    Type constructedRedisWrapperGenericType = typeof(IRedisConsumerStreamWrapper<>).MakeGenericType(type);

                    // Request the wrapper object
                    var service = (IRedisConsumerStreamerWrapper)x.GetRequiredService(constructedRedisWrapperGenericType);

                    // Connect the consumer to the stream
                    _ = service.Connect();
                }

                // Construct and then return the new hosted service
                return new RedisJobBackgroundService(x.GetRequiredService<IRedisQueueManager>());
            });

            services.AddSingleton<IRedisQueueManager, RedisQueueManager>();
            services.AddSingleton<IRedisDirectQueue, RedisDirectQueueService>();
            services.AddSingleton<IRedisQueueStreamService, RedisQueueStreamService>();
            services.AddSingleton<IRedisRouter, RedisRouter>();
            services.AddSingleton(typeof(IRedisConsumerWrapper<>), typeof(RedisConsumerWrapper<>));
            services.AddSingleton(typeof(IRedisConsumerStreamWrapper<>), typeof(RedisConsumerStreamWrapper<>));

            var config = new RedisJobServiceConfig();
            configuration?.Invoke(config, uberRedisConfigurator);
            services.AddSingleton(config);
            services.AddSingleton<IRedisTypeCache>(x => new RedisTypeCache(uberRedisConfigurator.ConsumerClasses, uberRedisConfigurator.ConsumerStreamerClasses));

            return services;
        }
    }
}
