#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["admin-projections/admin-projections.csproj", "admin-projections/"]
RUN dotnet restore "admin-projections/admin-projections.csproj"

COPY . .

WORKDIR "/src/admin-projections"
RUN dotnet build "admin-projections.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "admin-projections.csproj" -c Release -o /app/publish /p:UseAppadmin=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "admin-projections.dll"]