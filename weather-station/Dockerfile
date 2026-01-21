ARG DOTNET_VERSION
ARG EF_CORE_VERSION
ARG BUILD_CONFIGURATION

FROM mcr.microsoft.com/dotnet/aspnet:$DOTNET_VERSION AS base
USER $APP_UID

EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:$DOTNET_VERSION AS build
COPY ./*.csproj ./src/

WORKDIR /src
RUN dotnet restore

COPY ./* ./src/

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION
RUN dotnet publish "./weather-station.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "weather-station.dll"]
