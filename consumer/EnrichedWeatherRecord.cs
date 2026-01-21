namespace consumer;

public record EnrichedWeatherRecord(Guid messageId, string weatherStationId, double value, long originalMessageTimestamp, long currentMessageTimestamp);
