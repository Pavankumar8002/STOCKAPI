# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy .csproj and restore as distinct layers
COPY STOCKWEBAPI/STOCKWEBAPI.csproj STOCKWEBAPI/
RUN dotnet restore STOCKWEBAPI/STOCKWEBAPI.csproj

# Copy everything else and publish
COPY . .
WORKDIR /src/STOCKWEBAPI
RUN dotnet publish -c Release -o /app/publish

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "STOCKWEBAPI.dll"]
