using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UberQueue.Core.Jobs;

namespace UberQueue.Core.Serialisation
{
    public class UknownTypeConverter : JsonConverter
    {
        public override void WriteJson(
            JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(
            JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = new JobObject();

            JObject obj = serializer.Deserialize<JObject>(reader);

            serializer.Populate(obj.CreateReader(), result);

            result.Payload =
                obj.GetValue("payload", StringComparison.OrdinalIgnoreCase)
                   .ToObject(result.PayloadType, serializer);

            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(JobObject);
        }

        public override bool CanWrite
        {
            get { return false; }
        }
    }
}
