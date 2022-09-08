using Newtonsoft.Json;
using UberQueue.Core.Serialisation;

namespace UberQueue.Core.Jobs
{
    [JsonConverter(typeof(UnknownTypeConverter))]
    public class RecurringJobObject : JobObject
    {
        public override RecurringType RecurringType => RecurringType.Recurrence;

        [JsonProperty("recurrence")]
        public string Recurrence { get; set; }
    }
}
