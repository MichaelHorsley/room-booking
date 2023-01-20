#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["host-projections/host-projections.csproj", "host-projections/"]
RUN dotnet restore "host-projections/host-projections.csproj"

COPY . .

WORKDIR "/src/host-projections"
RUN dotnet build "host-projections.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "host-projections.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "host-projections.dll"]