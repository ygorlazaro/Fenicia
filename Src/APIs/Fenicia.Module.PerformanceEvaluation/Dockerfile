FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Src/APIs/Fenicia.Module.PerformanceEvaluation/Fenicia.Module.PerformanceEvaluation.csproj", "Src/APIs/Fenicia.Module.PerformanceEvaluation/"]
COPY ["Src/Libs/Fenicia.Common/Fenicia.Common.csproj", "Src/Libs/Fenicia.Common/"]
COPY ["Src/Libs/Fenicia.Common.API/Fenicia.Common.API.csproj", "Src/Libs/Fenicia.Common.API/"]
COPY ["Src/Libs/Fenicia.Common.Data/Fenicia.Common.Data.csproj", "Src/Libs/Fenicia.Common.Data/"]
RUN dotnet restore "Src/APIs/Fenicia.Module.PerformanceEvaluation/Fenicia.Module.PerformanceEvaluation.csproj"
COPY . .
WORKDIR "/src/Src/APIs/Fenicia.Module.PerformanceEvaluation"
RUN dotnet build "./Fenicia.Module.PerformanceEvaluation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Fenicia.Module.PerformanceEvaluation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV DOTNET_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080;https://+:8081
ENTRYPOINT ["dotnet", "Fenicia.Module.PerformanceEvaluation.dll"]
