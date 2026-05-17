<#
.SYNOPSIS
  Generates the pre-calculated data for the interactive solutions web page.

.DESCRIPTION
  Reads web-export.config (next to this script), builds the solution, then
  drives the console application over piped stdin (one 'webexport' command per
  configured combination) and captures its stdout. Each emitted JSON block is
  annotated with the per-line comment from the config, grouped by test-cases
  file, and written to web/data/<testCases>.json. An aggregate
  web/data/index.json is also produced. A PASS/FAIL summary is printed.

  Re-run this whenever you change web-export.config or the model.
#>
[CmdletBinding()]
param(
    [string]$ConfigPath,
    [string]$OutputDir,
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

# This script and web-export.config live in web/; the data folder is web/data.
$repoRoot = Split-Path -Parent $PSScriptRoot
if (-not $ConfigPath) { $ConfigPath = Join-Path $PSScriptRoot "web-export.config" }
if (-not $OutputDir)  { $OutputDir  = Join-Path $PSScriptRoot "data" }

$solutionPath = Join-Path $repoRoot "Pawelsberg.GeneticNeuralNetwork\Pawelsberg.GeneticNeuralNetwork.sln"
$consoleDll   = Join-Path $repoRoot ("Pawelsberg.GeneticNeuralNetwork\Pawelsberg.GeneticNeuralNetworkConsole\bin\{0}\net6.0\Pawelsberg.GeneticNeuralNetworkConsole.dll" -f $Configuration)

function Write-Utf8NoBom([string]$path, [string]$content) {
    $encoding = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText($path, $content, $encoding)
}

# Windows PowerShell 5.1's ConvertTo-Json pretty-printer is irregular (4-space,
# value-aligned) and has no indentation option, so re-indent a -Compress string
# ourselves. String contents are passed through untouched.
function Format-Json([string]$Json, [int]$IndentSize = 1) {
    $indentUnit = " " * $IndentSize
    $sb = New-Object System.Text.StringBuilder
    $indent = 0
    $inString = $false
    $escaped = $false
    $len = $Json.Length

    for ($i = 0; $i -lt $len; $i++) {
        $c = [string]$Json[$i]

        if ($inString) {
            [void]$sb.Append($c)
            if ($escaped) { $escaped = $false }
            elseif ($c -eq '\') { $escaped = $true }
            elseif ($c -eq '"') { $inString = $false }
            continue
        }

        switch ($c) {
            '"' { $inString = $true; [void]$sb.Append($c) }
            '{' {
                if ($i + 1 -lt $len -and [string]$Json[$i + 1] -eq '}') { [void]$sb.Append('{}'); $i++ }
                else { $indent++; [void]$sb.Append("{`n" + ($indentUnit * $indent)) }
            }
            '[' {
                if ($i + 1 -lt $len -and [string]$Json[$i + 1] -eq ']') { [void]$sb.Append('[]'); $i++ }
                else { $indent++; [void]$sb.Append("[`n" + ($indentUnit * $indent)) }
            }
            '}' { $indent--; [void]$sb.Append("`n" + ($indentUnit * $indent) + '}') }
            ']' { $indent--; [void]$sb.Append("`n" + ($indentUnit * $indent) + ']') }
            ',' { [void]$sb.Append(",`n" + ($indentUnit * $indent)) }
            ':' { [void]$sb.Append(': ') }
            default { [void]$sb.Append($c) }
        }
    }

    return $sb.ToString()
}

# --- 1. Parse the configuration ---------------------------------------------
if (-not (Test-Path $ConfigPath)) { throw "Config file not found: $ConfigPath" }

$combos = @()
foreach ($rawLine in [System.IO.File]::ReadAllLines($ConfigPath)) {
    $line = $rawLine.Trim()
    if ($line.Length -eq 0) { continue }
    if ($line.StartsWith("#")) { continue }

    $hashIndex = $line.IndexOf("#")
    $comment = ""
    $settings = $line
    if ($hashIndex -ge 0) {
        $comment = $line.Substring($hashIndex + 1).Trim()
        $settings = $line.Substring(0, $hashIndex).Trim()
    }

    $tokens = $settings -split "\s+"
    if ($tokens.Count -lt 3) { throw "Invalid config line (expected: testCases network propagations): $rawLine" }

    $combos += [PSCustomObject]@{
        TestCases    = $tokens[0]
        Network      = $tokens[1]
        Propagations = [int]$tokens[2]
        Comment      = $comment
    }
}

if ($combos.Count -eq 0) { throw "No combinations configured in $ConfigPath" }
Write-Host ("Configured combinations: {0}" -f $combos.Count)

# --- 2. Build the solution --------------------------------------------------
Write-Host "Building solution ($Configuration)..."
dotnet build $solutionPath -c $Configuration -v q -nologo
if ($LASTEXITCODE -ne 0) { throw "Build failed." }
if (-not (Test-Path $consoleDll)) { throw "Console assembly not found: $consoleDll" }

# --- 3. Drive the console over piped stdin ----------------------------------
$commandLines = @("init")
foreach ($combo in $combos) {
    $commandLines += ("webexport {0} {1} {2}" -f $combo.TestCases, $combo.Network, $combo.Propagations)
}
$commandLines += "quit"

Write-Host "Running console exporter..."
$consoleOutput = $commandLines | & dotnet $consoleDll
if ($LASTEXITCODE -ne 0) { throw "Console exporter exited with code $LASTEXITCODE." }

# --- 4. Extract JSON blocks (one line each, after each begin marker) ---------
$beginRegex = 'WEBEXPORT begin testCases=(\S+) network=(\S+)>>>'
$blocks = @{}
for ($i = 0; $i -lt $consoleOutput.Count; $i++) {
    $matchInfo = [regex]::Match($consoleOutput[$i], $beginRegex)
    if (-not $matchInfo.Success) { continue }
    $blockKey = "{0}|{1}" -f $matchInfo.Groups[1].Value, $matchInfo.Groups[2].Value
    $jsonLine = $consoleOutput[$i + 1]
    $blocks[$blockKey] = $jsonLine
}

# --- 5. Group results by test-cases file and attach comments ----------------
$byTestCases = [ordered]@{}
$summary = @()
foreach ($combo in $combos) {
    $blockKey = "{0}|{1}" -f $combo.TestCases, $combo.Network
    if (-not $blocks.ContainsKey($blockKey)) {
        $summary += [PSCustomObject]@{ Combo = $blockKey; Status = "NO OUTPUT"; Detail = "console produced no block" }
        continue
    }

    $parsed = $blocks[$blockKey] | ConvertFrom-Json

    $passCount = 0
    foreach ($result in $parsed.testCaseResults) { if ($result.pass) { $passCount++ } }
    $totalCount = $parsed.testCaseResults.Count
    $status = if ($parsed.solves) { "PASS" } else { "FAIL" }
    $summary += [PSCustomObject]@{
        Combo  = $blockKey
        Status = $status
        Detail = ("{0}/{1} cases pass, propagations={2}" -f $passCount, $totalCount, $combo.Propagations)
    }

    $networkEntry = [PSCustomObject]@{
        network         = $parsed.network
        propagations    = $parsed.propagations
        solves          = $parsed.solves
        comment         = $combo.Comment
        networkText     = $parsed.networkText
        nodes           = $parsed.nodes
        synapses        = $parsed.synapses
        testCaseResults = $parsed.testCaseResults
    }

    if (-not $byTestCases.Contains($combo.TestCases)) {
        $byTestCases[$combo.TestCases] = New-Object System.Collections.ArrayList
    }
    [void]$byTestCases[$combo.TestCases].Add($networkEntry)
}

# --- 6. Write per-test-cases JSON files and the index -----------------------
if (-not (Test-Path $OutputDir)) { New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null }

$indexEntries = @()
foreach ($testCasesName in $byTestCases.Keys) {
    $networkEntries = $byTestCases[$testCasesName]
    $fileName = "$testCasesName.json"

    $document = [PSCustomObject]@{
        testCases = $testCasesName
        networks  = @($networkEntries)
    }
    Write-Utf8NoBom (Join-Path $OutputDir $fileName) (Format-Json ($document | ConvertTo-Json -Depth 40 -Compress) 1)

    $networkNames = @()
    foreach ($entry in $networkEntries) { $networkNames += $entry.network }
    $indexEntries += [PSCustomObject]@{
        testCases = $testCasesName
        file      = $fileName
        networks  = $networkNames
    }
}

$index = [PSCustomObject]@{
    generatedUtc = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
    entries      = @($indexEntries)
}
Write-Utf8NoBom (Join-Path $OutputDir "index.json") (Format-Json ($index | ConvertTo-Json -Depth 10 -Compress) 1)

# --- 7. Summary -------------------------------------------------------------
Write-Host ""
Write-Host "Summary:"
foreach ($row in $summary) {
    Write-Host ("  [{0}] {1} - {2}" -f $row.Status, $row.Combo, $row.Detail)
}
Write-Host ""
Write-Host ("Wrote {0} data file(s) + index.json to {1}" -f $byTestCases.Count, $OutputDir)
