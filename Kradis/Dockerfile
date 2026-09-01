FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Kradis/Kradis.csproj", "Kradis/"]
RUN dotnet restore "Kradis/Kradis.csproj"
COPY . .
WORKDIR "/src/Kradis"
RUN dotnet build "./Kradis.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Kradis.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
USER root
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir data && chown -R $APP_UID:$APP_UID data
USER $APP_UID
ENTRYPOINT ["dotnet", "Kradis.dll"]
