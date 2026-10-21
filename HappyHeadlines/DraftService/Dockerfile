
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files and dependencies
COPY ["DraftService/DraftService.csproj", "DraftService/"]
COPY ["Observability/Observability.csproj", "Observability/"]

# Restore dependencies
RUN dotnet restore "DraftService/DraftService.csproj"

# Copy remaining source files
COPY . .

# Build
WORKDIR /src/DraftService
RUN dotnet build "DraftService.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build \
    --no-restore

# Publish
FROM build AS publish
RUN dotnet publish "DraftService.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DraftService.dll"]
