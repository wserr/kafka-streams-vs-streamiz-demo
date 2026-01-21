package com.example.streams.models;

import java.util.UUID;

public record EnrichedWeatherRecord(UUID messageId, String weatherStationId, double value, long originalMessageTimestamp, long currentMessageTimestamp) {}
