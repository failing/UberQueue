using UberQueue.Core.Queue.Interfaces;

namespace UberQueue.Core.Queue
{
    public class RedisRouter : IRedisRouter
    {
        private readonly IServiceProvider _provider;
        public RedisRouter(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task Route<TMessageConsumer>(TMessageConsumer message)
        {
            Type messageType = message.GetType();
            Type messageHandlerType = typeof(IRedisConsumerWrapper<>).MakeGenericType(messageType);

            var handler = _provider.GetService(messageHandlerType) as IRedisConsumerWrapper<TMessageConsumer>;

            await handler.Consumer.Consume(message);
        }
    }
}
