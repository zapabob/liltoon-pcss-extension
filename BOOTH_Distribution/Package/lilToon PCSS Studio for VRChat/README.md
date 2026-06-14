# lilToon PCSS Extension v2.8.0

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/Unity-2022.3%20LTS-blue.svg)](https://unity3d.com/get-unity/download)
[![lilToon Compatible](https://img.shields.io/badge/lilToon-2.3.2%2B-purple.svg)](https://github.com/lilxyzw/lilToon)
[![VRChat SDK3](https://img.shields.io/badge/VRChat%20SDK-3.10.3-green.svg)](https://creators.vrchat.com/)
[![VPM-Compatible](https://img.shields.io/badge/VPM-Compatible-brightgreen.svg)](https://vcc.docs.vrchat.com/)

**lilToonシェーダーのための、高品質なPCSS（Percentage-Closer Soft Shadows）拡張機能です。**
VRChatアバターに、映画のようなリアルタイムソフトシャドウをワンクリックで導入し、多彩な補助機能でアバター制作を強力にサポートします。

---

## 2.8.0 の更新点

- **VRChat SDK 3.10.3 / Unity 2022.3 系に合わせた更新**: `package.json` と VPM 依存を更新し、VRChat Creator Companion で扱いやすい構成に整えました。
- **ヌルテカ協調**: 影が強い場所でハイライトだけが浮く状態を抑え、明部では艶を伸ばせる `Gloss Shadow Coherence` 系パラメータを追加しました。
- **VRChat向け距離ゲート**: PCSS の見た目を近距離で保ちつつ、遠距離ではサンプル数と強度を下げる制御を追加しました。
- **nHaruka PCSSForVRC 研究反映**: アバター追従ライト、PhysBone制御、Expressionメニュー、ローカル/リモート影切替という競合側の強みを踏まえ、本パッケージでは lilToon 影レイヤー、VRC Light Volumes、Gloss Coherence、Hubプリセットの一体運用を強化しました。
- **配布コピーの整合**: ルート側と `Shaders/` 配下の shader/include/runtime/package コピーを同期しました。

---

## ✨ 主な機能

本パッケージは、単なるシェーダー拡張にとどまらず、アバター制作ワークフロー全体を効率化するための多機能ツールセットを提供します。

### **<ins>シェーダー・グラフィックス機能</ins>**
- **高品質PCSS（ソフトシャドウ）**: `lilToon/PCSS Extension`シェーダーを使用することで、リアルタイムで高品質なソフトシャドウをアバターに適用できます。
- **3つの影プリセット**: 「アニメ」「リアル影」「映画」など、ワンクリックで適用できる3種類の影プリセットを内蔵。
- **PhysBone連動エミッションライト**: PhysBoneの動きに連動して発光するライトを簡単にセットアップ可能。
- **表情赤み対策ライト**: 表情変更時に顔が不自然に赤くなる現象を抑制する専用ライト機能を搭載。

### **<ins>セットアップ・自動化機能</ins>**
- **アバター選択メニュー**: `Window > lilToon PCSS Extension > Avatar Selector` から、シーン内のアバターにワンクリックでPCSS機能をセットアップできます。
- **Modular Avatar完全連携**: Modular Avatar（MA）を利用した非破壊的なセットアップに対応。MAメニューにPCSS設定用のトグルを自動で構築します。
- **PhysBoneライト方向揺れ（任意）**: PhysBoneでライト方向を揺らし、自然な陰影変化を演出できます。揺れ強さはスライダーで調整可能です。
- **競合製品からの移行ウィザード**: `nHaruka PCSSForVRC`など、他のPCSS製品からの設定をスムーズに移行するための専用ウィザードを搭載。

### **<ins>エディタ補助・ユーティリティ機能</ins>**
- **Missingマテリアル自動修復**: `Tools > lilToon PCSS > Missing Material AutoFixer` から、モデルインポート時などに発生するマテリアルの参照切れ（Missing）を自動で検出し、復旧させることができます。
- **GUIDベースの強力なマテリアルバックアップ**: アバターのアップロード時や手動操作時に、マテリアル設定を安全にバックアップ・復元します。ファイル名やパスを変更しても追跡可能なため、事故からの復旧が容易です。
- **各種自動修正機能**: VRChatビルド時の一般的なエラーや、lilToonのバージョンアップに伴う非推奨設定（メッシュ暗号化、ファーのSubdivisionモードなど）を自動で検出し、修正を促します。
- **パフォーマンス最適化**: Quest環境などを考慮し、アバターのパフォーマンスを最適化するためのツールを提供します。

---

## 🚀 導入方法 (VCC推奨)

VRChat Creator Companion (VCC) を使った導入が最も簡単で確実です。

1.  VCCを開き、**[Settings] > [Packages]** を選択します。
2.  `User Packages` のリストに、以下のリポジトリURLを追加して **[Add]** ボタンを押します。
    ```
    https://zapabob.github.io/liltoon-pcss-extension/index.json
    ```
3.  管理したいプロジェクトの **[Manage Project]** を開き、「lilToon PCSS Extension」が表示されていることを確認し、**[+]** ボタンでプロジェクトに追加します。
4.  Unityが起動し、`Window`メニューに`lilToon PCSS Extension`が表示されていれば導入は完了です。

---

## 📖 基本的な使い方

### 1. ワンクリック・セットアップ（PC推奨）
最も簡単な方法は、PC向けの統合ハブを使用することです。

1.  Unityの上部メニューから **[Tools] > [lilToon-PCSS-Extension] > [PCSS Hub (PC)]** を選択します。
2.  アバターを選択し、**ライトプリセット（アニメ / リアル影 / 映画）**を選択します。
3.  **[PC向けセットアップ実行]** をクリックします。
4.  これだけで、アバターにPCSS対応シェーダーが適用され、VRC Light Volumesが有効化され、Modular Avatarのライトトグルが自動作成されます。

旧メニューは **[Tools] > [lilToon-PCSS-Extension] > [Legacy]** に移動しています。

### 2. シェーダーの手動設定
マテリアルごとに手動で設定することも可能です。

1.  アバターのマテリアルを選択し、Shaderのドロップダウンメニューから **[lilToon] > [PCSS Extension]** を選択します。
2.  マテリアルのインスペクターにPCSS関連の設定項目（影の柔らかさ、範囲など）が表示されるので、好みに合わせて調整します。

---

## 🛠️ 各種ツールの使い方

本パッケージには、制作を補助する様々なツールが含まれています。

-   **Missingマテリアル修復**:
    -   メニュー: `Tools > lilToon PCSS > Missing Material AutoFixer`
    -   機能: シーン内の`Missing`状態のマテリアルをスキャンし、プロジェクト内から同名のマテリアルを検索して自動的に割り当て直します。

-   **マテリアルバックアップ**:
    -   手動バックアップ: `Tools > lilToon PCSS > Material Backup`
    -   自動バックアップ: VRChatアバターのアップロード時に自動で実行されます。
    -   復元: 上記メニューから復元したいバックアップデータを選択して実行します。

-   **lilToonバージョン移行ツール**:
    -   メニュー: `Tools > lilToon PCSS > Migration Tools`
    -   機能: 古いlilToonの機能（ファーのShrinkモードなど）を使用しているマテリアルを検出し、最新バージョン（Subdivisionモード）へ設定を変換します。

---

## ⚙️ システム要件

| 項目 | 推奨バージョン |
| :--- | :--- |
| **Unity** | 2022.3.22f1 |
| **VRChat Avatars SDK** | 3.10.3 以降 |
| **lilToon** | 2.3.2 以降 |
| **Modular Avatar** | 1.17.1 以降 |
| **VRC Light Volumes** | 2.1.3 以降（任意） |

---

## 📜 ライセンス

このプロジェクトは [MIT License](LICENSE) の下で公開されています。

---

## 💬 サポート・連絡先

-   **公式ドキュメント**: [https://zapabob.github.io/liltoon-pcss-extension/](https://zapabob.github.io/liltoon-pcss-extension/)
-   **GitHub (不具合報告)**: [https://github.com/zapabob/liltoon-pcss-extension/issues](https://github.com/zapabob/liltoon-pcss-extension/issues)
-   **Booth**: [https://zapabob.booth.pm/](https://zapabob.booth.pm/)
