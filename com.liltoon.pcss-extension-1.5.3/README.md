# lilToon PCSS Extension

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/Unity-2022.3%20LTS-blue.svg)](https://unity3d.com/get-unity/download)
[![lilToon Compatible](https://img.shields.io/badge/lilToon-Compatible-purple.svg)](https://github.com/lilxyzw/lilToon)
[![Poiyomi Compatible](https://img.shields.io/badge/Poiyomi-Compatible-pink.svg)](https://poiyomi.com/)
[![VRChat SDK3](https://img.shields.io/badge/VRChat%20SDK3-Ready-green.svg)](https://docs.vrchat.com/)
[![VPM-Compatible](https://img.shields.io/badge/VPM-Compatible-brightgreen.svg)](https://vcc.docs.vrchat.com/)

**lilToonおよびPoiyomiシェーダーのための、高品質なPCSS（Percentage-Closer Soft Shadows）拡張機能です。**

VRChatアバターに映画品質のリアルなソフトシャドウを簡単に追加できます。Unityのメニューからワンクリックでセットアップが完了する「アバターセレクターメニュー」を搭載しており、複雑な設定は一切不要です。

---

## ✨ 主な機能

- **🎬 映画品質のソフトシャドウ:** 光源からの距離に応じて自然にぼける、物理ベースのリアルな影を生成します。
- **🎯 ワンクリック・セットアップ:** Unity上部のメニューから `Window > lilToon PCSS Extension > Avatar Selector` を選択するだけで、シーン内のアバターに最適な設定を自動で適用します。
- **🎨 4つのプリセット:** 「リアリスティック」「アニメ調」「シネマティック」「パフォーマンス最適化」など、用途に応じたプリセットを標準搭載。
- **🚀 パフォーマンス最適化:** VRChatの品質設定やQuest環境に自動で連動し、常に最高のパフォーマンスを維持します。
- **✅ 最新環境への対応:** Unity 2022.3 LTS、lilToon最新版、VRChat SDKに完全対応しています。

---

## 🚀 インストール方法 (v1.5.1)

### VCC (VRChat Creator Companion) でのインストール (強く推奨)

VCCを使えば、数クリックで安全かつ簡単に導入できます。

1.  **VCCを開き、「Settings」>「Packages」画面へ移動します。**
2.  **「Community Repositories」のリストにある「Add Repository」ボタンを押します。**
3.  **以下のURLをコピー＆ペーストして追加します。**
    ```
    https://zapabob.github.io/liltoon-pcss-extension/index.json
    ```
4.  **プロジェクト管理画面に戻り、「lilToon PCSS Extension」を選択してインストールします。**

### Unity Package Manager (Git URL) でのインストール

1.  Unity上部のメニューから `Window > Package Manager` を開きます。
2.  左上の `+` ボタンを押し、「Add package from git URL...」を選択します。
3.  以下のURLを入力して「Add」を押します。
    ```
    https://github.com/zapabob/liltoon-pcss-extension.git?path=com.liltoon.pcss-extension
    ```
    *注意: この方法はパッケージがAssetsフォルダに直接展開されるため、VCCでの管理を推奨します。*

---

## 📖 使い方

1.  **Unity上部のメニューから `Window > lilToon PCSS Extension > Avatar Selector` を選択します。**
2.  ウィンドウが開き、シーンに存在するあなたのアバターが自動的にリストアップされます。
3.  ドロップダウンメニューから、適用したい影のプリセット（例: Realistic Shadows）を選択します。
4.  **「適用 (Apply)」ボタンをクリックします。**

以上で完了です！　アバターに最適なPCSS設定が自動的に適用されます。

---

## 🔧 システム要件

| 項目 | バージョン |
| --- | --- |
| **Unity** | 2022.3.6f1 以降 |
| **lilToon** | 1.7.0 以降 |
| **VRChat Avatars SDK** | 3.5.0 以降 |
| **Modular Avatar** | 1.10.0 以降 |

---

## ライセンス

このプロジェクトは [MIT License](LICENSE) のもとで公開されています。