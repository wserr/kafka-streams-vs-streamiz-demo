namespace consumer;

public record EnrichedWeatherRecord(Guid messageId, double value, long originalMessageTimestamp, long currentMessageTimestamp);
