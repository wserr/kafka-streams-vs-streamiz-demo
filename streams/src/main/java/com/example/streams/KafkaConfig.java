package com.example.streams;

import java.util.HashMap;

import org.springframework.beans.factory.ObjectProvider;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.boot.autoconfigure.kafka.KafkaProperties;
import org.springframework.boot.ssl.SslBundles;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.kafka.annotation.EnableKafkaStreams;
import org.springframework.kafka.annotation.KafkaStreamsDefaultConfiguration;
import org.springframework.kafka.config.KafkaStreamsConfiguration;

@ConditionalOnProperty("kafka.enabled")
@Configuration
@EnableKafkaStreams
public class KafkaConfig {

    @Bean(KafkaStreamsDefaultConfiguration.DEFAULT_STREAMS_CONFIG_BEAN_NAME)
    public KafkaStreamsConfiguration kStreamsConfig(
            KafkaProperties kafkaProperties,
            ObjectProvider<SslBundles> sslBundles) {
        var properties = new HashMap<>(kafkaProperties.buildStreamsProperties(sslBundles.getIfAvailable()));
        return new KafkaStreamsConfiguration(properties);
    }

}
