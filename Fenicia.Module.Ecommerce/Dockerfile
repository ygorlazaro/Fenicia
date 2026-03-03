FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Fenicia.Module.Ecommerce/Fenicia.Module.Ecommerce.csproj", "Fenicia.Module.Ecommerce/"]
RUN dotnet restore "Fenicia.Module.Ecommerce/Fenicia.Module.Ecommerce.csproj"
COPY . .
WORKDIR "/src/Fenicia.Module.Ecommerce"
RUN dotnet build "./Fenicia.Module.Ecommerce.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Fenicia.Module.Ecommerce.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV DOTNET_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080;https://+:8081
ENTRYPOINT ["dotnet", "Fenicia.Module.Ecommerce.dll"]
