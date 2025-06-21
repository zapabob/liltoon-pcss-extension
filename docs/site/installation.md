---
layout: page
title: Installation Guide
permalink: /installation/
---

# Installation Guide

## 🚀 Quick Start

### Method 1: VCC (Recommended) 🌟

**VRChat Creator Companion** is the easiest way to install and manage the lilToon PCSS Extension.

[![Add to VCC](https://img.shields.io/badge/Add%20to%20VCC-One%20Click%20Install-blue?style=for-the-badge&logo=vrchat)](vcc://vpm/addRepo?url=https%3A%2F%2Fzapabob.github.io%2Fliltoon-pcss-extension%2Findex.json)

**Option A: One-Click Installation**
1. Click the "Add to VCC" button above
2. VCC will automatically open and add the repository
3. Install **"lilToon PCSS Extension - All-in-One Edition"**
4. Run the setup wizard that automatically appears

**Option B: Manual Repository Addition**
1. **Open VRChat Creator Companion**
2. **Add Repository**:
   - Go to **Settings** → **Packages** → **Add Repository**
   - Enter: `https://zapabob.github.io/liltoon-pcss-extension/index.json`
   - Click **Add**

3. **Install Package**:
   - Open your VRChat project in VCC
   - Search for **"lilToon PCSS Extension - All-in-One Edition"**
   - Click **Install**

4. **Run Setup Wizard**:
   - The setup wizard will automatically appear
   - Follow the guided setup process
   - Your project is now ready!

### Method 2: Unity Package Manager

For advanced users who prefer Unity's built-in package manager:

1. **Open Unity Package Manager**
   - Window → Package Manager

2. **Add Package from Git URL**
   - Click the **+** button
   - Select **Add package from git URL**
   - Enter: `https://github.com/zapabob/liltoon-pcss-extension.git`

3. **Import and Setup**
   - Wait for the package to import
   - Follow the setup instructions that appear

### Method 3: Direct Download

For offline installation or custom setups:

1. **Download Release**
   - Go to [GitHub Releases](https://github.com/zapabob/liltoon-pcss-extension/releases/latest)
   - Download `com.liltoon.pcss-extension-1.2.0.zip`

2. **Extract to Project**
   - Extract the ZIP file
   - Copy the contents to your project's `Packages` folder
   - Unity will automatically detect and import the package

## 📋 Prerequisites

Before installation, ensure your project meets these requirements:

### Required Components
- **Unity**: 2019.4.31f1 or later
- **VRChat Creator Companion**: v2.1.0 or later (for VCC installation)
- **VRChat SDK3 Avatars**: 3.7.0 or later
- **lilToon**: v1.10.3 (latest)

### Recommended Components
- **ModularAvatar**: 1.10.0 or later (for non-destructive setup)
- **VRC Light Volumes**: Latest version (for advanced lighting)

## 🔧 VPM Repository Details

Our VPM repository is hosted on GitHub Pages and follows VRChat's official VPM specification:

### Repository Information
- **Name**: lilToon PCSS Extension - All-in-One Edition Repository
- **URL**: `https://zapabob.github.io/liltoon-pcss-extension/index.json`
- **ID**: `com.liltoon.pcss-extension.vpm-repository`
- **Compatibility**: VCC 2.1.0 or later

### Package Information
- **Package ID**: `com.liltoon.pcss-extension`
- **Display Name**: lilToon PCSS Extension - All-in-One Edition
- **Current Version**: 1.2.0
- **License**: MIT

### Included Samples
When you install via VCC, you'll have access to these optional samples:

1. **PCSS Sample Materials**: Basic PCSS effect demonstration materials
2. **VRChat Expression Setup**: Pre-configured expression menus and parameters
3. **ModularAvatar Prefabs**: Ready-to-use prefabs for non-destructive setup

## 🔧 Post-Installation Setup

### 1. Automatic Setup (Recommended)

After installation, the **Setup Wizard** will automatically appear:

1. **Dependency Check**: Verifies all required packages
2. **lilToon Detection**: Automatically detects your lilToon version
3. **VRChat Integration**: Sets up VRChat SDK3 compatibility
4. **Performance Optimization**: Configures optimal settings for your platform

### 2. Manual Setup (Advanced)

If you prefer manual configuration:

1. **Open PCSS Settings**
   - Window → lilToon PCSS Extension → Settings

2. **Configure Dependencies**
   - Verify lilToon version compatibility
   - Set VRChat SDK3 integration options
   - Configure ModularAvatar settings (if installed)

3. **Apply to Materials**
   - Select your avatar materials
   - Apply PCSS Extension shader
   - Configure shadow parameters

## 🎯 Platform-Specific Setup

### PC Platform
- **Full Feature Set**: All PCSS features available
- **Performance**: Optimized for desktop VRChat
- **Quality**: Maximum shadow quality settings

### Quest Platform
- **Optimized Pipeline**: Reduced complexity for mobile GPU
- **Automatic Fallback**: Smart quality reduction
- **Performance First**: Maintains 72/90fps target

## ✅ Verification

After installation, verify everything is working:

### 1. Check VCC Package List
- Open VCC and go to your project
- Verify "lilToon PCSS Extension - All-in-One Edition" appears in the package list
- Version should show "1.2.0"

### 2. Check Unity Package Manager
- Open Unity Package Manager
- Switch to "In Project" view
- Verify "lilToon PCSS Extension" appears in the list

### 3. Test Setup Wizard
- Window → lilToon PCSS Extension → Setup Wizard
- All checks should show green ✅

### 4. Apply to Test Material
- Create a test material with lilToon shader
- Apply PCSS Extension
- Verify shadow parameters appear

## 🛠️ Troubleshooting

### VCC Installation Issues

#### "Repository not found" or "Invalid URL"
- **Solution**: Verify the repository URL is exactly: `https://zapabob.github.io/liltoon-pcss-extension/index.json`
- **Check**: Internet connection and GitHub Pages availability

#### "VCC version too old"
- **Solution**: Update VRChat Creator Companion to v2.1.0 or later
- **Download**: [VCC Latest Release](https://vrchat.com/home/download)

#### "Package not appearing in VCC"
- **Solution**: Refresh the repository list in VCC Settings
- **Manual**: Remove and re-add the repository

### Common Installation Issues

#### "Dependency missing"
- **Solution**: Install required packages first (lilToon, VRChat SDK3)
- **Use**: VCC's dependency resolution

#### "Setup wizard doesn't appear"
- **Solution**: Window → lilToon PCSS Extension → Setup Wizard
- **Manual**: Run setup manually from the menu

#### "Compilation errors"
- **Solution**: Ensure Unity version compatibility (2019.4.31f1+)
- **Check**: All dependencies are properly installed

### Advanced Troubleshooting

#### VPM Repository Issues
- **Clear VCC Cache**: Restart VCC and refresh repositories
- **Manual Download**: Use direct download method if VPM fails
- **Check Connectivity**: Ensure access to GitHub Pages

#### Package Conflicts
- **Version Mismatch**: Ensure all packages are compatible versions
- **Duplicate Packages**: Remove any duplicate PCSS packages
- **Clean Install**: Remove package and reinstall via VCC

### Getting Help

If you encounter issues:

1. **Check Documentation**: [Complete Guide](https://github.com/zapabob/liltoon-pcss-extension/blob/main/README.md)
2. **Search Issues**: [GitHub Issues](https://github.com/zapabob/liltoon-pcss-extension/issues)
3. **Ask for Help**: [GitHub Discussions](https://github.com/zapabob/liltoon-pcss-extension/discussions)
4. **Direct Support**: r.minegishi1987@gmail.com

## 🔄 Updates

### Automatic Updates (VCC)
- VCC will notify you of new versions automatically
- Click **Update** to get the latest features
- Your settings will be preserved during updates

### Manual Updates
- Download the latest release from GitHub
- Replace the old package files
- Run the setup wizard again if needed

### Update Notifications
- **GitHub Releases**: [Watch the repository](https://github.com/zapabob/liltoon-pcss-extension/subscription)
- **VCC Notifications**: Automatic update notifications
- **Twitter**: Follow [@zapabob_ouj](https://twitter.com/zapabob_ouj) for announcements

---

**Ready to transform your avatar lighting?** 🌟

[![Add to VCC](https://img.shields.io/badge/Add%20to%20VCC-Get%20Started-blue?style=for-the-badge&logo=vrchat)](vcc://vpm/addRepo?url=https%3A%2F%2Fzapabob.github.io%2Fliltoon-pcss-extension%2Findex.json)

[Back to Home]({{ site.baseurl }}/) | [Documentation]({{ site.baseurl }}/documentation/) | [Support]({{ site.baseurl }}/support/) 