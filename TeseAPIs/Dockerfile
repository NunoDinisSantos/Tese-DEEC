# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
WORKDIR /app

# Install SQLite tools
RUN apt-get update && apt-get install -y sqlite3 libsqlite3-dev && \
    groupadd -g 1000 hostgroup && \
    usermod -aG hostgroup app && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

USER app

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TeseAPIs.csproj", "."]
RUN dotnet restore "./TeseAPIs.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./TeseAPIs.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TeseAPIs.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8081
ENTRYPOINT ["dotnet", "TeseAPIs.dll"]