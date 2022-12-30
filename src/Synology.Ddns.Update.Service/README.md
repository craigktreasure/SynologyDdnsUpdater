# Synology DDN Updater

- [Synology DDN Updater](#synology-ddn-updater)
  - [Building the Docker container](#building-the-docker-container)
    - [Build Docker container using PowerShell script](#build-docker-container-using-powershell-script)
    - [Build Docker container manually](#build-docker-container-manually)
  - [Configuring the service](#configuring-the-service)
    - [Namecheap DDNS client options](#namecheap-ddns-client-options)
    - [Monitoring options](#monitoring-options)
    - [Rate limiting options](#rate-limiting-options)
    - [Docker options](#docker-options)
      - [Configure Docker container with https](#configure-docker-container-with-https)
      - [Configure Docker container with http only](#configure-docker-container-with-http-only)
  - [Running the service](#running-the-service)
    - [Run from Visual Studio](#run-from-visual-studio)
    - [Run in Docker](#run-in-docker)
    - [Run in Docker using https](#run-in-docker-using-https)
    - [Run in Docker using http](#run-in-docker-using-http)

An instance of this service is already hosted at <https://synologyddnsupdater.azurewebsites.net>, but the following
instructions will explain how to run the service yourself.

## Building the Docker container

Make sure you've installed the following pre-requisites:

- [Docker][docker]
  - [Docker Desktop][docker-desktop] recommended
- [.NET 7.0 SDK][dotnet-7-sdk-download]

> **Note**
>
> A prebuilt container image ([craigktreasure/synologyddnsupdater][docker-image]) is also available if you're
> comfortable with that.

### Build Docker container using PowerShell script

The Docker container is simple to build. There's even a PowerShell script to simplify it.

To build the service within a container:

```powershell
./BuildContainer.ps1 -UseContainerBuild
```

This method will use a .NET SDK container to build the service application.

To build the service using the host:

```powershell
./BuildContainer.ps1
```

This method will build the service application on the host machine and then use those bits to produce the container.

### Build Docker container manually

To build the service within a container:

```bash
# From the root of the repository
docker build -f ./src/Synology.Ddns.Update.Service/Dockerfile --force-rm -t synologyddnsupdater:local .
```

This method will use a .NET SDK container to build the service application.

To build the service using the host:

```bash
# From the root of the repository
dotnet publish ./src/Synology.Ddns.Update.Service -c Release
docker build -f ./src/Synology.Ddns.Update.Service/Dockerfile --build-arg finalStage=fromlocal --force-rm -t synologyddnsupdater:local __publish/Release/AnyCPU/src/Synology.Ddns.Update.Service/net7.0
```

This method will build the service application on the host machine and then use those bits to produce the container.

## Configuring the service

This list is not meant to be exhaustive, but here are some options that can be specified to change the behavior of the
service. There are many ways to [configure][aspnetcore-configuration] an ASP.NET Core application. We'll use simple
JSON notation here, but you can always replace `.` with `__` for environment variables.

### Namecheap DDNS client options

The Namecheap DDNS client is used to make calls to the Namecheap service to update DDNS records. You can enable a mock
client that never really calls Namecheap and only returns "good" responses by setting
`NamecheapDdnsClientOptions.MockClient` to `true`. This option defaults to `false`.

### Monitoring options

The application is configured with [OpenTelemetry][opentelemetry] for monitoring purposes. However, this is not yet
enabled by default in the Production environment. It is enabled when running the service in the Development environment.
You can override the default at any time by setting the `MonitoringOptions.OpenTelemetryEnabled` option to `true` or
`false`.

| Option                                   | Production Default | Development Default |
|------------------------------------------|--------------------|---------------------|
| `MonitoringOptions.OpenTelemetryEnabled` | `false`            | `true`              |

### Rate limiting options

The application is configured with [rate limiting][aspnetcore-ratelimiting] using the Fixed Window strategy. You can
find the default configuration in [GlobalRateLimiterOptions.cs][globalratelimiteroptions].

| Option                                 | Default    |
|----------------------------------------|------------|
| `GlobalRateLimiterOptions.PermitLimit` | `5`        |
| `GlobalRateLimiterOptions.QueueLimit`  | `5`        |
| `GlobalRateLimiterOptions.Window`      | `00:00:15` |

See the [docs][aspnetcore-ratelimiting] for detailed explanations of what these values represent.

### Docker options

The [Dockerfile][dockerfile] is configured to build the service itself in an intermediate container and to run the
application using ports 8080 and 5443 (https) in the ASP.NET Core 7.0 Mariner 2.0 Distroless container.

| Variable name           | Description                                                                                               | Container Default              |
|-------------------------|-----------------------------------------------------------------------------------------------------------|--------------------------------|
| `ASPNETCORE_HTTPS_PORT` | Configures the [HTTPS port][aspnetcore-https-port].                                                       | `5443`                         |
| `ASPNETCORE_URLS`       | Configures the [server URLs][aspnetcore-server-urls] ([also helpful][aspnetcore-server-urls-andrewlock]). | `http://+:8080;https://+:5443` |

The ports can easily be overridden using the `ASPNETCORE_URLS` and `ASPNETCORE_HTTPS_PORT` environment variables. Good
reading material can be found [here][aspnetcore-docker-https] and [here][aspnetcore-server-urls-andrewlock].

There is no certificate included in the container and one will be required in the default configuration. See
[below](#configure-docker-container-with-https).

#### Configure Docker container with https

First, you'll need a certificate file (pfx) and the password for that certificate file.

> **TIP**
>
> If you have a crt, ca bundle, and the private key, you can create a pfx using openssl:
>
> ```shell
> openssl pkcs12 --export -out mycertificate.pfx -inkey server.key -in mycertificate.crt -certfile mycertificate.ca-bundle
> ```
>
> More detailed instructions can be found elsewhere online, but I found [this page][create-pfx-openssl] helpful.

Put the certificate file (pfx) into a folder on the Docker host.

Configure the container with a volume mount that includes the certificate file. This is done by specifying the `-v`
Docker parameter with the value `<host path to certificate folder>:/https`.

You'll then configure the container with some key ASP.NET Core environment variables to tell the service where to find
the certificate and what the password is for that certificate.

| Variable name                                         | Description                             | Value                      |
|-------------------------------------------------------|-----------------------------------------|----------------------------|
| `ASPNETCORE_Kestrel__Certificates__Default__Path`     | Configures the path to the certificate. | `/https`                   |
| `ASPNETCORE_Kestrel__Certificates__Default__Password` | Configures the certificate password.    | Your certificate password. |

Example command:

```shell
docker run -d --rm \
    -p 58080:8080 \
    -p 55443:5443 \
    -e ASPNETCORE_Kestrel__Certificates__Default__Password="<certificate password>" \
    -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/<certificate file name> \
    -v <certificate folder path>:/https/ \
    craigktreasure/synologyddnsupdater:1.0.4
```

#### Configure Docker container with http only

By setting a key ASP.NET Core environment variable, you can easily reconfigure the container to run using http only.

| Variable name           | Description                                           | Value           |
|-------------------------|-------------------------------------------------------|-----------------|
| `ASPNETCORE_URLS`       | Configures the [server URLs][aspnetcore-server-urls]. | `http://+:8080` |

Example command:

```shell
docker run -d --rm \
    -p 58080:8080 \
    -e ASPNETCORE_URLS=http://+:8080 \
    craigktreasure/synologyddnsupdater:1.0.4
```

## Running the service

### Run from Visual Studio

After opening the solution file in Visual Studio, select the `Synology.Ddns.Update.Service` project as the startup
project. The, from the **Run** drop down menu, select any of the available options. Profiles have the following
attributes:

| Profile name segment | Description                                                                                     |
|----------------------|-------------------------------------------------------------------------------------------------|
| Project              | Runs the service on the host computer.                                                          |
| Docker               | Runs the service in a Docker container.                                                         |
| https                | Runs the service using https using the dev certificate.                                         |
| http                 | Runs the service using http without TSL\SSL.                                                    |
| Mock                 | Runs the service using the mock Namecheap DDNS client. No real calls to Namecheap will be made. |
| Real                 | Runs the service using the real Namecheap DDNS client. Real calls will be made to Namecheap.    |

One thing worth mentioning is that when Visual Studio builds the Docker container, it is configured to use a traditional
ASP.NET Core container image for the base, which is not the default.

### Run in Docker

The easiest way to run the container in Docker is to execute it from [Visual Studio](#run-from-visual-studio) as it
already handles a lot of things for you. But, not everyone uses Visual Studio and might find it more educational to run
the service manually.

Make sure you've installed the following pre-requisites:

- [Docker][docker]
  - [Docker Desktop][docker-desktop] recommended
- [.NET 7.0 SDK][dotnet-7-sdk-download]

All commands are expected to be run from the root of the repository. You'll also find that these instructions were
derived from [these docs][aspnetcore-docker-https].

### Run in Docker using https

1. Configure the dev certificates.

    In Powershell:

    ```powershell
    $certPassword = [System.Guid]::NewGuid()
    dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\aspnetapp.pfx -p $certPassword
    dotnet dev-certs https --trust
    ```

    In Bash:

    ```bash
    dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p <CERT_PASSWORD>
    dotnet dev-certs https --trust
    ```

2. Build and run the container by running:

    In PowerShell (assumes variables from previous PowerShell commands):

    ```powershell
    docker build -f ./src/Synology.Ddns.Update.Service/Dockerfile --force-rm -t synologyddnsupdater:local .
    docker run -it --rm -p 58080:8080 -p 55443:5443 -e ASPNETCORE_ENVIRONMENT=Development -e ASPNETCORE_Kestrel__Certificates__Default__Password=$certPassword -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx -v $env:USERPROFILE/.aspnet/https:/https/ synologyddnsupdater:local
    ```

    In Bash (using CERT_PASSWORD from previous Bash commands):

    ```bash
    docker build -f ./src/Synology.Ddns.Update.Service/Dockerfile --force-rm -t synologyddnsupdater:local .
    docker run --rm -it -p 58080:8080 -p 55443:5443 -e ASPNETCORE_ENVIRONMENT=Development -e ASPNETCORE_Kestrel__Certificates__Default__Password="<CERT_PASSWORD>" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx -v ${HOME}/.aspnet/https:/https/ synologyddnsupdater:local
    ```

3. Navigate to the web page at <https://localhost:55443/swagger>.
4. Press "Ctrl+C" to close and remove the container.

### Run in Docker using http

1. Build and run the container by running:

    In PowerShell or Bash:

    ```powershell
    docker build -f ./src/Synology.Ddns.Update.Service/Dockerfile --force-rm -t synologyddnsupdater:local .
    docker run --rm -it -p 58080:8080 -e ASPNETCORE_ENVIRONMENT=Development -e ASPNETCORE_URLS=http://+:8080 synologyddnsupdater:local
    ```

2. Navigate to the web page at <http://localhost:58080/swagger>.
3. Press "Ctrl+C" to close and remove the container.

The real key here is to set the `ASPNETCORE_URLS` environment variable and override the default set in the container
that includes an https port.

[aspnetcore-configuration]: https://learn.microsoft.com/aspnet/core/fundamentals/configuration/ "Configuration in ASP.NET Core"
[aspnetcore-docker-https]: https://learn.microsoft.com/aspnet/core/security/docker-https "Hosting ASP.NET Core images with Docker over HTTPS"
[aspnetcore-https-port]: https://learn.microsoft.com/aspnet/core/fundamentals/host/web-host#https-port "HTTPS Port"
[aspnetcore-ratelimiting]: https://learn.microsoft.com/aspnet/core/performance/rate-limit "Rate limiting middleware in ASP.NET Core"
[aspnetcore-server-urls]: https://learn.microsoft.com/aspnet/core/fundamentals/host/web-host#server-urls "Server URLs"
[aspnetcore-server-urls-andrewlock]: https://andrewlock.net/5-ways-to-set-the-urls-for-an-aspnetcore-app/ "5 ways to set the URLs for an ASP.NET Core app"
[docker]: https://www.docker.com/ "Docker"
[docker-desktop]: https://www.docker.com/products/docker-desktop/ "Docker Desktop"
[docker-image]: https://hub.docker.com/r/craigktreasure/synologyddnsupdater "craigktreasure/synologyddnsupdater"
[dockerfile]: /src/Synology.Ddns.Update.Service/Dockerfile "Dockerfile"
[dotnet-7-sdk-download]: https://dotnet.microsoft.com/en-us/download/dotnet/7.0 "Download .NET 7.0"
[globalratelimiteroptions]: /src/Synology.Ddns.Update.Service/Options/GlobalRateLimiterOptions.cs "GlobalRateLimiterOptions.cs"
[opentelemetry]: https://opentelemetry.io/ "OpenTelemetry"
[create-pfx-openssl]: https://www.ssl.com/how-to/create-a-pfx-p12-certificate-file-using-openssl/ "Create a .pfx/.p12 Certificate File Using OpenSSL"
