# VCC（VRChat Creator Companion）導入ガイド

## 📋 概要

lilToon PCSS Extension v1.2.0は、VCC（VRChat Creator Companion）に完全対応しており、ワンクリックで簡単にインストールできます。

## 🚀 クイックスタート（推奨）

### 1. VCCリポジトリの追加

1. **VCCを起動**
2. **Settings** → **Packages** → **Add Repository**
3. **リポジトリURL**を入力:
   ```
   https://zapabob.github.io/liltoon-pcss-extension/index.json
   ```
4. **Add**をクリック

### 2. パッケージのインストール

1. **プロジェクトを選択**
2. **Manage Project**をクリック
3. **lilToon PCSS Extension**を検索
4. **Install**をクリック

### 3. 自動セットアップ完了！

- 依存関係が自動的にインストールされます
- ModularAvatarコンポーネントが自動で設定されます
- エクスプレッションメニューが自動生成されます

## 📦 依存関係

VCCが以下の依存関係を自動的にインストールします：

### 必須依存関係
- **VRChat SDK3 Avatars** (3.4.0以降)
- **VRChat SDK3 Base** (3.4.0以降)
- **lilToon** (1.3.0以降)
- **ModularAvatar** (1.9.0以降)

### オプション依存関係
- **VRC Light Volumes** (推奨)
- **Poiyomi Toon Shader** (Poiyomi使用時)

## 🔧 手動インストール（上級者向け）

### 方法1: Unitypackageファイル

1. [Releases](https://github.com/zapabob/liltoon-pcss-extension/releases)から最新版をダウンロード
2. `com.liltoon.pcss-extension-1.2.0.unitypackage`をダウンロード
3. Unityで**Assets** → **Import Package** → **Custom Package**
4. ダウンロードしたファイルを選択してインポート

### 方法2: GitHubからクローン

```bash
cd YourProject/Packages
git clone https://github.com/zapabob/liltoon-pcss-extension.git com.liltoon.pcss-extension
```

## 🎯 セットアップ手順

### 1. 基本セットアップ

1. **アバターを選択**
2. **Window** → **lilToon PCSS Extension** → **Setup Wizard**
3. **Auto Setup**をクリック
4. 品質プリセットを選択（推奨: **Medium**）

### 2. マテリアル設定

1. **マテリアルを選択**
2. **Shader**を以下のいずれかに変更:
   - `lilToon/PCSS Extension`
   - `Poiyomi/PCSS Extension`
3. **Quality Preset**で品質を選択

### 3. ModularAvatar設定

1. **アバターのルート**に`MA PCSS Setup`コンポーネントが自動追加
2. **エクスプレッションメニュー**が自動生成
3. **パラメータ**が自動設定

## 🎮 VRChatでの使用方法

### エクスプレッションメニュー

**Action Menu** → **Options** → **PCSS**

- **Enable/Disable**: PCSS機能のON/OFF
- **Quality**: 品質設定（Low/Medium/High/Ultra）
- **Auto Optimize**: 自動最適化のON/OFF

### 品質設定ガイド

| 品質レベル | サンプル数 | 用途 | 推奨環境 |
|-----------|-----------|------|----------|
| **Low** | 8 | Quest・軽量化 | Quest 2/Pro |
| **Medium** | 16 | バランス重視 | PC標準 |
| **High** | 32 | 高品質 | PC高性能 |
| **Ultra** | 64 | 最高品質 | PC最高性能 |

## 🔧 トラブルシューティング

### よくある問題

#### 1. パッケージが見つからない
**解決方法:**
- VCCのリポジトリURLが正しく追加されているか確認
- インターネット接続を確認
- VCCを再起動

#### 2. 依存関係エラー
**解決方法:**
- VRChat SDK3が最新版か確認
- lilToonが正しくインストールされているか確認
- ModularAvatarが最新版か確認

#### 3. シェーダーが表示されない
**解決方法:**
- プロジェクトを再インポート
- シェーダーキャッシュをクリア（**Edit** → **Preferences** → **GI Cache** → **Clear Cache**）

#### 4. エクスプレッションメニューが表示されない
**解決方法:**
- アバターにVRC Avatar Descriptorがあるか確認
- ModularAvatarコンポーネントが正しく設定されているか確認
- **Tools** → **Modular Avatar** → **Manual bake avatar**を実行

### サポート

問題が解決しない場合は以下からサポートを受けられます：

- **GitHub Issues**: [問題を報告](https://github.com/zapabob/liltoon-pcss-extension/issues)
- **Discord**: [VRChatアバター制作サーバー](https://discord.gg/vrchat-creators)
- **Twitter**: [@zapabob_ouj](https://twitter.com/zapabob_ouj)

## 📚 追加リソース

### サンプルファイル

VCCインストール後、以下のサンプルが利用可能になります：

1. **Window** → **Package Manager**
2. **In Project**を選択
3. **lilToon PCSS Extension**を選択
4. **Samples**タブから以下をインポート:
   - **PCSS Sample Materials**
   - **VRChat Expression Setup**
   - **ModularAvatar Prefabs**

### ドキュメント

- **README**: [基本的な使用方法](README.md)
- **API Reference**: [開発者向けドキュメント](API_Reference.md)
- **Performance Guide**: [パフォーマンス最適化ガイド](Performance_Guide.md)

## 🔄 アップデート

### 自動アップデート

VCCが新しいバージョンを自動的に検出し、アップデート通知を表示します。

### 手動アップデート確認

1. **VCC**を起動
2. **プロジェクトを選択**
3. **Manage Project**
4. **Updates Available**セクションを確認

## 🎉 完了！

これでlilToon PCSS Extensionの導入が完了しました！

映画品質のソフトシャドウをVRChatアバターで楽しんでください！

---

**最終更新**: 2025年1月24日  
**対応バージョン**: v1.2.0  
**VCC対応**: ✅ 完全対応 