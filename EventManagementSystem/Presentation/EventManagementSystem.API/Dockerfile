# Use the official .NET 9 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["EventManagementSystem/Presentation/EventManagementSystem.API/EventManagementSystem.API.csproj", "EventManagementSystem.API/"]
COPY ["EventManagementSystem/Core/Application/EventManagementSystem.Application/EventManagementSystem.Application.csproj", "EventManagementSystem.Application/"]
COPY ["EventManagementSystem/Core/Domain/EventManagementSystem.Domain/EventManagementSystem.Domain.csproj", "EventManagementSystem.Domain/"]
COPY ["EventManagementSystem/Infrastructure/Identity/EventManagementSystem.Identity/EventManagementSystem.Identity.csproj", "EventManagementSystem.Identity/"]
COPY ["EventManagementSystem/Infrastructure/Persistence/EventManagementSystem.Persistence/EventManagementSystem.Persistence.csproj", "EventManagementSystem.Persistence/"]
COPY ["EventManagementSystem/Infrastructure/Storage/EventManagementSystem.Storage/EventManagementSystem.Storage.csproj", "EventManagementSystem.Storage/"]

# Restore dependencies
RUN dotnet restore "EventManagementSystem.API/EventManagementSystem.API.csproj"

# Copy source code
COPY EventManagementSystem/ .

# Build the application
WORKDIR "/src/Presentation/EventManagementSystem.API"
RUN dotnet build "EventManagementSystem.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "EventManagementSystem.API.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventManagementSystem.API.dll"]