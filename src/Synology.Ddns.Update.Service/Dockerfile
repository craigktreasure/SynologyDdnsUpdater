#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

ARG aspNetVersion=9.0
# https://mcr.microsoft.com/en-us/product/dotnet/aspnet/tags
ARG baseTag=${aspNetVersion}-azurelinux3.0-distroless
ARG buildTag=${aspNetVersion}
ARG appPath=./

# Set to "fromlocal" to simply copy in locally built bits from appPath.
ARG finalStage=frompublish

FROM mcr.microsoft.com/dotnet/aspnet:${baseTag} AS base

WORKDIR /app

EXPOSE 8080
EXPOSE 5443

ENV \
    # Configure web servers to bind to port 8080 and 5443 when present
    ASPNETCORE_URLS=http://+:8080;https://+:5443 \
    ASPNETCORE_HTTPS_PORT=5443

# Condition: finalStage=frompublish
FROM mcr.microsoft.com/dotnet/sdk:${buildTag} AS build

WORKDIR /repo

COPY [".editorconfig", "."]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["global.json", "."]
COPY ["nuget.config", "."]
COPY ["version.json", "."]
COPY ["eng", "eng"]
COPY ["src/.editorconfig", "src/"]
COPY ["src/Directory.Build.props", "src/"]
COPY ["src/Synology.Ddns.Update.Service/Synology.Ddns.Update.Service.csproj", "src/Synology.Ddns.Update.Service/"]
COPY ["src/Namecheap.Library/Namecheap.Library.csproj", "src/Namecheap.Library/"]
COPY ["src/Synology.Namecheap.Adapter.Library/Synology.Namecheap.Adapter.Library.csproj", "src/Synology.Namecheap.Adapter.Library/"]
RUN dotnet restore "src/Synology.Ddns.Update.Service/Synology.Ddns.Update.Service.csproj"

COPY . .

WORKDIR "/repo/src/Synology.Ddns.Update.Service"

RUN dotnet build "Synology.Ddns.Update.Service.csproj" -c Release /p:WithoutGitRepository=true

# Condition: finalStage=frompublish
FROM build AS publish

RUN dotnet publish "Synology.Ddns.Update.Service.csproj" -c Release /p:WithoutGitRepository=true /p:UseAppHost=false

# Condition: finalStage=frompublish
FROM base AS frompublish

WORKDIR /app

COPY --from=publish /repo/__artifacts/publish/Synology.Ddns.Update.Service/release .

# Condition: finalStage=fromlocal
FROM base AS fromlocal

COPY $appPath /app

FROM ${finalStage} AS final

ENTRYPOINT ["dotnet", "Synology.Ddns.Update.Service.dll"]
