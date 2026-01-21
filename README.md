# Streams vs. Streamiz Benchmark

This project has the main purpose of comparing the streamiz library with the Kafka Streams library.
We have 2 streaming operations. Each streaming operation will process 1.000.000 weather reading messages.

There is a weather reading topic that contains 3 partitions. We want 3 partitions, because we will have 3 streaming instances processing the messages.

The weather reading message we take as input:

And the key of the message will be the ID of the weather station.

```json
{
  "message_id": guid
  "value": 21,
  "timestamp": timestamp_unix()
}
```

There will be 100 weather stations, each sending out a weather reading message every 1/10th of a second.

## Stateless stream processing example



## Stateful stream processing

Each weather reading message will be mapped to the corresponding weather station. The output will be something like this:

```json
{
  "original_message_id": guid()
  "value": 21,
  "timestamp": timestamp_unix(),
  "weather_station_name": "Ghent"
}
```
