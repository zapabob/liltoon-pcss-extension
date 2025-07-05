# 2025-07-04 ExpressionMenu/Parameters/Animator自動生成 実装ログ

## 概要
- プリセットライト（美肌・シルエット・雰囲気等）追加時に、
  - Expression Parameters（bool型トグル）
  - Expression Menu（トグルコントロール）
  - Animator Controller（ON/OFFアニメ・レイヤー・遷移）
  を自動生成・自動追記する堅牢なEditor拡張を実装。
- Undo/Prefab/SetDirty/AutoFIX/拡張性も徹底対応。

## 要件・設計方針
- VRChat公式仕様・AutoFIX・VRCFury制約を厳守
  - Parameter名重複回避・256個制限
  - WD（Write Defaults）統一
  - 既存資産への追記・重複回避
- ModularAvatar推奨のObjectToggle方式でライトON/OFF
- Undo/Prefab/SetDirty対応
- 拡張性・保守性を考慮した構造体設計

## 実装内容
- プリセットライト生成時に、
  - GameObject＋Light＋ModularAvatarObjectToggle（MODULAR_AVATAR時）を自動生成
  - VRCExpressionsParameters.assetへbool型パラメータを重複回避で自動追加
  - VRCExpressionsMenu.assetへトグルコントロールを重複回避で自動追加
  - FXレイヤー想定でAnimator Controllerを自動生成し、ON/OFFアニメ・レイヤー・遷移を自動生成
- すべてUndo/SetDirty/AssetDatabase.SaveAssets対応

## 運用ポイント
- アセットパス（ExpressionParameters.asset/ExpressionsMenu.asset/FX.controller）は運用に合わせて調整可能
- 既存資産への追記・重複回避ロジックを徹底
- VRCSDK未導入環境では自動生成処理はスキップ

## 今後の展望
- Comfort系ライト・ワールド照明連携・ドキュメント自動生成など、次フェーズへ
- README/CHANGELOG/実装ログ自動生成の自動化

---

本実装により、VRChatアバター向けライト機能の自動化・運用・拡張性が大幅に向上。今後もAutoFIX/SDK/VRCFuryに弾かれない最高品質の自動生成・運用・拡張性を追求する。 