# 2025-06-24 MissingMaterialAutoFixer 実装ログ

## 概要
- Unity Editor拡張「MissingMaterialAutoFixer」を新規実装
- シーン内のMissingマテリアル検出＆自動修復ウィンドウを追加
- FBX/Prefabインポート時の自動リマップ機能（AssetPostprocessor）を追加
- lilToonシェーダーが見つからない場合は警告を表示
- Undo対応・ログ出力・バッチ処理対応

## 要件
- VRChatアバターアップロード時のlilToonマテリアルMissing問題を自動検出・修復
- FBX/Prefabインポート時にMissingスロットへlilToonマテリアルを自動割り当て
- Editor拡張ウィンドウから手動スキャン・自動修復も可能

## 実装内容
- com.liltoon.pcss-extension/Editor/MissingMaterialAutoFixer.cs を新規作成
    - EditorWindow: シーン内RendererのMissingマテリアル検出・自動修復
    - AssetPostprocessor: FBX/Prefabインポート時のMissingスロット自動リマップ
    - Undo/Redo対応、AssetDatabase.SaveAssetsによる即時反映
    - ログ出力で修復状況を可視化
- メニュー: Tools/lilToon PCSS/Missing Material AutoFixer

## 今後の拡張案
- 既存lilToonマテリアルの自動再利用（類似名・テクスチャ一致で割り当て）
- テクスチャ・色の自動引き継ぎ
- シーン外Prefab/FBXへのバッチ適用
- 他シェーダー（Poiyomi等）への対応
- Missing以外の参照切れ検出（[BxUni-MissingFinder](https://github.com/bexide/BxUni-MissingFinder)のような全般的参照切れ検出機能）

---

## 参考
- [Material Texture Fixer](https://github.com/ahabdeveloper/Unity-material-textures-tools)
- [BxUni-MissingFinder](https://github.com/bexide/BxUni-MissingFinder)
- [Material Usage Analyzer](https://github.com/robertrumney/material-usage)

---

実装日: 2025-06-24
担当: AIアシスタント 