FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MagicEntrypoint/MagicEntrypoint.csproj", "MagicEntrypoint/"]
RUN dotnet restore "MagicEntrypoint/MagicEntrypoint.csproj"
COPY . .
WORKDIR "/src/MagicEntrypoint"
RUN dotnet build "MagicEntrypoint.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MagicEntrypoint.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MagicEntrypoint.dll"]
