## 実装ログ: VRCPhysBone統一・AutoFix対策

- 日時: 2025-08-13
- 目的: アバターに含まれる独自ランタイム（PhysBoneLightController, VRCLightVolumesIntegration）をビルドから排除し、VRCPhysBone準拠に統一

### 変更
- `Runtime/PhysBoneLightController.cs` を `#if UNITY_EDITOR` でガード（エディタ専用化）
- `Runtime/VRCLightVolumesIntegration.cs` を `#if UNITY_EDITOR` でガード（エディタ専用化）
- `Editor/VRChatCleanupMenu.cs` の削除対象に `lilToon.PCSS.PhysBoneLightController` を追加

### 使い方
- アバターアップロード前に `Tools/lilToon-PCSS-Extension/VRChat向けクリーンアップ/不許可コンポーネント削除` を実行
- 物理挙動は `VRCPhysBone` と `VRCPhysBoneCollider` のみ使用

### なんJ所感
独自ランタイムはビルドから外してVRCPhysBone一本化。AutoFixのお叱り回避で通りやすくなるで。


