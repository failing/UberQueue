using JsonSubTypes;
using Newtonsoft.Json;

namespace UberQueue.Core.Jobs
{
    [JsonConverter(typeof(JsonSubtypes), "recurring_type")]
    [JsonSubtypes.KnownSubType(typeof(CronJobObject), RecurringType.Cron)]
    [JsonSubtypes.KnownSubType(typeof(RecurringJobObject), RecurringType.Recurrence)]
    [JsonSubtypes.KnownSubType(typeof(JobObject), RecurringType.Never)]
    [JsonSubtypes.FallBackSubType(typeof(JobData))]
    public class JobObject : JobData
    {

        public override JobDataType JobType => JobDataType.Object;

        [JsonProperty("recurring_type")]
        public virtual RecurringType RecurringType => RecurringType.Never;

        [JsonProperty("payload")]
        public dynamic Payload
        {
            get => _internalPayload;
            set
            {
                _internalPayload = value;
                PayloadType = (_internalPayload as object).GetType();
                PayloadTypeShortName = (_internalPayload as object).GetType().FullName;
            }
        }

        private dynamic _internalPayload { get; set; }

        [JsonProperty("payload_type")]
        public Type PayloadType { get; set; }

        [JsonProperty("payload_type_short")]
        public string PayloadTypeShortName { get; set; }

        [JsonProperty("max_num_failures")]
        public int MaxNumberOfFailures { get; set; } = 5;
    }
}
