#/bin/bash

kafka-topics --create --bootstrap-server broker:29092 --topic weather.data --partitions 6
kafka-topics --create --bootstrap-server broker:29092 --topic weather.data.enriched.streams --partitions 6
kafka-topics --create --bootstrap-server broker:29092 --topic weather.data.enriched.streamiz --partitions 6
kafka-topics --create --bootstrap-server broker:29092 --topic weather.stations --partitions 6

exit 0
