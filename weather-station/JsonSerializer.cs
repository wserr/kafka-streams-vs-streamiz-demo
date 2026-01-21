using Confluent.Kafka;
using System.Text;

namespace weather_station;

public class JsonSerializer<T> : ISerializer<T>
{
    public byte[] Serialize(T data, SerializationContext context)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(data);
        return Encoding.UTF8.GetBytes(json);
    }
}
