# lilToon PCSS Extension v1.5.9

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/Unity-2022.3%20LTS-blue.svg)](https://unity3d.com/get-unity/download)
[![lilToon Compatible](https://img.shields.io/badge/lilToon-Compatible-purple.svg)](https://github.com/lilxyzw/lilToon)
[![Poiyomi Compatible](https://img.shields.io/badge/Poiyomi-Compatible-pink.svg)](https://poiyomi.com/)
[![VRChat SDK3](https://img.shields.io/badge/VRChat%20SDK3-Ready-green.svg)](https://docs.vrchat.com/)
[![VPM-Compatible](https://img.shields.io/badge/VPM-Compatible-brightgreen.svg)](https://vcc.docs.vrchat.com/)

**lilToonおよびPoiyomiシェーダーのための、高品質なPCSS（Percentage-Closer Soft Shadows）拡張機能です。**

A professional PCSS (Percentage-Closer Soft Shadows) extension for lilToon and Poiyomi shaders, with full VRChat/Unity 2022.3 LTS/URP 14.0.10 support and one-click avatar setup.

## 主な修正点
- PhysBoneLightController: 先行製品互換性・安定性向上、外部ライト指定・自動検出の堅牢化、距離減衰・スムージング制御の最適化、エラー時の自動無効化・デバッグ強化
- Unity 2022.3 LTS/VRChat SDK 最新版での動作検証・最適化

## 新機能・主な変更点
- PhysBoneエミッシブ球体セットアップ機能を全面リファクタリング
    - Lightコンポーネント依存を完全排除し、Renderer/MaterialのEmission制御型に刷新
    - プリセット（Toon/Realistic/Dark）でEmission色・強度・位置を自動設定
    - AutoFIX完全対応・外部依存ゼロ
- CompetitorSetupWizard.csもEmission制御型に統一

---

## ✨ 主な機能 / Features

- 🎬 映画品質のソフトシャドウ / Cinematic-quality soft shadows
- 🎯 ワンクリック・セットアップ / One-click avatar setup (Avatar Selector Menu)
- 🎨 4つのプリセット / 4 built-in presets (Realistic, Anime, Cinematic, Custom)
- 🚀 VRChat/Quest自動パフォーマンス最適化 / Auto performance optimization for VRChat/Quest
- 🛡️ ModularAvatar連携・エラー耐性強化 / ModularAvatar integration & error resilience
- ⚡ URP 14.0.10/Unity 2022.3 LTS完全対応 / Full URP 14.0.10 & Unity 2022.3 LTS support
- 📝 サンプル・日本語UI・ドキュメント / Samples, Japanese UI, documentation
- 🧩 **MissingMaterialAutoFixer（Editor拡張）: シーン内のMissingマテリアル検出＆自動修復ウィンドウ**
- 🔄 **FBX/Prefabインポート時の自動リマップ: Missingスロットを自動でlilToonマテリアルに補完**

---

## 🚀 インストール方法 / Installation

### VCC (VRChat Creator Companion) 推奨
1. VCCの「Settings > Packages」で下記URLを追加
   https://zapabob.github.io/liltoon-pcss-extension/index.json
2. プロジェクトの「Add」から「lilToon PCSS Extension」を選択

### Unity Package Manager (Git URL)
1. Unityの `Window > Package Manager` を開く
2. `+` →「Add package from git URL...」で下記を入力
   https://github.com/zapabob/liltoon-pcss-extension.git?path=com.liltoon.pcss-extension

### Booth版（手動インストール）
1. Boothでzipをダウンロードし、解凍して`com.liltoon.pcss-extension`フォルダごとPackagesディレクトリにコピー
2. Unity再起動後、`Window > lilToon PCSS Extension > Avatar Selector`が表示されていれば導入完了

---

## 📖 使い方 / Usage
1. Unityメニュー `Window > lilToon PCSS Extension > Avatar Selector` を選択
2. シーン内のアバターが自動リストアップ
3. プリセットを選択し「適用 (Apply)」をクリック
4. **MissingMaterialAutoFixer**: `Tools > lilToon PCSS > Missing Material AutoFixer` からシーン内のMissingマテリアルを検出・自動修復可能
5. **FBX/Prefab自動リマップ**: FBXやPrefabインポート時にMissingスロットが自動でlilToonマテリアルに補完されます（手動操作不要）

## 使い方（PhysBoneエミッシブ球体セットアップ）
1. Unityエディタでアバターを選択
2. メニューから「Tools > lilToon PCSS > PhysBoneエミッシブセットアップウィザード」を開く
3. プリセットを選択し、「PhysBoneエミッシブを適用」ボタンを押す
4. アバター直下にエミッシブ球体が生成され、PhysBoneで掴んで動かせる発光オブジェクトとして機能

---

## 🛠️ システム要件 / Requirements
| 項目 / Item | バージョン / Version |
| --- | --- |
| **Unity** | 2022.3.6f1+ |
| **lilToon** | 1.7.0+ |
| **VRChat Avatars SDK** | 3.5.0+ |
| **Modular Avatar** | 1.10.0+ |

---

## 📝 サポート・公式情報 / Support & Official Info
- 公式サイト / Official: https://zapabob.github.io/liltoon-pcss-extension/
- GitHub: https://github.com/zapabob/liltoon-pcss-extension
- Booth: https://zapabob.booth.pm/
- 質問・不具合: GitHub IssueまたはBoothメッセージまで

---

## ライセンス / License
このプロジェクトは [MIT License](LICENSE) で公開されています。 