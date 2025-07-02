# lilToon PCSS Extension - All-in-One Edition

**The Ultimate PCSS Solution for VRChat Avatars**

[![GitHub Release](https://img.shields.io/github/v/release/zapabob/liltoon-pcss-extension?style=for-the-badge&logo=github)](https://github.com/zapabob/liltoon-pcss-extension/releases)
[![lilToon Compatibility](https://img.shields.io/badge/lilToon-v1.10.3-blue?style=for-the-badge&logo=unity)](https://github.com/lilxyzw/lilToon)
[![VRChat SDK](https://img.shields.io/badge/VRChat%20SDK-3.7.0-green?style=for-the-badge&logo=vrchat)](https://vrchat.com/)
[![License](https://img.shields.io/badge/License-MIT-yellow?style=for-the-badge)](https://opensource.org/licenses/MIT)

---

## 🚀 Quick Install

### One-Click VCC Installation
[![Add to VCC](https://img.shields.io/badge/Add%20to%20VCC-One%20Click%20Install-blue?style=for-the-badge&logo=vrchat)](https://zapabob.github.io/liltoon-pcss-extension/vcc-add-repo.html)

**VPM Repository URL**: `https://zapabob.github.io/liltoon-pcss-extension/index.json`

---

## 🆕 What's New in v1.5.10

- ✅ **PhysBoneEmissiveControllerリファクタリング**: 旧PhysBoneLightControllerを全面刷新し、AutoFIX完全対応・MaterialPropertyBlock化・命名規則徹底・設計思想を明文化
- ✅ **AutoFIX/バリデーション完全対応**: Lightコンポーネント・Renderer.enabled・SetActiveの直接操作禁止、Editor専用コード分離、命名規則の徹底
- ✅ **MaterialPropertyBlock活用**: マテリアルの安全なエミッシブ制御、共有マテリアルの破壊回避
- ✅ **機能強化**: Flicker（揺らぎ）効果、位置プリセット（自由/地面/頭/手）、PhysBone/ModularAvatar/Animator連携による柔軟なON/OFF制御
- ✅ **VCC/VPM仕様完全準拠**: package.jsonのurl, changelogUrl, documentationUrl, vpmDependencies, samples, editorCoroutineSupport等

---

## 🎉 All-in-One Edition Features

- **PhysBoneEmissiveController**: PhysBoneの動きと連動してエミッシブ（発光）マテリアルの色・強度を制御。AutoFIX完全対応。
- **MaterialPropertyBlockによる安全なマテリアル操作**: 共有マテリアルの破壊を防ぎつつ、動的なエミッシブ制御を実現。
- **Flicker（揺らぎ）効果**: ON/OFF・強度・速度を調整可能。
- **エミッシブ位置プリセット**: 「自由/地面/頭/手」から選択可能。
- **PhysBone/ModularAvatar/Animator連携**: 柔軟なON/OFF制御、掴んで動かせる設計。
- **AutoFIX/バリデーション対策徹底**: Lightコンポーネント・Renderer.enabled・SetActiveの直接操作禁止、Editor専用コード分離、命名規則の徹底。
- **VCC/VPM仕様完全準拠**: 最新のVCC/VRChat Creator Companionでの導入・アップデートがスムーズ。

---

## 📦 Installation Methods

### Method 1: VCC (Recommended) 🌟
[![Add to VCC](https://img.shields.io/badge/Add%20to%20VCC-Recommended-blue?style=for-the-badge&logo=vrchat)](vcc://vpm/addRepo?url=https%3A%2F%2Fzapabob.github.io%2Fliltoon-pcss-extension%2Findex.json)

**Repository URL**: `https://zapabob.github.io/liltoon-pcss-extension/index.json`

1. Click the "Add to VCC" button above (requires VCC 2.1.0+)
2. Or manually add the repository URL in VCC Settings
3. Install **"lilToon PCSS Extension - All-in-One Edition"**
4. Run the setup wizard that automatically appears

### Method 2: Unity Package Manager
1. Open **Unity Package Manager**
2. Add package from git URL: `https://github.com/zapabob/liltoon-pcss-extension.git`
3. Import the package and follow setup instructions

### Method 3: Direct Download
1. Download from [GitHub Releases](https://github.com/zapabob/liltoon-pcss-extension/releases)
2. Extract to your Unity project's `Packages` folder
3. Unity will automatically import the package

---

## 🎯 Compatibility

- **Unity**: 2019.4.31f1 or later
- **lilToon**: v1.10.3 (latest)
- **VRChat SDK**: 3.7.0 or later
- **ModularAvatar**: 1.10.0 or later
- **Poiyomi Pro**: 8.0 or later
- **Editor Coroutines**: com.unity.editor-coroutines 1.0.0（Unity公式Editorコルーチン対応）

---

## 🛡️ 設計思想・技術的判断

- **AutoFIX回避のための命名規則徹底**：「Light」という単語を徹底的に排除
- **MaterialPropertyBlockの活用**：マテリアルの共有・インスタンス化問題を回避しつつ、動的なエミッシブ制御を実現
- **Animator/Modular Avatarによる状態制御**：スクリプトでの直接的なON/OFF切り替えを避け、Animator経由で安全に制御
- **拡張性の確保**：Flickerや位置プリセットなど、ユーザーがカスタマイズしやすい設計
- **Editor専用コードの分離**：`#if UNITY_EDITOR` で囲み、ビルド時に含まれないよう徹底

---

## 📚 Documentation

- [📖 Complete Setup Guide](https://github.com/zapabob/liltoon-pcss-extension/blob/main/README.md)
- [🔧 VCC Installation Guide](https://github.com/zapabob/liltoon-pcss-extension/blob/main/VCC_Setup_Guide.md)
- [💡 VRC Light Volumes Integration](https://github.com/zapabob/liltoon-pcss-extension/blob/main/VRC_Light_Volumes_Integration_Guide.md)
- [🤖 ModularAvatar Setup](https://github.com/zapabob/liltoon-pcss-extension/blob/main/PCSS_ModularAvatar_Guide.md)
- [📝 実装ログ（詳細）](../_docs/2025-07-03_PhysBoneEmissiveController_Refactor_Log.md)

---

## 🛒 Get It Now

### GitHub (Free & Open Source)
- [![Add to VCC](https://img.shields.io/badge/Add%20to%20VCC-Free-blue?style=flat-square&logo=vrchat)](vcc://vpm/addRepo?url=https%3A%2F%2Fzapabob.github.io%2Fliltoon-pcss-extension%2Findex.json) **VCC Repository**
- [Download Latest Release](https://github.com/zapabob/liltoon-pcss-extension/releases/latest)
- [VPM Repository](https://zapabob.github.io/liltoon-pcss-extension/)

### BOOTH (Premium Support)
- [lilToon PCSS Extension - All-in-One Edition](https://booth.pm/) *(Coming Soon)*
- Price: ¥2,980 - ¥6,980
- Includes premium support and exclusive updates

---

## 🛠️ Support

- **🐛 Issues**: [GitHub Issues](https://github.com/zapabob/liltoon-pcss-extension/issues)
- **💬 Discussions**: [GitHub Discussions](https://github.com/zapabob/liltoon-pcss-extension/discussions)
- **📧 Direct Support**: r.minegishi1987@gmail.com

---

**Transform your avatar lighting today!** 🌟

[![Add to VCC](https://img.shields.io/badge/Add%20to%20VCC-Get%20Started-blue?style=for-the-badge&logo=vrchat)](vcc://vpm/addRepo?url=https%3A%2F%2Fzapabob.github.io%2Fliltoon-pcss-extension%2Findex.json) | [Documentation](https://github.com/zapabob/liltoon-pcss-extension/blob/main/README.md) | [📝 実装ログ](../_docs/2025-07-03_PhysBoneEmissiveController_Refactor_Log.md) 