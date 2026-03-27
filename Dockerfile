FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY WDA.sln .
COPY src/server/WDA.WebApi/WDA.WebApi.csproj src/WDA.WebApi/
# (copy other projects as needed)
RUN dotnet restore
COPY . .
RUN dotnet build -c $BUILD_CONFIGURATION --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish src/server/WDA.WebApi/WDA.WebApi.csproj -c $BUILD_CONFIGURATION -o /app/publish --no-build

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WDA.WebApi.dll"]