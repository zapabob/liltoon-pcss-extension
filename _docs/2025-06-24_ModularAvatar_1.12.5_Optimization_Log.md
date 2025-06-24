# 2025-06-24 ModularAvatar 1.12.5最適化 実装ログ

## 概要
ModularAvatar 1.12.5の新機能・APIに合わせて、lilToon PCSS Extensionのエディタ拡張（CompetitorSetupWizard, AvatarSelectorMenu等）を最適化。

---

## 主な実装内容

- RendererごとにMAMaterialSwap/MAPlatformFilterを自動追加し、プリセット適用・マテリアル切り替えをModularAvatar流に最適化
- Quest/PC分岐例もサポート（MAPlatformFilterで自動切り替え可能）
- 既存の手動マテリアル切り替えも残し、ModularAvatar未導入環境でも従来通り動作
- Prefab/アバターセットアップ時のAutoFIXもModularAvatar流で堅牢化

---

## 具体的な修正ファイル
- com.liltoon.pcss-extension/Editor/CompetitorSetupWizard.cs
- com.liltoon.pcss-extension/Editor/AvatarSelectorMenu.cs
- README.md, CHANGELOG.md

---

## 動作確認・リリース手順
- ModularAvatar 1.12.5環境下でプリセット適用・Quest/PC分岐・AutoFIXが正常動作することを確認
- VCC経由でのインストール・アップグレードも正常

---

## 備考
- ModularAvatar公式: https://modular-avatar.nadena.dev/
- Manual Bake/出力資産: https://modular-avatar.nadena.dev/docs/manual-processing 