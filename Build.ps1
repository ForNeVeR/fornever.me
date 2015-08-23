param (
    $stack = 'stack'
)

& $stack setup
if (-not $?) {
    return $LASTEXITCODE
}

& $stack build
if (-not $?) {
    return $LASTEXITCODE
}

& $stack exec forneverme build
return $LASTEXITCODE
