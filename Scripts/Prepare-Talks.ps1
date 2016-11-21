param (
    [string] $yarn = "$PSScriptRoot/../node_modules/.bin/yarn.cmd",
    [string] $TargetPath = "$PSScriptRoot/../ForneverMind/talks"
)

$ErrorActionPreference = 'Stop'

function modern-programming {
    $output = "$TargetPath/modern-programming"
    Push-Location $output
    try {
        Start-Process $yarn install, --ignore-engines -NoNewWindow -Wait
    } finally {
        Pop-Location
    }
}

if (!(Test-Path $TargetPath)) {
    New-Item -ItemType Directory $TargetPath
}

modern-programming
