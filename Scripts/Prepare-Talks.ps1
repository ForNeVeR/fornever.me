param (
    [string] $yarn = 'yarn',
    [string] $TargetPath = "$PSScriptRoot/../ForneverMind/talks"
)

$ErrorActionPreference = 'Stop'

function log($text, $header) {
    if (-not $header) {
        $header = 'Prepare-Talks.ps1'
    }
    Write-Output "[$header] $text"
}

function exec($command) {
    log "$command $args" 'Prepare-Talks.exec'
    & $command $args
    if (!$?) {
        throw "[build error] $command $args = $LASTEXITCODE"
    }
}

function yarnInstall($name, $flags) {
    $output = "$TargetPath/$name"
    Push-Location $output
    try {
        log "Installing $name"
        exec $yarn install $flags
    } finally {
        Pop-Location
    }
}

yarnInstall modern-programming --ignore-engines 
yarnInstall net-core-slides
