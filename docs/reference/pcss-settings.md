# PCSS設定リファレンス

## 📋 概要

このドキュメントでは、lilToon PCSS ExtensionのPCSS（Percentage-Closer Soft Shadows）設定について詳細に説明します。開発者や高度なユーザー向けの技術仕様書です。

## 🎯 PCSS設定クラス

### PCSSPresetManager.PCSSSettings

```csharp
[System.Serializable]
public class PCSSSettings
{
    // 基本設定
    public float shadowDistance = 3.0f;
    public float shadowSoftness = 0.7f;
    public float shadowIntensity = 1.2f;
    public int shadowSamples = 32;
    public float filterRadius = 1.5f;
    public float shadowBias = 0.03f;
    public Color shadowColor = new Color(0.1f, 0.1f, 0.15f, 1.0f);
    public float ambientOcclusion = 0.4f;
    
    // 外部ライト設定
    public bool enableExternalLights = true;
    public float externalLightIntensity = 1.5f;
    public Color externalLightColor = new Color(1.0f, 0.95f, 0.9f, 1.0f);
    public float externalLightAngle = 30.0f;
    
    // スタイル設定
    public bool enableAnimeStyle = false;
    public float celShadingThreshold = 0.6f;
    public float celShadingSmoothness = 0.05f;
    public bool enableCinematicStyle = false;
    public float filmGrain = 0.15f;
    public float vignette = 0.3f;
    public float bloom = 0.4f;
}
```

## 📊 パラメータ詳細

### 基本設定

#### shadowDistance
- **型**: `float`
- **デフォルト値**: `3.0f`
- **範囲**: `0.1f` ～ `10.0f`
- **説明**: 影が表示される最大距離を指定します
- **単位**: Unity単位（メートル）
- **推奨値**:
  - ポートレート: `2.0f` ～ `3.0f`
  - ゲーム: `1.5f` ～ `2.5f`
  - 映画: `3.0f` ～ `5.0f`

#### shadowSoftness
- **型**: `float`
- **デフォルト値**: `0.7f`
- **範囲**: `0.0f` ～ `1.0f`
- **説明**: 影の境界の柔らかさを指定します
- **効果**:
  - `0.0f`: 硬い影（シャープな境界）
  - `0.5f`: 中間的な影
  - `1.0f`: 非常に柔らかい影（ぼやけた境界）

#### shadowIntensity
- **型**: `float`
- **デフォルト値**: `1.2f`
- **範囲**: `0.0f` ～ `2.0f`
- **説明**: 影の濃さを指定します
- **効果**:
  - `0.0f`: 影なし
  - `1.0f`: 標準的な影
  - `2.0f`: 非常に濃い影

#### shadowSamples
- **型**: `int`
- **デフォルト値**: `32`
- **範囲**: `8` ～ `64`
- **説明**: 影の品質を決定するサンプル数を指定します
- **パフォーマンス影響**: 数値が高いほど高品質だが重い
- **推奨値**:
  - 低品質: `8` ～ `16`
  - 中品質: `16` ～ `32`
  - 高品質: `32` ～ `64`

#### filterRadius
- **型**: `float`
- **デフォルト値**: `1.5f`
- **範囲**: `0.1f` ～ `5.0f`
- **説明**: 影のぼかしの範囲を指定します
- **効果**: 値が大きいほど影がぼやける

#### shadowBias
- **型**: `float`
- **デフォルト値**: `0.03f`
- **範囲**: `0.001f` ～ `0.1f`
- **説明**: 影のずれ（ピーターパン効果）を防ぐための調整値
- **効果**: 値が小さいほど影が正確だが、アーティファクトが発生しやすい

#### shadowColor
- **型**: `Color`
- **デフォルト値**: `new Color(0.1f, 0.1f, 0.15f, 1.0f)`
- **説明**: 影の色を指定します
- **推奨値**:
  - 自然な影: `new Color(0.1f, 0.1f, 0.15f, 1.0f)`
  - 暖色系: `new Color(0.15f, 0.1f, 0.1f, 1.0f)`
  - 冷色系: `new Color(0.1f, 0.1f, 0.2f, 1.0f)`

#### ambientOcclusion
- **型**: `float`
- **デフォルト値**: `0.4f`
- **範囲**: `0.0f` ～ `1.0f`
- **説明**: 環境光の遮蔽効果の強度を指定します
- **効果**: 値が大きいほど陰影が強調される

### 外部ライト設定

#### enableExternalLights
- **型**: `bool`
- **デフォルト値**: `true`
- **説明**: 外部ライトの有効/無効を指定します

#### externalLightIntensity
- **型**: `float`
- **デフォルト値**: `1.5f`
- **範囲**: `0.0f` ～ `5.0f`
- **説明**: 外部ライトの強度を指定します

#### externalLightColor
- **型**: `Color`
- **デフォルト値**: `new Color(1.0f, 0.95f, 0.9f, 1.0f)`
- **説明**: 外部ライトの色を指定します

#### externalLightAngle
- **型**: `float`
- **デフォルト値**: `30.0f`
- **範囲**: `0.0f` ～ `90.0f`
- **説明**: 外部ライトの角度を指定します（度）

### スタイル設定

#### enableAnimeStyle
- **型**: `bool`
- **デフォルト値**: `false`
- **説明**: アニメ風効果の有効/無効を指定します

#### celShadingThreshold
- **型**: `float`
- **デフォルト値**: `0.6f`
- **範囲**: `0.1f` ～ `1.0f`
- **説明**: セルシェーディングの閾値を指定します
- **効果**: 値が小さいほど段階が少なくなる

#### celShadingSmoothness
- **型**: `float`
- **デフォルト値**: `0.05f`
- **範囲**: `0.01f` ～ `0.2f`
- **説明**: セルシェーディングの滑らかさを指定します

#### enableCinematicStyle
- **型**: `bool`
- **デフォルト値**: `false`
- **説明**: 映画風効果の有効/無効を指定します

#### filmGrain
- **型**: `float`
- **デフォルト値**: `0.15f`
- **範囲**: `0.0f` ～ `1.0f`
- **説明**: フィルムグレイン効果の強度を指定します

#### vignette
- **型**: `float`
- **デフォルト値**: `0.3f`
- **範囲**: `0.0f` ～ `1.0f`
- **説明**: ビネット効果の強度を指定します

#### bloom
- **型**: `float`
- **デフォルト値**: `0.4f`
- **範囲**: `0.0f` ～ `2.0f`
- **説明**: ブルーム効果の強度を指定します

## 🔧 シェーダープロパティ

### PCSS関連プロパティ

```csharp
// シェーダーに設定されるプロパティ名
"_PCSSShadowDistance"     // shadowDistance
"_PCSSShadowSoftness"     // shadowSoftness
"_PCSSShadowIntensity"    // shadowIntensity
"_PCSSShadowSamples"      // shadowSamples
"_PCSSFilterRadius"        // filterRadius
"_PCSSShadowBias"         // shadowBias
"_PCSSShadowColor"        // shadowColor
"_PCSSAmbientOcclusion"   // ambientOcclusion
```

### スタイル関連プロパティ

```csharp
// アニメ風効果
"_CelShadingThreshold"    // celShadingThreshold
"_CelShadingSmoothness"   // celShadingSmoothness

// 映画風効果
"_FilmGrain"              // filmGrain
"_Vignette"               // vignette
"_Bloom"                  // bloom
```

## 📊 パフォーマンスガイドライン

### 推奨設定（用途別）

#### 高品質（ポートレート、映画）
```csharp
{
    shadowDistance = 3.0f,
    shadowSoftness = 0.8f,
    shadowIntensity = 1.2f,
    shadowSamples = 48,
    filterRadius = 2.0f,
    shadowBias = 0.02f,
    ambientOcclusion = 0.5f
}
```

#### 中品質（ゲーム、アニメ）
```csharp
{
    shadowDistance = 2.0f,
    shadowSoftness = 0.5f,
    shadowIntensity = 1.0f,
    shadowSamples = 24,
    filterRadius = 1.2f,
    shadowBias = 0.04f,
    ambientOcclusion = 0.3f
}
```

#### 低品質（モバイル、VR）
```csharp
{
    shadowDistance = 1.5f,
    shadowSoftness = 0.3f,
    shadowIntensity = 0.8f,
    shadowSamples = 16,
    filterRadius = 0.8f,
    shadowBias = 0.06f,
    ambientOcclusion = 0.2f
}
```

### パフォーマンス最適化のコツ

1. **shadowSamples**: 最も影響が大きいパラメータ
2. **shadowDistance**: 短くすることで大幅に軽量化
3. **shadowSoftness**: 低い値で軽量化
4. **特殊効果**: 必要に応じて無効化

## 🔍 デバッグとトラブルシューティング

### よくある問題と解決策

#### 影が表示されない
```csharp
// 確認すべき設定
shadowDistance > 0.1f
shadowIntensity > 0.0f
shadowSamples >= 8
```

#### 影がずれる（ピーターパン効果）
```csharp
// shadowBiasを調整
shadowBias = 0.02f;  // より小さい値
```

#### パフォーマンスが悪い
```csharp
// 以下の設定を調整
shadowSamples = 16;      // 減らす
shadowDistance = 1.5f;   // 短くする
shadowSoftness = 0.3f;   // 硬くする
```

#### 影が粗い
```csharp
// 品質を上げる
shadowSamples = 48;      // 増やす
filterRadius = 2.0f;     // 大きくする
shadowSoftness = 0.8f;   // 柔らかくする
```

## 📚 関連ドキュメント

- [ライト設定リファレンス](light-settings.md)
- [パフォーマンス最適化ガイド](../how-to-guides/performance-optimization.md)
- [API リファレンス](api.md)

---

**なんｊ風に言うと「これでPCSSの奥義が分かったぜ！パラメータ調整で影の達人になれる！」** 💪🔥 