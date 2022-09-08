using Newtonsoft.Json;
using StackExchange.Redis;

namespace UberQueue.Core.Streams
{
    public class RedisStream<T> : IRedisStream<T>
    {
        private readonly IDatabase _redisDatabase;
        private readonly RedisStreamConfig<T> _config;

        public RedisStream(IDatabase redisDatabase, RedisStreamConfig<T> config)
        {
            _config = config;
            _redisDatabase = redisDatabase;

            if (_config.ConsumerGroup != null)
            {
                if (_config.ConsumerGroup == null)
                {
                    _config.ConsumerGroup = Guid.NewGuid().ToString();
                }

                if (!_redisDatabase.KeyExists(_config.StreamName) || _redisDatabase.StreamGroupInfo(_config.StreamName).All(r => r.Name != _config.ConsumerGroup))
                {
                    _redisDatabase.StreamCreateConsumerGroup(_config.StreamName, _config.ConsumerGroup, StreamPosition.NewMessages, true);
                }
            }
        }

        public async Task Consume(Func<T, Task> consumeFunc, CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested)
            {
                StreamEntry[]? results;

                if (_config.ConsumerGroup == null)
                {
                    results = await _redisDatabase.StreamReadAsync(_config.StreamName, ">", 50);
                }
                else
                {
                    results = await _redisDatabase.StreamReadGroupAsync(_config.StreamName, _config.ConsumerGroup, _config.ConsumerName, ">");
                }

                foreach (var ret in results)
                {
                    if (ret.Values.Length > 0)
                    {
                        try
                        {
                            T? message = JsonConvert.DeserializeObject<T>(ret.Values[0].Value!);

                            if (message != null)
                            {
                                try
                                {
                                    await consumeFunc(message);
                                }
                                catch (Exception e)
                                {
                                    continue;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                        await _redisDatabase.StreamAcknowledgeAsync(_config.StreamName, _config.ConsumerGroup, ret.Id);
                    }
                }
            }
        }

        public async Task Publish(T[] message)
        {
            RedisValue redisValue = new RedisValue(JsonConvert.SerializeObject(message));
            var nameValueEntries = message.Select(r => new NameValueEntry("object", JsonConvert.SerializeObject(r))).ToArray();

            await _redisDatabase.StreamAddAsync(_config.StreamName, nameValueEntries);
        }

        public async Task Publish(T message)
        {
            RedisValue redisValue = new RedisValue(JsonConvert.SerializeObject(message));
            RedisValue key = "object";

            await _redisDatabase.StreamAddAsync(_config.StreamName, key, redisValue);
        }
    }
}
