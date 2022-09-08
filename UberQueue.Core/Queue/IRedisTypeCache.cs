namespace UberQueue.Core.Queue
{
    public interface IRedisTypeCache
    {
        Type GetServiceForType(Type type);
    }
}