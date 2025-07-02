# PhysBoneエミッシブ球体セットアップ機能 実装ログ

## 日時
2024-07-09

## 概要
VRChatアバター向けに「PhysBoneで掴んで動かせるエミッシブ球体」を自動生成・セットアップするエディタウィザード（CompetitorSetupWizard.cs）を実装。  
AutoFIXで弾かれず、外部ライトやLightコンポーネントを一切使わない構成。  
Modular Avatarのトグル連携やプリセット位置・色・強度の自動設定にも対応。

---

## 変更・実装内容

### 1. 既存の外部ライト/Lightコンポーネント制御の完全削除
- `Light`型フィールドや外部ライト参照、生成・設定・初期化・削除処理を全て削除。
- `PhysBoneLightController`の外部ライト関連プロパティ・メソッドも削除。

### 2. PhysBoneエミッシブ球体の自動生成・セットアップ
- アバター直下に`PCSS_Emissive`という名前の球体（Sphere）を自動生成（既存があれば再利用）。
- 球体の`Renderer`にStandardシェーダー＋Emission有効なマテリアルを自動割り当て。
- プリセット（Toon/Realistic/Dark）に応じてエミッシブ色・強度・位置を自動設定。
- 球体のスケールは0.15に統一。

### 3. PhysBoneLightControllerの自動アタッチ・設定
- 球体に`PhysBoneLightController`を自動アタッチ。
- `emissiveRenderer`/`baseEmission`/`maxEmission`/`emissionStrength`を自動設定。

### 4. PhysBone（掴み用）の自動追加
- VRCPhysBoneが利用可能な場合、球体に`VRCPhysBone`を自動追加（既存ならスキップ）。

### 5. マテリアル設定の自動適用
- アバター配下の全Renderer/Materialに対し、lilToon/PCSS Extensionシェーダーの各種影・透過・シャドウマスク設定をプリセットに応じて自動適用。
- Modular Avatar導入時は`MAMaterialSwap`も自動追加。

### 6. エディタウィザードUIの改修
- 「PhysBoneエミッシブセットアップ」ウィザードとしてリネーム。
- プリセット選択、エミッシブ色・強度・位置の自動表示。
- 「PhysBoneエミッシブを適用」ボタンで一括処理。

---

## 使い方

1. Unityエディタでアバターを選択
2. メニューから「Tools > lilToon PCSS > PhysBoneエミッシブセットアップウィザード」を開く
3. プリセットを選択し、「PhysBoneエミッシブを適用」ボタンを押す
4. アバター直下にエミッシブ球体が生成され、PhysBoneで掴んで動かせる発光オブジェクトとして機能

---

## AutoFIX対策

- Lightコンポーネントの参照・制御を完全排除
- Renderer/MaterialのEmissionのみで発光表現
- PhysBone/Modular Avatarの標準機能のみ利用
- publicフィールドやEditor専用コードも厳密に管理

---

## 参考・関連リンク

- [Modular Avatar公式 Simple Object Toggle](https://modular-avatar.nadena.dev/docs/tutorials/object_toggle)
- [Unity公式 Emissive Material解説](https://docs.unity3d.com/Manual/StandardShaderMaterialParameterEmission.html)

---

## 今後の拡張案

- 複数エミッシブ球体の自動生成・管理
- Modular Avatarトグル連携の自動化
- 色・強度・位置のカスタムプリセット追加
- アニメーションやOSC連携による動的制御

---

*この実装ログは2024-07-09時点の内容です。*

---

## 2025-07-03 リファクタリング・最終実装内容

### PhysBoneエミッシブ球体セットアップ機能の完全リファクタリング

- Lightコンポーネント依存を完全排除し、Renderer/MaterialのEmission制御型に全面刷新
- CompetitorSetupWizard.csで球体生成時にLightを付与せず、Emission有効なStandardマテリアルを自動割り当て
- プリセット（Toon/Realistic/Dark）でEmission色・強度・位置を自動設定
- PhysBoneLightControllerもRenderer/MaterialベースのEmission制御に書き換え
- VRCPhysBoneの自動追加・AutoFIX完全対応
- Inspector/ウィザードUIもEmission制御型に統一

### 技術的ポイント
- Unity公式推奨のEmission制御方式を採用（_EmissionColor, EnableKeyword("_EMISSION")）
- 完全自己完結型・外部依存ゼロ
- VRChat/AutoFIX/ModularAvatar環境での高い互換性

### 動作確認
- Unityエディタ・AutoFIX・ビルド・実機で正常動作を確認
- 既存アバターへの適用・複数プリセット切替も問題なし

--- 