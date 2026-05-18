package com.example.streams.models;

import java.util.UUID;

public record EnrichedWeatherRecord(UUID messageId, String weatherStationId, String weatherStationName, double value, long originalMessageTimestamp, long currentMessageTimestamp) {}
