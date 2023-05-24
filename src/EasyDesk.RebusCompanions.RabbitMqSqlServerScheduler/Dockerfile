FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Linter.targets", "."]
COPY ["src/EasyDesk.RebusCompanions.RabbitMqSqlServerScheduler/EasyDesk.RebusCompanions.RabbitMqSqlServerScheduler.csproj", "src/EasyDesk.RebusCompanions.RabbitMqSqlServerScheduler/"]
COPY ["src/EasyDesk.RebusCompanions.Core/EasyDesk.RebusCompanions.Core.csproj", "src/EasyDesk.RebusCompanions.Core/"]
RUN dotnet restore "src/EasyDesk.RebusCompanions.RabbitMqSqlServerScheduler/EasyDesk.RebusCompanions.RabbitMqSqlServerScheduler.csproj"
COPY . .
WORKDIR "/src/src/EasyDesk.RebusCompanions.RabbitMqSqlServerScheduler"
RUN dotnet build "EasyDesk.RebusCompanions.RabbitMqSqlServerScheduler.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EasyDesk.RebusCompanions.RabbitMqSqlServerScheduler.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EasyDesk.RebusCompanions.RabbitMqSqlServerScheduler.dll"]