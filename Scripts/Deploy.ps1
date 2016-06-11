param (
    $Frontend = "$PSScriptRoot/../ForneverMind.Frontend",

    $npm = 'npm',
    $nuget = 'NuGet',
    $msbuild = 'msbuild'
)

$ErrorActionPreference = 'Stop'

Push-Location $Frontend
try {
    & $npm install
    if (-not $?) {
        exit $LASTEXITCODE
    }

    & $npm run optimize
    if (-not $?) {
        exit $LASTEXITCODE
    }
} finally {
    Pop-Location
}

& $nuget restore
if (-not $?) {
    exit $LASTEXITCODE
}

& $msbuild /p:Platform="Any CPU" /p:Configuration=Release /p:DeployBackend=true /p:PublishProfile=Production ForneverMind.sln
exit $LASTEXITCODE
