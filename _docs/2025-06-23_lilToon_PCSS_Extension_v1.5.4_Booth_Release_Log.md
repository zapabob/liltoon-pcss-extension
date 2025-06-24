# lilToon PCSS Extension v1.5.4 Booth版 リリースノート・実装ログ

## リリース日
2025-06-23

## 概要
- 本バージョンはBooth配布専用の完全パッケージです。
- 1.5.3までの全機能・修正を統合し、Runtime配下の全スクリプト・asmdefを含む完全版です。
- Unity 2022.3 LTS、lilToon最新版、Poiyomi最新版、VRChat SDK3、ModularAvatarに完全対応。

## 主な機能
- 🎬 映画品質のPCSS（Percentage-Closer Soft Shadows）拡張
- 🎯 ワンクリック・アバターセットアップ（Avatar Selector Menu）
- 🎨 4種プリセット（リアリスティック／アニメ／シネマティック／カスタム）
- 🚀 VRChat/Quest自動パフォーマンス最適化
- 🛡️ ModularAvatar連携・エラー耐性強化
- ⚡ URP 14.0.10/Unity 2022.3 LTS完全対応
- 📝 サンプル・ドキュメント・日本語UI

## Booth版独自ポイント
- Runtime/配下の全スクリプト（PhysBoneLightController, VRChatPerformanceOptimizer, PCSSUtilities, PoiyomiPCSSIntegration）を完全同梱
- Editor/・Shaders/・Includes/も全て最新版
- package.json, asmdef, README, CHANGELOGもBooth配布向けに最適化
- zipパッケージ内容と元ディレクトリの完全一致をdiffで検証済み

## 実装・検証ログ
- 1.5.3→1.5.4へのバージョンアップ（package.json, CHANGELOG, README）
- Runtime/配下のスクリプトを1.5.3版から完全復元
- Editor/・Shaders/・Includes/の全ファイルを再確認
- Booth用zip化（liltoon-pcss-extension_v1.5.4_booth.zip）
- zip展開＆diffで内容完全一致を検証
- README.mdをBooth配布向けに調整

## インストール・利用方法
1. Boothでzipをダウンロードし、解凍して`com.liltoon.pcss-extension`フォルダごとUnityプロジェクトのPackagesディレクトリにコピー
2. Unity再起動後、`Window > lilToon PCSS Extension > Avatar Selector`が表示されていれば導入完了

---

本パッケージはMITライセンスで公開されています。
ご質問・不具合はGitHubまたはBoothメッセージまでご連絡ください。 