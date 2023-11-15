[CmdletBinding(DefaultParameterSetName = 'local-build')]
param (
    [Parameter(ParameterSetName = 'container-build')]
    [switch] $UseContainerBuild,

    [Parameter(ParameterSetName = 'local-build')]
    [switch] $SkipPublish,

    [Parameter(ParameterSetName = 'local-build')]
    [string] $Configuration = 'Release',

    [Parameter(ParameterSetName = 'local-build')]
    [Parameter(ParameterSetName = 'container-build')]
    [string] $RegistryName,

    [Parameter(ParameterSetName = 'local-build')]
    [Parameter(ParameterSetName = 'container-build')]
    [string] $RepositoryName = 'synologyddnsupdater',

    [Parameter(ParameterSetName = 'local-build')]
    [Parameter(ParameterSetName = 'container-build')]
    [string] $TagSuffix
)

$ErrorActionPreference = 'Stop'

$repoRoot = & git rev-parse --show-toplevel
$projectFolder = $PSScriptRoot

function CheckLastExitCode($message) {
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Command failed with $LASTEXITCODE. $message"
    }
}

Push-Location $projectFolder
try {
    Write-Host 'Restoring tools...' -ForegroundColor Magenta
    dotnet tool restore
    CheckLastExitCode 'Restore failed.'
}
finally {
    Pop-Location
}

$version = & dotnet nbgv get-version --variable NuGetPackageVersion

Write-Host "Got version: $version" -ForegroundColor Cyan

$tag = "$($RepositoryName):$version"

if ($TagSuffix) {
    $tag += $TagSuffix
}

if ($RegistryName) {
    $tag = "$RegistryName/$tag"
}

Write-Host "Will use tag: $tag" -ForegroundColor Cyan

if ($PsCmdlet.ParameterSetName -eq 'local-build') {

    if (-not $SkipPublish) {
        Write-Host "Performing a publish with $Configuration configuration..." -ForegroundColor Magenta

        dotnet publish $projectFolder --configuration $Configuration
        CheckLastExitCode 'Publish failed.'
    }
    $configurationLower = $Configuration.ToLower()
    $context = Join-Path $repoRoot "__artifacts/publish/Synology.Ddns.Update.Service/$configurationLower"

    if (-not (Test-Path $context)) {
        throw "Expected publish output was not found: $context"
    }

    Write-Host 'Building the container using host built application...' -ForegroundColor Magenta

    docker build -f $projectFolder/Dockerfile --build-arg finalStage=fromlocal --force-rm -t $tag $context
    CheckLastExitCode 'Docker build failed.'
}
else {
    $context = $repoRoot

    Write-Host 'Building the container using container built application...' -ForegroundColor Magenta

    docker build -f $projectFolder/Dockerfile --force-rm -t $tag $context
    CheckLastExitCode 'Docker build failed.'
}

Write-Host "Built and tagged the container: $tag" -ForegroundColor Magenta

Write-Host 'Done' -ForegroundColor Green
