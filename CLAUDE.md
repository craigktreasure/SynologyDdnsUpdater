# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is an ASP.NET Core application that adapts Namecheap DDNS update responses to the format expected by Synology Customized DDNS Providers. It acts as a proxy between Synology NAS devices and the Namecheap DDNS API.

## Build Commands

```bash
# Restore, build, and test
dotnet restore
dotnet build
dotnet test

# Run a single test
dotnet test --filter "FullyQualifiedName~TestClassName.TestMethodName"

# Format check
dotnet format --verify-no-changes

# Publish
dotnet publish ./src/Synology.Ddns.Update.Service -c Release

# Build Docker container (using PowerShell script)
./src/Synology.Ddns.Update.Service/BuildContainer.ps1

# Build Docker container manually
docker build -f ./src/Synology.Ddns.Update.Service/Dockerfile --force-rm -t synologyddnsupdater:local .
```

## Package Management

- **Dependency Management:** This repo uses Central Package Management (CPM).
  - Versions are defined in `Directory.Packages.props`.
  - Individual `.csproj` files typically do NOT contain `<Version>` tags.
  - **NEVER** run `dotnet add package` to update versions. Instead, edit `Directory.Packages.props` directly.
- **SDK Version:** Defined in `global.json`.
- **Validation:** Always run `dotnet build` and `dotnet test` after performing updates.

Determine latest NuGet package versions by querying NuGet using `curl -sL https://api.nuget.org/v3-flatcontainer/<package-id-lowercase>/index.json`.

## Architecture

### Project Structure

- **Synology.Ddns.Update.Service** - ASP.NET Core web service (main entry point)
- **Namecheap.Library** - HTTP client for Namecheap DDNS API (`INamecheapDdnsClient`)
- **Synology.Namecheap.Adapter.Library** - Translates Namecheap responses to Synology format (`NamecheapResponseAdapter`)

### Key Flow

1. Synology NAS calls `/namecheap/ddns/update` with host, domain, password, and IP
2. `NamecheapDdns.Update` endpoint receives the request
3. `INamecheapDdnsClient` calls Namecheap's DDNS API
4. `NamecheapResponseAdapter` converts Namecheap's XML response to Synology's expected format (e.g., "good", "nohost", "badauth")

### Test Projects

- **Namecheap.Library.Tests** - Unit tests for Namecheap client
- **Synology.Namecheap.Adapter.Library.Tests** - Unit tests for response adapter
- **Synology.Ddns.Update.Service.Tests** - Unit tests for service endpoints and options
- **Synology.Ddns.Update.Service.ScenarioTests** - Integration tests using `WebApplicationFactory`
- **Test.Library** - Shared test utilities including `MockHttpMessageHandler`

### Configuration Options

- `NamecheapDdnsClientOptions.MockClient` - Set to `true` to use mock client (no real Namecheap calls)
- `MonitoringOptions.OpenTelemetryEnabled` - Toggle OpenTelemetry (enabled by default in Development)
- `GlobalRateLimiterOptions` - Fixed window rate limiting configuration

## Technical Details

- Target Framework: .NET 10.0
- Uses Central Package Management (`Directory.Packages.props`)
- Artifacts output to `__artifacts/` directory
- Tests use xUnit v3 with coverlet for code coverage
- Docker container runs on ports 8080 (HTTP) and 5443 (HTTPS)
