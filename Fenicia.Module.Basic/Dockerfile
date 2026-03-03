# ==================================================================
# Dockerfile for Fenicia.Module.Basic
# Supports debugging in Rider/VSCode
# ==================================================================

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Fenicia.Module.Basic/Fenicia.Module.Basic.csproj", "Fenicia.Module.Basic/"]
RUN dotnet restore "Fenicia.Module.Basic/Fenicia.Module.Basic.csproj"
COPY . .
WORKDIR "/src/Fenicia.Module.Basic"
RUN dotnet build "./Fenicia.Module.Basic.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Fenicia.Module.Basic.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV DOTNET_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080;https://+:8081
ENTRYPOINT ["dotnet", "Fenicia.Module.Basic.dll"]
