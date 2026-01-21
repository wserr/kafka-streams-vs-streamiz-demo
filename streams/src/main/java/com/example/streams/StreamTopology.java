package com.example.streams;

import com.example.streams.models.*;
import org.apache.kafka.common.serialization.Serdes;
import org.apache.kafka.streams.StreamsBuilder;
import org.apache.kafka.streams.kstream.*;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.kafka.support.serializer.JsonSerde;
import org.springframework.stereotype.Component;

@ConditionalOnProperty("kafka.enabled")
@Component
public class StreamTopology {

    static final String WeatherRecordTopic = "weather.data";
    static final String WeatherStationRecordTopic = "weather.stations";
    static final String TargetTopic = "weather.data.enriched.streams";

    @Autowired
    public void configure(StreamsBuilder builder) {
        var weatherRecordSerde = new JsonSerde<>(WeatherRecord.class);
        var weatherStationRecordSerde = new JsonSerde<>(WeatherStationRecord.class);
        var enrichedWeatherRecordSerde = new JsonSerde<>(EnrichedWeatherRecord.class);

        var weatherStationsTable = builder.globalTable(WeatherStationRecordTopic, Materialized.with(Serdes.String(), weatherStationRecordSerde));

        var weatherRecordStream =
                builder.stream(WeatherRecordTopic, Consumed.with(Serdes.String(), weatherRecordSerde));

        ValueJoiner<WeatherRecord, WeatherStationRecord, EnrichedWeatherRecord> valueJoiner = (leftValue, rightValue) -> new EnrichedWeatherRecord(leftValue.messageId(), rightValue.weatherStationId(), leftValue.value(), leftValue.timeStamp(), System.currentTimeMillis());

        weatherRecordStream
                .join(weatherStationsTable, 
                      (key, value) -> value.weatherStationId(), // key mapper to extract station ID from weather record
                      valueJoiner)
                .to(TargetTopic, Produced.with(Serdes.String(), enrichedWeatherRecordSerde));

        var result = builder.build();
        System.out.println(result.describe());
    }
}
