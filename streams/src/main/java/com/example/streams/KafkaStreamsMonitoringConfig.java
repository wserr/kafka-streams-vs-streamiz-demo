package com.example.streams;

import io.micrometer.core.instrument.MeterRegistry;
import io.micrometer.core.instrument.binder.kafka.KafkaStreamsMetrics;
import jakarta.annotation.PostConstruct;
import org.apache.kafka.streams.KafkaStreams;
import org.springframework.context.annotation.Configuration;
import org.springframework.kafka.config.StreamsBuilderFactoryBean;

@Configuration
public class KafkaStreamsMonitoringConfig {

    private final StreamsBuilderFactoryBean factoryBean;
    private final MeterRegistry meterRegistry;

    public KafkaStreamsMonitoringConfig(StreamsBuilderFactoryBean factoryBean, MeterRegistry meterRegistry) {
        this.factoryBean = factoryBean;
        this.meterRegistry = meterRegistry;
    }

    @PostConstruct
    public void bindKafkaStreamsMetricsWhenReady() {
        factoryBean.setStateListener((newState, oldState) -> {
            if (newState == KafkaStreams.State.RUNNING) {
                KafkaStreams kafkaStreams = factoryBean.getKafkaStreams();
                if (kafkaStreams != null) {
                    new KafkaStreamsMetrics(kafkaStreams).bindTo(meterRegistry);
                    System.out.println("Kafka Streams metrics bound to Micrometer.");
                } else {
                    System.err.println("KafkaStreams instance is still null.");
                }
            }
        });
    }
}
