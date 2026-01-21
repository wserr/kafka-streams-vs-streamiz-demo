using Confluent.Kafka;
using Prometheus;

namespace consumer;

public class Worker : BackgroundService
{
    private static readonly Histogram MessageProcessingDurationStreams = Metrics.CreateHistogram(
        "messageProcessingDuration",
        "Histogram of message processing durations"
    );
    
    private static readonly Histogram MessageProcessingDurationStreamiz = Metrics.CreateHistogram(
        "messageProcessingDuration",
        "Histogram of message processing durations"
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

    public Task Consume(CancellationToken stoppingToken)
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
                            
                            var histogramObserver = cr.Topic == "weather.data.enriched.streams" ? MessageProcessingDurationStreams : MessageProcessingDurationStreamiz;

			    var result = cr.Message.Value.currentMessageTimestamp - cr.Message.Value.originalMessageTimestamp;
			    histogramObserver.Observe(result);

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
