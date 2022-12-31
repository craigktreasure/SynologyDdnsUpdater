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

$PSNativeCommandErrorActionPreference = $true
$ErrorActionPreference = 'Stop'


$repoRoot = & git rev-parse --show-toplevel
$projectFolder = $PSScriptRoot

Push-Location $projectFolder
try {
    Write-Host 'Restoring tools...' -ForegroundColor Magenta
    dotnet tool restore
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
    }

    $context = Join-Path $repoRoot "__publish/$Configuration/AnyCPU/src/Synology.Ddns.Update.Service/net7.0"

    if (-not (Test-Path $context)) {
        throw "Expected publish output was not found: $context"
    }

    Write-Host 'Building the container using host built application...' -ForegroundColor Magenta

    docker build -f $projectFolder/Dockerfile --build-arg finalStage=fromlocal --force-rm -t $tag $context
}
else {
    $context = $repoRoot

    Write-Host 'Building the container using container built application...' -ForegroundColor Magenta

    docker build -f $projectFolder/Dockerfile --force-rm -t $tag $context
}

Write-Host "Built and tagged the container: $tag" -ForegroundColor Magenta

Write-Host 'Done' -ForegroundColor Green
