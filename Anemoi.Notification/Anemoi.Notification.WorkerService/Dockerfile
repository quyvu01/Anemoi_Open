FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Anemoi.Notification/Anemoi.Notification.WorkerService/Anemoi.Notification.WorkerService.csproj", "Anemoi.Notification/Anemoi.Notification.WorkerService/"]
COPY ["Anemoi.Orchestration/Anemoi.Orchestration.Contract/Anemoi.Orchestration.Contract.csproj", "Anemoi.Orchestration/Anemoi.Orchestration.Contract/"]
COPY ["Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Infrastructure/Anemoi.BuildingBlock.Infrastructure.csproj", "Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Infrastructure/"]
COPY ["Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Application/Anemoi.BuildingBlock.Application.csproj", "Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Application/"]
COPY ["Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Domain/Anemoi.BuildingBlock.Domain.csproj", "Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Domain/"]
COPY ["Anemoi.Notification/Anemoi.Notification.Infrastructure/Anemoi.Notification.Infrastructure.csproj", "Anemoi.Notification/Anemoi.Notification.Infrastructure/"]
COPY ["Anemoi.Notification/Anemoi.Notification.Application/Anemoi.Notification.Application.csproj", "Anemoi.Notification/Anemoi.Notification.Application/"]
COPY ["Anemoi.Contract/Anemoi.Contract.Secure/Anemoi.Contract.Secure.csproj", "Anemoi.Contract/Anemoi.Contract.Secure/"]
COPY ["Anemoi.Contract/Anemoi.Contract.CrossCuttingConcern/Anemoi.Contract.CrossCuttingConcern.csproj", "Anemoi.Contract/Anemoi.Contract.CrossCuttingConcern/"]
COPY ["Anemoi.Notification/Anemoi.Notification.Domain/Anemoi.Notification.Domain.csproj", "Anemoi.Notification/Anemoi.Notification.Domain/"]
COPY ["Anemoi.Contract/Anemoi.Contract.Notification/Anemoi.Contract.Notification.csproj", "Anemoi.Contract/Anemoi.Contract.Notification/"]
RUN dotnet restore "Anemoi.Notification/Anemoi.Notification.WorkerService/Anemoi.Notification.WorkerService.csproj"
COPY . .
WORKDIR "/src/Anemoi.Notification/Anemoi.Notification.WorkerService"
RUN dotnet build "Anemoi.Notification.WorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Anemoi.Notification.WorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Anemoi.Notification.WorkerService.dll"]
