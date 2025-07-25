# Changelog

## [1.5.10] - 2025-07-04
### Changed
- PhysBoneEmissiveController設計思想・AutoFIX完全対応・命名規則徹底・MaterialPropertyBlock活用・ModularAvatar/PhysBone連携リファクタリング
- 実装ログに基づく厳密なリファクタリング

## [1.5.9] - 2025-07-03
### Changed
- PhysBoneLightControllerをPhysBoneEmissiveControllerへリファクタリング
- クラス名・ファイル名・AddComponentMenu・コメントを一括修正
- 命名規約・Unityバリデーション・VRChat AutoFIX完全対応
- エミッシブ制御用途に特化した設計へ
- マテリアルのバックアップ・リストアがGUIDベースになり、パス変更やリネーム後も正確に復元可能。
- バックアップはJSON形式で保存され、復元時はGUID一致でマテリアルを特定。
- テクスチャもGUIDで管理し、参照切れを大幅に防止。

## [1.5.7] - 2025-06-24

### Added / Changed
- MissingMaterialAutoFixer（Editor拡張）を追加: シーン内のMissingマテリアル検出＆自動修復ウィンドウ
- FBX/Prefabインポート時の自動リマップ機能を追加（AssetPostprocessorによるMissingスロット自動補完）
- package.jsonのバージョンを1.5.7に更新

## [1.5.5] - 2025-06-24

### Added / Changed
- AutoFIX Prefab対応: Prefabインスタンスのマテリアル自動修復時、スクリプト（MonoBehaviour等）が無効化される現象を防止
- マテリアル自動修復強化: missingやlilToon系シェーダーも自動で復旧・補完
- シーン/プロジェクト全体のマテリアルを一括修復可能
- ModularAvatar 1.12.5最適化: MAMaterialSwap/MAPlatformFilter自動追加、プリセット適用のModularAvatar流対応、Quest/PC分岐例もサポート

## [1.5.4] - 2025-06-23

### Booth版リリース
- Booth向けパッケージとしてv1.5.4をリリース。
- 1.5.3の全機能・修正を含む。
- Runtime/配下のスクリプト・asmdefを含めた完全版。

## [1.5.3] - 2025-06-23

### Booth版リリース
- Booth向けパッケージとしてv1.5.3をリリース。
- 1.5.3の全機能・修正を含む。
- Runtime/配下のスクリプト・asmdefを含めた完全版。

## [1.5.8] - 2025-07-03
### Fixed
- PhysBoneLightController: 先行製品互換性・安定性向上、外部ライト指定・自動検出の堅牢化、距離減衰・スムージング制御の最適化、エラー時の自動無効化・デバッグ強化
- Unity 2022.3 LTS/VRChat SDK 最新版での動作検証・最適化 