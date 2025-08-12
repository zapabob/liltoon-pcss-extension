# lilToon PCSS Extension — Advanced Realistic Shadow System

[![Version](https://img.shields.io/badge/version-2.6.0-blue.svg)](https://semver.org/)
[![Unity](https://img.shields.io/badge/Unity-2022.3%2B-green.svg)](https://unity.com/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![lilToon](https://img.shields.io/badge/lilToon-2.1.4%2B-brightgreen.svg)](https://github.com/lilxyzw/lilToon)
[![Poiyomi](https://img.shields.io/badge/Poiyomi-Compatible-pink.svg)](https://poiyomi.com/)
[![ModularAvatar](https://img.shields.io/badge/ModularAvatar-AAO%20Compatible-orange.svg)](https://github.com/bdunderscore/modular-avatar)
[![VRChat](https://img.shields.io/badge/VRChat-SDK%20Compatible-purple.svg)](https://vrchat.com/)
[![VCC](https://img.shields.io/badge/VCC-Compatible-cyan.svg)](https://vcc.docs.vrchat.com/)
[![Semantic Versioning](https://img.shields.io/badge/SemVer-2.0.0-lightgrey.svg)](https://semver.org/)

## 概要 / Overview

- 日本語: lilToon/Poiyomi 向けのプロフェッショナルな PCSS（Percentage‑Closer Soft Shadows）拡張。Modular Avatar 連携のワンクリック導入、Quick Add プリセット（Realistic/Anime/Cinematic）、Missing Material AutoFixer、FBX/Prefab 自動リマップ、VRC Light Volumes 2.0.0 連携、電源断保護（Power Protection）などを搭載。Unity 2022.3 LTS・URP 12.1.12 対応。
- English: A professional PCSS extension for lilToon/Poiyomi with one‑click Modular Avatar integration, Quick Add presets (Realistic/Anime/Cinematic), Missing Material AutoFixer, FBX/Prefab auto‑remap, VRC Light Volumes 2.0.0 integration, and a Power Protection system. Supports Unity 2022.3 LTS and URP 12.1.12.

## 主な機能 / Key Features

- PCSS ソフトシャドウ / PCSS soft shadows (dynamic quality, penumbra control)
- Quick Add メニューとプリセット（Realistic/Anime/Cinematic） / Quick Add menu and built‑in presets
- Modular Avatar 連携（AAO・反射ベース・安全な依存管理） / Modular Avatar AAO integration (reflection‑based)
- Missing Material AutoFixer と GUID ベースのバックアップ / Missing material auto‑fix with GUID‑based backups
- FBX/Prefab インポート時の自動マテリアルリマップ / Auto material remap on FBX/Prefab import
- VRC Light Volumes 2.0.0 連携ユーティリティ / VRC Light Volumes 2.0.0 utilities
- Power Protection（自動チェックポイント5分/緊急保存/バックアップ10個/復旧） / Power Protection (auto checkpoint every 5 min, emergency save, rotation backups, recovery)
- Editor Coroutines 正式対応（`com.unity.editor-coroutines`） / Official Editor Coroutines support

## 動作環境 / Requirements

- Unity 2022.3 LTS 以上 / Unity 2022.3 LTS or later
- URP 12.1.12 以上 / URP 12.1.12 or later
- lilToon 2.1.4 以上（推奨）/ lilToon 2.1.4+ (recommended)
- VRChat SDK 3.5.0 以上 / VRChat SDK 3.5.0+
- Modular Avatar 1.12.5 以上 / Modular Avatar 1.12.5+
- `com.unity.editor-coroutines` 1.0.0

## インストール / Installation

### VCC（推奨）/ VCC (Recommended)

- Repository URL: `https://zapabob.github.io/liltoon-pcss-extension/index.json`
- One‑click: `vcc://vpm/addRepo?url=https%3A%2F%2Fzapabob.github.io%2Fliltoon-pcss-extension%2Findex.json`

1) VCC を開く → 2) Settings > Packages に上記 URL を追加 → 3) プロジェクトで「lilToon PCSS Extension」を追加

### Unity Package Manager（Git URL）

Add package from git URL:

```text
https://github.com/zapabob/liltoon-pcss-extension.git
```

### 直接ダウンロード / Direct download

- Releases: `https://github.com/zapabob/liltoon-pcss-extension/releases`
- Latest ZIP (2.6.0): `https://github.com/zapabob/liltoon-pcss-extension/releases/download/v2.6.0/com.liltoon.pcss-extension-2.6.0.zip`

## 使い方 / Usage

- 日本語: Unity メニューの `Window/` または `Tools/` 配下にある `lilToon PCSS Extension` から、Quick Add プリセット適用、Modular Avatar 連携セットアップ、AutoFixer などを実行できます。
- English: From the `lilToon PCSS Extension` menu under `Window/` or `Tools/`, run Quick Add presets, set up Modular Avatar integration, and use the Missing Material AutoFixer.

典型的なフロー / Typical flow:

1. アバターを選択 → `lilToon PCSS Extension` メニューを開く / Select avatar → open the extension menu
2. Quick Add からプリセットを適用 / Apply a preset from Quick Add
3. 必要に応じて Modular Avatar 連携や Light Volumes ユーティリティを実行 / Optionally run MA integration or Light Volumes utilities

## エディタメニュー / Editor Menus

- Tools/lilToon-PCSS-Extension/Quick Add
  - PCSS Light: PCSSライトを即時追加 / Quickly adds a PCSS light
  - VRC Light Volumes Integration: 選択中の対象に`VRCLightVolumesIntegration`を付与 / Adds `VRCLightVolumesIntegration` to selection
  - All-in-One: 上記の一括適用 / Applies the above in one go
  - Presets/
    - Realistic
    - Anime
    - Cinematic
- Tools/lilToon-PCSS-Extension/Open Setup Wizard: セットアップウィザードを表示 / Opens setup wizard
- Tools/lilToon-PCSS-Extension/統合メニューシステム: ダッシュボードを開く / Opens the unified dashboard
- Tools/lilToon-PCSS-Extension/Missing Material AutoFixer: マテリアル修復ツール / Material repair tool

キーボードショートカット / Keyboard shortcut:

- 統合メニューシステム (Shortcut): Ctrl+Shift+M (`%#m`)

## ドキュメント / Documentation

- Site: `https://zapabob.github.io/liltoon-pcss-extension/`
- Getting Started: `docs/tutorials/getting-started.md`
- PCSS Settings Reference: `docs/reference/pcss-settings.md`

## トラブルシューティング / Troubleshooting

- lilToon の導入と URP 設定を確認 / Ensure lilToon is installed and URP is configured
- Modular Avatar は任意（反射ベースで安全）/ MA is optional (reflection‑based integration)
- パフォーマンス低下時はプリセット/サンプル数/距離を調整 / Tune preset, samples, and distances for performance

Issues: `https://github.com/zapabob/liltoon-pcss-extension/issues`

## 変更履歴 / Changelog

- `CHANGELOG.md` を参照（最新: 2.6.0 に合わせた機能と互換性の更新）/ See `CHANGELOG.md` (aligned with 2.6.0 features and compatibility)

## ライセンス / License

MIT License — see `LICENSE`.

## クレジット / Credits

- lilToon Team, Modular Avatar Team, Unity Technologies, VRChat Community

---

メタ情報 / Meta

- Version: 2.6.0
- Last Updated: 2025-08-13
- Unity: 2022.3+
- Author: lilToon PCSS Extension Team
