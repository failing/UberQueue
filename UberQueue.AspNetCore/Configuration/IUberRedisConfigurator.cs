using StackExchange.Redis;
using UberQueue.Core.Consumers;

namespace UberQueue.AspNetCore.Configuration
{
    public interface IUberRedisConfigurator
    {
        void ConfigureRedis(Action<ConfigurationOptions, EndPointCollection> redisConfigOptions);
        IUberRedisConfigurator AddConsumer<T>() where T : class, IConsumer;
    }
}
