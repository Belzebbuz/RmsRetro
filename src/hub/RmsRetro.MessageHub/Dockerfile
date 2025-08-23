FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5080
EXPOSE 5081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/hub/RmsRetro.MessageHub/RmsRetro.MessageHub.csproj", "src/hub/RmsRetro.MessageHub/"]
COPY ["src/hub/RmsRetro.MessagHub.Protos/RmsRetro.MessagHub.Protos.csproj", "src/hub/RmsRetro.MessagHub.Protos/"]
COPY ["src/RmsRetro.Protos/RmsRetro.Protos.csproj", "src/RmsRetro.Protos/"]
RUN dotnet restore "src/hub/RmsRetro.MessageHub/RmsRetro.MessageHub.csproj"
COPY . .
WORKDIR "/src/src/hub/RmsRetro.MessageHub"
RUN dotnet build "./RmsRetro.MessageHub.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RmsRetro.MessageHub.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RmsRetro.MessageHub.dll"]
