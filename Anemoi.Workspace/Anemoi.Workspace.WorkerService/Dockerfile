FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Anemoi.Workspace/Anemoi.Workspace.WorkerService/Anemoi.Workspace.WorkerService.csproj", "Anemoi.Workspace/Anemoi.Workspace.WorkerService/"]
COPY ["Anemoi.Workspace/Anemoi.Workspace.Infrastructure/Anemoi.Workspace.Infrastructure.csproj", "Anemoi.Workspace/Anemoi.Workspace.Infrastructure/"]
COPY ["Anemoi.Workspace/Anemoi.Workspace.Application/Anemoi.Workspace.Application.csproj", "Anemoi.Workspace/Anemoi.Workspace.Application/"]
COPY ["Anemoi.Workspace/Anemoi.Workspace.Domain/Anemoi.Workspace.Domain.csproj", "Anemoi.Workspace/Anemoi.Workspace.Domain/"]
COPY ["Anemoi.Contract/Anemoi.Contract.Workspace/Anemoi.Contract.Workspace.csproj", "Anemoi.Contract/Anemoi.Contract.Workspace/"]
COPY ["Anemoi.Contract/Anemoi.Contract.CrossCuttingConcern/Anemoi.Contract.CrossCuttingConcern.csproj", "Anemoi.Contract/Anemoi.Contract.CrossCuttingConcern/"]
COPY ["Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Infrastructure/Anemoi.BuildingBlock.Infrastructure.csproj", "Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Infrastructure/"]
COPY ["Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Application/Anemoi.BuildingBlock.Application.csproj", "Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Application/"]
COPY ["Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Domain/Anemoi.BuildingBlock.Domain.csproj", "Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Domain/"]
COPY ["Anemoi.Orchestration/Anemoi.Orchestration.Contract/Anemoi.Orchestration.Contract.csproj", "Anemoi.Orchestration/Anemoi.Orchestration.Contract/"]
COPY ["Anemoi.Contract/Anemoi.Contract.Identity/Anemoi.Contract.Identity.csproj", "Anemoi.Contract/Anemoi.Contract.Identity/"]
RUN dotnet restore "Anemoi.Workspace/Anemoi.Workspace.WorkerService/Anemoi.Workspace.WorkerService.csproj"
COPY . .
WORKDIR "/src/Anemoi.Workspace/Anemoi.Workspace.WorkerService"
RUN dotnet build "Anemoi.Workspace.WorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Anemoi.Workspace.WorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Anemoi.Workspace.WorkerService.dll"]
