# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).


## [2.2.0] - 2025-07-23

### Added
- **NEW**: Version 2.2.0 with enhanced VCC compatibility
- **NEW**: Semantic versioning implementation following [Semantic Versioning 2.0.0](https://semver.org/)

### Changed
- **IMPROVEMENT**: Updated package version to 2.2.0 following semantic versioning


## [2.1.0] - 2025-01-24

### Added
- **NEW**: Enhanced semantic versioning implementation following [Semantic Versioning 2.0.0](https://semver.org/) specification
- **NEW**: Improved VCC (VRChat Creator Companion) compatibility with enhanced package metadata
- **NEW**: Added semantic-versioning and power-protection keywords for better discoverability
- **NEW**: Enhanced package description with VCC compatibility information
- **NEW**: Added Power Protection System sample demonstrating checkpoint and recovery features
- **NEW**: Improved version matching system for better dependency management
- **NEW**: Enhanced package repository structure for VCC integration
- **NEW**: Added comprehensive version history tracking in VPM repository

### Changed
- **IMPROVEMENT**: Updated package version to 2.1.0 following semantic versioning (MINOR version bump for new features)
- **IMPROVEMENT**: Enhanced VPM repository structure with multiple version support
- **IMPROVEMENT**: Updated package description to highlight VCC compatibility
- **IMPROVEMENT**: Added new keywords for better package discovery in VCC
- **IMPROVEMENT**: Enhanced sample organization with Power Protection System examples
- **IMPROVEMENT**: Updated download URLs to reflect new version structure
- **IMPROVEMENT**: Enhanced changelog organization with semantic versioning compliance

### Fixed
- **IMPROVEMENT**: Better version compatibility checking in VCC package manager
- **IMPROVEMENT**: Enhanced package metadata validation for VCC integration
- **IMPROVEMENT**: Improved dependency resolution for VRChat SDK compatibility
- **IMPROVEMENT**: Better error handling for version conflicts in VCC

### Security
- **NEW**: Enhanced package integrity verification for VCC distribution
- **NEW**: Improved dependency security with semantic versioning constraints
- **NEW**: Better package signing and validation for VCC repository

## [2.0.0] - 2025-07-24

### Fixed
- **CRITICAL**: Fixed ApplyModularAvatarIntegration function undeclared identifier error in AdvancedLilToonModularAvatarShader.shader
- **CRITICAL**: Fixed _PCSSFilterRadius redefinition errors in AdvancedRealisticShadowSystem.shader and UniversalAdvancedShadowSystem.shader
- **CRITICAL**: Fixed VRCPhysBoneCollider namespace error in PhysBoneColliderCreator.cs
- **CRITICAL**: Fixed shader compilation errors and function definition order issues
- **CRITICAL**: Fixed property naming conflicts across multiple shader files
- **IMPROVEMENT**: Enhanced shader compilation stability and error handling
- **IMPROVEMENT**: Improved VRChat SDK compatibility with correct namespace usage
- **IMPROVEMENT**: Better ModularAvatar integration stability

### Added
- **NEW**: Power protection system with automatic checkpoint saving (5-minute intervals)
- **NEW**: Emergency save functionality for Ctrl+C and abnormal termination
- **NEW**: Backup rotation system with maximum 10 automatic backups
- **NEW**: Session management with unique ID for complete session tracking
- **NEW**: Signal handler support for SIGINT, SIGTERM, SIGBREAK
- **NEW**: Abnormal termination detection with automatic data protection
- **NEW**: Recovery system for automatic restoration from previous session
- **NEW**: Data integrity with JSON+Pickle composite saving
- **NEW**: Automatic implementation log saving in _docs directory
- **NEW**: Enhanced error handling and validation throughout the system
- **NEW**: Improved shader function definition order normalization
- **NEW**: Unified property naming conventions across shaders
- **NEW**: Correct VRC SDK namespace usage verification

### Changed
- **BREAKING CHANGE**: Updated package version to 2.0.0 to reflect major fixes
- **BREAKING CHANGE**: Enhanced package description with power protection features
- **BREAKING CHANGE**: Updated keywords to include power-protection and error-fixes
- **BREAKING CHANGE**: Added new sample for Power Protection functionality
- **BREAKING CHANGE**: Updated changelog with comprehensive fix documentation
- **BREAKING CHANGE**: Enhanced package metadata with fixes information

### Security
- **NEW**: Power protection system prevents data loss during unexpected shutdowns
- **NEW**: Automatic checkpoint saving ensures work is never lost
- **NEW**: Emergency save functionality protects against crashes and interruptions
- **NEW**: Backup rotation system maintains multiple recovery points
- **NEW**: Session tracking provides complete development history
- **NEW**: Signal handling ensures graceful shutdown and data protection

## [2.0.0-fixed] - 2025-01-24

### Fixed
- **CRITICAL**: Fixed VRChat SDK namespace errors (VRCPhysBoneCollider, VRCPhysBone)
- **CRITICAL**: Fixed shader redefinition errors (_PCSSFilterRadius)
- **CRITICAL**: Fixed duplicate method errors (SetDefaultValues)
- **CRITICAL**: Fixed unused field warnings (showPerformanceStats)
- **CRITICAL**: Fixed compilation errors in PhysBoneColliderCreator.cs
- **CRITICAL**: Fixed compilation errors in AdvancedShadowSystemGUI.cs
- **CRITICAL**: Fixed shader compilation errors in UniversalAdvancedShadowSystem.shader
- **CRITICAL**: Fixed shader compilation errors in AdvancedRealisticShadowSystem.shader
- **CRITICAL**: Fixed shader compilation errors in AdvancedLilToonModularAvatarShader.shader
- **IMPROVEMENT**: Enhanced error handling and validation
- **IMPROVEMENT**: Improved shader compilation stability
- **IMPROVEMENT**: Better VRChat SDK compatibility
- **IMPROVEMENT**: Enhanced ModularAvatar integration stability

## [2.0.0] - 2025-07-22

### Added
- **BREAKING CHANGE**: Complete rewrite of the Advanced Realistic Shadow System
- **BREAKING CHANGE**: New Modular Avatar AAO integration system
- **BREAKING CHANGE**: Advanced lighting system with dynamic control
- **BREAKING CHANGE**: Dynamic material system with quality-based, time-based, and distance-based control
- **BREAKING CHANGE**: Advanced shader system with real-time optimization
- **BREAKING CHANGE**: Performance optimization system with LOD, frame rate, memory, and GPU optimization
- **BREAKING CHANGE**: New custom editor GUI with 18 categorized settings
- **BREAKING CHANGE**: Complete lilToon integration with property mapping
- **BREAKING CHANGE**: Reflection-based ModularAvatar integration for safe dependency management
- **BREAKING CHANGE**: Real-time parameter control and automatic menu generation
- **BREAKING CHANGE**: Blend shape control for dynamic avatar customization
- **BREAKING CHANGE**: Volumetric shadows with density and step controls
- **BREAKING CHANGE**: Real-time reflection with strength and roughness controls
- **BREAKING CHANGE**: Subsurface scattering with strength and color controls
- **BREAKING CHANGE**: Distance-based quality adjustment system
- **BREAKING CHANGE**: VRC Light Volumes integration with intensity and tint controls
- **BREAKING CHANGE**: Anime shadow system with strength and smoothness controls
- **BREAKING CHANGE**: Cinematic shadow system with contrast and saturation controls
- **BREAKING CHANGE**: Advanced performance benchmarking system
- **BREAKING CHANGE**: Quality presets (Ultra Realistic, Photorealistic, Cinematic, Anime Enhanced, Performance)
- **BREAKING CHANGE**: Auto-upgrade system for existing lilToon materials
- **BREAKING CHANGE**: Force select functionality for advanced shadow system
- **BREAKING CHANGE**: Comprehensive error handling and validation
- **BREAKING CHANGE**: New shader variants and pre-compilation system
- **BREAKING CHANGE**: Unity 2019.4 compatibility (downgraded from 2022.3)
- **BREAKING CHANGE**: Updated dependencies to URP >=7.0.0 and Core >=7.0.0

### Changed
- **BREAKING CHANGE**: Package name updated to reflect advanced features
- **BREAKING CHANGE**: Display name changed to "lilToon PCSS Extension - Advanced Realistic Shadow System"
- **BREAKING CHANGE**: Description updated to highlight new advanced features
- **BREAKING CHANGE**: Keywords updated to reflect new capabilities
- **BREAKING CHANGE**: Author information updated with new contact details
- **BREAKING CHANGE**: Repository structure reorganized for better maintainability
- **BREAKING CHANGE**: Sample structure updated with new feature demonstrations
- **BREAKING CHANGE**: License changed to MIT for broader adoption
- **BREAKING CHANGE**: Documentation URLs updated to new repository structure

### Removed
- **BREAKING CHANGE**: Removed old PCSS implementation in favor of advanced system
- **BREAKING CHANGE**: Removed old ModularAvatar integration in favor of reflection-based system
- **BREAKING CHANGE**: Removed old editor extensions in favor of new comprehensive GUI
- **BREAKING CHANGE**: Removed old shader variants in favor of new advanced system
- **BREAKING CHANGE**: Removed Unity 2022.3 specific features for broader compatibility
- **BREAKING CHANGE**: Removed old dependency requirements in favor of new minimal requirements

### Fixed
- **BREAKING CHANGE**: Complete rewrite of all systems to eliminate legacy issues
- **BREAKING CHANGE**: Improved error handling and validation throughout
- **BREAKING CHANGE**: Enhanced performance optimization algorithms
- **BREAKING CHANGE**: Better compatibility with different Unity versions
- **BREAKING CHANGE**: More robust ModularAvatar integration
- **BREAKING CHANGE**: Improved shader compilation and variant management

### Security
- **BREAKING CHANGE**: Updated to reflection-based ModularAvatar integration for safer dependency management
- **BREAKING CHANGE**: Enhanced validation and error handling for better security
- **BREAKING CHANGE**: Improved resource management and memory optimization

## [1.8.1] - 2024-12-15

### Added
- MissingMaterialAutoFixer Editor extension
- FBX/Prefab auto-remap for missing materials
- Enhanced avatar selector menu functionality
- Improved preset system with Realistic/Anime/Cinematic options
- One-click avatar setup with ModularAvatar integration
- Base layer red face light implementation
- Official support for Unity Editor Coroutines

### Changed
- Updated to Unity 2022.3 LTS compatibility
- Enhanced URP 14.0.10 support

### Fixed
- Various bug fixes and performance improvements
- Enhanced compatibility with different Unity versions
- Improved ModularAvatar integration stability

## [1.8.0] - 2024-11-20

### Added
- Professional PCSS (Percentage-Closer Soft Shadows) extension
- lilToon and Poiyomi shader support
- Avatar Selector Menu functionality
- Built-in preset system
- ModularAvatar integration
- Performance optimization features

### Changed
- Initial release with core PCSS functionality
- Basic ModularAvatar integration
- Standard preset system

### Fixed
- Initial bug fixes and stability improvements

## [1.0.0] - 2024-10-01

### Added
- Initial release of lilToon PCSS Extension
- Basic PCSS implementation
- lilToon shader integration
- Simple preset system

---

## Version History Summary

### Major Version Changes (X.0.0)
- **2.0.0**: Complete rewrite with Advanced Realistic Shadow System
- **1.0.0**: Initial release

### Minor Version Changes (X.Y.0)
- **1.8.0**: Professional PCSS extension with advanced features
- **1.8.1**: Enhanced functionality with MissingMaterialAutoFixer

### Patch Version Changes (X.Y.Z)
- Various bug fixes and minor improvements

## Migration Guide

### From 1.8.1 to 2.0.0
Due to the complete rewrite in version 2.0.0, migration requires:

1. **Backup your project** before upgrading
2. **Remove old package** completely
3. **Install new package** version 2.0.0
4. **Run auto-upgrade** using the new menu items
5. **Review and adjust** settings in the new advanced GUI
6. **Test thoroughly** with your existing avatars

### Breaking Changes in 2.0.0
- Complete API rewrite
- New shader names and properties
- Different menu structure
- New dependency requirements
- Changed Unity version compatibility

## Support

For support and migration assistance:
- GitHub Issues: https://github.com/liltoon-pcss-extension/com.liltoon.pcss-extension/issues
- Documentation: https://github.com/liltoon-pcss-extension/com.liltoon.pcss-extension/wiki
- Email: support@liltoon-pcss.com 