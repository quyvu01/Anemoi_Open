FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Anemoi.Secure/Anemoi.Secure.WorkerService/Anemoi.Secure.WorkerService.csproj", "Anemoi.Secure/Anemoi.Secure.WorkerService/"]
COPY ["Anemoi.Secure/Anemoi.Secure.Infrastructure/Anemoi.Secure.Infrastructure.csproj", "Anemoi.Secure/Anemoi.Secure.Infrastructure/"]
COPY ["Anemoi.Secure/Anemoi.Secure.Application/Anemoi.Secure.Application.csproj", "Anemoi.Secure/Anemoi.Secure.Application/"]
COPY ["Anemoi.Secure/Anemoi.Secure.Domain/Anemoi.Secure.Domain.csproj", "Anemoi.Secure/Anemoi.Secure.Domain/"]
COPY ["Anemoi.Contract/Anemoi.Contract.Secure/Anemoi.Contract.Secure.csproj", "Anemoi.Contract/Anemoi.Contract.Secure/"]
COPY ["Anemoi.Contract/Anemoi.Contract.CrossCuttingConcern/Anemoi.Contract.CrossCuttingConcern.csproj", "Anemoi.Contract/Anemoi.Contract.CrossCuttingConcern/"]
COPY ["Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Infrastructure/Anemoi.BuildingBlock.Infrastructure.csproj", "Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Infrastructure/"]
COPY ["Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Application/Anemoi.BuildingBlock.Application.csproj", "Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Application/"]
COPY ["Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Domain/Anemoi.BuildingBlock.Domain.csproj", "Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Domain/"]
RUN dotnet restore "Anemoi.Secure/Anemoi.Secure.WorkerService/Anemoi.Secure.WorkerService.csproj"
COPY . .
WORKDIR "/src/Anemoi.Secure/Anemoi.Secure.WorkerService"
RUN dotnet build "Anemoi.Secure.WorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Anemoi.Secure.WorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Anemoi.Secure.WorkerService.dll"]
