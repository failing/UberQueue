using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using UberQueue.Core.Consumers;
using UberQueue.Core.Streams;

namespace UberQueue.AspNetCore.Configuration
{
    public class UberRedisConfigurator : IUberRedisConfigurator
    {
        public List<Type> ConsumerClasses => _consumerTypes;
        public List<Type> ConsumerStreamerClasses => _consumerStreamerTypes;

        private readonly IServiceCollection _serviceCollection;
        private readonly List<Type> _consumerTypes = new List<Type>();
        private readonly List<Type> _consumerStreamerTypes = new List<Type>();

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

        public IUberRedisConfigurator AddConsumer<T>(Action<RedisConsumerOptions>? streamActions = null) where T : class, IConsumer
        {
            Type redisConsumer = typeof(T).GetInterfaces()[0];
            Type consumerClass = redisConsumer.GenericTypeArguments[0];

            if (_consumerTypes.Contains(consumerClass))
            {
                throw new Exception();
            }

            _consumerTypes.Add(consumerClass);
            _serviceCollection.AddSingleton(redisConsumer, typeof(T));

            RedisConsumerOptions options = new RedisConsumerOptions();
            streamActions?.Invoke(options);

            if (options.UseStreams)
            {
                _consumerStreamerTypes.Add(consumerClass);
                Type redisStream = typeof(IRedisStream<>).MakeGenericType(consumerClass);
                Type redisStreamClass = typeof(RedisStream<>).MakeGenericType(consumerClass);
                Type redisStreamConfig = typeof(RedisStreamConfig<>).MakeGenericType(consumerClass);

                _serviceCollection.AddSingleton(redisStream, redisStreamClass);
                _serviceCollection.AddSingleton(redisStreamConfig);
            }

            return this;
        }
    }
}
