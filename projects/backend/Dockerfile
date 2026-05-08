# ── Fast build (for demos / local dev) ──
# Uses pre-published output. Run `dotnet publish -c Release -o publish` first.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY publish/ .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 8080
ENTRYPOINT ["dotnet", "SwarmDemo.Api.dll"]

# ── Production multi-stage build (alternative) ──
# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# WORKDIR /src
# COPY ["SwarmDemo.Api/SwarmDemo.Api.csproj", "SwarmDemo.Api/"]
# COPY ["SwarmDemo.Application/SwarmDemo.Application.csproj", "SwarmDemo.Application/"]
# COPY ["SwarmDemo.Domain/SwarmDemo.Domain.csproj", "SwarmDemo.Domain/"]
# COPY ["SwarmDemo.Infrastructure/SwarmDemo.Infrastructure.csproj", "SwarmDemo.Infrastructure/"]
# RUN dotnet restore "SwarmDemo.Api/SwarmDemo.Api.csproj"
# COPY . .
# WORKDIR /src/SwarmDemo.Api
# RUN dotnet publish "SwarmDemo.Api.csproj" -c Release -o /app/publish --no-restore
# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
# WORKDIR /app
# COPY --from=build /app/publish .
# ENV ASPNETCORE_URLS=http://+:8080
# EXPOSE 8080
# ENTRYPOINT ["dotnet", "SwarmDemo.Api.dll"]
