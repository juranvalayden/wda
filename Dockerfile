FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY WDA.sln .
COPY src/server/WDA.WebApi/WDA.WebApi.csproj src/WDA.WebApi/
COPY src/server/WDA.Application/WDA.Application.csproj src/WDA.Application/
COPY src/server/WDA.Domain/WDA.Domain.csproj src/WDA.Domain/
COPY src/server/WDA.Infrastructure/WDA.Infrastructure.csproj src/WDA.Infrastructure/
COPY src/server/WDA.Shared/WDA.Shared.csproj src/WDA.Shared/
COPY tests/server tests/WDA.Tests/WDA.Tests.csproj tests/WDA.Tests/

RUN dotnet restore

COPY . .

RUN dotnet build -c $BUILD_CONFIGURATION --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish src/server/WDA.WebApi/WDA.WebApi.csproj -c $BUILD_CONFIGURATION -o /app/publish --no-build

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN adduser --disabled-password --gecos "" appuser \
 && chown -R appuser:appuser /app

USER appuser

ENTRYPOINT ["dotnet", "WDA.WebApi.dll"]