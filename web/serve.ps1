<#
.SYNOPSIS
  Serves this folder over http://localhost:8080/ so the page can fetch data/*.json.

.DESCRIPTION
  Opening index.html directly via file:// makes browsers block fetch() of the
  local JSON files. This minimal static file server avoids that. Press Ctrl+C
  to stop. Optionally pass -Port to change the port.
#>
[CmdletBinding()]
param([int]$Port = 8080)

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot

$contentTypes = @{
    ".html" = "text/html; charset=utf-8"
    ".js"   = "application/javascript; charset=utf-8"
    ".css"  = "text/css; charset=utf-8"
    ".json" = "application/json; charset=utf-8"
}

$listener = New-Object System.Net.HttpListener
$listener.Prefixes.Add("http://localhost:$Port/")
$listener.Start()
Write-Host "Serving $root at http://localhost:$Port/  (Ctrl+C to stop)"

try {
    while ($listener.IsListening) {
        $context = $listener.GetContext()
        $relativePath = [Uri]::UnescapeDataString($context.Request.Url.AbsolutePath.TrimStart("/"))
        if ($relativePath -eq "") { $relativePath = "index.html" }

        $fullPath = Join-Path $root $relativePath
        if ((Test-Path $fullPath -PathType Leaf) -and ($fullPath.StartsWith($root))) {
            $extension = [System.IO.Path]::GetExtension($fullPath).ToLower()
            $contentType = if ($contentTypes.ContainsKey($extension)) { $contentTypes[$extension] } else { "application/octet-stream" }
            $bytes = [System.IO.File]::ReadAllBytes($fullPath)
            $context.Response.ContentType = $contentType
            $context.Response.ContentLength64 = $bytes.Length
            $context.Response.OutputStream.Write($bytes, 0, $bytes.Length)
        } else {
            $context.Response.StatusCode = 404
        }
        $context.Response.OutputStream.Close()
    }
}
finally {
    $listener.Stop()
}
