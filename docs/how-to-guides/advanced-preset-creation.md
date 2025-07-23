# 高度なプリセット作成ガイド

## 🎯 概要

このガイドでは、lilToon PCSS Extensionを使用して高度なカスタムプリセットを作成する方法を説明します。基本的な使用方法をマスターした方向けの実践的なガイドです。

## 📋 前提条件

- [入門チュートリアル](../tutorials/getting-started.md)を完了していること
- Unity Editorの基本的な操作に慣れていること
- シェーダーの基本的な概念を理解していること

## 🎨 ステップ1: プリセットの分析

### 既存プリセットの理解

まず、既存のプリセットがどのような設定を使用しているかを理解しましょう：

```csharp
// リアル風セットの例
{
    shadowDistance = 3.0f,        // 影の距離
    shadowSoftness = 0.7f,        // 影の柔らかさ
    shadowIntensity = 1.2f,       // 影の強度
    shadowSamples = 32,           // シャドウサンプル数
    filterRadius = 1.5f,          // フィルター半径
    shadowBias = 0.03f,           // シャドウバイアス
    shadowColor = new Color(0.1f, 0.1f, 0.15f, 1.0f), // 影の色
    ambientOcclusion = 0.4f       // アンビエントオクルージョン
}
```

### パラメータの意味

- **shadowDistance**: 影が表示される最大距離
- **shadowSoftness**: 影の境界の柔らかさ（0.0=硬い、1.0=柔らかい）
- **shadowIntensity**: 影の濃さ
- **shadowSamples**: 影の品質（数値が高いほど高品質だが重い）
- **filterRadius**: 影のぼかしの範囲
- **shadowBias**: 影のずれを防ぐための調整値
- **shadowColor**: 影の色（RGB + アルファ）
- **ambientOcclusion**: 環境光の遮蔽効果

## 🔧 ステップ2: カスタムプリセットの作成

### 基本的な作成手順

1. **Tools > lilToon PCSS > 高度な統合プリセット管理**を開く
2. **カスタムプリセット**タブを選択
3. **新しいカスタムプリセットを作成**をクリック
4. 以下の設定を調整：

### PCSS設定の調整

```csharp
// カスタムプリセットの例
var customPreset = new PCSSPresetManager.PCSSSettings
{
    // 基本設定
    shadowDistance = 2.5f,
    shadowSoftness = 0.6f,
    shadowIntensity = 1.1f,
    shadowSamples = 24,
    filterRadius = 1.2f,
    shadowBias = 0.04f,
    
    // 色設定
    shadowColor = new Color(0.15f, 0.15f, 0.2f, 1.0f),
    
    // 効果設定
    ambientOcclusion = 0.35f,
    
    // 外部ライト設定
    enableExternalLights = true,
    externalLightIntensity = 1.3f,
    externalLightColor = new Color(1.0f, 0.98f, 0.95f, 1.0f),
    externalLightAngle = 40.0f,
    
    // スタイル設定
    enableAnimeStyle = false,
    enableCinematicStyle = true,
    filmGrain = 0.1f,
    vignette = 0.2f,
    bloom = 0.3f
};
```

### ライト設定の調整

```csharp
var lightSettings = new ExternalLightManager.ExternalLightSettings
{
    // 基本設定
    lightName = "カスタムライト",
    lightType = LightType.Directional,
    intensity = 1.3f,
    color = new Color(1.0f, 0.98f, 0.95f, 1.0f),
    
    // 位置と角度
    position = new Vector3(0, 3, 5),
    rotation = new Vector3(40, 0, 0),
    distance = 5.0f,
    
    // シャドウ設定
    shadows = LightShadows.Soft,
    shadowStrength = 0.9f,
    shadowResolution = 2048,
    
    // スタイル設定
    enableAnimeStyle = false,
    enableCinematicStyle = true,
    cinematicLightColor = new Color(1.0f, 0.98f, 0.95f, 1.0f),
    cinematicIntensity = 1.3f,
    cinematicAngle = 40.0f
};
```

## 🎯 ステップ3: 効果的なプリセット設計

### 用途別の設計パターン

#### ポートレート撮影向け

```csharp
// 美しい肌の質感を強調
{
    shadowDistance = 2.0f,        // 近距離で詳細な影
    shadowSoftness = 0.8f,        // 柔らかい影で自然な印象
    shadowIntensity = 1.1f,       // 適度な影の強度
    shadowSamples = 32,           // 高品質な影
    shadowColor = new Color(0.12f, 0.12f, 0.18f, 1.0f), // 暖かみのある影
    ambientOcclusion = 0.3f,      // 軽いAO効果
    externalLightColor = new Color(1.0f, 0.98f, 0.95f, 1.0f), // 暖色系ライト
    filmGrain = 0.05f,            // 軽いフィルムグレイン
    vignette = 0.15f              // 軽いビネット効果
}
```

#### ゲーム向け（パフォーマンス重視）

```csharp
// パフォーマンスを重視した設定
{
    shadowDistance = 1.5f,        // 短距離で軽量化
    shadowSoftness = 0.4f,        // 硬い影で軽量化
    shadowIntensity = 0.9f,       // 適度な影
    shadowSamples = 16,           // 少ないサンプル数
    filterRadius = 0.8f,          // 小さなフィルター半径
    shadowColor = new Color(0.2f, 0.2f, 0.25f, 1.0f), // 標準的な影
    ambientOcclusion = 0.2f,      // 軽いAO効果
    externalLightIntensity = 1.0f, // 標準的なライト強度
    enableAnimeStyle = false,      // 特殊効果を無効化
    enableCinematicStyle = false   // 特殊効果を無効化
}
```

#### アニメ風向け

```csharp
// アニメやマンガ風の表現
{
    shadowDistance = 1.0f,        // 短距離でシャープな影
    shadowSoftness = 0.2f,        // 硬い影でクリアな表現
    shadowIntensity = 0.8f,       // 適度な影
    shadowSamples = 16,           // 標準的なサンプル数
    shadowColor = new Color(0.25f, 0.25f, 0.35f, 1.0f), // 青みがかった影
    ambientOcclusion = 0.15f,     // 軽いAO効果
    enableAnimeStyle = true,       // アニメ風効果を有効化
    celShadingThreshold = 0.6f,   // セルシェーディング閾値
    celShadingSmoothness = 0.05f, // セルシェーディングの滑らかさ
    externalLightColor = new Color(1.0f, 1.0f, 1.0f, 1.0f) // 純白のライト
}
```

## 🔄 ステップ4: プリセットのテストと調整

### テスト手順

1. **リアルタイムプレビュー**を使用して即座に効果を確認
2. 異なるアバターでテスト
3. 異なるライティング環境でテスト
4. パフォーマンスを確認

### 調整のコツ

- **shadowDistance**: シーンのスケールに合わせて調整
- **shadowSoftness**: アートスタイルに合わせて調整
- **shadowSamples**: パフォーマンスと品質のバランス
- **shadowColor**: 全体的なムードに合わせて調整

## 📁 ステップ5: プリセットの管理

### カテゴリ分類

プリセットを効果的に管理するために、以下のカテゴリを使用：

- **ポートレート**: 人物撮影向け
- **ゲーム**: ゲーム向け（パフォーマンス重視）
- **アニメ**: アニメ風表現
- **映画**: 映画風表現
- **カスタム**: 独自の設定

### メタデータの活用

```csharp
var preset = new CustomPreset
{
    name = "美肌ポートレート",
    description = "美しい肌の質感を強調するポートレート向けプリセット",
    category = "ポートレート",
    author = "ユーザー名",
    creationDate = System.DateTime.Now,
    // 設定内容...
};
```

## 🚀 ステップ6: 高度なテクニック

### 条件付き設定

```csharp
// アバターの種類に応じて設定を変更
if (avatarType == AvatarType.Human)
{
    settings.shadowSoftness = 0.8f;  // 人間向けの柔らかい影
    settings.externalLightColor = new Color(1.0f, 0.98f, 0.95f, 1.0f); // 暖色系
}
else if (avatarType == AvatarType.Anime)
{
    settings.shadowSoftness = 0.3f;  // アニメ向けの硬い影
    settings.enableAnimeStyle = true;
}
```

### 動的調整

```csharp
// 時間に応じてライトの色を変更
var timeOfDay = System.DateTime.Now.Hour;
if (timeOfDay >= 6 && timeOfDay < 18)
{
    // 昼間の設定
    settings.externalLightColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
}
else
{
    // 夜間の設定
    settings.externalLightColor = new Color(0.8f, 0.8f, 1.0f, 1.0f);
}
```

## 📚 参考資料

- [PCSS設定リファレンス](../reference/pcss-settings.md)
- [ライト設定リファレンス](../reference/light-settings.md)
- [パフォーマンス最適化ガイド](../how-to-guides/performance-optimization.md)

## 🆘 トラブルシューティング

### よくある問題

**Q: プリセットが期待通りに動作しない**
A: 以下を確認してください：
- アバターのマテリアルがlilToonシェーダーを使用しているか
- URPが正しく設定されているか
- 依存関係のバージョンが正しいか

**Q: パフォーマンスが悪い**
A: 以下を試してください：
- shadowSamplesを減らす
- shadowDistanceを短くする
- 特殊効果を無効化する

---

**なんｊ風に言うと「これでプロ級のプリセットが作れるぜ！カスタマイズで差別化だ！」** 💪🔥 