# SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
#
# SPDX-License-Identifier: MIT

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

function assertNotEmpty() {
    $items = Get-ChildItem .
    if (-not $items) {
        throw "[assertion error] Location $PWD is empty"
    }
}

function yarnInstall($name, $flags) {
    $output = "$RepoPath/$name"
    Push-Location $output
    try {
        log "Installing $name"
        assertNotEmpty
        & $yarn install $flags
        if ($LASTEXITCODE -ne 0) {
            throw "[build error] $yarn install $flags = $LASTEXITCODE"
        }
    } finally {
        Pop-Location
    }
}

function npmInstall($name) {
    $output = "$RepoPath/$name"
    Push-Location $output
    $ErrorActionPreference = 'Continue'
    try {
        log "Installing $name"
        assertNotEmpty
        npm install
        if ($LASTEXITCODE -ne 0) {
            throw "[build error] npm install = $LASTEXITCODE"
        }
    } finally {
        $ErrorActionPreference = 'Stop'
        Pop-Location
    }
}

yarnInstall modern-programming --ignore-engines
yarnInstall net-core-slides
yarnInstall git-basics
npmInstall talk-javascriptservices
