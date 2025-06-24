# lilToon PCSS Extension v1.5.5

## 概要

**lilToon PCSS Extension** は、lilToonおよびPoiyomiシェーダー向けにプロフェッショナルなPCSS（Percentage-Closer Soft Shadows）機能を追加するUnity拡張パッケージです。VRChatアバターやワールドで美しいソフトシャドウを実現し、VCC（VRChat Creator Companion）からワンクリックで導入できます。

---

## 主な機能
- 高品質なPCSS（ソフトシャドウ）をlilToon/Poiyomiに追加
- VRChat/ModularAvatar連携・PhysBone制御
- アバターセレクターメニュー・プリセット切替
- Quest/URP対応・パフォーマンス最適化
- シーン/プロジェクト全体のマテリアル自動修復（AutoFIX）
- Prefabインスタンス修正時のスクリプト無効化防止（v1.5.5）

---

## VCC（VRChat Creator Companion）でのインストール手順

1. VCCを起動し「パッケージリポジトリを追加」から下記URLを入力：

   ```
   https://zapabob.github.io/liltoon-pcss-extension/index.json
   ```

2. 「lilToon PCSS Extension」を検索し、インストール
3. Unityプロジェクトに自動で導入されます

> ⚠️ **jsonファイルを直接開くと生データが表示されます。**
> 見やすいHTML形式で内容を確認したい場合は [index.html](https://zapabob.github.io/liltoon-pcss-extension/index.html) または [json-viewer.html](https://zapabob.github.io/liltoon-pcss-extension/json-viewer.html) をご利用ください。

---

## ドキュメント・サポート
- [公式ドキュメント（GitHub Pages）](https://zapabob.github.io/liltoon-pcss-extension/index.html)
- [GitHubリポジトリ](https://github.com/zapabob/liltoon-pcss-extension)
- [CHANGELOG](https://github.com/zapabob/liltoon-pcss-extension/blob/main/CHANGELOG.md)

ご質問・不具合報告はGitHub Issuesまたはメール（r.minegishi1987@gmail.com）までご連絡ください。

---

## ライセンス

本パッケージはMITライセンスで提供されます。

---

## 更新履歴

- **v1.5.5**: AutoFIX Prefab対応・マテリアル自動修復強化（Prefabインスタンス修正時のスクリプト無効化防止）
- **v1.5.4**: VRChat/Booth対応・シェーダーエラー修正・表情ライト強化
- **v1.5.3**: パフォーマンス最適化・競合互換ウィザード追加
- **v1.5.0**: VRChat/ModularAvatar連携・PhysBone制御・PCSSプリセット拡張
- **v1.4.x**: URP対応・VRC Light Volumes連携・競合シェーダー互換
- **v1.2.x**: 初期リリース

## v1.5.5の新機能・最適化

- ModularAvatar 1.12.5完全対応
- MAMaterialSwap/MAPlatformFilter自動追加によるプリセット適用のModularAvatar流最適化
- Quest/PC分岐例もサポート（MAPlatformFilterで自動切り替え可能）
- 既存の手動マテリアル切り替えも残し、ModularAvatar未導入環境でも従来通り動作 
