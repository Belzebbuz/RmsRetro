FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5050

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/core/RmsRetro.Silo/RmsRetro.Silo.csproj", "src/core/RmsRetro.Silo/"]
COPY ["src/core/RmsRetro.Grains/RmsRetro.Grains.csproj", "src/core/RmsRetro.Grains/"]
COPY ["src/core/RmsRetro.Abstractions/RmsRetro.Abstractions.csproj", "src/core/RmsRetro.Abstractions/"]
COPY ["src/RmsRetro.Common/RmsRetro.Common.csproj", "src/RmsRetro.Common/"]
COPY ["src/RmsRetro.Protos/RmsRetro.Protos.csproj", "src/RmsRetro.Protos/"]
COPY ["src/hub/RmsRetro.MessagHub.Protos/RmsRetro.MessagHub.Protos.csproj", "src/hub/RmsRetro.MessagHub.Protos/"]
RUN dotnet restore "src/core/RmsRetro.Silo/RmsRetro.Silo.csproj"
COPY . .
WORKDIR "/src/src/core/RmsRetro.Silo"
RUN dotnet build "./RmsRetro.Silo.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RmsRetro.Silo.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RmsRetro.Silo.dll"]
