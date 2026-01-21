using Confluent.Kafka;
using NameGenerator.Generators;

namespace weather_station;

public class Worker : BackgroundService
{
    private Random _random;
    private IProducer<string, WeatherRecord> _weatherDataProducer;
    private IProducer<string, WeatherStationRecord> _weatherStationProducer;
    private string _weatherStationId;

    public Worker()
    {
        _random = new Random();
        var producerConfig = new ProducerConfig { BootstrapServers = "broker:29092" };
        _weatherDataProducer = new ProducerBuilder<string, WeatherRecord>(producerConfig)
            .SetKeySerializer(Serializers.Utf8)
            .SetValueSerializer(new JsonSerializer<WeatherRecord>())
            .Build();
        _weatherStationProducer = new ProducerBuilder<string, WeatherStationRecord>(producerConfig)
            .SetKeySerializer(Serializers.Utf8)
            .SetValueSerializer(new JsonSerializer<WeatherStationRecord>())
            .Build();
        _weatherStationId = System.Environment.MachineName;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var generator = new RealNameGenerator();
        var weatherStationRecord = new WeatherStationRecord(
            _weatherStationId,
            generator.Generate()
        );

        var weatherStationMessage = new Message<string, WeatherStationRecord>()
        {
            Key = weatherStationRecord.weatherStationId,
            Value = weatherStationRecord,
        };

        await _weatherStationProducer.ProduceAsync(
            "weather.stations",
            weatherStationMessage,
            stoppingToken
        );

        while (!stoppingToken.IsCancellationRequested)
        {
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var temperature = _random.NextDouble() * 30;
            var record = new WeatherRecord(
                Guid.NewGuid(),
                _weatherStationId,
                temperature,
                timestamp
            );
            var message = new Message<string, WeatherRecord>()
            {
                Key = _weatherStationId,
                Value = record,
            };
            await _weatherDataProducer.ProduceAsync("weather.data", message, stoppingToken);
        }
    }
}
