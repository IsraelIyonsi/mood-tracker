FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props ./
COPY src/MoodTracker.Api/MoodTracker.Api.csproj src/MoodTracker.Api/
RUN dotnet restore src/MoodTracker.Api/MoodTracker.Api.csproj

COPY src/MoodTracker.Api/ src/MoodTracker.Api/
RUN dotnet publish src/MoodTracker.Api/MoodTracker.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app

USER app

ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_USE_POLLING_FILE_WATCHER=false \
    Database__ConnectionString="Data Source=/tmp/mood.db;Cache=Shared;Pooling=True"

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=3s --start-period=15s --retries=3 \
    CMD wget -qO- http://localhost:8080/health/live || exit 1

COPY --from=build --chown=app:app /app/publish ./
COPY --chown=app:app src/MoodTracker.Api/wwwroot ./wwwroot

ENTRYPOINT ["dotnet", "MoodTracker.Api.dll"]
