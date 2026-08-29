FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Src/APIs/Fenicia.Auth/Fenicia.Auth.csproj", "Src/APIs/Fenicia.Auth/"]
COPY ["Src/Libs/Fenicia.Common/Fenicia.Common.csproj", "Src/Libs/Fenicia.Common/"]
COPY ["Src/Libs/Fenicia.Common.API/Fenicia.Common.API.csproj", "Src/Libs/Fenicia.Common.API/"]
COPY ["Src/Libs/Fenicia.Common.Data/Fenicia.Common.Data.csproj", "Src/Libs/Fenicia.Common.Data/"]
COPY ["Src/Libs/Fenicia.Externals.Email/Fenicia.Externals.Email.csproj", "Src/Libs/Fenicia.Externals.Email/"]
RUN dotnet restore "Src/APIs/Fenicia.Auth/Fenicia.Auth.csproj"
COPY . .
WORKDIR "/src/Src/APIs/Fenicia.Auth"
RUN dotnet build "./Fenicia.Auth.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Fenicia.Auth.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fenicia.Auth.dll"]
