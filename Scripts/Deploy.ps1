param (
    $nuget = 'NuGet',
    $msbuild = 'msbuild'
)

& $nuget restore
if (-not $?) {
    return $LASTEXITCODE
}

& $msbuild /p:Platform="Any CPU" /p:Configuration=Release /p:ProductionDeploy=true /p:PublishProfile=Production ForneverMind.sln
return $LASTEXITCODE
