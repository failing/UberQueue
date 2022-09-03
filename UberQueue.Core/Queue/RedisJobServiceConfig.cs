namespace UberQueue.Core.Queue
{
    public class RedisJobServiceConfig
    {
        public string Key { get; set; } = "jobs";

        public int BatchSize { get; set; } = 500;
    }
}
