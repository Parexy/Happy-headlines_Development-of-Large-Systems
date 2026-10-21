
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files and dependencies
COPY ["CommentService/CommentService.csproj", "CommentService/"]
COPY ["Observability/Observability.csproj", "Observability/"]

# Restore dependencies
RUN dotnet restore "CommentService/CommentService.csproj"

# Copy remaining source files
COPY . .

# Build
WORKDIR /src/CommentService
RUN dotnet build "CommentService.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build \
    --no-restore

# Publish
FROM build AS publish
RUN dotnet publish "CommentService.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CommentService.dll"]
