FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["src/EasyDesk.RebusCompanions.RabbitMqEmailErrorManager/EasyDesk.RebusCompanions.RabbitMqEmailErrorManager.csproj", "src/EasyDesk.RebusCompanions.RabbitMqEmailErrorManager/"]
COPY ["src/EasyDesk.RebusCompanions.Core/EasyDesk.RebusCompanions.Core.csproj", "src/EasyDesk.RebusCompanions.Core/"]
COPY ["src/EasyDesk.RebusCompanions.Email/EasyDesk.RebusCompanions.Email.csproj", "src/EasyDesk.RebusCompanions.Email/"]
RUN dotnet restore "src/EasyDesk.RebusCompanions.RabbitMqEmailErrorManager/EasyDesk.RebusCompanions.RabbitMqEmailErrorManager.csproj"
COPY . .
WORKDIR "/src/src/EasyDesk.RebusCompanions.RabbitMqEmailErrorManager"
RUN dotnet build "EasyDesk.RebusCompanions.RabbitMqEmailErrorManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EasyDesk.RebusCompanions.RabbitMqEmailErrorManager.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EasyDesk.RebusCompanions.RabbitMqEmailErrorManager.dll"]