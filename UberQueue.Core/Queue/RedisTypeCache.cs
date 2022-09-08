namespace UberQueue.Core.Queue
{
    public class RedisTypeCache : IRedisTypeCache
    {
        private readonly List<Type> ConsumerClasses;
        private readonly List<Type> ConsumerStreamerClasses;

        public RedisTypeCache(List<Type> consumerTypes, List<Type> streamerTypes)
        {
            ConsumerClasses = consumerTypes;
            ConsumerStreamerClasses = streamerTypes;
        }

        public Type GetServiceForType(Type type)
        {
            if (ConsumerStreamerClasses.Contains(type))
            {
                return typeof(IRedisQueueStreamService);
            }
            else if (ConsumerClasses.Contains(type))
            {
                return typeof(IRedisDirectQueue);
            }

            throw new Exception();
        }
    }
}
