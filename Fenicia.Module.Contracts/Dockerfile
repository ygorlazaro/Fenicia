FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Fenicia.Module.Contracts/Fenicia.Module.Contracts.csproj", "Fenicia.Module.Contracts/"]
RUN dotnet restore "Fenicia.Module.Contracts/Fenicia.Module.Contracts.csproj"
COPY . .
WORKDIR "/src/Fenicia.Module.Contracts"
RUN dotnet build "./Fenicia.Module.Contracts.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Fenicia.Module.Contracts.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV DOTNET_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080;https://+:8081
ENTRYPOINT ["dotnet", "Fenicia.Module.Contracts.dll"]
