using JsonSubTypes;
using Newtonsoft.Json;

namespace UberQueue.Core.Jobs
{
    [JsonConverter(typeof(JsonSubtypes), "job_type")]
    [JsonSubtypes.KnownSubType(typeof(JobObject), JobDataType.Object)]
    [JsonSubtypes.KnownSubType(typeof(JobExpression), JobDataType.Expression)]
    public abstract class JobData
    {
        [JsonProperty("key")]
        public string Key => Guid.NewGuid().ToString();

        [JsonProperty("job_type")]
        public abstract JobDataType JobType { get; }
    }
}
