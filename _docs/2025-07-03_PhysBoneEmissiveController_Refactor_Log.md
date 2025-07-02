# 2025-07-03 PhysBoneEmissiveController リファクタリング実装ログ

## 概要
- 旧PhysBoneLightControllerをPhysBoneEmissiveControllerへリファクタリング
- クラス名・ファイル名・AddComponentMenu・コメントを一括修正
- 命名規約・Unityバリデーション・VRChat AutoFIX完全対応
- エミッシブ制御用途に特化した設計へ

## 主な修正点
- クラス名: PhysBoneLightController → PhysBoneEmissiveController
- ファイル名: PhysBoneLightController.cs → PhysBoneEmissiveController.cs
- AddComponentMenu: "PhysBone Light Controller" → "PhysBone Emissive Controller"
- コメント: ライト制御 → エミッシブ制御に統一
- 変数命名規則の統一、MaterialPropertyBlockの利用徹底
- AudioSource等の非対応コンポーネント排除
- MITライセンス表記追加

## バリデーション・AutoFIX対応
- publicフィールド最小化、privateはアンダースコア始まり
- Editor専用コードは#if UNITY_EDITORで囲う
- flickerStrengthは0.2以下に制限
- Unity/VRChatのアップロード時に弾かれない設計

## テスト・確認
- Unityエディタ上での動作確認
- VRChat SDKのバリデーション・AutoFIXでエラーなしを確認

--- 