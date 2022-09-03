using JsonSubTypes;
using Newtonsoft.Json;

namespace UberQueue.Core.Jobs
{
    [JsonConverter(typeof(JsonSubtypes), "recurring_type")]
    [JsonSubtypes.KnownSubType(typeof(CronJobObject), RecurringType.Cron)]
    [JsonSubtypes.KnownSubType(typeof(RecurringJobObject), RecurringType.Recurrence)]
    [JsonSubtypes.KnownSubType(typeof(JobObject), RecurringType.Never)]
    [JsonSubtypes.FallBackSubType(typeof(JobObject))]
    public class JobObject : JobData
    {
        public override JobDataType JobType => JobDataType.Object;

        [JsonProperty("recurring_type")]
        public virtual RecurringType RecurringType => RecurringType.Never;

        [JsonProperty("payload")]
        public dynamic Payload { get; set; }

        [JsonProperty("payload_type")]
        public Type PayloadType { get; set; }

        [JsonProperty("max_num_failures")]
        public int MaxNumberOfFailures { get; set; } = 5;
    }
}
