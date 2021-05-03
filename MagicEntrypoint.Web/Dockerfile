FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MagicEntrypoint.Web/MagicEntrypoint.Web.csproj", "MagicEntrypoint.Web/"]
RUN dotnet restore "MagicEntrypoint.Web/MagicEntrypoint.Web.csproj"
COPY . .
WORKDIR "/src/MagicEntrypoint.Web"
RUN dotnet build "MagicEntrypoint.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MagicEntrypoint.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MagicEntrypoint.Web.dll"]
