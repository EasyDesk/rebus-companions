#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["src/EasyDesk.RebusCompanion.RabbitMqEmailErrorHandler/EasyDesk.RebusCompanion.RabbitMqEmailErrorHandler.csproj", "src/EasyDesk.RebusCompanion.RabbitMqEmailErrorHandler/"]
COPY ["src/EasyDesk.RebusCompanion.Core/EasyDesk.RebusCompanion.Core.csproj", "src/EasyDesk.RebusCompanion.Core/"]
COPY ["src/EasyDesk.RebusCompanion.Email/EasyDesk.RebusCompanion.Email.csproj", "src/EasyDesk.RebusCompanion.Email/"]
RUN dotnet restore "src/EasyDesk.RebusCompanion.RabbitMqEmailErrorHandler/EasyDesk.RebusCompanion.RabbitMqEmailErrorHandler.csproj"
COPY . .
WORKDIR "/src/src/EasyDesk.RebusCompanion.RabbitMqEmailErrorHandler"
RUN dotnet build "EasyDesk.RebusCompanion.RabbitMqEmailErrorHandler.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EasyDesk.RebusCompanion.RabbitMqEmailErrorHandler.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EasyDesk.RebusCompanion.RabbitMqEmailErrorHandler.dll"]