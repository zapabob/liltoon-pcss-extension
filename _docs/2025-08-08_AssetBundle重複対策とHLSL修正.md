日時: 2025-08-08 21:39:02 JST

目的:
- Shader redefinition 対策: `_ShadowMapTexture` の再定義エラー解消
- AssetBundle 断片化バグ回避: 同名アセット・重複HLSLディレクトリ対策

実装:
- `Shaders/Includes/lil_pcss_shadows.hlsl` と `Shaders/Shaders/Includes/lil_pcss_shadows.hlsl`
  - `LIL_PCSS_SHADOWMAP_DECLARED` ガードと `#if !defined(SHADOWS_SCREEN)` で重複宣言防止
- `Editor/AssetBundleIssueFixer.cs`
  - 重複アセット名スキャナを追加
  - `Shaders/Shaders/Includes` が `Shaders/Includes` と同一なら安全削除ユーティリティ追加

メニュー:
- Tools/lilToon PCSS/ビルド/重複アセット名スキャン
- Tools/lilToon PCSS/ビルド/重複HLSL(Shaders/Shaders/Includes)安全削除

確認事項:
- Unity再インポート後、Shader redefinition エラー解消
- AssetBundleビルド: クリーンビルド（ForceRebuild）推奨

備考:
- 同名アセットは AssetBundle で `bundleArchiveFile` assertion を誘発可。ユーティリティで洗い出し・改名対応推奨。

