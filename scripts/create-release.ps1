# lilToon PCSS Extension v1.2.0 Release Creation Script
# PowerShell script for Windows environment

param(
    [string]$Version = "1.2.0",
    [string]$OutputDir = ".\Release",
    [switch]$CleanBuild = $false
)

Write-Host "🚀 lilToon PCSS Extension v$Version Release Builder" -ForegroundColor Cyan
Write-Host "=================================================" -ForegroundColor Cyan

# Set error handling
$ErrorActionPreference = "Stop"

# Clean previous build if requested
if ($CleanBuild -and (Test-Path $OutputDir)) {
    Write-Host "🧹 Cleaning previous build..." -ForegroundColor Yellow
    Remove-Item -Recurse -Force $OutputDir
}

# Create output directory
if (!(Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
}

# Define package structure
$PackageName = "com.liltoon.pcss-extension"
$PackageDir = Join-Path $OutputDir $PackageName
$ZipFile = Join-Path $OutputDir "$PackageName-$Version.zip"

Write-Host "📦 Creating package structure..." -ForegroundColor Green

# Create package directory
if (Test-Path $PackageDir) {
    Remove-Item -Recurse -Force $PackageDir
}
New-Item -ItemType Directory -Path $PackageDir | Out-Null

# Copy core files
$CoreFiles = @(
    "package.json",
    "VPMManifest.json",
    "README.md",
    "CHANGELOG.md",
    "LICENSE"
)

foreach ($file in $CoreFiles) {
    if (Test-Path $file) {
        Copy-Item $file $PackageDir -Force
        Write-Host "  ✓ Copied $file" -ForegroundColor Gray
    } else {
        Write-Warning "  ⚠ Missing file: $file"
    }
}

# Copy directories
$Directories = @(
    "Editor",
    "Runtime", 
    "Shaders"
)

foreach ($dir in $Directories) {
    if (Test-Path $dir) {
        Copy-Item $dir $PackageDir -Recurse -Force
        Write-Host "  ✓ Copied $dir/" -ForegroundColor Gray
    } else {
        Write-Warning "  ⚠ Missing directory: $dir"
    }
}

# Create Samples~ directory structure
$SamplesDir = Join-Path $PackageDir "Samples~"
New-Item -ItemType Directory -Path $SamplesDir | Out-Null

# Create sample directories
$SampleDirs = @(
    "PCSSMaterials",
    "VRChatExpressions", 
    "ModularAvatarPrefabs"
)

foreach ($sampleDir in $SampleDirs) {
    $samplePath = Join-Path $SamplesDir $sampleDir
    New-Item -ItemType Directory -Path $samplePath | Out-Null
    
    # Create placeholder README
    $readmePath = Join-Path $samplePath "README.md"
    $readmeContent = @"
# $sampleDir Sample

This sample demonstrates the usage of lilToon PCSS Extension v$Version.

## Installation

1. Import this sample through Unity Package Manager
2. Follow the setup instructions in the main documentation

## Contents

- Sample materials and prefabs
- Configuration examples
- Best practice demonstrations

For detailed documentation, visit: https://github.com/zapabob/liltoon-pcss-extension
"@
    Set-Content -Path $readmePath -Value $readmeContent -Encoding UTF8
    Write-Host "  ✓ Created sample: $sampleDir" -ForegroundColor Gray
}

# Validate package.json version
$packageJsonPath = Join-Path $PackageDir "package.json"
if (Test-Path $packageJsonPath) {
    $packageJson = Get-Content $packageJsonPath -Raw | ConvertFrom-Json
    if ($packageJson.version -ne $Version) {
        Write-Warning "Version mismatch in package.json: $($packageJson.version) vs $Version"
    }
}

# Create release notes
$ReleaseNotesPath = Join-Path $PackageDir "RELEASE_NOTES.md"
$ReleaseNotes = @"
# lilToon PCSS Extension v$Version Release Notes

## 🎉 新機能

### VCC (VRChat Creator Companion) 完全対応
- ワンクリックインストール対応
- 自動依存関係解決
- VPMリポジトリ対応

### VRC Light Volumes統合
- 次世代ボクセルライティングシステム対応
- リアルタイム光源追跡
- パフォーマンス最適化

### ModularAvatar統合強化
- 自動エクスプレッション設定
- プレハブベースの簡単導入
- 非破壊的アバター改変

## 🔧 改善点

### パフォーマンス最適化
- VRChat品質設定連動
- Quest対応強化
- メモリ使用量削減

### ユーザビリティ向上
- セットアップウィザード追加
- エラーハンドリング強化
- 詳細ドキュメント整備

## 🐛 バグ修正

- シェーダーコンパイルエラー修正
- Quest環境での安定性向上
- メモリリーク問題解決

## 📚 ドキュメント

- VCC導入ガイド追加
- トラブルシューティング強化
- API リファレンス更新

## 🔗 リンク

- GitHub: https://github.com/zapabob/liltoon-pcss-extension
- VPM Repository: https://zapabob.github.io/liltoon-pcss-extension/
- Documentation: https://github.com/zapabob/liltoon-pcss-extension/blob/main/README.md

---
**Release Date**: $(Get-Date -Format "yyyy-MM-dd")
**Build**: Automated Release Build
"@

Set-Content -Path $ReleaseNotesPath -Value $ReleaseNotes -Encoding UTF8
Write-Host "  ✓ Created release notes" -ForegroundColor Gray

# Create ZIP package
Write-Host "📦 Creating ZIP package..." -ForegroundColor Green

try {
    # Use PowerShell's Compress-Archive
    Compress-Archive -Path "$PackageDir\*" -DestinationPath $ZipFile -Force
    Write-Host "  ✓ Created: $ZipFile" -ForegroundColor Green
} catch {
    Write-Error "Failed to create ZIP package: $_"
    exit 1
}

# Validate ZIP file
if (Test-Path $ZipFile) {
    $zipInfo = Get-Item $ZipFile
    $sizeKB = [math]::Round($zipInfo.Length / 1KB, 2)
    Write-Host "  📊 Package size: $sizeKB KB" -ForegroundColor Gray
} else {
    Write-Error "ZIP file was not created successfully"
    exit 1
}

# Generate checksums
Write-Host "🔐 Generating checksums..." -ForegroundColor Green

$sha256Hash = Get-FileHash $ZipFile -Algorithm SHA256
$md5Hash = Get-FileHash $ZipFile -Algorithm MD5

$checksumFile = Join-Path $OutputDir "checksums.txt"
$checksumContent = @"
# lilToon PCSS Extension v$Version Checksums
# Generated: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")

## SHA256
$($sha256Hash.Hash.ToLower())  $PackageName-$Version.zip

## MD5  
$($md5Hash.Hash.ToLower())  $PackageName-$Version.zip

## File Information
Size: $sizeKB KB
Created: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
"@

Set-Content -Path $checksumFile -Value $checksumContent -Encoding UTF8
Write-Host "  ✓ SHA256: $($sha256Hash.Hash.ToLower())" -ForegroundColor Gray
Write-Host "  ✓ MD5: $($md5Hash.Hash.ToLower())" -ForegroundColor Gray

# Create GitHub release script
$GithubReleaseScript = Join-Path $OutputDir "github-release.ps1"
$GithubScript = @"
# GitHub Release Creation Script
# Run this after uploading the ZIP file to GitHub Releases

`$Version = "$Version"
`$ZipFile = "$PackageName-`$Version.zip"
`$ReleaseTitle = "lilToon PCSS Extension v`$Version"

Write-Host "🚀 GitHub Release Information" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host "Version: `$Version" -ForegroundColor Green
Write-Host "Title: `$ReleaseTitle" -ForegroundColor Green
Write-Host "ZIP File: `$ZipFile" -ForegroundColor Green
Write-Host "SHA256: $($sha256Hash.Hash.ToLower())" -ForegroundColor Green

Write-Host ""
Write-Host "📋 Release Creation Steps:" -ForegroundColor Yellow
Write-Host "1. Go to: https://github.com/zapabob/liltoon-pcss-extension/releases/new"
Write-Host "2. Tag: v`$Version"
Write-Host "3. Title: `$ReleaseTitle"
Write-Host "4. Upload: `$ZipFile"
Write-Host "5. Copy release notes from RELEASE_NOTES.md"
Write-Host "6. Mark as latest release"
Write-Host "7. Publish release"

Write-Host ""
Write-Host "🔗 VPM Repository URL:"
Write-Host "https://github.com/zapabob/liltoon-pcss-extension/releases/download/v`$Version/`$ZipFile"
"@

Set-Content -Path $GithubReleaseScript -Value $GithubScript -Encoding UTF8

# Summary
Write-Host ""
Write-Host "✅ Release Build Complete!" -ForegroundColor Green
Write-Host "=========================" -ForegroundColor Green
Write-Host "📦 Package: $ZipFile" -ForegroundColor White
Write-Host "📊 Size: $sizeKB KB" -ForegroundColor White
Write-Host "🔐 SHA256: $($sha256Hash.Hash.ToLower())" -ForegroundColor White
Write-Host ""
Write-Host "📁 Output Directory: $OutputDir" -ForegroundColor Yellow
Write-Host "🚀 Next: Run github-release.ps1 for GitHub release instructions" -ForegroundColor Yellow

# Open output directory
if (Test-Path $OutputDir) {
    Start-Process explorer.exe $OutputDir
}

Write-Host ""
Write-Host "🎉 Ready for release!" -ForegroundColor Cyan 