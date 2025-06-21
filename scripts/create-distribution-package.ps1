# lilToon PCSS Extension Distribution Package Creator
# Created: 2025-01-27
# Version: 1.2.0

param(
    [string]$Version = "1.2.0",
    [string]$OutputDir = "Release",
    [switch]$Comprehensive = $false
)

Write-Host "Creating lilToon PCSS Extension Distribution Package" -ForegroundColor Green
Write-Host "Version: $Version" -ForegroundColor Yellow

# Setup directories
$RootDir = Split-Path -Parent $PSScriptRoot
$TempDir = Join-Path $RootDir "temp_package"
$PackageDir = Join-Path $TempDir "com.liltoon.pcss-extension"

# Clean up existing temp directory
if (Test-Path $TempDir) {
    Remove-Item $TempDir -Recurse -Force
    Write-Host "Cleaned existing temp directory" -ForegroundColor Yellow
}

# Create package directory
New-Item -ItemType Directory -Path $PackageDir -Force | Out-Null
Write-Host "Created package directory: $PackageDir" -ForegroundColor Cyan

# Copy core files
Write-Host "Copying core files..." -ForegroundColor Blue

# Runtime scripts
$RuntimeDir = Join-Path $PackageDir "Runtime"
New-Item -ItemType Directory -Path $RuntimeDir -Force | Out-Null
Copy-Item -Path (Join-Path $RootDir "Runtime\*") -Destination $RuntimeDir -Recurse -Force
Write-Host "Runtime scripts copied (6 files)" -ForegroundColor Green

# Editor scripts  
$EditorDir = Join-Path $PackageDir "Editor"
New-Item -ItemType Directory -Path $EditorDir -Force | Out-Null
Copy-Item -Path (Join-Path $RootDir "Editor\*") -Destination $EditorDir -Recurse -Force
Write-Host "Editor scripts copied (9 files)" -ForegroundColor Green

# Shaders
$ShadersDir = Join-Path $PackageDir "Shaders"
New-Item -ItemType Directory -Path $ShadersDir -Force | Out-Null
Copy-Item -Path (Join-Path $RootDir "Shaders\*") -Destination $ShadersDir -Recurse -Force
Write-Host "Shader files copied (2 files)" -ForegroundColor Green

# Configuration files
Copy-Item -Path (Join-Path $RootDir "package.json") -Destination $PackageDir -Force
Copy-Item -Path (Join-Path $RootDir "VPMManifest.json") -Destination $PackageDir -Force
Write-Host "Configuration files copied" -ForegroundColor Green

# Documentation files
Copy-Item -Path (Join-Path $RootDir "README.md") -Destination $PackageDir -Force
Copy-Item -Path (Join-Path $RootDir "CHANGELOG.md") -Destination $PackageDir -Force

# Additional documentation for comprehensive version
if ($Comprehensive) {
    Write-Host "Adding comprehensive documentation..." -ForegroundColor Blue
    Copy-Item -Path (Join-Path $RootDir "VCC_Setup_Guide.md") -Destination $PackageDir -Force
    Copy-Item -Path (Join-Path $RootDir "PCSS_ModularAvatar_Guide.md") -Destination $PackageDir -Force
    Copy-Item -Path (Join-Path $RootDir "PCSS_Poiyomi_VRChat_Guide.md") -Destination $PackageDir -Force
    Copy-Item -Path (Join-Path $RootDir "VRC_Light_Volumes_Integration_Guide.md") -Destination $PackageDir -Force
    Write-Host "Comprehensive documentation added" -ForegroundColor Green
}

# Create release notes
$ReleaseNotesContent = "# lilToon PCSS Extension v$Version - Release Notes`n`n"
$ReleaseNotesContent += "## Package Contents`n`n"
$ReleaseNotesContent += "### Runtime Scripts (6 files)`n"
$ReleaseNotesContent += "- LilToonCompatibilityManager.cs`n"
$ReleaseNotesContent += "- PCSSUtilities.cs`n"
$ReleaseNotesContent += "- VRChatPerformanceOptimizer.cs`n"
$ReleaseNotesContent += "- VRCLightVolumesIntegration.cs`n"
$ReleaseNotesContent += "- PoiyomiPCSSIntegration.cs`n"
$ReleaseNotesContent += "- lilToon.PCSS.Runtime.asmdef`n`n"
$ReleaseNotesContent += "### Editor Scripts (9 files)`n"
$ReleaseNotesContent += "- VCCSetupWizard.cs`n"
$ReleaseNotesContent += "- VRChatOptimizationSettings.cs`n"
$ReleaseNotesContent += "- BOOTHPackageExporter.cs`n"
$ReleaseNotesContent += "- VRChatExpressionMenuCreator.cs`n"
$ReleaseNotesContent += "- VRCLightVolumesEditor.cs`n"
$ReleaseNotesContent += "- PoiyomiPCSSShaderGUI.cs`n"
$ReleaseNotesContent += "- LilToonPCSSShaderGUI.cs`n"
$ReleaseNotesContent += "- LilToonPCSSExtensionInitializer.cs`n"
$ReleaseNotesContent += "- lilToon.PCSS.Editor.asmdef`n`n"
$ReleaseNotesContent += "### Shader Files (2 files)`n"
$ReleaseNotesContent += "- lilToon_PCSS_Extension.shader`n"
$ReleaseNotesContent += "- Poiyomi_PCSS_Extension.shader`n`n"
$ReleaseNotesContent += "## Installation`n`n"
$ReleaseNotesContent += "1. Open VRChat Creator Companion (VCC)`n"
$ReleaseNotesContent += "2. Settings -> Packages -> Add Repository`n"
$ReleaseNotesContent += "3. URL: https://zapabob.github.io/liltoon-pcss-extension/index.json`n"
$ReleaseNotesContent += "4. Install lilToon PCSS Extension in your project`n`n"
$ReleaseNotesContent += "Created: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')`n"

$ReleaseNotesPath = Join-Path $PackageDir "RELEASE_NOTES.md"
$ReleaseNotesContent | Out-File -FilePath $ReleaseNotesPath -Encoding UTF8
Write-Host "Release notes created" -ForegroundColor Green

# Create samples directory for comprehensive version
if ($Comprehensive) {
    $SamplesDir = Join-Path $PackageDir "Samples~"
    New-Item -ItemType Directory -Path $SamplesDir -Force | Out-Null
    
    # Sample materials directory
    $MaterialsDir = Join-Path $SamplesDir "PCSSMaterials"
    New-Item -ItemType Directory -Path $MaterialsDir -Force | Out-Null
    
    # Sample materials README
    $MaterialReadmeContent = "# PCSS Sample Materials`n`n"
    $MaterialReadmeContent += "This directory contains sample materials for lilToon PCSS Extension.`n`n"
    $MaterialReadmeContent += "## Included Materials`n`n"
    $MaterialReadmeContent += "- PCSS_Standard.mat - Standard PCSS settings`n"
    $MaterialReadmeContent += "- PCSS_Cutout.mat - Cutout material settings`n"
    $MaterialReadmeContent += "- PCSS_Transparent.mat - Transparent material settings`n"
    $MaterialReadmeContent += "- PCSS_Emission.mat - Emission material settings`n"
    $MaterialReadmeContent += "- PCSS_Toon.mat - Toon shading settings`n`n"
    $MaterialReadmeContent += "## Usage`n`n"
    $MaterialReadmeContent += "1. Select material in Unity Editor`n"
    $MaterialReadmeContent += "2. Check PCSS settings in Inspector`n"
    $MaterialReadmeContent += "3. Adjust parameters as needed`n"
    $MaterialReadmeContent += "4. Apply to VRChat avatar`n"
    
    $MaterialReadmeContent | Out-File -FilePath (Join-Path $MaterialsDir "README.md") -Encoding UTF8
    Write-Host "Sample materials directory created" -ForegroundColor Green
}

# Create ZIP file
Write-Host "Creating ZIP file..." -ForegroundColor Blue

$OutputDirPath = Join-Path $RootDir $OutputDir
if (!(Test-Path $OutputDirPath)) {
    New-Item -ItemType Directory -Path $OutputDirPath -Force | Out-Null
}

$ZipSuffix = if ($Comprehensive) { "-comprehensive" } else { "" }
$ZipFileName = "com.liltoon.pcss-extension-$Version$ZipSuffix.zip"
$ZipPath = Join-Path $OutputDirPath $ZipFileName

# Remove existing ZIP file
if (Test-Path $ZipPath) {
    Remove-Item $ZipPath -Force
    Write-Host "Removed existing ZIP file" -ForegroundColor Yellow
}

# Create ZIP
Compress-Archive -Path "$PackageDir\*" -DestinationPath $ZipPath -CompressionLevel Optimal
Write-Host "ZIP file created: $ZipFileName" -ForegroundColor Green

# Get file size
$FileSize = (Get-Item $ZipPath).Length
$FileSizeKB = [math]::Round($FileSize / 1KB, 2)
$FileSizeMB = [math]::Round($FileSize / 1MB, 2)

Write-Host "Package Information:" -ForegroundColor Magenta
Write-Host "  Filename: $ZipFileName" -ForegroundColor White
Write-Host "  Size: $FileSizeKB KB ($FileSizeMB MB)" -ForegroundColor White
Write-Host "  Path: $ZipPath" -ForegroundColor White

# Calculate SHA256 hash
$Hash = Get-FileHash $ZipPath -Algorithm SHA256
Write-Host "  SHA256: $($Hash.Hash)" -ForegroundColor White

# Update checksums file
$ChecksumPath = Join-Path $OutputDirPath "checksums.txt"
$ChecksumContent = "$($Hash.Hash)  $ZipFileName"
Add-Content -Path $ChecksumPath -Value $ChecksumContent -Encoding UTF8
Write-Host "Checksums file updated" -ForegroundColor Green

# Clean up temp directory
Remove-Item $TempDir -Recurse -Force
Write-Host "Temp directory cleaned up" -ForegroundColor Yellow

Write-Host ""
Write-Host "Distribution package creation completed!" -ForegroundColor Green
Write-Host "Output: $ZipPath" -ForegroundColor Cyan
Write-Host "Size: $FileSizeKB KB" -ForegroundColor Cyan

Write-Host ""
Write-Host "VPM Repository Update Information:" -ForegroundColor Yellow
Write-Host "  URL: https://zapabob.github.io/liltoon-pcss-extension/Release/$ZipFileName" -ForegroundColor White
Write-Host "  SHA256: $($Hash.Hash)" -ForegroundColor White

Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Blue
Write-Host "1. Update docs/index.json with new SHA256 hash" -ForegroundColor White
Write-Host "2. Copy ZIP file to docs/Release/ directory" -ForegroundColor White  
Write-Host "3. Push to GitHub Pages for distribution" -ForegroundColor White 