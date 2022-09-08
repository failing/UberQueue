namespace UberQueue.Core.Streams
{
    public class RedisStreamConfig<T>
    {
        public string StreamName { get; set; } = typeof(T).FullName;
        public string? ConsumerName { get; set; } = Guid.NewGuid().ToString();
        public string? ConsumerGroup { get; set; } = "test";
    }
}
