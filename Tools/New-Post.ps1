param (
    [Parameter(Mandatory = $true)]
    [string] $FileName,

    [Parameter(Mandatory = $true)]
    [string] $Title,

    [string] $Description = '',
    [DateTime] $Date = (Get-Date),
    [string] $ProjectPath = "$PSScriptRoot/../ForneverMind/ForneverMind.fsproj"
)

$ErrorActionPreference = 'Stop'

$dateFormatted = $Date.ToString('yyyy-MM-dd')
$partialPath = "posts\$dateFormatted-$FileName.md"
$path = "$PSScriptRoot/../ForneverMind/$partialPath"

$content = @"
    title: $Title
    description: $Description
---
"@
[IO.File]::WriteAllLines($path, $content)

[xml]$project = Get-Content $ProjectPath
$contentGroup = $project.Project.ItemGroup | ? { $_.Content }
$latestPost = $contentGroup.ChildNodes | ? { $_.Include -and $_.Include.StartsWith('posts\') } | Select-Object -Last 1
$element = $project.CreateElement('Content', $project.Project.NamespaceURI)
$element.SetAttribute('Include', $partialPath)
$contentGroup.InsertAfter($element, $latestPost)
$project.Save($ProjectPath)
