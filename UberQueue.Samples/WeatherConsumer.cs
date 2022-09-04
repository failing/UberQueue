using UberQueue.Core.Consumers;
using UberRedQueueApi;

namespace UberQueue.Samples
{
    public class WeatherConsumer : IRedisConsumer<WeatherForecast>
    {
        public async Task Consume(WeatherForecast message)
        {
            Console.WriteLine(message);
        }
    }
}
