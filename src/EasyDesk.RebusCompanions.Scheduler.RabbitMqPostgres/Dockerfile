FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["src/EasyDesk.RebusCompanions.Scheduler.RabbitMqPostgres/EasyDesk.RebusCompanions.Scheduler.RabbitMqPostgres.csproj", "src/EasyDesk.RebusCompanions.Scheduler.RabbitMqPostgres/"]
COPY ["src/EasyDesk.RebusCompanions.Core/EasyDesk.RebusCompanions.Core.csproj", "src/EasyDesk.RebusCompanions.Core/"]
COPY ["src/EasyDesk.RebusCompanions.EfCore/EasyDesk.RebusCompanions.EfCore.csproj", "src/EasyDesk.RebusCompanions.EfCore/"]
RUN dotnet restore "src/EasyDesk.RebusCompanions.Scheduler.RabbitMqPostgres/EasyDesk.RebusCompanions.Scheduler.RabbitMqPostgres.csproj"
COPY . .
WORKDIR "/src/src/EasyDesk.RebusCompanions.Scheduler.RabbitMqPostgres"
RUN dotnet build "EasyDesk.RebusCompanions.Scheduler.RabbitMqPostgres.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EasyDesk.RebusCompanions.Scheduler.RabbitMqPostgres.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EasyDesk.RebusCompanions.Scheduler.RabbitMqPostgres.dll"]