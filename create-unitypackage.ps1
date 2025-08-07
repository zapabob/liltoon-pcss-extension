# lilToon PCSS Extension - Semantic Versioning Package Script
# Version: 2.5.0 (MINOR - New features added, backward compatible)

param(
    [string]$UnityPath = "C:\Program Files\Unity\Hub\Editor\2022.3.22f1\Editor\Unity.exe",
    [string]$ProjectPath = "C:\Users\downl\Desktop\LilToon-PCSS-extension\liltoon-pcss-extension",
    [string]$OutputPath = "C:\Users\downl\Desktop\LilToon-PCSS-extension\ExportedPackages",
    [string]$Version = "2.5.0"
)

Write-Host "🎯 lilToon PCSS Extension Package Creation Started" -ForegroundColor Green
Write-Host "📦 Version: $Version (Semantic Versioning Compatible)" -ForegroundColor Yellow
Write-Host "🔧 Unity Path: $UnityPath" -ForegroundColor Cyan
Write-Host "📁 Project Path: $ProjectPath" -ForegroundColor Cyan
Write-Host "📦 Output Path: $OutputPath" -ForegroundColor Cyan

# Create output directory
if (!(Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force
    Write-Host "✅ Output directory created: $OutputPath" -ForegroundColor Green
}

# Check Unity processes
$unityProcesses = Get-Process | Where-Object { $_.ProcessName -like "*Unity*" }
if ($unityProcesses) {
    Write-Host "⚠️ Unity processes are running. Please close them manually before continuing." -ForegroundColor Yellow
    Write-Host "Running Unity processes:" -ForegroundColor Yellow
    $unityProcesses | ForEach-Object { Write-Host "  - $($_.ProcessName) (PID: $($_.Id))" -ForegroundColor Yellow }
    Read-Host "Press Enter after closing Unity"
}

# Set package name
$packageName = "com.liltoon.pcss-extension-$Version"
$unityPackagePath = Join-Path $OutputPath "$packageName.unitypackage"
$zipPackagePath = Join-Path $OutputPath "$packageName.zip"

Write-Host "📦 Package name: $packageName" -ForegroundColor Magenta

# Export package using Unity batchmode
Write-Host "🚀 Exporting package using Unity batchmode..." -ForegroundColor Green

$exportArgs = @(
    "-batchmode",
    "-quit",
    "-projectPath", $ProjectPath,
    "-exportPackage", "Assets", $unityPackagePath,
    "-logFile", (Join-Path $OutputPath "unity-export.log")
)

try {
    Write-Host "Executing command: $UnityPath $($exportArgs -join ' ')" -ForegroundColor Gray
    
    $process = Start-Process -FilePath $UnityPath -ArgumentList $exportArgs -Wait -PassThru -NoNewWindow
    
    if ($process.ExitCode -eq 0) {
        Write-Host "✅ Unity package export completed successfully" -ForegroundColor Green
        Write-Host "📦 Package path: $unityPackagePath" -ForegroundColor Green
        
        # Check package size
        if (Test-Path $unityPackagePath) {
            $packageSize = (Get-Item $unityPackagePath).Length
            $packageSizeMB = [math]::Round($packageSize / 1MB, 2)
            Write-Host "📊 Package size: $packageSizeMB MB" -ForegroundColor Cyan
        }
    } else {
        Write-Host "❌ Unity package export failed (Exit Code: $($process.ExitCode))" -ForegroundColor Red
        if (Test-Path (Join-Path $OutputPath "unity-export.log")) {
            Write-Host "📋 Log file: $(Join-Path $OutputPath "unity-export.log")" -ForegroundColor Yellow
        }
        exit 1
    }
} catch {
    Write-Host "❌ Error occurred during Unity package export: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Create ZIP package
Write-Host "🗜️ Creating ZIP package..." -ForegroundColor Green

try {
    # Create temporary directory
    $tempDir = Join-Path $OutputPath "temp-$packageName"
    if (Test-Path $tempDir) {
        Remove-Item $tempDir -Recurse -Force
    }
    New-Item -ItemType Directory -Path $tempDir -Force | Out-Null
    
    # Copy package file
    Copy-Item $unityPackagePath $tempDir -Force
    
    # Create README file
    $readmeContent = @"
# lilToon PCSS Extension v$Version

## Semantic Versioning Support

This version is a **MINOR** update.
- **New Features**: Modular Avatar Universal Avatar Support
- **Backward Compatible**: Existing features work as before
- **No API Changes**: Existing APIs remain unchanged

## New Features

### 🌟 Modular Avatar Universal Avatar Support
- Light toggle system compatible with any avatar
- Relative path mode for portability
- FX layer integration with automatic parameter setup

### 💡 Universal Light Toggle System
- Multiple light type support (Point, Spot, Area)
- Automatic animator controller generation
- Modular Avatar auto-detection

### 🎨 Advanced Preset System
- 5 preset types (Realistic, Anime, Cinematic, Portrait, Game)
- Advanced lighting settings and effects
- Universal avatar compatibility

## Installation

1. Import package via Unity Package Manager
2. Ensure Modular Avatar is installed
3. Use features from Tools/lilToon PCSS Extension menu

## Dependencies

- Unity 2022.3 LTS
- URP 14.0.10
- lilToon 2.1.4
- Modular Avatar 1.12.5 (for new features)

## Semantic Versioning

This version follows [Unity's official semantic versioning](https://docs.unity3d.com/2020.1/Documentation/Manual/upm-semver.html).

- **MAJOR**: Breaking changes
- **MINOR**: New features (backward compatible)
- **PATCH**: Bug fixes (no API changes)

## Changelog

### v2.5.0 (MINOR)
- Modular Avatar universal avatar support
- Universal Light Toggle System implementation
- Advanced Preset System implementation
- Relative path mode for portability
- FX layer integration with automatic parameter setup
- 5 preset types (Realistic/Anime/Cinematic/Portrait/Game)
- Advanced light toggle system (multiple light types)
- Modular Avatar auto-detection
- Unity menu organization (product-ready)
- Semantic versioning support

### v2.4.0 (PATCH)
- Fixed duplicate definition errors
- Fixed VRCPhysBoneCollider namespace errors
- Fixed VRChat SDK reference errors
- Fixed character encoding errors
- Other bug fixes

## License

MIT License

## Author

lilToon PCSS Extension Team
"@
    
    Set-Content -Path (Join-Path $tempDir "README.md") -Value $readmeContent -Encoding UTF8
    
    # Copy package.json file
    Copy-Item (Join-Path $ProjectPath "Assets\package.json") $tempDir -Force
    
    # Create ZIP file
    Compress-Archive -Path "$tempDir\*" -DestinationPath $zipPackagePath -Force
    
    # Remove temporary directory
    Remove-Item $tempDir -Recurse -Force
    
    Write-Host "✅ ZIP package creation completed" -ForegroundColor Green
    Write-Host "📦 ZIP package path: $zipPackagePath" -ForegroundColor Green
    
    # Check ZIP file size
    if (Test-Path $zipPackagePath) {
        $zipSize = (Get-Item $zipPackagePath).Length
        $zipSizeMB = [math]::Round($zipSize / 1MB, 2)
        Write-Host "📊 ZIP size: $zipSizeMB MB" -ForegroundColor Cyan
    }
    
} catch {
    Write-Host "❌ Error occurred during ZIP package creation: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Display package information
Write-Host "`n📋 Package Information" -ForegroundColor Yellow
Write-Host "==================" -ForegroundColor Yellow
Write-Host "📦 Package name: $packageName" -ForegroundColor White
Write-Host "🔢 Version: $Version (MINOR - New features added)" -ForegroundColor White
Write-Host "📁 Unity Package: $unityPackagePath" -ForegroundColor White
Write-Host "🗜️ ZIP Package: $zipPackagePath" -ForegroundColor White
Write-Host "🌟 New Feature: Modular Avatar Universal Avatar Support" -ForegroundColor Green
Write-Host "💡 New Feature: Universal Light Toggle System" -ForegroundColor Green
Write-Host "🎨 New Feature: Advanced Preset System" -ForegroundColor Green

# Semantic versioning information
Write-Host "`n📚 Semantic Versioning Information" -ForegroundColor Yellow
Write-Host "=================================" -ForegroundColor Yellow
Write-Host "🔢 Current version: $Version" -ForegroundColor White
Write-Host "📈 Version type: MINOR (New features added, backward compatible)" -ForegroundColor Green
Write-Host "🔄 Backward compatibility: ✅ Yes" -ForegroundColor Green
Write-Host "🔧 API changes: ❌ No" -ForegroundColor Green
Write-Host "📦 Package manager: Auto-update compatible" -ForegroundColor Green

# Completion message
Write-Host "`n🎉 Package creation completed!" -ForegroundColor Green
Write-Host "📦 Unity Package: $unityPackagePath" -ForegroundColor Cyan
Write-Host "🗜️ ZIP Package: $zipPackagePath" -ForegroundColor Cyan
Write-Host "🌟 Semantic versioning support completed" -ForegroundColor Yellow
Write-Host "🚀 Ready for distribution!" -ForegroundColor Green 