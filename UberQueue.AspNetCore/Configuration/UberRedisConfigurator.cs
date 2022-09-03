using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using UberQueue.Core.Consumers;
using UberQueue.Core.Queue;
using UberQueue.Core.Queue.Interfaces;

namespace UberQueue.AspNetCore.Configuration
{
    public class UberRedisConfigurator : IUberRedisConfigurator
    {
        private readonly IServiceCollection _serviceCollection;
        public UberRedisConfigurator(IServiceCollection services)
        {
            _serviceCollection = services;
        }

        public void ConfigureRedis(Action<ConfigurationOptions, EndPointCollection> redisConfigOptions)
        {
            _serviceCollection.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                EndPointCollection collection = new EndPointCollection();
                ConfigurationOptions options = new ConfigurationOptions() { EndPoints = collection };

                redisConfigOptions?.Invoke(options, collection);

                return ConnectionMultiplexer.Connect(options);
            });

            _serviceCollection.AddSingleton(sp =>
            {
                return sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase();
            });
        }

        public IUberRedisConfigurator AddConsumer<T>() where T : class, IConsumer
        {
            Type redisConsumer = typeof(T).GetInterfaces()[0];

            _serviceCollection.AddSingleton(redisConsumer, typeof(T));

            Type consumerClass = redisConsumer.GenericTypeArguments[0];
            Type redisWrapper = typeof(IRedisConsumerWrapper<>).MakeGenericType(consumerClass);
            Type redisWrapperClass = typeof(RedisConsumerWrapper<>).MakeGenericType(consumerClass);

            _serviceCollection.AddSingleton(redisWrapper, redisWrapperClass);

            return this;
        }
    }
}
