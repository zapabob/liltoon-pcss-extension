# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.2.0] - 2025-01-24

### Added
- **VRChat SDK3完全対応**: 最新VRChatシステムとの完全互換性
- **ModularAvatar統合**: ワンクリックでエクスプレッション自動セットアップ
- **VRC Light Volumes統合**: 次世代ボクセルライティングシステム対応
- **インテリジェント最適化**: VRChat品質設定に自動連動
- **VCC（VRChat Creator Companion）対応**: ワンクリックインストール
- **パフォーマンス最適化システム**: フレームレート監視・自動調整
- **Quest自動最適化**: Quest使用時の自動品質調整
- **アバター密度対応**: 周囲のアバター数に応じた動的調整
- **エクスプレッションメニュー自動生成**: ModularAvatarによる自動セットアップ
- **品質プリセットシステム**: Low/Medium/High/Ultra 4段階品質設定
- **包括的ドキュメント**: 詳細なセットアップガイドとAPI リファレンス

### Changed
- **アセンブリ定義ファイル更新**: VRChat SDK3とModularAvatarの依存関係追加
- **シェーダー最適化**: パフォーマンス向上とQuest対応強化
- **コード構造改善**: 条件コンパイルディレクティブによる互換性向上
- **パッケージ構造整理**: 重複ファイルの削除と適切なフォルダ構成

### Fixed
- **VRChat SDK3互換性問題**: 最新SDKとの互換性問題を解決
- **Quest対応問題**: Quest環境での動作不良を修正
- **パフォーマンス問題**: 高負荷時のフレームレート低下を改善
- **エクスプレッション問題**: メニュー表示とパラメータ連動の不具合修正

### Removed
- **重複ファイル削除**: 
  - `liltoon_pcss_extension.shader`
  - `liltoon_pcss_urp.shader`
  - `LilToonPCSSMenuIntegration.cs`
  - `ModularAvatarPCSSIntegration.cs`

## [1.1.0] - 2024-12-15

### Added
- **基本PCSS機能**: Percentage-Closer Soft Shadows実装
- **lilToon統合**: lilToonシェーダーとの基本統合
- **Poiyomi統合**: Poiyomiシェーダーとの基本統合
- **VRChatエクスプレッション**: 基本的なリアルタイム制御機能

### Changed
- **初期実装**: 基本的なPCSS機能の実装

## [1.0.0] - 2024-11-01

### Added
- **プロジェクト開始**: lilToon PCSS Extension開発開始
- **基本構造**: 基本的なパッケージ構造とシェーダー実装
- **ドキュメント**: 基本的なREADMEとセットアップガイド

---

## リリースノート

### v1.2.0 - 次世代統合アップデート 🚀

この大型アップデートにより、lilToon PCSS Extensionは単なるシェーダー拡張から、VRChatアバター制作のための包括的なソリューションへと進化しました。

**主な改善点:**
- VCC対応によるワンクリックインストール
- ModularAvatarによる自動セットアップ
- インテリジェント最適化による自動パフォーマンス調整
- VRC Light Volumesとの完全統合

**互換性:**
- Unity 2019.4 LTS以降（2022.3 LTS推奨）
- VRChat SDK3 Avatars 3.4.0以降
- lilToon v1.3.0以降
- ModularAvatar v1.9.0以降

**アップグレード方法:**
1. VCCから最新版をインストール
2. 既存のマテリアルは自動的に新しいシェーダーに移行
3. ModularAvatarコンポーネントで自動セットアップ完了

---

**サポート:** [GitHub Issues](https://github.com/zapabob/liltoon-pcss-extension/issues)  
**ドキュメント:** [README.md](https://github.com/zapabob/liltoon-pcss-extension/blob/main/README.md)  
**ライセンス:** [MIT License](https://github.com/zapabob/liltoon-pcss-extension/blob/main/LICENSE) 