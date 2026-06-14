# 導入手順

## 1. 事前準備

VCCで対象プロジェクトを開き、次のパッケージを先に導入してください。

- VRChat Avatars SDK
- lilToon
- Modular Avatar
- Avatar Optimizer
- VRC Light Volumes は必要な場合のみ

## 2. パッケージを入れる

1. ZIPを解凍します。
2. `Package/lilToon PCSS Studio for VRChat` フォルダーをUnityプロジェクトの `Assets` 配下へ入れます。
3. Unityへ戻り、スクリプトのコンパイルが終わるまで待ちます。
4. Consoleに赤いC#エラーがないことを確認します。

## 3. PCSS Hubでセットアップする

1. `Tools > lilToon-PCSS-Extension > PCSS Hub (PC)` を開きます。
2. `Target Avatar` にアバターのルートを指定します。
3. 初回は `AAO Performance Safe (0 Lights)` を有効にします。
4. PCSS / 艶プリセットを選びます。
5. `PC向けセットアップ実行` を押します。
6. NDMF ConsoleとVRChat SDK Builderの警告を確認します。

## 4. プリセットの選び方

- `VRChat Safe`: 最初の確認用。軽めで扱いやすい設定です。
- `Dewy Skin Gloss`: 控えめなうるみ艶を足します。
- `Soft Flush Skin`: 顔まわりに温かい血色感を足します。
- `Excited Tone / 高揚発色`: 運動後やライブ照明のような温かい色調を足します。健全な撮影向け表現です。
- `Studio Boost`: PC撮影・確認向けの強め設定です。販売標準では必要なときだけ使ってください。

## 5. カメラ深度プレビュー

SceneViewやシーン内Cameraで深度テクスチャを確認したい場合は、次を実行します。

```text
Tools > lilToon-PCSS-Extension > Preview > Enable Camera Depth Texture
```

戻す場合は次を実行します。

```text
Tools > lilToon-PCSS-Extension > Preview > Disable Camera Depth Texture
```

この機能はEditor用の確認補助です。アバター本体へRuntimeコンポーネントを残しません。

## 6. 旧PCSS v2.2.0導入済みアバター

旧PCSS v2.2.0系の設定が残っている場合は、アバターのルートを選択して次を実行します。

```text
Tools > lilToon-PCSS-Extension > Repair > Legacy PCSS v2.2.0 Intake
```

旧プロパティを現在のAAO併用向け設定へ移し、不要な旧Runtimeコンポーネントを整理します。

