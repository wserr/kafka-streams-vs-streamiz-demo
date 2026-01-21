package com.example.streams.models;

import java.util.UUID;

public record WeatherRecord(UUID messageId, String weatherStationId, double value, long timeStamp) {

}
