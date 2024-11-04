FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Anemoi.Orchestration/Anemoi.Orchestration.Processor/Anemoi.Orchestration.Processor.csproj", "Anemoi.Orchestration/Anemoi.Orchestration.Processor/"]
COPY ["Anemoi.Orchestration/Anemoi.Orchestration.Infrastructure/Anemoi.Orchestration.Infrastructure.csproj", "Anemoi.Orchestration/Anemoi.Orchestration.Infrastructure/"]
COPY ["Anemoi.Orchestration/Anemoi.Orchestration.Machine/Anemoi.Orchestration.Machine.csproj", "Anemoi.Orchestration/Anemoi.Orchestration.Machine/"]
COPY ["Anemoi.Orchestration/Anemoi.Orchestration.Contract/Anemoi.Orchestration.Contract.csproj", "Anemoi.Orchestration/Anemoi.Orchestration.Contract/"]
COPY ["Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Infrastructure/Anemoi.BuildingBlock.Infrastructure.csproj", "Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Infrastructure/"]
COPY ["Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Application/Anemoi.BuildingBlock.Application.csproj", "Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Application/"]
COPY ["Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Domain/Anemoi.BuildingBlock.Domain.csproj", "Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Domain/"]
COPY ["Anemoi.Contract/Anemoi.Contract.Secure/Anemoi.Contract.Secure.csproj", "Anemoi.Contract/Anemoi.Contract.Secure/"]
COPY ["Anemoi.Contract/Anemoi.Contract.CrossCuttingConcern/Anemoi.Contract.CrossCuttingConcern.csproj", "Anemoi.Contract/Anemoi.Contract.CrossCuttingConcern/"]
RUN dotnet restore "Anemoi.Orchestration/Anemoi.Orchestration.Processor/Anemoi.Orchestration.Processor.csproj"
COPY . .
WORKDIR "/src/Anemoi.Orchestration/Anemoi.Orchestration.Processor"
RUN dotnet build "Anemoi.Orchestration.Processor.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Anemoi.Orchestration.Processor.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Anemoi.Orchestration.Processor.dll"]
