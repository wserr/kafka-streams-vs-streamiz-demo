using Confluent.Kafka;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.SerDes;

namespace streamiz;

public class Worker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new StreamConfig<StringSerDes, StringSerDes>();
        config.ApplicationId = "Streamiz";
        config.BootstrapServers = "broker:29092";
        config.AllowAutoCreateTopics = true;
        config.AutoOffsetReset = AutoOffsetReset.Earliest;

        var builder = new StreamBuilder();

        var weatherStationsTable = builder.GlobalTable<string, WeatherStationRecord>(
            "weather.stations",
            new StringSerDes(),
            new JsonSerDes<WeatherStationRecord>()
        );

        var weatherRecordStream = builder.Stream<string, WeatherRecord>(
            "weather.data",
            new StringSerDes(),
            new JsonSerDes<WeatherRecord>()
        );

        var enrichedWeatherRecordStream = weatherRecordStream.Join(
            weatherStationsTable,
            (id, weatherRecord) => weatherRecord.weatherStationId,
            (weatherRecord, station) =>
                new EnrichedWeatherRecord(
                    weatherRecord.messageId,
                    weatherRecord.value,
                    weatherRecord.timeStamp,
                    DateTimeOffset.Now.ToUnixTimeMilliseconds()
                )
        );

	enrichedWeatherRecordStream.To<StringSerDes, JsonSerDes<EnrichedWeatherRecord>>("weather.data.enriched.streamiz");

	var t = builder.Build();
	var stream = new KafkaStream(t, config);

	await stream.StartAsync(stoppingToken);
    }
}
