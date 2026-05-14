FROM maven:3.9.15-amazoncorretto-25-debian-trixie as BUILD

WORKDIR /app
COPY ./pom.xml ./

RUN mvn dependency:resolve

COPY ./ ./
RUN mvn package -Dmaven.test.skip -Prelease

FROM amazoncorretto:25 as RELEASE

WORKDIR /app
COPY --from=BUILD ./app/target ./

ENTRYPOINT ["java", "-jar", "streams-0.0.1-SNAPSHOT.jar"]
