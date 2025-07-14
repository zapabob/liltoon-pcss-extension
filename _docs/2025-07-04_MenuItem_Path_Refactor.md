# 2025-07-04 MenuItem Path Refactor Implementation Log

## 概要
Unity Editor拡張のMenuItemパスを全て `Tools/lilToon PCSS Extension/` 配下に英語で統一し、サブグループごとに整理した。

## 背景・目的
- 日本語・英語混在や旧パス（Tools/lilToon PCSS/ など）が存在し、ユーザー体験・保守性が低下していた
- ブランド名「lilToon PCSS Extension」に統一し、全てのメニューを一元管理することで、
  - ユーザーが迷わず機能にアクセスできる
  - 将来の機能追加・整理が容易になる

## 実装方針
- ルートは `Tools/lilToon PCSS Extension/`
- サブグループ例: Utilities, VRChat, Compatibility, About, Documentation など
- MenuItemパスは全て定数化し、重複や表記揺れを排除
- 旧パス・日本語混在のMenuItemは全て新パスに統一

## 主な修正内容
- 各EditorスクリプトのMenuItem属性を新パスに修正
- パスの定数化（MenuRoot, UtilitiesMenu, PCSSMenu, ...）
- 旧パスや日本語混在のMenuItemを全て削除・置換
- 今後の拡張に備え、MenuItemを持たないスクリプトにもMenuPath定数を導入

## 対象ファイル
- com.liltoon.pcss-extension/Editor/AvatarSelectorMenu.cs
- com.liltoon.pcss-extension/Editor/CompetitorSetupWizard.cs
- com.liltoon.pcss-extension/Editor/MaterialBackup.cs
- com.liltoon.pcss-extension/Editor/MaterialUpgrader.cs
- com.liltoon.pcss-extension/Editor/LilToonPCSSShaderGUI.cs
- com.liltoon.pcss-extension/Editor/LilToonPCSSExtensionInitializer.cs
- com.liltoon.pcss-extension/Editor/MissingMaterialAutoFixer.cs
- com.liltoon.pcss-extension/Editor/ModularAvatarLightToggleMenu.cs
- com.liltoon.pcss-extension/Editor/PerformanceOptimizerMenu.cs
- com.liltoon.pcss-extension/Editor/PoiyomiPCSSShaderGUI.cs
- com.liltoon.pcss-extension/Editor/VRChatExpressionMenuCreator.cs
- com.liltoon.pcss-extension/Editor/VRChatOptimizationSettings.cs
- com.liltoon.pcss-extension/Editor/VRCLightVolumesEditor.cs
- com.liltoon.pcss-extension/Editor/AutomatedMaterialBackup.cs

## 今後の保守性
- MenuItemパスの定数化により、ブランド名や構造変更も一括で対応可能
- 英語表記統一で国際展開やチーム開発にも対応しやすくなった

---

本実装により、全てのEditorメニューが一貫したブランド・構造で提供され、ユーザー体験と保守性が大幅に向上した。 