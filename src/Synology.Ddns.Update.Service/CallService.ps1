[CmdletBinding()]
param (
    [string] $HostName = '@',

    [Parameter(Mandatory = $true)]
    [string] $DomainName,

    [Parameter(Mandatory = $true)]
    [string] $DdnsPassword,

    [string] $Ip,

    [string] $ServiceHost = 'http://localhost:5064'
)

$ErrorActionPreference = 'Stop'

$url = "$ServiceHost/namecheap/ddns/update?host=$HostName&domain=$DomainName&password=$DdnsPassword&ip=$Ip"

Invoke-WebRequest -Uri $url -Method Get
