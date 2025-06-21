# lilToon PCSS Extension

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4%2B-blue.svg)](https://unity3d.com/get-unity/download)
[![lilToon Compatible](https://img.shields.io/badge/lilToon-Compatible-purple.svg)](https://github.com/lilxyzw/lilToon)
[![Poiyomi Compatible](https://img.shields.io/badge/Poiyomi-Compatible-pink.svg)](https://poiyomi.com/)
[![VRChat Ready](https://img.shields.io/badge/VRChat-Ready-green.svg)](https://hello.vrchat.com/)
[![Expression Control](https://img.shields.io/badge/VRChat-Expression%20Control-red.svg)](https://hello.vrchat.com/)
[![VRC Light Volumes](https://img.shields.io/badge/VRC%20Light%20Volumes-Supported-orange.svg)](https://github.com/REDSIM/VRCLightVolumes)

**高品質なソフトシャドウをアバターに。lilToon & Poiyomi用PCSS（Percentage-Closer Soft Shadows）拡張プラグイン**

> **🆕 v1.2.0 新機能**: VRC Light Volumes統合！次世代ライティングシステムとPCSSの完全融合！

![PCSS Extension Banner](https://via.placeholder.com/800x300/2D3748/FFFFFF?text=lilToon+PCSS+Extension)

## ✨ 特徴

### 🎨 **映画品質のソフトシャドウ**
- **PCSS技術採用**: 光源からの距離に応じて自然にぼけるリアルな影
- **高品質サンプリング**: 64点Poisson Diskサンプリングによるノイズレス処理
- **アンチエイリアス**: 滑らかで美しい影の境界
- **🆕 VRC Light Volumes統合**: 次世代ボクセルライティングシステム対応

### 🚀 **簡単操作**
- **ワンクリック設定**: 品質プリセットで即座に最適化
- **lilToon & Poiyomi統合**: 既存のワークフローを変更せずに使用可能
- **VRChatエクスプレッション**: ゲーム内でのリアルタイム制御
- **GUI統合**: 直感的なインスペクター

### ⚡ **高性能**
- **適応的サンプリング**: 品質とパフォーマンスの最適バランス
- **VRChat最適化**: パフォーマンスランクを考慮した設計
- **ModularAvatar対応**: アバター改変に最適

## 📸 スクリーンショット

| 通常のシャドウ | PCSS有効 |
|:---:|:---:|
| ![Standard Shadow](https://via.placeholder.com/300x200/E2E8F0/718096?text=Standard+Shadow) | ![PCSS Shadow](https://via.placeholder.com/300x200/E2E8F0/718096?text=PCSS+Soft+Shadow) |

## 🎯 対象ユーザー

- **VRChatアバター制作者**: より美しいアバターを作りたい方
- **lilToon & Poiyomiユーザー**: 既存のワークフローを活かしたい方
- **品質重視のクリエイター**: 映画品質の影表現を求める方
- **エクスプレッション活用者**: ゲーム内で動的に設定を変更したい方

## 🔧 システム要件

| 項目 | 要件 |
|------|------|
| **Unity** | 2019.4 LTS以降 |
| **lilToon** | 最新版推奨 |
| **Poiyomi** | Toon Shader 8.0以降（オプション） |
| **VRChat SDK** | 最新版（エクスプレッション機能使用時） |
| **レンダーパイプライン** | Built-in RP / URP |
| **プラットフォーム** | PC（モバイルは制限あり） |

## 📦 インストール

### 方法1: UPMパッケージ（推奨）

1. **Unityを開く**
2. **Window > Package Manager**を選択
3. **「+」> Add package from git URL**をクリック
4. 以下のURLを入力：
   ```
   https://github.com/zapabob/liltoon-pcss-extension.git
   ```

### 方法2: UnityPackage

1. [Releases](https://github.com/zapabob/liltoon-pcss-extension/releases)から最新版をダウンロード
2. UnityプロジェクトにD&Dでインポート

> **⚠️ 注意**: 事前に[lilToon](https://github.com/lilxyzw/lilToon)をインストールしてください

## 🚀 クイックスタート

### 1. マテリアル設定

#### lilToon使用時
1. **新しいマテリアルを作成**
2. **シェーダーを「lilToon/PCSS Extension」に変更**
3. **「PCSS効果を使用」をON**
4. **品質プリセットを選択**

#### Poiyomi使用時
1. **新しいマテリアルを作成**
2. **シェーダーを「Poiyomi/Toon/PCSS Extension」に変更**
3. **「Enable PCSS」をON**
4. **品質プリセットを選択**

```
推奨設定:
- VRChat使用: Medium
- PC環境: High  
- 軽量化重視: Low
```

### 2. アバター一括適用

1. **アバターを選択**
2. **「lilToon > PCSS Avatar Setup」を開く**
3. **品質設定を選択して「適用」**

![Setup Window](https://via.placeholder.com/400x300/F7FAFC/4A5568?text=PCSS+Setup+Window)

## ⚙️ 品質設定

| 品質レベル | サンプル数 | 用途 | パフォーマンス |
|------------|------------|------|----------------|
| **Low** | 8 | モバイル・軽量化 | ⭐⭐⭐⭐⭐ |
| **Medium** | 16 | VRChat・バランス | ⭐⭐⭐⭐ |
| **High** | 32 | PC・高品質 | ⭐⭐⭐ |
| **Ultra** | 64 | 最高品質 | ⭐⭐ |

## 🎮 ModularAvatar統合

### 自動生成される制御

- **PCSS_Enable**: エフェクトのON/OFF
- **PCSS_Quality**: 品質レベル制御（0-3）

### セットアップ手順

1. **ModularAvatarをインストール**
2. **PCSS Extensionを適用**
3. **自動的にメニューとパラメータが生成**

## 📊 パフォーマンス最適化

### 推奨設定

```hlsl
// VRChat用推奨設定
Blocker Search Radius: 0.01
Filter Radius: 0.02
Sample Count: 16
Light Size: 1.5
```

### 最適化のコツ

- **不要なオブジェクト**: Cast Shadowsをオフ
- **小さなオブジェクト**: Receive Shadowsをオフ
- **Shadow Distance**: 適切な値に設定（50-150）

## 🔌 API リファレンス

### 基本的な使用方法

```csharp
using lilToon.PCSS;

// アバターにPCSSを適用
PCSSUtilities.ApplyToAvatar(avatar, PCSSQuality.Medium, true);

// マテリアルでPCSSを有効化
PCSSUtilities.EnablePCSS(material, true);

// 品質設定を適用
PCSSUtilities.ApplyQualitySettings(material, PCSSQuality.High);

// 設定の検証
bool isValid = PCSSUtilities.ValidatePCSSSettings(material);
```

### 高度な制御

```csharp
// パフォーマンス警告の確認
string warning = PCSSUtilities.GetPerformanceWarning(material);
if (!string.IsNullOrEmpty(warning))
{
    Debug.LogWarning(warning);
}

// PCSS対応マテリアルの確認
bool isPCSSMaterial = PCSSUtilities.IsPCSSMaterial(material);
```

## 🛠️ トラブルシューティング

### よくある問題

<details>
<summary><strong>影が表示されない</strong></summary>

**解決方法:**
1. Lightの**Shadows**が**Soft Shadows**になっているか確認
2. オブジェクトの**Cast Shadows**と**Receive Shadows**が有効か確認
3. **Use Shadow**が有効になっているか確認
</details>

<details>
<summary><strong>パフォーマンスが悪い</strong></summary>

**解決方法:**
1. **Sample Count**を下げる（16以下推奨）
2. **Shadow Resolution**を下げる
3. **Shadow Distance**を短くする
</details>

<details>
<summary><strong>影が粗い</strong></summary>

**解決方法:**
1. **Shadow Resolution**を上げる（2048以上）
2. **Sample Count**を増やす
3. **Shadow Bias**を調整する（0.05-0.1）
</details>

## 📚 ドキュメント

- [📖 基本使用ガイド](liltoon_pcss_readme.md)
- [🎮 ModularAvatar統合ガイド](PCSS_ModularAvatar_Guide.md)
- [🎨 Poiyomi & VRChatエクスプレッションガイド](PCSS_Poiyomi_VRChat_Guide.md)
- [📋 実装ログ](_docs/2025-01-24_liltoon-pcss-extension.md)

## 🤝 コントリビューション

プロジェクトへの貢献を歓迎します！

1. **Forkする**
2. **フィーチャーブランチを作成** (`git checkout -b feature/amazing-feature`)
3. **変更をコミット** (`git commit -m 'Add amazing feature'`)
4. **ブランチにプッシュ** (`git push origin feature/amazing-feature`)
5. **Pull Requestを作成**

## 📄 ライセンス

このプロジェクトは[MIT License](LICENSE)の下で公開されています。

## 🙏 謝辞

- **[lilxyzw](https://github.com/lilxyzw)** - 素晴らしいlilToonシェーダーシステム
- **VRChatコミュニティ** - フィードバックとサポート
- **Unity Technologies** - 開発環境の提供

## 📞 サポート

- **Issues**: [GitHub Issues](https://github.com/zapabob/liltoon-pcss-extension/issues)
- **Discord**: [VRChatアバター制作サーバー](#)
- **Twitter**: [@zapabob](#)

## 🔗 関連リンク

- [lilToon公式](https://github.com/lilxyzw/lilToon)
- [VRChat公式](https://hello.vrchat.com/)
- [ModularAvatar](https://modular-avatar.nadena.dev/)
- [Unity公式](https://unity.com/)

---

<div align="center">

**🌟 このプロジェクトが役に立ったら、スターをつけてください！ 🌟**

[![GitHub stars](https://img.shields.io/github/stars/zapabob/liltoon-pcss-extension?style=social)](https://github.com/zapabob/liltoon-pcss-extension/stargazers)

</div> 