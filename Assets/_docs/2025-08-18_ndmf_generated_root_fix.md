# 2025-08-18 NDMF Generated Assets Root Fix

## 概要
VRChatビルド中に `nadena.dev.ndmf.AssetSaver` が `Can't create asset because path is empty` を吐いて失敗する件の恒久対策を実装。

## 変更点
- `Editor/NDMFGeneratedAssetsRootFix.cs` 追加
  - 既定の生成先 `Assets/NDMFGenerated` を強制作成
  - NDMFのGeneratedAssetsRootへリフレクションで反映（失敗時は`EditorPrefs`フォールバック）
  - アバター名の無効文字を除去するサニタイズメニューを追加

## 使い方
- Unityメニュー: `Tools/NDMF/Set Generated Assets Root (Fix)` 実行
- 必要なら `Tools/NDMF/Sanitize Avatar Names (Remove invalid chars)` で選択中のアバター名を修正
- その後ビルドを再試行

## 備考
- `VRCAvatarDescriptor` 参照は `#if VRC_SDK_VRCSDK3` ガードで保護し、SDK未導入環境でのCS0246を回避
- `LilToonPCSSShaderGUI.cs` はShaderGUIのみで、NDMFの空パス問題の直接原因ではない

## タイムスタンプ
- 2025-08-18


