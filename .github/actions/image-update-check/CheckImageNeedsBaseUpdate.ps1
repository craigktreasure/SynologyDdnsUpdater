#Requires -PSEdition Core

<#
.SYNOPSIS
Checks if the image needs to be updated due to changes in the specified base image.
Returns `true` if the image needs to be updated and `false` if the image is up to date.
.PARAMETER BaseImageName
The name of the base image to check against.
.PARAMETER ImageName
The name of the image to check.
.PARAMETER CheckDateCreated
If specified, the image creation date will be checked to ensure the image was created after the base image.
#>
[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)]
    [string] $BaseImageName,

    [Parameter(Mandatory = $true)]
    [string] $ImageName,

    [switch] $CheckDateCreated
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

function CheckLastExitCode($message) {
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Command failed with $LASTEXITCODE. $message"
    }
}

function PullImage($imageName) {
    Write-Host "Pulling image $imageName..." -ForegroundColor Magenta
    docker pull $imageName
    CheckLastExitCode "Failed to pull image $imageName"
}

function GetDateCreated($imageName) {
    Write-Host "Getting image creation date for $imageName..." -ForegroundColor Magenta
    $imageCreated = docker image inspect $imageName --format='{{.Created}}'
    CheckLastExitCode "Failed to get image creation date for $imageName"
    return $imageCreated
}

function CompareImageCreationDates($baseImageName, $imageName) {
    Write-Host 'Comparing image creation dates...' -ForegroundColor Magenta

    $baseImageCreated = GetDateCreated $baseImageName
    $imageCreated = GetDateCreated $imageName

    $baseImageCreatedDate = [DateTime]::Parse($baseImageCreated)
    $imageCreatedDate = [DateTime]::Parse($imageCreated)

    if ($baseImageCreatedDate -gt $imageCreatedDate) {
        Write-Verbose "Base image was created on $baseImageCreatedDate, but image was created on $imageCreatedDate."

        return $false
    }

    return $true
}

function GetImageLayers($imageName) {
    Write-Host "Getting image layers for $imageName..." -ForegroundColor Magenta
    $imageLayers = docker image inspect $imageName --format='{{range .RootFS.Layers}}{{println .}}{{end}}'
    CheckLastExitCode "Failed to get image layers for $imageName"
    return $imageLayers | Where-Object { $_ -ne '' }
}

function CheckImageContainsBaseLayers($baseImageLayers, $imageLayers) {
    Write-Host 'Comparing image layers...' -ForegroundColor Magenta

    for ($i = 0; $i -lt $baseImageLayers.Length; $i++) {
        if ($baseImageLayers[$i] -ne $imageLayers[$i]) {
            Write-Verbose "Image layers do not match. Base image layer $i is $($baseImageLayers[$i]), but image layer $i is $($imageLayers[$i])."

            return $false
        }
    }

    return $true
}

PullImage $BaseImageName
PullImage $ImageName

if ($CheckDateCreated) {
    if (CompareImageCreationDates $BaseImageName $ImageName) {
        Write-Host 'Image was created after base image.' -ForegroundColor Green
    }
    else {
        Write-Host 'Image was created before base image. Image needs an update.' -ForegroundColor Red

        return $true
    }
}

$baseImageLayers = GetImageLayers $BaseImageName
$imageLayers = GetImageLayers $ImageName

$imageIsUpToDate = CheckImageContainsBaseLayers $baseImageLayers $imageLayers

if ($imageIsUpToDate) {
    Write-Host 'Image contains base layers. Image is up to date.' -ForegroundColor Green
}
else {
    Write-Host 'Image does NOT contain all base layers. Image needs an update.' -ForegroundColor Red
}

return !$imageIsUpToDate
