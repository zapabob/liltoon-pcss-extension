# 2025-07-04 Runtime/Editor一括リファクタ実装ログ

## 概要
- Runtime/Editorディレクトリ全体のコードを、Unity/VRChat/ModularAvatarのベストプラクティスと過去の実装ログに基づき一括リファクタリング。

## 主な修正方針
- Editor/Runtimeの厳密な責務分離
- publicフィールドの[SerializeField] private化（AutoFIX/SDK警告防止）
- Editor依存の排除（Runtimeからusing UnityEditor等を除去）
- Undo/SetDirtyの徹底（Editor拡張での編集時）
- ModularAvatar/VRChat/LightVolumes連携の堅牢化
- マテリアル修復・アップグレード・バックアップの堅牢化
- 実装ログの内容を全て反映

## 具体的な修正内容
- Runtime配下の全publicフィールドを[SerializeField] privateに統一
- 必要に応じてEditor拡張からアクセス可能なプロパティを追加
- 名前空間をEditor: lilToon.PCSS.Editor / Runtime: lilToon.PCSS.Runtimeで統一
- Editor拡張でUndo.RecordObject/EditorUtility.SetDirtyを徹底
- ModularAvatarトグル/メニュー自動生成の堅牢化
- VRChat/LightVolumes最適化機能の責務分離・堅牢化
- マテリアル修復・バックアップ・リストアの堅牢化

## 今後の運用ポイント
- Editor/Runtimeの責務分離を厳守
- publicフィールドは原則[SerializeField] privateで
- Editor拡張での編集はUndo/SetDirtyを必ず呼ぶ
- ModularAvatar/VRChat/LightVolumes連携は公式API/推奨パターンに従う
- 実装ログは必ず_docs/に残す

---
本リファクタにより、AutoFIX/SDK警告の根絶・運用保守性の大幅向上・将来の機能追加/拡張の容易化を実現。 