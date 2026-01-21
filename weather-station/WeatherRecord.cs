namespace weather_station;

public record WeatherRecord(Guid messageId, string weatherStationId, double value, long timeStamp);
