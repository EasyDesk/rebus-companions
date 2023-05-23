FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Linter.targets", "."]
COPY ["src/EasyDesk.RebusCompanion.RabbitMqPostgresScheduler/EasyDesk.RebusCompanion.RabbitMqPostgresScheduler.csproj", "src/EasyDesk.RebusCompanion.RabbitMqPostgresScheduler/"]
COPY ["src/EasyDesk.RebusCompanion.Core/EasyDesk.RebusCompanion.Core.csproj", "src/EasyDesk.RebusCompanion.Core/"]
RUN dotnet restore "src/EasyDesk.RebusCompanion.RabbitMqPostgresScheduler/EasyDesk.RebusCompanion.RabbitMqPostgresScheduler.csproj"
COPY . .
WORKDIR "/src/src/EasyDesk.RebusCompanion.RabbitMqPostgresScheduler"
RUN dotnet build "EasyDesk.RebusCompanion.RabbitMqPostgresScheduler.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EasyDesk.RebusCompanion.RabbitMqPostgresScheduler.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EasyDesk.RebusCompanion.RabbitMqPostgresScheduler.dll"]