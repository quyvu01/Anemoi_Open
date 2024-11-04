FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Anemoi.MasterData/Anemoi.MasterData.WorkerService/Anemoi.MasterData.WorkerService.csproj", "Anemoi.MasterData/Anemoi.MasterData.WorkerService/"]
COPY ["Anemoi.MasterData/Anemoi.MasterData.Infrastructure/Anemoi.MasterData.Infrastructure.csproj", "Anemoi.MasterData/Anemoi.MasterData.Infrastructure/"]
COPY ["Anemoi.MasterData/Anemoi.MasterData.Application/Anemoi.MasterData.Application.csproj", "Anemoi.MasterData/Anemoi.MasterData.Application/"]
COPY ["Anemoi.MasterData/Anemoi.MasterData.Domain/Anemoi.MasterData.Domain.csproj", "Anemoi.MasterData/Anemoi.MasterData.Domain/"]
COPY ["Anemoi.Contract/Anemoi.Contract.MasterData/Anemoi.Contract.MasterData.csproj", "Anemoi.Contract/Anemoi.Contract.MasterData/"]
COPY ["Anemoi.Contract/Anemoi.Contract.CrossCuttingConcern/Anemoi.Contract.CrossCuttingConcern.csproj", "Anemoi.Contract/Anemoi.Contract.CrossCuttingConcern/"]
COPY ["Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Infrastructure/Anemoi.BuildingBlock.Infrastructure.csproj", "Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Infrastructure/"]
COPY ["Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Application/Anemoi.BuildingBlock.Application.csproj", "Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Application/"]
COPY ["Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Domain/Anemoi.BuildingBlock.Domain.csproj", "Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Domain/"]
RUN dotnet restore "Anemoi.MasterData/Anemoi.MasterData.WorkerService/Anemoi.MasterData.WorkerService.csproj"
COPY . .
WORKDIR "/src/Anemoi.MasterData/Anemoi.MasterData.WorkerService"
RUN dotnet build "Anemoi.MasterData.WorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Anemoi.MasterData.WorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Anemoi.MasterData.WorkerService.dll"]
