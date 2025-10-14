param(
    [string]$BaseUrl = "https://synologyddnsupdater.azurewebsites.net",
    [string]$HostParam = "@",
    [string]$Domain = "example.com",
    [string]$Password = "dummy",
    [string]$Ip = "1.1.1.1"
)

# Test root endpoint
$rootUrl = "$BaseUrl/"

try {
    $response = Invoke-WebRequest -Uri $rootUrl -Method Get
    Write-Host "Root Endpoint - Status Code: $($response.StatusCode)"
    Write-Host "Root Endpoint - Response: $($response.Content)"
} catch {
    Write-Host "Root Endpoint - Error: $($_.Exception.Message)"
}

Write-Host ""

# Test DDNS update endpoint
$url = "$BaseUrl/namecheap/ddns/update?host=$HostParam&domain=$Domain&password=$Password&ip=$Ip"

try {
    $response = Invoke-WebRequest -Uri $url -Method Get
    Write-Host "DDNS Update Endpoint - Status Code: $($response.StatusCode)"
    Write-Host "DDNS Update Endpoint - Response: $($response.Content)"
} catch {
    Write-Host "DDNS Update Endpoint - Error: $($_.Exception.Message)"
}