namespace streamiz;

public record EnrichedWeatherRecord(Guid messageId, string weatherStationId, string weatherStationName, double value, long originalMessageTimestamp, long currentMessageTimestamp);
