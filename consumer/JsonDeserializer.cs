using Confluent.Kafka;
using System.Text;

namespace consumer;

public class JsonDeserializer<T> : IDeserializer<T>
{
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
            return default(T);

        var json = Encoding.UTF8.GetString(data);
        return System.Text.Json.JsonSerializer.Deserialize<T>(json);
    }
}
