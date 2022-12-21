#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["src/Synology.Ddns.Update.Service/Synology.Ddns.Update.Service.csproj", "src/Synology.Ddns.Update.Service/"]
COPY ["src/Namecheap.Library/Namecheap.Library.csproj", "src/Namecheap.Library/"]
COPY ["src/Synology.Namecheap.Adapter.Library/Synology.Namecheap.Adapter.Library.csproj", "src/Synology.Namecheap.Adapter.Library/"]
RUN dotnet restore "src/Synology.Ddns.Update.Service/Synology.Ddns.Update.Service.csproj"
COPY . .
WORKDIR "/src/src/Synology.Ddns.Update.Service"
RUN dotnet build "Synology.Ddns.Update.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Synology.Ddns.Update.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Synology.Ddns.Update.Service.dll"]