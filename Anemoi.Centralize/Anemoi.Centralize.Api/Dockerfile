FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Anemoi.Centralize/Anemoi.Centralize.Api/Anemoi.Centralize.Api.csproj", "Anemoi.Centralize/Anemoi.Centralize.Api/"]
COPY ["Anemoi.Centralize/Anemoi.Centralize.Infrastructure/Anemoi.Centralize.Infrastructure.csproj", "Anemoi.Centralize/Anemoi.Centralize.Infrastructure/"]
COPY ["Anemoi.Centralize/Anemoi.Centralize.Application/Anemoi.Centralize.Application.csproj", "Anemoi.Centralize/Anemoi.Centralize.Application/"]
COPY ["Anemoi.Contract/Anemoi.Contract.Notification/Anemoi.Contract.Notification.csproj", "Anemoi.Contract/Anemoi.Contract.Notification/"]
COPY ["Anemoi.Contract/Anemoi.Contract.CrossCuttingConcern/Anemoi.Contract.CrossCuttingConcern.csproj", "Anemoi.Contract/Anemoi.Contract.CrossCuttingConcern/"]
COPY ["Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Infrastructure/Anemoi.BuildingBlock.Infrastructure.csproj", "Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Infrastructure/"]
COPY ["Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Application/Anemoi.BuildingBlock.Application.csproj", "Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Application/"]
COPY ["Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Domain/Anemoi.BuildingBlock.Domain.csproj", "Anemoi.BuildingBlocks/Anemoi.BuildingBlock.Domain/"]
COPY ["Anemoi.Contract/Anemoi.Contract.Identity/Anemoi.Contract.Identity.csproj", "Anemoi.Contract/Anemoi.Contract.Identity/"]
COPY ["Anemoi.Contract/Anemoi.Contract.MasterData/Anemoi.Contract.MasterData.csproj", "Anemoi.Contract/Anemoi.Contract.MasterData/"]
COPY ["Anemoi.Contract/Anemoi.Contract.Workspace/Anemoi.Contract.Workspace.csproj", "Anemoi.Contract/Anemoi.Contract.Workspace/"]
COPY ["Anemoi.Grpc/Anemoi.Grpc.Identity/Anemoi.Grpc.Identity.csproj", "Anemoi.Grpc/Anemoi.Grpc.Identity/"]
COPY ["Anemoi.Centralize/Anemoi.Centralize.Domain/Anemoi.Centralize.Domain.csproj", "Anemoi.Centralize/Anemoi.Centralize.Domain/"]
RUN dotnet restore "Anemoi.Centralize/Anemoi.Centralize.Api/Anemoi.Centralize.Api.csproj"
COPY . .
WORKDIR "/src/Anemoi.Centralize/Anemoi.Centralize.Api"
RUN dotnet build "Anemoi.Centralize.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Anemoi.Centralize.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Anemoi.Centralize.Api.dll"]
