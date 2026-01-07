# 2025-08-18 Modular Avatar Conditional Compilation Fix

## 概要
Modular Avatar未インストール環境でのCS0246エラー（nadena名前空間が見つからない）を条件付きコンパイルで解決。

## 修正内容
- `Editor/ModularAvatarLightController.cs` - 条件付きusingディレクティブ
- `Editor/AdvancedLightControlSystem.cs` - 早期リターンガード追加
- `Editor/LightPresetManager.cs` - 条件付きusingディレクティブ
- `Editor/LightPreviewSystem.cs` - 条件付きusingディレクティブ
- `Editor/LightAnimationSequencer.cs` - 条件付きusingディレクティブ
- `Editor/lilToon.PCSS.Editor.asmdef` - バージョン定義で`MODULAR_AVATAR_EXISTS`シンボル追加

## 修正原理
```json
"versionDefines": [
    {
        "name": "nadena.dev.modular-avatar",
        "expression": "1.0.0",
        "define": "MODULAR_AVATAR_EXISTS"
    }
]
```

Modular Avatar 1.0.0以上がインストールされている場合のみ、`MODULAR_AVATAR_EXISTS`シンボルが定義され、関連コードがコンパイルされる。

## 未インストール時の動作
- 警告ログ出力
- 該当機能の無効化
- 基本的なライト制御のみ利用可能

## インストール時の動作
- 全機能有効化
- Modular Avatar統合による高度な制御

## タイムスタンプ
- 2025-08-18
