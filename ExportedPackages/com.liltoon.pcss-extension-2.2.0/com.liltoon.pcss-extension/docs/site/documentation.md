---
layout: page
title: Documentation
permalink: /documentation/
---

# Documentation

## 📚 Complete Guides

### Getting Started
- [🚀 Installation Guide]({{ site.baseurl }}/installation/) - Step-by-step installation instructions
- [⚡ Quick Start Tutorial](https://github.com/zapabob/liltoon-pcss-extension/blob/main/README.md) - Get up and running in 5 minutes
- [🔧 VCC Setup Guide](https://github.com/zapabob/liltoon-pcss-extension/blob/main/VCC_Setup_Guide.md) - VRChat Creator Companion integration

### Advanced Features
- [💡 VRC Light Volumes Integration](https://github.com/zapabob/liltoon-pcss-extension/blob/main/VRC_Light_Volumes_Integration_Guide.md) - Next-generation lighting
- [🤖 ModularAvatar Setup](https://github.com/zapabob/liltoon-pcss-extension/blob/main/PCSS_ModularAvatar_Guide.md) - Non-destructive avatar modification
- [🎮 Poiyomi Integration](https://github.com/zapabob/liltoon-pcss-extension/blob/main/PCSS_Poiyomi_VRChat_Guide.md) - Poiyomi shader compatibility

## 🎯 Feature Overview

### Core PCSS Technology

**Percentage-Closer Soft Shadows (PCSS)** creates realistic, distance-based shadow softening that mimics real-world lighting behavior.

#### Key Benefits
- **Realistic Shadows**: Natural shadow softening based on light distance
- **Performance Optimized**: Efficient implementation for VRChat
- **Cross-Platform**: Works on both PC and Quest

#### Technical Details
```hlsl
// PCSS Parameters
_PCSSEnabled: Enable/disable PCSS effect
_PCSSLightSize: Controls light source size (affects softness)
_PCSSFilterRadius: Shadow filter radius for softening
_PCSSBlockerSearchRadius: Search radius for shadow blockers
```

### lilToon Integration

#### Automatic Compatibility
- **Version Detection**: Automatically detects lilToon version
- **Material Synchronization**: Seamlessly integrates with existing materials
- **Property Mapping**: Maintains all lilToon features while adding PCSS

#### Supported lilToon Features
- ✅ **All Shading Models**: Toon, Realistic, Cloth, etc.
- ✅ **Outline System**: Compatible with all outline modes
- ✅ **Emission Features**: Works with all emission types
- ✅ **Special Effects**: Dissolve, glitter, parallax, etc.

### VRChat SDK3 Integration

#### Expression System
- **Automatic Menu Generation**: Creates VRChat expression menus
- **Parameter Setup**: Configures expression parameters
- **Real-time Control**: Toggle PCSS effects in-world

#### Performance Optimization
- **Quality Scaling**: Automatically adjusts based on VRChat performance settings
- **Platform Detection**: Optimizes for PC vs Quest
- **Avatar Density**: Adapts to avatar complexity

### ModularAvatar Support

#### Non-Destructive Workflow
- **Prefab-Based**: Easy drag-and-drop setup
- **Automatic Integration**: No manual avatar modification required
- **Reversible Changes**: Easy to remove or modify

#### Setup Process
1. Import ModularAvatar prefab
2. Drag to avatar hierarchy
3. Configure PCSS settings
4. Upload avatar

## 🔧 Configuration Reference

### Basic Settings

#### PCSS Parameters
| Parameter | Range | Description |
|-----------|--------|-------------|
| **PCSS Enabled** | On/Off | Master switch for PCSS effect |
| **Light Size** | 0.0 - 10.0 | Controls shadow softness |
| **Filter Radius** | 0.1 - 5.0 | Shadow edge softening |
| **Blocker Search** | 0.1 - 2.0 | Shadow caster detection range |

#### Performance Settings
| Setting | Options | Description |
|---------|---------|-------------|
| **Quality Level** | Low/Medium/High/Ultra | Overall quality preset |
| **Platform Target** | PC/Quest/Auto | Platform-specific optimization |
| **Avatar Density** | Low/Medium/High | Adapts to avatar complexity |

### Advanced Configuration

#### Shader Properties
```hlsl
// Main PCSS Controls
[Toggle] _PCSSEnabled ("PCSS Enabled", Float) = 1
_PCSSLightSize ("Light Size", Range(0, 10)) = 1
_PCSSFilterRadius ("Filter Radius", Range(0.1, 5)) = 1
_PCSSBlockerSearchRadius ("Blocker Search", Range(0.1, 2)) = 1

// Performance Controls
[Enum(Low,0,Medium,1,High,2,Ultra,3)] _PCSSQuality ("Quality", Float) = 2
[Toggle] _PCSSQuestOptimization ("Quest Optimization", Float) = 0
```

#### VRChat Expression Parameters
```
PCSS_Enable: Bool - Master toggle
PCSS_Intensity: Float (0-1) - Effect intensity
PCSS_Quality: Int (0-3) - Quality level
```

## 🎮 Usage Examples

### Basic Setup
1. **Apply Shader**: Change material shader to "lilToon PCSS Extension"
2. **Enable PCSS**: Check "PCSS Enabled" checkbox
3. **Adjust Settings**: Tune Light Size and Filter Radius to taste
4. **Test in Play Mode**: Verify shadows look correct

### VRChat Integration
1. **Run Setup Wizard**: Window → lilToon PCSS Extension → Setup Wizard
2. **Configure Expressions**: Set up VRChat expression menu
3. **Upload Avatar**: Test in VRChat world
4. **Fine-tune Settings**: Adjust based on in-world performance

### Performance Optimization
1. **Check Platform**: Verify PC vs Quest optimization
2. **Monitor Performance**: Use VRChat performance stats
3. **Adjust Quality**: Lower settings if needed for Quest
4. **Test with Others**: Verify performance in populated worlds

## 🔍 Troubleshooting

### Common Issues

#### Shadows Not Appearing
- **Check Light Setup**: Ensure directional light is configured
- **Verify Shader**: Confirm PCSS shader is applied
- **Enable PCSS**: Make sure PCSS Enabled is checked

#### Performance Issues
- **Lower Quality**: Reduce PCSS Quality setting
- **Enable Quest Mode**: Turn on Quest Optimization
- **Check Avatar Limits**: Ensure avatar meets VRChat performance guidelines

#### Expression Menu Not Working
- **Verify SDK3**: Ensure VRChat SDK3 Avatars is installed
- **Check Parameters**: Confirm expression parameters are set up
- **Re-run Wizard**: Use setup wizard to reconfigure

### Advanced Troubleshooting

#### Shader Compilation Errors
- **Check Unity Version**: Ensure 2019.4.31f1 or later
- **Verify Dependencies**: All required packages installed
- **Clear Cache**: Delete Library folder and reimport

#### Material Issues
- **Backup Materials**: Always backup before applying PCSS
- **Check Compatibility**: Verify lilToon version compatibility
- **Reset Settings**: Use default PCSS settings first

## 📖 API Reference

### C# Scripting API

#### PCSSUtilities Class
```csharp
// Enable/disable PCSS on material
PCSSUtilities.EnablePCSS(Material material, bool enabled);

// Set PCSS quality level
PCSSUtilities.SetQualityLevel(Material material, PCSSQuality quality);

// Optimize for platform
PCSSUtilities.OptimizeForPlatform(Material material, BuildTarget platform);
```

#### VRChatPerformanceOptimizer Class
```csharp
// Auto-optimize based on VRChat settings
VRChatPerformanceOptimizer.OptimizeForVRChat(GameObject avatar);

// Check performance rank
PerformanceRank rank = VRChatPerformanceOptimizer.GetPerformanceRank(GameObject avatar);
```

### Shader Integration

#### Custom Shader Support
```hlsl
// Include PCSS functions
#include "Packages/com.liltoon.pcss-extension/Shaders/PCSSCore.hlsl"

// In fragment shader
float4 shadowCoord = TransformWorldToShadowCoord(worldPos);
float shadow = SamplePCSSShadow(shadowCoord, _PCSSLightSize, _PCSSFilterRadius);
```

## 🔄 Version History

### v1.2.0 (Current)
- ✅ lilToon v1.10.3 compatibility
- ✅ VRC Light Volumes integration
- ✅ Enhanced Quest optimization
- ✅ Improved VCC setup wizard

### v1.1.0
- ✅ ModularAvatar integration
- ✅ VRChat expression automation
- ✅ Performance optimization system

### v1.0.0
- ✅ Initial PCSS implementation
- ✅ Basic lilToon integration
- ✅ VRChat SDK3 support

---

**Need more help?** 📞

[Installation Guide]({{ site.baseurl }}/installation/) | [Support]({{ site.baseurl }}/support/) | [Download]({{ site.baseurl }}/download/) 