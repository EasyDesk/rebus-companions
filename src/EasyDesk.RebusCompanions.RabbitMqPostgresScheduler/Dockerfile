FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Linter.targets", "."]
COPY ["src/EasyDesk.RebusCompanions.RabbitMqPostgresScheduler/EasyDesk.RebusCompanions.RabbitMqPostgresScheduler.csproj", "src/EasyDesk.RebusCompanions.RabbitMqPostgresScheduler/"]
COPY ["src/EasyDesk.RebusCompanions.Core/EasyDesk.RebusCompanions.Core.csproj", "src/EasyDesk.RebusCompanions.Core/"]
RUN dotnet restore "src/EasyDesk.RebusCompanions.RabbitMqPostgresScheduler/EasyDesk.RebusCompanions.RabbitMqPostgresScheduler.csproj"
COPY . .
WORKDIR "/src/src/EasyDesk.RebusCompanions.RabbitMqPostgresScheduler"
RUN dotnet build "EasyDesk.RebusCompanions.RabbitMqPostgresScheduler.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EasyDesk.RebusCompanions.RabbitMqPostgresScheduler.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EasyDesk.RebusCompanions.RabbitMqPostgresScheduler.dll"]