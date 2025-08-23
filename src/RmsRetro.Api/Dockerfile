FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5013

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/RmsRetro.Api/RmsRetro.Api.csproj", "src/RmsRetro.Api/"]
COPY ["src/core/RmsRetro.Abstractions/RmsRetro.Abstractions.csproj", "src/core/RmsRetro.Abstractions/"]
COPY ["src/RmsRetro.Common/RmsRetro.Common.csproj", "src/RmsRetro.Common/"]
COPY ["src/RmsRetro.Protos/RmsRetro.Protos.csproj", "src/RmsRetro.Protos/"]
RUN dotnet restore "src/RmsRetro.Api/RmsRetro.Api.csproj"
COPY . .
WORKDIR "/src/src/RmsRetro.Api"
RUN dotnet build "./RmsRetro.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RmsRetro.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RmsRetro.Api.dll"]
