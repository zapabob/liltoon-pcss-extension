# 2025-07-04 PCSS Extension セットアップセンター統合実装ログ

## 概要

lilToon PCSS Extensionの全セットアップ・最適化・プリセット・ウィザード等を一元管理する「セットアップセンター」（PCSSSetupCenterWindow）を新規実装。

---

## 目的
- 機能ごとに分散していたEditorメニュー・ウィンドウを統合し、迷子防止・UX向上
- すべてのセットアップ・最適化・修復・ドキュメントを一画面で直感的に操作可能に
- 今後の機能追加・拡張もこのウィンドウに集約

---

## 主な実装内容
- `com.liltoon.pcss-extension/Editor/PCSSSetupCenterWindow.cs` を新規作成
    - タブUIで「一括セットアップ」「プリセット適用」「競合ウィザード」「最適化」「マテリアル修復」「LightVolumes」「ドキュメント」各機能にアクセス
    - 各タブから既存の個別ウィンドウ/機能を呼び出し
- 既存の個別MenuItem（AvatarSelectorMenu, CompetitorSetupWizard, PerformanceOptimizerMenu, MaterialBackup, MissingMaterialAutoFixer等）をすべてこのウィンドウ呼び出しにリダイレクト
- README.mdに「セットアップセンター」導入を追記

---

## 今後の運用方針
- すべてのセットアップ・管理は「lilToon/PCSS Extension/セットアップセンター」から行う
- 旧来の個別メニューは廃止・リダイレクト
- 新機能追加時もこのウィンドウにタブ追加するだけでOK

---

## 備考
- Unity標準のEditorWindow/タブUI/ドッキング/レイアウト保存に完全対応
- 迷ったら「セットアップセンター」から全ての機能にアクセスすることを推奨 