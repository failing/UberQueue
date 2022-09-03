using JsonSubTypes;
using Newtonsoft.Json;

namespace UberQueue.Core.Jobs
{
    [JsonConverter(typeof(JsonSubtypes), "job_type")]
    [JsonSubtypes.KnownSubType(typeof(JobObject), JobDataType.Object)]
    [JsonSubtypes.KnownSubType(typeof(JobExpression), JobDataType.Expression)]
    public abstract class JobData
    {
        [JsonProperty("job_type")]
        public abstract JobDataType JobType { get; }
    }
}
