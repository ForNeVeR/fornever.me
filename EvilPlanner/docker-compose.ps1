param (
    [int16] $HttpPort = 5002,
    [switch] $Detach,
    [string] $ProjectName = 'evilplanner'
)

$ErrorActionPreference = 'Stop'

Push-Location $PSScriptRoot
try {
    $env:evilplanner_web_port = $HttpPort
    Write-Output "evilplanner_web_port = $HttpPort"

    $arguments = @('--project-name', $ProjectName, 'up', '--build', '--force-recreate')
    if ($Detach) {
        $arguments += '-d'
    }

    Write-Output "=> docker-compose $arguments"
    docker-compose $arguments
    if (-not $?) {
        throw "docker-compose returned error exit code"
    }
} finally {
    Pop-Location
}
