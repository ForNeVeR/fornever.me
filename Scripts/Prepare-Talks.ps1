param (
    [string] $yarn = 'yarn',
    [string] $RepoPath = "$PSScriptRoot/../ForneverMind/wwwroot/talks"
)

$ErrorActionPreference = 'Stop'

function log($text, $header) {
    if (-not $header) {
        $header = 'Prepare-Talks'
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

function execIgnoreStdErr($command) {
    log "$command $args" 'Prepare-Talks.execIgnoreStdErr'
    cmd /c "$command $args 2>&1"
    if (!$?) {
        throw "[build error] $command $args = $LASTEXITCODE"
    }
}

function yarnInstall($name, $flags) {
    $output = "$RepoPath/$name"
    Push-Location $output
    try {
        log "Installing $name"
        exec $yarn install $flags
    } finally {
        Pop-Location
    }
}

function npmInstall($name) {
    $output = "$RepoPath/$name"
    Push-Location $output
    try {
        log "Installing $name"
        execIgnoreStdErr npm install
    } finally {
        Pop-Location
    }
}

yarnInstall modern-programming --ignore-engines
yarnInstall net-core-slides
yarnInstall git-basics
npmInstall talk-javascriptservices
