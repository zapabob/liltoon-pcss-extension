# PCSS技術の概念と背景

## 🎯 概要

このドキュメントでは、lilToon PCSS Extensionで使用されているPCSS（Percentage-Closer Soft Shadows）技術について、その概念、背景、技術的詳細を説明します。

## 📚 PCSSとは

### 定義
PCSS（Percentage-Closer Soft Shadows）は、リアルタイムレンダリングにおいて柔らかい影を生成するための技術です。従来のハードシャドウ（硬い影）とは異なり、現実世界のような自然な影の境界を再現します。

### 従来技術との比較

#### ハードシャドウ（従来技術）
```csharp
// 従来のハードシャドウ
{
    shadowType = ShadowType.Hard,
    shadowQuality = ShadowQuality.Low,
    shadowSoftness = 0.0f,  // 硬い影
    shadowSamples = 1        // 単一サンプル
}
```

#### PCSS（本技術）
```csharp
// PCSSによる柔らかい影
{
    shadowType = ShadowType.PCSS,
    shadowQuality = ShadowQuality.High,
    shadowSoftness = 0.7f,   // 柔らかい影
    shadowSamples = 32       // 複数サンプル
}
```

## 🔬 技術的原理

### 1. パーセンテージ・クローザー法

PCSSの核心となる技術は「パーセンテージ・クローザー法」です。この手法では、光源から影を落とす物体までの距離に基づいて、影の柔らかさを動的に計算します。

```csharp
// PCSSの計算原理
float CalculatePCSS(float lightDistance, float blockerDistance)
{
    float penumbraSize = (lightDistance - blockerDistance) / lightDistance;
    return Mathf.Clamp01(penumbraSize);
}
```

### 2. サンプリング技術

PCSSでは、影の品質を向上させるために複数のサンプルを使用します：

```csharp
// サンプリングパターンの例
Vector2[] samplingPattern = new Vector2[]
{
    new Vector2(0.0f, 0.0f),    // 中心
    new Vector2(-0.5f, -0.5f),  // 左上
    new Vector2(0.5f, -0.5f),   // 右上
    new Vector2(-0.5f, 0.5f),   // 左下
    new Vector2(0.5f, 0.5f),    // 右下
    // ... さらに多くのサンプル
};
```

### 3. フィルタリング技術

影の境界を滑らかにするために、様々なフィルタリング技術が使用されます：

```csharp
// ガウシアンフィルターの例
float GaussianFilter(float distance, float sigma)
{
    return Mathf.Exp(-(distance * distance) / (2.0f * sigma * sigma));
}
```

## 🎨 アートスタイルへの応用

### リアル風表現

リアル風表現では、現実世界の物理法則に基づいた影を再現します：

```csharp
// リアル風PCSS設定
{
    shadowDistance = 3.0f,        // 現実的な距離
    shadowSoftness = 0.7f,        // 自然な柔らかさ
    shadowIntensity = 1.2f,       // 適度な影の強度
    shadowSamples = 32,           // 高品質サンプリング
    shadowColor = new Color(0.1f, 0.1f, 0.15f, 1.0f) // 自然な影色
}
```

### アニメ風表現

アニメ風表現では、アートスタイルに合わせて影を調整します：

```csharp
// アニメ風PCSS設定
{
    shadowDistance = 1.5f,        // 短距離でシャープ
    shadowSoftness = 0.3f,        // 硬い影でクリア
    shadowIntensity = 0.8f,       // 軽い影
    shadowSamples = 16,           // 軽量サンプリング
    shadowColor = new Color(0.2f, 0.2f, 0.3f, 1.0f) // 青みがかった影
}
```

### 映画風表現

映画風表現では、ドラマチックな効果を強調します：

```csharp
// 映画風PCSS設定
{
    shadowDistance = 4.0f,        // 長距離でドラマチック
    shadowSoftness = 0.9f,        // 非常に柔らかい影
    shadowIntensity = 1.5f,       // 強い影
    shadowSamples = 48,           // 最高品質サンプリング
    shadowColor = new Color(0.05f, 0.05f, 0.1f, 1.0f) // 深い影
}
```

## ⚡ パフォーマンス最適化

### 品質とパフォーマンスのバランス

PCSSは高品質な影を提供しますが、計算コストが高いため、用途に応じた最適化が重要です：

```csharp
// パフォーマンスレベル別設定
public enum PerformanceLevel
{
    Low,    // モバイル、VR
    Medium, // ゲーム、アニメ
    High    // ポートレート、映画
}

// パフォーマンスレベルに応じた設定
var settings = GetPCSSSettings(PerformanceLevel.Medium);
```

### 動的品質調整

リアルタイムでパフォーマンスに応じて品質を調整：

```csharp
// 動的品質調整の例
float targetFPS = 60.0f;
float currentFPS = GetCurrentFPS();

if (currentFPS < targetFPS * 0.8f)
{
    // パフォーマンスが悪い場合、品質を下げる
    shadowSamples = Mathf.Max(8, shadowSamples - 8);
    shadowDistance = Mathf.Max(1.0f, shadowDistance - 0.5f);
}
```

## 🔧 技術的実装

### シェーダー実装

PCSSはシェーダーレベルで実装され、GPUの並列処理能力を活用します：

```hlsl
// PCSSシェーダーコードの例
float CalculatePCSSShadow(float3 worldPos, float3 lightDir)
{
    float shadow = 0.0;
    
    // 複数サンプルで影を計算
    for (int i = 0; i < SHADOW_SAMPLES; i++)
    {
        float2 offset = samplingPattern[i] * filterRadius;
        shadow += SampleShadowMap(worldPos + offset);
    }
    
    return shadow / SHADOW_SAMPLES;
}
```

### 条件付きコンパイル

異なるプラットフォームやSDKに対応するための条件付きコンパイル：

```csharp
#if VRCHAT_SDK_AVAILABLE
    // VRChat SDK固有の実装
    var vrcAvatar = avatar.GetComponent<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>();
    if (vrcAvatar != null)
    {
        ApplyPCSSToVRCAvatar(vrcAvatar, settings);
    }
#endif

#if MODULAR_AVATAR_AVAILABLE
    // Modular Avatar固有の実装
    var modularAvatar = avatar.GetComponent<ModularAvatarInformation>();
    if (modularAvatar != null)
    {
        ApplyPCSSToModularAvatar(modularAvatar, settings);
    }
#endif
```

## 📊 技術的メリット

### 1. 視覚的品質の向上

- **自然な影**: 現実世界のような柔らかい影
- **アートスタイル対応**: 様々な表現スタイルに対応
- **一貫性**: 全体的な視覚的品質の向上

### 2. パフォーマンス効率

- **動的調整**: リアルタイムでの品質調整
- **最適化**: 用途に応じた最適化
- **スケーラビリティ**: 様々なハードウェアに対応

### 3. 開発効率

- **プリセットシステム**: 即座に適用可能な設定
- **リアルタイムプレビュー**: 即座に効果を確認
- **バッチ処理**: 大量のアセットを効率的に処理

## 🔮 将来の展望

### 1. リアルタイムレイトレーシング

将来的には、リアルタイムレイトレーシングとの統合により、さらに高品質な影が実現される可能性があります。

### 2. AI支援最適化

機械学習を活用した自動最適化により、より効率的なパフォーマンス調整が可能になります。

### 3. クロスプラットフォーム対応

様々なプラットフォーム（PC、モバイル、VR、AR）での最適化が進められます。

## 📚 参考資料

- [Unity公式ドキュメント - シャドウ](https://docs.unity3d.com/Manual/Shadows.html)
- [VRChat SDK ドキュメント](https://docs.vrchat.com/)
- [Modular Avatar ドキュメント](https://modular-avatar.nadena.dev/)

## 🎯 まとめ

PCSS技術は、リアルタイムレンダリングにおける影の品質を大幅に向上させる革新的な技術です。lilToon PCSS Extensionでは、この技術を活用して、様々なアートスタイルに対応した高品質な影システムを提供しています。

技術的な詳細を理解することで、より効果的なプリセットの作成やカスタマイズが可能になります。

---

**なんｊ風に言うと「PCSSの奥義を理解したぜ！影の技術で表現力が格段に上がる！」** 💪🔥 