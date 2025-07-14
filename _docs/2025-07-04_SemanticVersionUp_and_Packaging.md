# 2025-07-04 Semantic Version Up & Packaging Implementation Log

## 概要
- MenuItemパス統一・保守性向上を反映し、セマンティックバージョンを1.8.1にアップデート
- パッケージを `com.liltoon.pcss-extension-1.8.1.zip` として出力

## 実施内容
1. `package.json` の version を 1.8.1 に更新
2. url を新パッケージ名に修正
3. `CHANGELOG.md` に [1.8.1] エントリを追加
4. powershell Compress-Archive でパッケージ化
5. GitHubリリース用URLも更新

## 成果物
- com.liltoon.pcss-extension-1.8.1.zip
- package.json, CHANGELOG.md, 実装ログ

## 今後の運用指針
- 機能追加・修正時は必ずセマンティックバージョンを更新し、CHANGELOG/実装ログを残す
- パッケージURLも都度更新し、VCC/UPM/Booth配布に活用

---

本バージョンより、全Editor拡張のメニュー構造・保守性が大幅に向上し、国際展開・チーム開発にも最適化された。 