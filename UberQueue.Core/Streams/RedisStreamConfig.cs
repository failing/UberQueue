namespace UberQueue.Core.Streams
{
    public class RedisStreamConfig
    {
        public string StreamName { get; set; }
        public string? ConsumerGroup { get; set; } = Guid.NewGuid().ToString();
        public string? GroupName { get; set; }
    }
}
