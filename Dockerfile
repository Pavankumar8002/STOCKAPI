# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy everything and publish
COPY . .
RUN dotnet publish "./STOCKWEBAPI/STOCKWEBAPI.csproj" -c Release -o /out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /out .

# Expose port 80
EXPOSE 80
ENTRYPOINT ["dotnet", "STOCKWEBAPI.dll"]
