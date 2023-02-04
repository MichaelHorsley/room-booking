#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["admin-domain/admin-domain.csproj", "admin-domain/"]
RUN dotnet restore "admin-domain/admin-domain.csproj"

COPY . .

WORKDIR "/src/admin-domain"
RUN dotnet build "admin-domain.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "admin-domain.csproj" -c Release -o /app/publish /p:UseAppadmin=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "admin-domain.dll"]