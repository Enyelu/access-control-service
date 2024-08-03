#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Base image for the runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["access-control.api/access-control.api.csproj", "access-control.api/"]
COPY ["access-control.core/access-control.core.csproj", "access-control.core/"]

COPY ["access-control.domain/access-control.domain.csproj", "access-control.domain/"]
COPY ["access-control.infrastructure/access-control.infrastructure.csproj", "access-control.infrastructure/"]
COPY ["access-control.test/access-control.test.csproj", "access-control.test/"]
RUN dotnet restore "access-control.api/access-control.api.csproj"

# Copy the entire source code and build the project
COPY . .
WORKDIR "/src/access-control.api"
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "access-control.api.dll"]
