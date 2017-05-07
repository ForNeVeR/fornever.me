param (
    $HttpPort = 5001,
    [switch] $Detach
)

$ErrorActionPreference = 'Stop'

Push-Location $PSScriptRoot
try {
    Remove-Item -Force -Recurse out -ErrorAction Ignore
    Copy-Item -Recurse ../../ForneverMind/out

    $env:fornevermind_web_port = $HttpPort
    Write-Output "fornevermind_web_port = $HttpPort"

    $arguments = @('--project-name', 'fornevermind', 'up', '--build', '--force-recreate')
    if ($Detach) {
        $arguments += '-d'
    }

    Write-Output "=> docker-compose $arguments"
    docker-compose $arguments
} finally {
    Pop-Location
}
