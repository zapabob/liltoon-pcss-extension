# lilToon PCSS Extension v2.7.0

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/Unity-2022.3%20LTS-blue.svg)](https://unity3d.com/get-unity/download)
[![lilToon Compatible](https://img.shields.io/badge/lilToon-2.3.2%2B-purple.svg)](https://github.com/lilxyzw/lilToon)
[![Modular Avatar Compatible](https://img.shields.io/badge/Modular%20Avatar-1.15.1%2B-orange.svg)](https://github.com/bdunderscore/modular-avatar)
[![VRChat SDK3](https://img.shields.io/badge/VRChat%20SDK3-Ready-green.svg)](https://docs.vrchat.com/)
[![VPM-Compatible](https://img.shields.io/badge/VPM-Compatible-brightgreen.svg)](https://vcc.docs.vrchat.com/)

**lilToonシェーダーのための、高品質なPCSS（Percentage-Closer Soft Shadows）拡張機能です。**
VRChatアバターに、映画のようなリアルタイムソフトシャドウをワンクリックで導入し、多彩な補助機能でアバター制作を強力にサポートします。

---

**A high-quality PCSS (Percentage-Closer Soft Shadows) extension for lilToon shaders.**
Add cinematic real-time soft shadows to VRChat avatars with one-click setup, supported by various auxiliary features that powerfully assist avatar creation.

---

## ✨ 主な機能 / Main Features

本パッケージは、単なるシェーダー拡張にとどまらず、アバター制作ワークフロー全体を効率化するための多機能ツールセットを提供します。

This package provides a multifunctional toolset that goes beyond mere shader extensions to streamline the entire avatar creation workflow.

### **<ins>シェーダー・グラフィックス機能 / Shader & Graphics Features</ins>**
- **高品質PCSS（ソフトシャドウ）**: `lilToon/PCSS Extension`シェーダーを使用することで、リアルタイムで高品質なソフトシャドウをアバターに適用できます。
- **High-Quality PCSS (Soft Shadows)**: Use the `lilToon/PCSS Extension` shader to apply high-quality soft shadows to avatars in real-time.
- **4つの影プリセット**: 「リアリスティック」「アニメ調」「映画的」など、ワンクリックで適用できる4種類の影プリセットを内蔵。
- **4 Shadow Presets**: Includes 4 shadow presets such as "Realistic", "Anime-style", and "Cinematic" that can be applied with one click.
- **PhysBone連動エミッションライト**: PhysBoneの動きに連動して発光するライトを簡単にセットアップ可能。
- **PhysBone-Linked Emission Lights**: Easily set up lights that emit based on PhysBone movements.
- **表情赤み対策ライト**: 表情変更時に顔が不自然に赤くなる現象を抑制する専用ライト機能を搭載。
- **Facial Redness Prevention Lights**: Features dedicated lights to prevent unnatural facial redness during expression changes.

### **<ins>セットアップ・自動化機能 / Setup & Automation Features</ins>**
- **アバター選択メニュー**: `Window > lilToon PCSS Extension > Avatar Selector` から、シーン内のアバターにワンクリックでPCSS機能をセットアップできます。
- **Avatar Selector Menu**: Set up PCSS features for avatars in the scene with one click from `Window > lilToon PCSS Extension > Avatar Selector`.
- **Modular Avatar完全連携**: Modular Avatar（MA）を利用した非破壊的なセットアップに対応。MAメニューにPCSS設定用のトグルを自動で構築します。
- **Full Modular Avatar Integration**: Supports non-destructive setup using Modular Avatar (MA). Automatically builds PCSS setting toggles in MA menus.
- **競合製品からの移行ウィザード**: `nHaruka PCSSForVRC`など、他のPCSS製品からの設定をスムーズに移行するための専用ウィザードを搭載。
- **Migration Wizard from Competing Products**: Includes a dedicated wizard for smooth migration of settings from other PCSS products like `nHaruka PCSSForVRC`.

### **<ins>エディタ補助・ユーティリティ機能 / Editor Assistance & Utility Features</ins>**
- **Missingマテリアル自動修復**: `Tools > lilToon PCSS > Missing Material AutoFixer` から、モデルインポート時などに発生するマテリアルの参照切れ（Missing）を自動で検出し、復旧させることができます。
- **Missing Material Auto-Fixer**: Automatically detects and recovers missing material references that occur during model imports from `Tools > lilToon PCSS > Missing Material AutoFixer`.
- **GUIDベースの強力なマテリアルバックアップ**: アバターのアップロード時や手動操作時に、マテリアル設定を安全にバックアップ・復元します。ファイル名やパスを変更しても追跡可能なため、事故からの復旧が容易です。
- **Powerful GUID-Based Material Backup**: Safely backs up and restores material settings during avatar uploads or manual operations. Can track materials even if filenames or paths change, making recovery from accidents easy.
- **各種自動修正機能**: VRChatビルド時の一般的なエラーや、lilToonのバージョンアップに伴う非推奨設定（メッシュ暗号化、ファーのSubdivisionモードなど）を自動で検出し、修正を促します。
- **Various Auto-Fix Features**: Automatically detects common VRChat build errors and deprecated settings from lilToon version updates (mesh encryption, fur Subdivision mode, etc.) and prompts for corrections.
- **パフォーマンス最適化**: Quest環境などを考慮し、アバターのパフォーマンスを最適化するためのツールを提供します。
- **Performance Optimization**: Provides tools to optimize avatar performance, considering environments like Quest.

---

## 🚀 導入方法 (VCC推奨) / Installation (VCC Recommended)

VRChat Creator Companion (VCC) を使った導入が最も簡単で確実です。

Installation using VRChat Creator Companion (VCC) is the easiest and most reliable method.

1.  VCCを開き、**[Settings] > [Packages]** を選択します。
2.  Open VCC and select **[Settings] > [Packages]**.
3.  `User Packages` のリストに、以下のリポジトリURLを追加して **[Add]** ボタンを押します。
4.  Add the following repository URL to the `User Packages` list and click **[Add]**.
    ```
    https://zapabob.github.io/liltoon-pcss-extension/index.json
    ```
5.  管理したいプロジェクトの **[Manage Project]** を開き、「lilToon PCSS Extension」が表示されていることを確認し、**[+]** ボタンでプロジェクトに追加します。
6.  Open **[Manage Project]** for the project you want to manage, confirm that "lilToon PCSS Extension" is displayed, and add it to the project with the **[+]** button.
7.  Unityが起動し、`Window`メニューに`lilToon PCSS Extension`が表示されていれば導入は完了です。
8.  Launch Unity, and if `lilToon PCSS Extension` appears in the `Window` menu, the installation is complete.

---

## 📖 基本的な使い方 / Basic Usage

### 1. ワンクリック・セットアップ / One-Click Setup
最も簡単な方法は、統合セットアップメニューを使用することです。

The easiest method is to use the integrated setup menu.

1.  Unityの上部メニューから **[Window] > [lilToon PCSS Extension] > [Avatar Selector]** を選択します。
2.  Select **[Window] > [lilToon PCSS Extension] > [Avatar Selector]** from Unity's top menu.
3.  ウィンドウが開くと、シーンに存在するアバターが自動的にリストアップされます。
4.  When the window opens, avatars in the scene are automatically listed.
5.  設定を適用したいアバターをリストから選択します。
6.  Select the avatar you want to apply settings to from the list.
7.  好みの **プリセット**（例: Realistic）を選択し、**[適用 (Apply)]** ボタンをクリックします。
8.  Select your preferred **preset** (e.g., Realistic) and click the **[適用 (Apply)]** button.
9.  これだけで、アバターにPCSS対応シェーダーが適用され、Modular Avatarを使っている場合は表情メニューにライトのON/OFFトグルが自動的に追加されます。
10. With this alone, the PCSS-compatible shader is applied to the avatar, and if using Modular Avatar, light ON/OFF toggles are automatically added to the expression menu.

### 2. シェーダーの手動設定 / Manual Shader Setup
マテリアルごとに手動で設定することも可能です。

You can also configure each material manually.

1.  アバターのマテリアルを選択し、Shaderのドロップダウンメニューから **[lilToon] > [PCSS Extension]** を選択します。
2.  Select the avatar's material and choose **[lilToon] > [PCSS Extension]** from the Shader dropdown menu.
3.  マテリアルのインスペクターにPCSS関連の設定項目（影の柔らかさ、範囲など）が表示されるので、好みに合わせて調整します。
4.  PCSS-related settings (shadow softness, range, etc.) will appear in the material's inspector, so adjust them to your preference.

---

## 🛠️ 各種ツールの使い方 / Tool Usage

本パッケージには、制作を補助する様々なツールが含まれています。

This package includes various tools to assist with creation.

-   **Missingマテリアル修復 / Missing Material Repair**:
    -   メニュー: `Tools > lilToon PCSS > Missing Material AutoFixer`
    -   Menu: `Tools > lilToon PCSS > Missing Material AutoFixer`
    -   機能: シーン内の`Missing`状態のマテリアルをスキャンし、プロジェクト内から同名のマテリアルを検索して自動的に割り当て直します。
    -   Function: Scans for materials in `Missing` state in the scene, searches for materials with the same name within the project, and automatically reassigns them.

-   **マテリアルバックアップ / Material Backup**:
    -   手動バックアップ: `Tools > lilToon PCSS > Material Backup`
    -   Manual Backup: `Tools > lilToon PCSS > Material Backup`
    -   自動バックアップ: VRChatアバターのアップロード時に自動で実行されます。
    -   Auto Backup: Automatically executed when uploading VRChat avatars.
    -   復元: 上記メニューから復元したいバックアップデータを選択して実行します。
    -   Restore: Select and execute the backup data you want to restore from the menu above.

-   **lilToonバージョン移行ツール / lilToon Version Migration Tool**:
    -   メニュー: `Tools > lilToon PCSS > Migration Tools`
    -   Menu: `Tools > lilToon PCSS > Migration Tools`
    -   機能: 古いlilToonの機能（ファーのShrinkモードなど）を使用しているマテリアルを検出し、最新バージョン（Subdivisionモード）へ設定を変換します。
    -   Function: Detects materials using old lilToon features (such as fur Shrink mode) and converts settings to the latest version (Subdivision mode).

---

## ⚙️ システム要件 / System Requirements

| 項目 / Item | 推奨バージョン / Recommended Version |
| :--- | :--- |
| **Unity** | 2022.3.6f1 以降 / 2022.3.6f1 or later |
| **lilToon** | 1.7.0 以降 (2.3.2以上を強く推奨) / 1.7.0 or later (2.3.2+ strongly recommended) |
| **VRChat Avatars SDK** | 3.5.0 以降 / 3.5.0 or later |
| **Modular Avatar** | 1.15.1 以降 / 1.15.1 or later |

---

## 📜 ライセンス / License

このプロジェクトは [MIT License](LICENSE) の下で公開されています。

This project is released under the [MIT License](LICENSE).

---

## 💬 サポート・連絡先 / Support & Contact

-   **公式ドキュメント / Official Documentation**: [https://zapabob.github.io/liltoon-pcss-extension/](https://zapabob.github.io/liltoon-pcss-extension/)
-   **GitHub (不具合報告) / GitHub (Bug Reports)**: [https://github.com/zapabob/liltoon-pcss-extension/issues](https://github.com/zapabob/liltoon-pcss-extension/issues)
-   **Booth**: [https://zapabob.booth.pm/](https://zapabob.booth.pm/)