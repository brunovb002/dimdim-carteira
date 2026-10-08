# Gera publish.zip com caminhos internos usando "/" (o Compress-Archive do PowerShell usa "\",
# o que quebra o rsync do App Service no Linux). Rode a partir da raiz do repositório:
#   dotnet publish src/DimDim.Web -c Release -o publish
#   powershell -ExecutionPolicy Bypass -File scripts/make-zip.ps1

Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem

$repoRoot = Split-Path -Parent $PSScriptRoot
$sourceDir = Join-Path $repoRoot "publish"
$zipPath = Join-Path $repoRoot "publish.zip"
$backslash = [char]92
$slash = [char]47

if (Test-Path $zipPath) { Remove-Item -Path $zipPath -Force }

$zipStream = [System.IO.File]::Open($zipPath, [System.IO.FileMode]::Create)
$archive = New-Object System.IO.Compression.ZipArchive($zipStream, [System.IO.Compression.ZipArchiveMode]::Create)

$files = Get-ChildItem -Path $sourceDir -Recurse -File
foreach ($file in $files) {
    $relativePath = $file.FullName.Substring($sourceDir.Length + 1)
    $relativePath = $relativePath.Replace($backslash, $slash)
    $entry = $archive.CreateEntry($relativePath, [System.IO.Compression.CompressionLevel]::Optimal)
    $entryStream = $entry.Open()
    $fileStream = [System.IO.File]::OpenRead($file.FullName)
    $fileStream.CopyTo($entryStream)
    $fileStream.Close()
    $entryStream.Close()
}

$archive.Dispose()
$zipStream.Close()
Write-Host "Zip created with $($files.Count) files"
