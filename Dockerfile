FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/

# Copy solution and project files
COPY WDA.slnx .
COPY src/server/WDA.WebApi/WDA.WebApi.csproj src/WDA.WebApi/
COPY src/server/WDA.Application/WDA.Application.csproj src/WDA.Application/
COPY src/server/WDA.Domain/WDA.Domain.csproj src/WDA.Domain/
COPY src/server/WDA.Infrastructure/WDA.Infrastructure.csproj src/WDA.Infrastructure/
COPY src/server/WDA.Shared/WDA.Shared.csproj src/WDA.Shared/
COPY tests/server tests/WDA.Tests/WDA.Tests.csproj tests/WDA.Tests/

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY . .

# Build
RUN dotnet build -c Release --no-restore

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish src/server/WDA.WebApi/WDA.WebApi.csproj -c Release -o /app/publish --no-build

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WDA.WebApi.dll"]