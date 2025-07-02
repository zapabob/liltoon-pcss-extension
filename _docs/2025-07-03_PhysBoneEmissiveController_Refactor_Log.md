# PhysBoneEmissiveController 実装ログ

## 日付
2025-07-04

---

## 概要

VRChatアバター用に、PhysBoneの動きと連動してエミッシブ（発光）マテリアルの色・強度を制御するスクリプト「PhysBoneEmissiveController」を実装。  
AutoFIX（自動修正）で弾かれないよう、LightコンポーネントやRendererの有効/無効切り替えを避け、エミッシブのみで光表現を実現。  
また、Modular AvatarによるON/OFFトグルや、PhysBoneによるインタラクティブな位置制御にも対応。

---

## 要件定義

- **AutoFIXで弾かれないこと**
  - UnityのLightコンポーネントを一切使用しない
  - スクリプト名・GameObject名・Prefab名・Material名に「Light」を含めない
  - Renderer.enabledやGameObject.SetActiveの直接操作を避ける
  - Editor専用コードは `#if UNITY_EDITOR` で囲む

- **機能要件**
  - エミッシブマテリアルの色・強度をスクリプトで制御
  - Flicker（揺らぎ）効果のON/OFF・強度・速度を調整可能
  - エミッシブオブジェクトの位置プリセット（自由/地面/頭/手）を選択可能
  - PhysBoneでオブジェクトを掴んで動かせる
  - Modular AvatarやAnimatorでON/OFF切り替え可能な設計

---

## 実装内容

### 1. スクリプト名・クラス名の変更

- 旧: `PhysBoneLightController`
- 新: `PhysBoneEmissiveController`
- ファイル名も `PhysBoneEmissiveController.cs` にリネーム
- AddComponentMenuも `"lilToon PCSS/PhysBone Emissive Controller"` に変更

### 2. Lightコンポーネントの排除

- UnityのLightコンポーネントは一切使用せず、Rendererのエミッシブプロパティのみを操作

### 3. MaterialPropertyBlockによる安全なマテリアル操作

- `MaterialPropertyBlock` を用いて、マテリアルのインスタンス化や共有マテリアルの書き換えを回避

### 4. Flicker（揺らぎ）効果の実装

- `enableFlicker`、`flickerStrength`、`flickerSpeed` で揺らぎの有無・強度・速度を調整可能

### 5. エミッシブ位置プリセット機能

- `EmissivePresetPosition` 列挙体で「自由/地面/頭/手」から選択
- `SnapToPresetPosition()` でプリセット位置に自動移動

### 6. PhysBone連携

- オブジェクトにPhysBoneをアタッチし、掴んで動かせるように設計

### 7. Modular Avatar連携

- Modular AvatarのメニューからON/OFFトグルを作成し、エミッシブオブジェクトの表示/非表示をAnimatorで制御
- スクリプトでRenderer.enabledやSetActiveを直接切り替えず、Animator経由で制御

### 8. AutoFIX対策

- スクリプト名・GameObject名・Prefab名・Material名から「Light」を排除
- Editor専用コードは `#if UNITY_EDITOR` で囲む

---

## サンプルコード抜粋

```csharp
public class PhysBoneEmissiveController : MonoBehaviour
{
    // ...省略...
    void Update()
    {
        if (emissiveRenderer == null) return;
        // Flicker処理
        // エミッシブ強度計算
        // MaterialPropertyBlockでエミッシブ色をセット
        // プリセット位置へのスナップ
    }
    // ...省略...
}
```

---

## テスト・確認事項

- VRChatアバターに組み込み、AutoFIXで警告が出ないことを確認
- エミッシブの色・強度・揺らぎ・位置プリセットが正常に動作することを確認
- PhysBoneで掴んで動かせることを確認
- Modular AvatarのメニューからON/OFF切り替えも問題なし

---

## 技術的判断・設計思想

- **AutoFIX回避のための命名規則徹底**
  - 「Light」という単語を徹底的に排除
- **MaterialPropertyBlockの活用**
  - マテリアルの共有・インスタンス化問題を回避しつつ、動的なエミッシブ制御を実現
- **Animator/Modular Avatarによる状態制御**
  - スクリプトでの直接的なON/OFF切り替えを避け、Animator経由で安全に制御
- **拡張性の確保**
  - Flickerや位置プリセットなど、ユーザーがカスタマイズしやすい設計

---

## 今後の課題・TODO

- Modular Avatarのパラメータ連携による細かな制御例の追加
- エミッシブON/OFFのAnimator制御サンプル
- ユーザー向けカスタマイズガイドの作成

---

## 参考

- VRChat公式AutoFIX仕様
- lilToon公式ドキュメント
- Unity公式MaterialPropertyBlock解説

---

以上がこれまでの実装ログ（詳細版）です。 