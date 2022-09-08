using UberQueue.Core.Consumers;

namespace UberQueue.Samples
{
    public class TestConsumer : IRedisConsumer<Class>
    {
        public async Task Consume(Class message)
        {
            Console.WriteLine(message);
        }
    }
}
