FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Linter.targets", "."]
COPY ["src/EasyDesk.RebusScheduler.RabbitMqPostgres/EasyDesk.RebusScheduler.RabbitMqPostgres.csproj", "src/EasyDesk.RebusScheduler.RabbitMqPostgres/"]
COPY ["src/EasyDesk.RebusScheduler.Core/EasyDesk.RebusScheduler.Core.csproj", "src/EasyDesk.RebusScheduler.Core/"]
RUN dotnet restore "src/EasyDesk.RebusScheduler.RabbitMqPostgres/EasyDesk.RebusScheduler.RabbitMqPostgres.csproj"
COPY . .
WORKDIR "/src/src/EasyDesk.RebusScheduler.RabbitMqPostgres"
RUN dotnet build "EasyDesk.RebusScheduler.RabbitMqPostgres.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EasyDesk.RebusScheduler.RabbitMqPostgres.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EasyDesk.RebusScheduler.RabbitMqPostgres.dll"]