# lilToon PCSS Extension - Release Package Creator
# All-in-One Edition v1.2.0

param(
    [string]$Version = "1.2.0",
    [string]$OutputDir = "Release",
    [switch]$CleanBuild = $false,
    [switch]$CreateZip = $true,
    [switch]$UpdateGit = $false
)

Write-Host "🚀 lilToon PCSS Extension - Release Package Creator" -ForegroundColor Cyan
Write-Host "All-in-One Edition v$Version" -ForegroundColor Blue
Write-Host "=" * 60 -ForegroundColor Blue

$startTime = Get-Date

# Clean build if requested
if ($CleanBuild) {
    Write-Host "🧹 Cleaning previous build..." -ForegroundColor Yellow
    if (Test-Path "$OutputDir\com.liltoon.pcss-extension") {
        Remove-Item "$OutputDir\com.liltoon.pcss-extension" -Recurse -Force
    }
    if (Test-Path "$OutputDir\com.liltoon.pcss-extension-$Version.zip") {
        Remove-Item "$OutputDir\com.liltoon.pcss-extension-$Version.zip" -Force
    }
}

# Create output directory
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

$packageDir = "$OutputDir\com.liltoon.pcss-extension"
New-Item -ItemType Directory -Path $packageDir -Force | Out-Null

Write-Host "📦 Creating release package..." -ForegroundColor Cyan

# Copy core files
Write-Host "Copying core files..." -ForegroundColor Yellow
Copy-Item "package.json" -Destination $packageDir
Copy-Item "README.md" -Destination $packageDir
Copy-Item "CHANGELOG.md" -Destination $packageDir
Copy-Item "VPMManifest.json" -Destination $packageDir

# Copy Runtime folder
Write-Host "Copying Runtime..." -ForegroundColor Yellow
Copy-Item "Runtime" -Destination $packageDir -Recurse

# Copy Editor folder
Write-Host "Copying Editor..." -ForegroundColor Yellow
Copy-Item "Editor" -Destination $packageDir -Recurse

# Copy Shaders folder
Write-Host "Copying Shaders..." -ForegroundColor Yellow
Copy-Item "Shaders" -Destination $packageDir -Recurse

# Create Samples~ directory
Write-Host "Creating Samples..." -ForegroundColor Yellow
$samplesDir = "$packageDir\Samples~"
New-Item -ItemType Directory -Path $samplesDir -Force | Out-Null

# Create sample directories with README files
$sampleDirs = @(
    @{Name="PCSSMaterials"; Desc="Sample materials demonstrating PCSS effects"},
    @{Name="VRChatExpressions"; Desc="Pre-configured VRChat Expression Menu and Parameters"},
    @{Name="ModularAvatarPrefabs"; Desc="Ready-to-use ModularAvatar prefabs"}
)

foreach ($sample in $sampleDirs) {
    $samplePath = "$samplesDir\$($sample.Name)"
    New-Item -ItemType Directory -Path $samplePath -Force | Out-Null
    
    $readmeContent = @"
# $($sample.Name)

$($sample.Desc)

## Installation

1. Open VCC (VRChat Creator Companion)
2. Add this package to your project
3. Import this sample from the Package Manager
4. Follow the setup instructions in the main README

## Support

For support and documentation, visit:
https://github.com/zapabob/liltoon-pcss-extension
"@
    
    $readmeContent | Out-File -FilePath "$samplePath\README.md" -Encoding UTF8
}

# Create release notes
Write-Host "Creating release notes..." -ForegroundColor Yellow
$releaseNotes = @"
# lilToon PCSS Extension - All-in-One Edition v$Version

## 🎉 Release Highlights

- **lilToon v1.10.3 Compatibility**: Full support for the latest lilToon version
- **VRC Light Volumes Integration**: Next-generation lighting system support
- **All-in-One Solution**: Complete PCSS implementation with automatic setup
- **VRChat SDK3 Optimization**: Performance-optimized for VRChat environments
- **One-Click Installation**: VCC integration for easy setup

## 📋 What's Included

### Core Components
- **Runtime Scripts**: 5 optimized C# scripts for PCSS functionality
- **Editor Tools**: 6 editor scripts including VCC Setup Wizard
- **Shaders**: lilToon and Poiyomi PCSS extensions
- **Samples**: Ready-to-use materials and prefabs

### Key Features
- ✅ Automatic lilToon compatibility management
- ✅ VRChat performance optimization
- ✅ Quest platform support
- ✅ ModularAvatar integration
- ✅ Expression menu generation
- ✅ VRC Light Volumes support

## 🔧 Installation

### Method 1: VCC (Recommended)
1. Open VRChat Creator Companion
2. Add repository: \`https://github.com/zapabob/liltoon-pcss-extension\`
3. Install "lilToon PCSS Extension - All-in-One Edition"
4. Run the setup wizard that appears

### Method 2: Unity Package Manager
1. Open Unity Package Manager
2. Add package from git URL: \`https://github.com/zapabob/liltoon-pcss-extension.git\`
3. Import the package and run setup

## 🎯 Compatibility

- **Unity**: 2019.4.31f1 or later
- **lilToon**: v1.10.3 (latest)
- **VRChat SDK**: 3.7.0 or later
- **ModularAvatar**: 1.10.0 or later
- **VRC Light Volumes**: Supported

## 📚 Documentation

Complete documentation available at:
https://github.com/zapabob/liltoon-pcss-extension/blob/main/README.md

## 🛠️ Support

- **Issues**: https://github.com/zapabob/liltoon-pcss-extension/issues
- **Discord**: [Community Server]
- **Email**: r.minegishi1987@gmail.com

---

**Release Date**: $(Get-Date -Format "yyyy-MM-dd")  
**Version**: v$Version  
**License**: MIT
"@

$releaseNotes | Out-File -FilePath "$packageDir\RELEASE_NOTES.md" -Encoding UTF8

# Create ZIP package if requested
if ($CreateZip) {
    Write-Host "📦 Creating ZIP package..." -ForegroundColor Cyan
    $zipPath = "$OutputDir\com.liltoon.pcss-extension-$Version.zip"
    
    if (Test-Path $zipPath) {
        Remove-Item $zipPath -Force
    }
    
    # Use PowerShell 5.0+ Compress-Archive
    Compress-Archive -Path "$packageDir\*" -DestinationPath $zipPath -CompressionLevel Optimal
    
    Write-Host "✅ ZIP package created: $zipPath" -ForegroundColor Green
    
    # Calculate file size
    $zipSize = (Get-Item $zipPath).Length
    $zipSizeMB = [math]::Round($zipSize / 1MB, 2)
    Write-Host "Package size: $zipSizeMB MB" -ForegroundColor Blue
}

# Create checksums
Write-Host "🔐 Creating checksums..." -ForegroundColor Cyan
$checksumFile = "$OutputDir\checksums.txt"
$checksumContent = @"
# lilToon PCSS Extension v$Version - Checksums
# Generated: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")

"@

if ($CreateZip -and (Test-Path "$OutputDir\com.liltoon.pcss-extension-$Version.zip")) {
    $zipHash = Get-FileHash "$OutputDir\com.liltoon.pcss-extension-$Version.zip" -Algorithm SHA256
    $checksumContent += "SHA256: $($zipHash.Hash) - com.liltoon.pcss-extension-$Version.zip`n"
}

$checksumContent | Out-File -FilePath $checksumFile -Encoding UTF8
Write-Host "✅ Checksums created: $checksumFile" -ForegroundColor Green

# Git operations
if ($UpdateGit) {
    Write-Host "📝 Updating Git repository..." -ForegroundColor Cyan
    
    try {
        # Add all changes
        git add .
        Write-Host "✅ Files staged" -ForegroundColor Green
        
        # Commit changes
        git commit -m "Release v$Version - All-in-One Edition with lilToon v1.10.3 support"
        Write-Host "✅ Changes committed" -ForegroundColor Green
        
        # Create tag
        git tag -a "v$Version" -m "lilToon PCSS Extension All-in-One Edition v$Version"
        Write-Host "✅ Tag v$Version created" -ForegroundColor Green
        
        Write-Host "📤 Ready to push to GitHub:" -ForegroundColor Yellow
        Write-Host "  git push origin main" -ForegroundColor White
        Write-Host "  git push origin v$Version" -ForegroundColor White
        
    } catch {
        Write-Host "❌ Git operation failed: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Completion summary
$endTime = Get-Date
$duration = $endTime - $startTime

Write-Host "🎉 Release package creation completed!" -ForegroundColor Green
Write-Host "=" * 60 -ForegroundColor Blue

Write-Host "📊 Summary:" -ForegroundColor Cyan
Write-Host "Version: v$Version" -ForegroundColor Blue
Write-Host "Output: $OutputDir" -ForegroundColor Blue
Write-Host "Duration: $([math]::Round($duration.TotalSeconds, 1)) seconds" -ForegroundColor Blue

if ($CreateZip) {
    Write-Host "ZIP: com.liltoon.pcss-extension-$Version.zip" -ForegroundColor Blue
}

Write-Host "🚀 Next Steps:" -ForegroundColor Cyan
Write-Host "1. Test the package in a clean Unity project" -ForegroundColor White
Write-Host "2. Upload to GitHub Releases" -ForegroundColor White
Write-Host "3. Update VCC repository listing" -ForegroundColor White
Write-Host "4. Prepare BOOTH product page" -ForegroundColor White

Write-Host "✨ Ready for distribution!" -ForegroundColor Green 