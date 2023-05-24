#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["src/EasyDesk.RebusCompanions.RabbitMqEmailErrorHandler/EasyDesk.RebusCompanions.RabbitMqEmailErrorHandler.csproj", "src/EasyDesk.RebusCompanions.RabbitMqEmailErrorHandler/"]
COPY ["src/EasyDesk.RebusCompanions.Core/EasyDesk.RebusCompanions.Core.csproj", "src/EasyDesk.RebusCompanions.Core/"]
COPY ["src/EasyDesk.RebusCompanions.Email/EasyDesk.RebusCompanions.Email.csproj", "src/EasyDesk.RebusCompanions.Email/"]
RUN dotnet restore "src/EasyDesk.RebusCompanions.RabbitMqEmailErrorHandler/EasyDesk.RebusCompanions.RabbitMqEmailErrorHandler.csproj"
COPY . .
WORKDIR "/src/src/EasyDesk.RebusCompanions.RabbitMqEmailErrorHandler"
RUN dotnet build "EasyDesk.RebusCompanions.RabbitMqEmailErrorHandler.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EasyDesk.RebusCompanions.RabbitMqEmailErrorHandler.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EasyDesk.RebusCompanions.RabbitMqEmailErrorHandler.dll"]