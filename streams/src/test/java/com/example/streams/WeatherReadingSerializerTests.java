package com.example.streams;

import com.google.gson.Gson;
import org.junit.jupiter.api.Test;
import org.springframework.boot.test.context.SpringBootTest;

import static org.junit.jupiter.api.Assertions.assertEquals;

@SpringBootTest
public class WeatherReadingSerializerTests {
    @Test
    void WeatherReadingSerializer_ShouldDeserialize_Correctly() {
        String json = "{\"type\":\"Temperature\",\"value\":30,\"humidity\":\"20\", \"timestamp\":\"0\"}";

        WeatherReading reading = new Gson().fromJson(json, WeatherReading.class);

        assertEquals("Temperature", reading.type());

    }
}
