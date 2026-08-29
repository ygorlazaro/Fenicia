FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Fenicia.Auth/Fenicia.Auth.csproj", "Fenicia.Auth/"]
COPY ["Fenicia.Common/Fenicia.Common.csproj", "Fenicia.Common/"]
COPY ["Fenicia.Common.API/Fenicia.Common.API.csproj", "Fenicia.Common.API/"]
COPY ["Fenicia.Common.Data/Fenicia.Common.Data.csproj", "Fenicia.Common.Data/"]
COPY ["Fenicia.Externals.Email/Fenicia.Externals.Email.csproj", "Fenicia.Externals.Email/"]
RUN dotnet restore "Fenicia.Auth/Fenicia.Auth.csproj"
COPY . .
WORKDIR "/src/Fenicia.Auth"
RUN dotnet build "./Fenicia.Auth.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Fenicia.Auth.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fenicia.Auth.dll"]
