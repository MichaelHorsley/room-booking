#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["admin-acl/admin-acl.csproj", "admin-acl/"]
RUN dotnet restore "admin-acl/admin-acl.csproj"

COPY . .

WORKDIR "/src/admin-acl"
RUN dotnet build "admin-acl.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "admin-acl.csproj" -c Release -o /app/publish /p:UseAppadmin=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "admin-acl.dll"]