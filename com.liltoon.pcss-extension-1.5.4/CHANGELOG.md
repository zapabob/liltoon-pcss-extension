# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.5.4] - 2025-06-24

### 🔴 Added
- **表情が赤くなるライト機能のBaseレイヤー実装**
  - FXレイヤーからBaseレイヤーに移行し、より安定した動作を実現
  - ModularAvatarとの連携機能を強化（`lilToon/PCSS Extension/ModularAvatar/表情赤くなるライトメニューを追加 (Base)`）
  - 自動的に頭部にライトを生成する機能を追加
  - ライトの強度調整機能を実装（3段階：弱・中・強）
  - VRChat Expression Menu対応（`lilToon/PCSS Extension/Create RedFace Light Menu (Base)`）

### 🛠️ Improved
- **GitHub Pagesの整備**
  - 最新バージョン情報の更新
  - 表情が赤くなるライト機能のドキュメント追加
  - VPMリポジトリの更新

### 📦 Added
- **新しいサンプル**
  - 「🔴 Red Face Light」サンプルの追加
  - 表情が赤くなるライト機能のBaseレイヤー実装例

## [1.5.0] - 2025-06-23

### Added
- Added a new Unity menu `Tools > lilToon PCSS Extension` to apply PCSS effects to selected avatars or the entire world with a single click.

## [1.4.9] - 2025-06-22

### 🎯 Major Features Added
- **Unity Menu Avatar Selector**: `Window/lilToon PCSS Extension/🎯 Avatar Selector`
  - Automatic avatar detection with VRCAvatarDescriptor and ModularAvatar support
  - Built-in preset system: 🎬 Realistic, 🎨 Anime, 🎭 Cinematic, ⚙️ Custom
  - One-click setup with VRChat Expression Menu creation
  - Real-time preset preview and material application
  - Performance optimization integration

### 🛠️ Critical Shader Fixes
- **Macro Redefinition Resolution**: Fixed URP 14.0.10 compatibility issues
  - Resolved `PackHeightmap` redefinition error
  - Fixed `SAMPLE_DEPTH_TEXTURE` macro conflicts
  - Eliminated `TRANSFORM_TEX` and Unity matrix macro redefinitions
  - Implemented pipeline-specific conditional includes

### ⚡ Rendering Pipeline Improvements
- **Enhanced URP Support**: Full compatibility with Unity 2022.3 LTS + URP 14.0.10
  - Proper render pipeline detection and separation
  - Custom sampling macros to avoid conflicts
  - Built-in and URP specific texture declarations
  - Improved shader compilation performance

### 🎨 PCSS Preset System
- **Realistic Preset**: Fine detail shadows for photorealistic avatars
  - FilterRadius: 0.005, LightSize: 0.05, Bias: 0.0005, Intensity: 1.0
- **Anime Preset**: Soft-edged shadows for toon-style avatars
  - FilterRadius: 0.015, LightSize: 0.1, Bias: 0.001, Intensity: 0.8
- **Cinematic Preset**: Dramatic shadows for movie-like presentation
  - FilterRadius: 0.025, LightSize: 0.2, Bias: 0.002, Intensity: 1.2
- **Custom Preset**: Manual configuration for advanced users

### 🔧 ModularAvatar Integration
- **Conditional Compilation**: Safe ModularAvatar usage without hard dependencies
- **Avatar Component Management**: Automatic PCSS preset information storage
- **Compatibility Layer**: Works with or without ModularAvatar installed

### 📊 Performance Optimization
- **Runtime Statistics**: Real-time performance monitoring
- **Dynamic Quality Adjustment**: Automatic optimization based on avatar count and framerate
- **Platform-Specific Optimization**: Quest and VR-specific performance tuning

### 🛡️ Error Resolution
- **Assembly Reference Fixes**: Resolved `lilToon.PCSS.Editor` assembly resolution errors
- **Namespace Issues**: Fixed `nadena` namespace compilation errors
- **Burst Compiler Compatibility**: Eliminated entry-point search failures

### 🎮 VRChat Enhancements
- **Expression Menu Integration**: Automatic VRChat expression menu creation
- **Performance Analytics**: Comprehensive avatar performance statistics
- **VRC Light Volumes**: Enhanced lighting integration support

### 🎯 User Experience Improvements
- **Intuitive UI**: Beautiful and modern Avatar Selector interface
- **Visual Feedback**: Real-time preset parameter display
- **Batch Operations**: Apply presets to multiple materials simultaneously
- **Undo Support**: Full Unity Undo system integration

### 📋 Technical Specifications
- **Unity Version**: 2022.3.22f1+ required
- **URP Version**: 14.0.10 compatible
- **VRChat SDK**: 3.5.0+ supported
- **ModularAvatar**: Optional dependency
- **Platform Support**: Windows, Quest, Android

### 🔍 Debug & Development
- **Enhanced Logging**: Detailed operation feedback
- **Performance Monitoring**: Real-time FPS and optimization level display
- **Material Validation**: Automatic PCSS compatibility checking
- **Error Handling**: Graceful fallback for missing dependencies

### 📦 Package Structure
- **Clean Organization**: Improved directory structure
- **Reduced Size**: Optimized package content
- **Better Documentation**: Enhanced inline code documentation
- **Sample Integration**: Ready-to-use preset examples

## [1.4.8] - 2025-01-28

### 🛠️ Critical Shader Fixes
- **Macro Argument Fix**: Resolved `UNITY_TRANSFER_SHADOW` macro argument issues
  - Fixed `UNITY_TRANSFER_SHADOW(o, o.worldPos)` → `UNITY_TRANSFER_SHADOW(o, v.uv)`
  - Corrected `TRANSFER_SHADOW(o)` → `UNITY_TRANSFER_SHADOW(o, v.uv)`
  - Applied fixes to both lilToon and Poiyomi PCSS Extension shaders

### 🔧 Unity 2022.3 LTS Compatibility
- **Texture Declaration Fix**: Resolved `TEXTURE2D` identifier issues
  - Added proper URP and Built-in Render Pipeline conditional compilation
  - Fixed texture sampling function compatibility
  - Implemented `lil_pcss_common.hlsl` pipeline detection

### 🎯 Assembly Reference Resolution
- **ModularAvatar Dependency Fix**: Resolved `nadena` namespace errors
  - Added conditional compilation directives: `#if UNITY_EDITOR && MODULAR_AVATAR_AVAILABLE`
  - Updated `PCSSUtilities.cs` and `VRChatPerformanceOptimizer.cs`
  - Removed hard dependencies from assembly definition files

### ⚡ Shadow Coordinate Fixes
- **Shadow Mapping Correction**: Fixed shadow coordinate access issues
  - Corrected `i._ShadowCoord` → `i._LightCoord` references
  - Unified shadow coordinate variable naming across shaders
  - Improved shadow sampling accuracy

### 📦 Package Updates
- **Version Management**: Updated to v1.4.8 with proper Git URL
  - New package directory: `com.liltoon.pcss-extension-1.4.8`
  - Updated Git URL: `https://github.com/zapabob/liltoon-pcss-extension.git?path=com.liltoon.pcss-extension-1.4.8#v1.4.8`
  - Enhanced Unity Package Manager installation guide

### 🛡️ Error Prevention
- **Compilation Safety**: Eliminated all Unity 2022.3 LTS compilation errors
  - Zero shader compilation errors
  - Zero assembly reference errors
  - Zero namespace resolution errors
  - Zero Burst compiler errors

### 📚 Documentation Improvements
- **Installation Guide**: Created comprehensive `UNITY_INSTALLATION_GUIDE.md`
  - Step-by-step Unity Package Manager setup
  - Troubleshooting for common installation issues
  - Version verification methods
  - Alternative installation methods

## [1.4.7] - 2025-01-27

### 🎯 Brand Unification
- **Consistent Naming**: Standardized all branding to "lilToon PCSS Extension"
  - Updated package names and descriptions
  - Unified menu paths and component names
  - Consistent documentation and README files

### 🚀 VCC Integration
- **VRChat Creator Companion**: Enhanced VCC compatibility
  - Improved package listing and installation
  - Better dependency management
  - Streamlined setup process

### 📊 GitHub Pages Enhancement
- **Beautiful JSON Display**: Redesigned VPM repository interface
  - Modern and responsive design
  - Real-time package information
  - Enhanced user experience

### 🔧 Repository Organization
- **Clean Structure**: Improved project organization
  - Removed unnecessary files and directories
  - Better separation of concerns
  - Optimized package size

## [1.4.6] - 2025-01-27

### 🎯 Revolutionary Avatar Selector Menu
- **All-in-One Integration**: Complete avatar setup solution
  - Automatic avatar detection and selection
  - Integrated PCSS, VRChat Expressions, and ModularAvatar setup
  - One-click professional avatar preparation

### 🎨 Advanced Material System
- **Smart Material Detection**: Intelligent shader compatibility checking
  - Automatic lilToon and Poiyomi material identification
  - Batch material processing capabilities
  - Material validation and optimization

### ⚡ Performance Monitoring
- **Real-time Analytics**: Live performance tracking
  - Frame rate monitoring and optimization
  - Avatar count-based quality adjustment
  - Performance warning system

## [1.4.5] - 2025-01-26

### 🛠️ Unity 2022.3 LTS URP Compatibility
- **Complete URP Support**: Full Universal Render Pipeline integration
  - URP 12.1.12+ compatibility
  - Proper lighting and shadow integration
  - Mobile and Quest optimization

### 🎮 VRChat SDK3 Integration
- **Native VRChat Support**: Seamless VRChat SDK integration
  - VRCAvatarDescriptor compatibility
  - Expression menu integration
  - Performance optimization for VRChat

### 💡 VRC Light Volumes Support
- **Advanced Lighting**: VRC Light Volumes integration
  - Dynamic lighting response
  - Bakery lightmapping support
  - Professional lighting workflows

## [1.4.4] - 2025-01-25

### 🔄 Automated Version Management
- **Smart Updates**: Automated version detection and updates
  - Git integration for version tracking
  - Automatic changelog generation
  - Seamless upgrade process

### 🏆 Competitor Analysis Integration
- **Market Intelligence**: Comprehensive competitor feature analysis
  - Feature parity tracking
  - Performance benchmarking
  - Competitive advantage identification

## [1.4.3] - 2025-01-24

### 🔧 Dependency Resolution
- **ModularAvatar Compatibility**: Enhanced ModularAvatar integration
  - Optional dependency management
  - Graceful fallback when not available
  - Improved compatibility layer

### 📦 Final Packaging
- **Distribution Ready**: Complete package preparation
  - All dependencies resolved
  - Comprehensive testing completed
  - Ready for public distribution

## [1.4.2] - 2025-01-24

### 🔍 Code Review and Quality
- **Professional Standards**: Comprehensive code review
  - Code quality improvements
  - Performance optimizations
  - Documentation enhancements

### 📋 Final Testing
- **Quality Assurance**: Extensive testing across platforms
  - Unity 2022.3 LTS validation
  - VRChat compatibility testing
  - Performance benchmarking

## [1.4.1] - 2025-01-24

### 🎯 Final Release Preparation
- **Production Ready**: Complete final release preparation
  - All features implemented and tested
  - Documentation completed
  - Distribution packages created

### 🚀 GitHub Release
- **Public Release**: Official GitHub release creation
  - Release notes and documentation
  - Download packages and installation guides
  - Community support setup

## [1.4.0] - 2025-01-24

### 🌟 Ultimate Commercial Edition
- **Professional Features**: Enterprise-grade functionality
  - Advanced PCSS algorithms
  - Professional licensing
  - Commercial use rights

### 🎯 Advanced Avatar Tools
- **Complete Solution**: Comprehensive avatar development tools
  - Integrated workflow
  - Professional results
  - Industry-standard quality

## [1.3.0] - 2025-01-24

### 💼 Professional Edition
- **Enhanced Features**: Professional-grade enhancements
  - Advanced shadow quality
  - Performance optimizations
  - Professional support

### 🔧 Workflow Improvements
- **Streamlined Process**: Improved development workflow
  - Faster setup and configuration
  - Better user experience
  - Enhanced productivity

## [1.2.2] - 2025-01-24

### 🔄 Version Updates
- **Incremental Improvements**: Minor version updates
  - Bug fixes and stability improvements
  - Performance optimizations
  - Documentation updates

## [1.2.1] - 2025-01-24

### 📦 Package Release
- **Official Package**: First official package release
  - Unity Package Manager support
  - Proper versioning
  - Distribution ready

## [1.2.0] - 2025-01-24

### 🎮 VRC Light Volumes Integration
- **Advanced Lighting**: VRC Light Volumes support
  - Dynamic lighting integration
  - Bakery compatibility
  - Professional lighting workflows

### 🔧 VCC Integration
- **VRChat Creator Companion**: VCC support implementation
  - Easy installation and updates
  - Dependency management
  - Streamlined workflow

## [1.1.0] - 2025-01-24

### 🎯 PCSS Implementation
- **Core PCSS Features**: Percentage-Closer Soft Shadows implementation
  - High-quality soft shadows
  - Performance optimized
  - Mobile compatible

### 🎨 Shader Integration
- **lilToon Compatibility**: lilToon shader integration
  - Seamless integration
  - Preserved lilToon features
  - Enhanced shadow quality

## [1.0.0] - 2025-01-24

### 🚀 Initial Release
- **Foundation**: Basic PCSS extension for lilToon
  - Core shadow functionality
  - Basic Unity integration
  - Initial documentation