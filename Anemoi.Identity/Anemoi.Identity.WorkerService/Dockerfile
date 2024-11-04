FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Anemoi.Identity/Anemoi.Identity.WorkerService/Anemoi.Identity.WorkerService.csproj", "Anemoi.Identity/Anemoi.Identity.WorkerService/"]
COPY ["Anemoi.Identity/Anemoi.Identity.Infrastructure/Anemoi.Identity.Infrastructure.csproj", "Anemoi.Identity/Anemoi.Identity.Infrastructure/"]
COPY ["Anemoi.Identity/Anemoi.Identity.Application/Anemoi.Identity.Application.csproj", "Anemoi.Identity/Anemoi.Identity.Application/"]
COPY ["Anemoi.Grpc/Anemoi.Grpc.Identity/Anemoi.Grpc.Identity.csproj", "Anemoi.Grpc/Anemoi.Grpc.Identity/"]
COPY ["Anemoi.Identity/Anemoi.Identity.Domain/Anemoi.Identity.Domain.csproj", "Anemoi.Identity/Anemoi.Identity.Domain/"]
COPY ["Anemoi.Contract/Anemoi.Contract.Identity/Anemoi.Contract.Identity.csproj", "Anemoi.Contract/Anemoi.Contract.Identity/"]
COPY ["Anemoi.Contract/Anemoi.Contract.CrossCuttingConcern/Anemoi.Contract.CrossCuttingConcern.csproj", "Anemoi.Contract/Anemoi.Contract.CrossCuttingConcern/"]
COPY ["Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Infrastructure/Anemoi.BuildingBlock.Infrastructure.csproj", "Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Infrastructure/"]
COPY ["Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Application/Anemoi.BuildingBlock.Application.csproj", "Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Application/"]
COPY ["Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Domain/Anemoi.BuildingBlock.Domain.csproj", "Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Domain/"]
COPY ["Anemoi.Orchestration/Anemoi.Orchestration.Contract/Anemoi.Orchestration.Contract.csproj", "Anemoi.Orchestration/Anemoi.Orchestration.Contract/"]
RUN dotnet restore "Anemoi.Identity/Anemoi.Identity.WorkerService/Anemoi.Identity.WorkerService.csproj"
COPY . .
WORKDIR "/src/Anemoi.Identity/Anemoi.Identity.WorkerService"
RUN dotnet build "Anemoi.Identity.WorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Anemoi.Identity.WorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Anemoi.Identity.WorkerService.dll"]
