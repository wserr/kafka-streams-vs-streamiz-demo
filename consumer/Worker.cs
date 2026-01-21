using Confluent.Kafka;
using Prometheus;

namespace consumer;

public class Worker : BackgroundService
{
    private static readonly Histogram MessageProcessingDuration = Metrics.CreateHistogram(
        "message_processing_duration_seconds",
        "Histogram of message processing durations in seconds",
        new HistogramConfiguration
        {
            LabelNames = new[] { "processor_type" },
            Buckets = new[] { 1.0, 10.0, 20.0, 30.0, 40.0, 50.0, 60.0, 70.0, 80.0, 90.0, 100.0 },
        }
    );

    private readonly IConsumer<string, EnrichedWeatherRecord> _enrichedWeatherDataConsumer;

    public Worker(ILogger<Worker> logger)
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "broker:29092",
            GroupId = "Consumer",
        };
        _enrichedWeatherDataConsumer = new ConsumerBuilder<string, EnrichedWeatherRecord>(
            consumerConfig
        )
            .SetValueDeserializer(new JsonDeserializer<EnrichedWeatherRecord>())
            .Build();
        _enrichedWeatherDataConsumer.Subscribe(["weather.data.enriched.streams", "weather.data.enriched.streamiz"]);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Consume(stoppingToken);
        }
    }

    private Task Consume(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = _enrichedWeatherDataConsumer.Consume();
                            
                            var processorType = cr.Topic == "weather.data.enriched.streams" ? "streams" : "streamiz";
                            
                            var processingDurationMs = cr.Message.Value.currentMessageTimestamp - cr.Message.Value.originalMessageTimestamp;
                            
                            MessageProcessingDuration.WithLabels(processorType).Observe(processingDurationMs);

                            _enrichedWeatherDataConsumer.Commit();
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    _enrichedWeatherDataConsumer.Close();
                }

                return Task.CompletedTask;
            },
            stoppingToken
        );
    }
}
