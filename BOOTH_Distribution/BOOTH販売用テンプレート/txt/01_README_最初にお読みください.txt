# lilToon PCSS Studio for VRChat

このたびは `lilToon PCSS Studio for VRChat` をお迎えくださり、ありがとうございます。

本製品は、PC向けVRChatアバターのlilToonマテリアルに、柔らかいPCSS風の影、艶のまとまり、肌向けの自然な血色感を追加するUnity Editor拡張です。標準ワークフローではAAOとの併用を考え、アバター内に不要なUnity Lightを残しにくい構成にしています。

## 最初に読む順番

1. `01_INSTALL_導入手順`
2. `03_TERMS_利用規約`
3. `05_FAQ_トラブルシュート`

## 対応環境

- Unity 2022.3.22f1
- VRChat Avatars SDK 3.10.3以降
- lilToon 2.3.2以降
- Modular Avatar 1.17.1以降
- VRC Light Volumes 2.1.3以降は任意

## 注意

- 本製品のPCSS表現はPC向けです。Quest/Android向けアバターにはPCSS表現を含めないでください。
- VRChat公式機能、lilToon公式機能、Modular Avatar公式機能ではありません。
- 外部依存パッケージは同梱していません。VCCで先に導入してください。
- 高揚発色 / Excited Tone は健全な撮影向け色調プリセットです。成人向け表現や自動的な性的表現を目的にした機能ではありません。

## 同梱内容

- `Package/lilToon PCSS Studio for VRChat`: Unityへ入れる本体
- `Docs`: 技術メモ、検証メモ、更新記録
- `00_README`、`01_INSTALL`、`03_TERMS`、`05_FAQ`: 購入者向け文書

## 重要なメニュー

```text
Tools > lilToon-PCSS-Extension > PCSS Hub (PC)
Tools > lilToon-PCSS-Extension > Preview > Enable Camera Depth Texture
Tools > lilToon-PCSS-Extension > Repair > Legacy PCSS v2.2.0 Intake
```

標準では `AAO Performance Safe (0 Lights)` を有効にした導入を推奨します。VeryPoorが残る場合は、PCSSではなく三角形数、Skinned Mesh Renderer数、PhysBone、Contact、Material Slotなどアバター本体の要因も確認してください。

