# lilToon PCSS Extension - Advanced Realistic Shadow System

[![Version](https://img.shields.io/badge/version-2.1.0-blue.svg)](https://semver.org/)
[![Unity](https://img.shields.io/badge/Unity-2019.4+-green.svg)](https://unity.com/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![lilToon](https://img.shields.io/badge/lilToon-Compatible-brightgreen.svg)](https://github.com/lilxyzw/lilToon)
[![ModularAvatar](https://img.shields.io/badge/ModularAvatar-AAO%20Compatible-orange.svg)](https://github.com/bdunderscore/modular-avatar)
[![VRChat](https://img.shields.io/badge/VRChat-SDK%20Compatible-purple.svg)](https://vrchat.com/)
[![VCC](https://img.shields.io/badge/VCC-Compatible-cyan.svg)](https://vcc.docs.vrchat.com/)
[![Semantic Versioning](https://img.shields.io/badge/Semantic%20Versioning-2.0.0-lightgrey.svg)](https://semver.org/)

Advanced Realistic Shadow System for lilToon with Modular Avatar AAO integration. Features real-time PCSS, volumetric shadows, subsurface scattering, and performance optimization. **Now with enhanced VCC compatibility and semantic versioning support.**

## 🎯 Features

### ✨ Advanced Realistic Shadow System
- **Real-time PCSS**: Percentage-Closer Soft Shadows with dynamic quality adjustment
- **Volumetric Shadows**: Advanced volumetric shadow effects with density and step controls
- **Subsurface Scattering**: Realistic subsurface scattering with strength and color controls
- **Real-time Reflection**: Dynamic reflection system with strength and roughness controls
- **Distance-based Quality**: Automatic quality adjustment based on distance
- **Performance Optimization**: LOD, frame rate, memory, and GPU optimization

### 🔧 Modular Avatar AAO Integration
- **Reflection-based Integration**: Safe dependency management without direct dependencies
- **Real-time Parameter Control**: Dynamic parameter changes through ModularAvatar
- **Automatic Menu Generation**: Automatic ModularAvatar menu setup
- **Blend Shape Control**: Dynamic blend shape control for avatar customization

### 🎨 Advanced Lighting System
- **Dynamic Light Control**: Real-time light intensity adjustment
- **Environmental Light Control**: Dynamic environmental light adjustment
- **Volumetric Lighting**: Advanced volumetric lighting effects
- **Real-time Reflection**: Dynamic reflection control

### 📦 Dynamic Material System
- **Quality-based Control**: Dynamic adjustment based on quality level
- **Time-based Control**: Dynamic changes based on time progression
- **Distance-based Control**: Dynamic adjustment based on distance
- **Performance Optimization**: Automatic performance adjustment

### ⚡ Performance Optimization System
- **Dynamic LOD**: Distance-based LOD control
- **Frame Rate Optimization**: Dynamic frame rate adjustment
- **Memory Optimization**: Automatic memory usage optimization
- **GPU Optimization**: Dynamic GPU usage optimization

### 🎛️ Advanced Custom Editor GUI
- **18 Categorized Settings**: Organized settings in 18 categories
- **Auto-upgrade Functionality**: Automatic conversion of existing lilToon materials
- **Quick Actions**: Quality presets and bulk feature control
- **Modular Avatar Integration**: Dedicated setup functionality

### 🛡️ Power Protection System
- **Automatic Checkpoint Saving**: 5-minute interval automatic saves
- **Emergency Save Functionality**: Ctrl+C and abnormal termination protection
- **Backup Rotation System**: Maximum 10 automatic backups
- **Session Management**: Unique ID for complete session tracking
- **Signal Handler Support**: SIGINT, SIGTERM, SIGBREAK handling
- **Recovery System**: Automatic restoration from previous session
- **Data Integrity**: JSON+Pickle composite saving

### 🔄 VCC & Semantic Versioning
- **VCC Compatibility**: Full VRChat Creator Companion support
- **Semantic Versioning**: [Semantic Versioning 2.0.0](https://semver.org/) compliance
- **Version Management**: Automated version bumping and tracking
- **Package Repository**: Enhanced VPM repository structure
- **Dependency Management**: Improved dependency resolution

## 📋 Requirements

### Unity
- **Unity 2019.4** or later
- **Universal Render Pipeline (URP)** 7.0.0 or later
- **Core Render Pipeline** 7.0.0 or later

### Dependencies
- **lilToon**: Compatible with lilToon shaders
- **Modular Avatar AAO**: Optional integration (reflection-based, no direct dependency)

### VCC Requirements
- **VRChat Creator Companion**: For VCC package management
- **VRChat SDK**: 3.5.0 or later
- **Modular Avatar**: 1.12.5 or later

## 🚀 Installation

### Via VCC (Recommended)
1. Open VRChat Creator Companion
2. Go to "Worlds" or "Avatars" project
3. Click "Add Package"
4. Search for "lilToon PCSS Extension"
5. Click "Install"

### Via Unity Package Manager
1. Open Unity Package Manager
2. Click the "+" button
3. Select "Add package from git URL"
4. Enter: `https://github.com/liltoon-pcss-extension/com.liltoon.pcss-extension.git`

### Via Git Submodule
```bash
git submodule add https://github.com/liltoon-pcss-extension/com.liltoon.pcss-extension.git
```

### Manual Installation
1. Download the latest release
2. Extract to your Unity project's `Packages` folder
3. Restart Unity

## 🎮 Usage

### Basic Setup
1. Select your avatar's root GameObject
2. Go to `Tools/lilToon PCSS Extension/Advanced Integration/Setup Advanced Integration`
3. All systems will be automatically set up

### Individual Setup
- **Create lilToon Modular Avatar**: Modular Avatar integration only
- **Setup Advanced Lighting System**: Advanced lighting only
- **Create Dynamic Material System**: Dynamic materials only
- **Setup Advanced Shader System**: Advanced shaders only
- **Create Performance Optimizer**: Performance optimization only

### Shader Usage
1. Select a material
2. Change shader to "lilToon/Advanced Modular Avatar Integration"
3. Configure detailed settings in the custom editor GUI
4. Apply presets using quick actions

### Version Management
```bash
# マイナーバージョンをインクリメント（新機能追加）
python scripts/version_manager.py bump-minor

# パッチバージョンをインクリメント（バグ修正）
python scripts/version_manager.py bump-patch

# メジャーバージョンをインクリメント（破壊的変更）
python scripts/version_manager.py bump-major

# 特定のバージョンを設定
python scripts/version_manager.py set-version --version 2.1.0
```

## 📊 Performance Comparison

| Feature | This Implementation | Competitor A | Competitor B |
|---------|-------------------|--------------|--------------|
| lilToon Integration | ✅ Complete | ⚠️ Partial | ❌ None |
| Modular Avatar Integration | ✅ Complete | ⚠️ Partial | ❌ None |
| Advanced Lighting | ✅ Implemented | ⚠️ Basic | ❌ None |
| Dynamic Materials | ✅ Implemented | ❌ None | ❌ None |
| Advanced Shaders | ✅ Implemented | ❌ None | ❌ None |
| Performance Optimization | ✅ Implemented | ⚠️ Basic | ❌ None |
| Custom Editor GUI | ✅ Implemented | ⚠️ Basic | ❌ None |

| Metric | This Implementation | Competitor A | Competitor B |
|--------|-------------------|--------------|--------------|
| Memory Usage | Low | Medium | High |
| GPU Usage | Optimized | Basic | Unoptimized |
| Frame Rate | Stable | Unstable | Unstable |
| Load Time | Fast | Medium | Slow |

## 🔧 Configuration

### Quality Presets
- **Ultra Realistic**: Maximum quality with all features enabled
- **Photorealistic**: High quality with balanced features
- **Cinematic**: Medium quality optimized for cinematic effects
- **Anime Enhanced**: Optimized for anime-style rendering
- **Performance**: Maximum performance with minimal quality loss

### Advanced Settings
The custom editor GUI provides 18 categories of settings:
1. Basic Settings
2. Modular Avatar Integration
3. Advanced Lighting System
4. Dynamic Material System
5. Advanced Shader System
6. Performance Optimization
7. Shadow Settings
8. PCSS Settings
9. Volumetric Settings
10. Reflection Settings
11. Subsurface Scattering
12. Quality Settings
13. VRC Light Volumes
14. Anime Settings
15. Cinematic Settings
16. Rendering Settings
17. Stencil Settings

## 🐛 Troubleshooting

### Common Issues

#### Shader Not Found
- Ensure lilToon is properly installed
- Check Unity version compatibility
- Verify URP is installed and configured

#### Modular Avatar Integration Issues
- Modular Avatar AAO is optional (reflection-based integration)
- Check for any ModularAvatar version conflicts
- Ensure proper avatar setup

#### Performance Issues
- Use performance presets for better frame rates
- Enable performance optimization features
- Adjust quality settings based on your hardware

### Getting Help
- **GitHub Issues**: [Report bugs and request features](https://github.com/liltoon-pcss-extension/com.liltoon.pcss-extension/issues)
- **Documentation**: [Wiki and guides](https://github.com/liltoon-pcss-extension/com.liltoon.pcss-extension/wiki)
- **Email Support**: support@liltoon-pcss.com

## 🔄 Migration Guide

### From Version 1.8.1 to 2.0.0
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

## 📈 Roadmap

### Short-term Goals (1-3 months)
- [ ] Additional quality presets
- [ ] More detailed benchmark features
- [ ] User setting save/restore functionality

### Medium-term Goals (3-6 months)
- [ ] Enhanced real-time reflection
- [ ] More advanced volumetric shadow features
- [ ] Custom shader feature additions

### Long-term Goals (6+ months)
- [ ] Integration with other shaders (Poiyomi, Unity Standard)
- [ ] Cloud-based setting synchronization
- [ ] AI-driven automatic optimization

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.

### Development Setup
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **lilToon Team**: For the excellent base shader system
- **Modular Avatar Team**: For the powerful avatar customization system
- **Unity Technologies**: For the amazing game engine
- **VRChat Community**: For inspiration and feedback

## 📞 Contact

- **GitHub**: [liltoon-pcss-extension](https://github.com/liltoon-pcss-extension)
- **Email**: support@liltoon-pcss.com
- **Discord**: [Join our community](https://discord.gg/liltoon-pcss)

---

**Version**: 2.0.0  
**Last Updated**: 2025-01-28  
**Unity Compatibility**: 2019.4+  
**License**: MIT
