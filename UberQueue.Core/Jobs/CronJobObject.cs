using Newtonsoft.Json;
using UberQueue.Core.Serialisation;

namespace UberQueue.Core.Jobs
{
    [JsonConverter(typeof(UnknownTypeConverter))]
    public class CronJobObject
    {
    }
}
