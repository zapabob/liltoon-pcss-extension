# PCSS Extension - Poiyomi & VRChat Expression Guide

## 概要

lilToon PCSS ExtensionがPoiyomiシェーダーとVRChatエクスプレッション機能に対応しました。この機能により、VRChatでのアバター使用時にエクスプレッションメニューからPCSSのオン/オフと品質設定を直接制御できます。

## 🎯 主要機能

### Poiyomi対応
- **Poiyomi Toon Shader互換**: 既存のPoiyomiマテリアルにPCSS機能を追加
- **自動検出**: Poiyomiマテリアルの自動識別と設定適用
- **品質プリセット**: Low/Medium/High/Ultraの4段階品質設定
- **互換性維持**: Poiyomiの既存機能を損なわない統合

### VRChatエクスプレッション対応
- **Baseレイヤー対応**: FXではなくBaseレイヤーでの制御（推奨）
- **リアルタイム制御**: ゲーム内でのPCSSオン/オフ切り替え
- **品質調整**: エクスプレッションメニューからの品質レベル変更
- **パフォーマンス配慮**: 状況に応じた自動品質調整

## 📦 インストール方法

### 1. 基本インストール
```
1. UnityパッケージまたはUPMでインストール
2. Poiyomi Toon Shaderがインストール済みであることを確認
3. VRChat SDKがインストール済みであることを確認
```

### 2. シェーダー設定
```
1. マテリアルのシェーダーを "Poiyomi/Toon/PCSS Extension" に変更
2. PCSS設定を調整
3. VRChat Expression Controlを有効化
```

## 🎮 使用方法

### Poiyomiマテリアルでの使用

#### 1. 基本設定
```csharp
// マテリアルのシェーダーを変更
material.shader = Shader.Find("Poiyomi/Toon/PCSS Extension");

// PCSS基本設定
material.SetFloat("_PCSSEnabled", 1f);
material.SetFloat("_PCSSQuality", 1f); // Medium quality
```

#### 2. 品質プリセット
| 品質レベル | サンプル数 | 用途 |
|-----------|-----------|------|
| Low | 8 | モバイル・低スペックPC |
| Medium | 16 | 標準的なPC |
| High | 32 | 高性能PC |
| Ultra | 64 | ハイエンドPC |

#### 3. 自動統合コンポーネント
```csharp
// アバターにPoiyomiPCSSIntegrationを追加
var integration = avatar.AddComponent<PoiyomiPCSSIntegration>();
integration.InitializePCSSIntegration();
```

### VRChatエクスプレッション設定

#### 1. エクスプレッションメニュー作成
```
1. マテリアルを選択
2. "Setup VRChat Expression Menu"ボタンをクリック
3. "Use Base Layer"を選択（推奨）
4. "Create Expression Menu"を実行
```

#### 2. 作成されるファイル
- `PCSS_Parameters.asset` - エクスプレッションパラメータ
- `MaterialName_PCSS_Menu.asset` - エクスプレッションメニュー
- `MaterialName_PCSS_Base.controller` - Baseレイヤーアニメーター
- アニメーションファイル群

#### 3. パラメータ設定
| パラメータ名 | 型 | 範囲 | 説明 |
|-------------|---|------|------|
| PCSS_Enable | Bool | 0/1 | PCSS有効/無効 |
| PCSS_Quality | Float | 0-1 | 品質レベル（0=Low, 1=Ultra） |

## 🔧 詳細設定

### PoiyomiPCSSIntegrationコンポーネント

#### 基本設定
```csharp
[Header("PCSS Configuration")]
public bool enablePCSS = true;
public PCSSQuality quality = PCSSQuality.Medium;
public float blockerSearchRadius = 0.01f;
public float filterRadius = 0.01f;
public int sampleCount = 16;
public float lightSize = 0.1f;
public float shadowBias = 0.001f;
```

#### VRChatエクスプレッション設定
```csharp
[Header("VRChat Expression")]
public bool enableVRChatExpression = false;
public bool useBaseLayer = true;
public string parameterPrefix = "PCSS";
```

#### Poiyomi互換設定
```csharp
[Header("Poiyomi Compatibility")]
public bool maintainPoiyomiFeatures = true;
public bool autoDetectPoiyomiShader = true;
```

### シェーダープロパティ

#### PCSS制御プロパティ
```hlsl
// 基本制御
_PCSSEnabled ("Enable PCSS", Float) = 1
_PCSSQuality ("Quality Preset", Float) = 1

// VRChatエクスプレッション
_VRChatExpression ("Enable VRChat Expression Control", Float) = 0
_PCSSToggleParam ("PCSS Toggle Parameter", Float) = 1
_PCSSQualityParam ("PCSS Quality Parameter", Float) = 1
```

#### 品質パラメータ
```hlsl
_PCSSBlockerSearchRadius ("Blocker Search Radius", Range(0.001, 0.1)) = 0.01
_PCSSFilterRadius ("Filter Radius", Range(0.001, 0.1)) = 0.01
_PCSSSampleCount ("Sample Count", Range(4, 64)) = 16
_PCSSLightSize ("Light Size", Range(0.001, 1.0)) = 0.1
_PCSSShadowBias ("Shadow Bias", Range(0.0001, 0.01)) = 0.001
```

## 🎛️ エクスプレッションメニュー構成

### メニュー階層
```
Main Menu
├── PCSS Control
│   ├── Enable/Disable Toggle
│   └── Quality Settings
│       ├── Low Quality
│       ├── Medium Quality
│       ├── High Quality
│       └── Ultra Quality
```

### アニメーターレイヤー構成
```
Base Layer: PCSS Control
├── PCSS Off State
├── PCSS On State
├── Quality Low State
├── Quality Medium State
├── Quality High State
└── Quality Ultra State
```

## 🚀 パフォーマンス最適化

### 推奨設定

#### VRChatワールド別推奨品質
| ワールドタイプ | 推奨品質 | 理由 |
|--------------|---------|------|
| 大人数イベント | Low | フレームレート優先 |
| 写真撮影 | High/Ultra | 画質優先 |
| 一般的な交流 | Medium | バランス重視 |
| ダンス・パフォーマンス | Medium | 動作の滑らかさ重視 |

#### 自動品質調整
```csharp
// フレームレートに基づく自動調整
public void AutoAdjustQuality()
{
    float fps = 1.0f / Time.deltaTime;
    
    if (fps < 30f)
        SetPCSSQuality(PCSSQuality.Low);
    else if (fps < 45f)
        SetPCSSQuality(PCSSQuality.Medium);
    else if (fps < 60f)
        SetPCSSQuality(PCSSQuality.High);
    else
        SetPCSSQuality(PCSSQuality.Ultra);
}
```

## 🔍 トラブルシューティング

### よくある問題と解決方法

#### 1. エクスプレッションメニューが表示されない
**原因**: VRChat SDKの設定不備
**解決**: 
- VRChat SDKが正しくインストールされているか確認
- エクスプレッションパラメータが正しく設定されているか確認

#### 2. PCSS効果が適用されない
**原因**: シェーダー設定の問題
**解決**:
- マテリアルのシェーダーが"Poiyomi/Toon/PCSS Extension"になっているか確認
- "_PCSSEnabled"プロパティが1に設定されているか確認

#### 3. パフォーマンスが低下する
**原因**: 品質設定が高すぎる
**解決**:
- 品質レベルをLowまたはMediumに下げる
- サンプル数を減らす

#### 4. Poiyomiの既存機能が動作しない
**原因**: 互換性の問題
**解決**:
- "Maintain Poiyomi Features"オプションを有効にする
- 既存のPoiyomiプロパティを再設定する

## 📝 スクリプトAPI

### 基本的な使用例

#### PCSS制御
```csharp
// PCSSコンポーネントを取得
var pcssIntegration = GetComponent<PoiyomiPCSSIntegration>();

// PCSSのオン/オフ
pcssIntegration.TogglePCSS();

// 品質設定
pcssIntegration.SetPCSSQuality(PCSSQuality.High);

// 状態確認
bool isEnabled = pcssIntegration.IsPCSSEnabled();
PCSSQuality currentQuality = pcssIntegration.GetPCSSQuality();
```

#### VRChatパラメータ制御
```csharp
// パラメータ設定（テスト用）
pcssIntegration.SetVRChatParameter("PCSS_Enable", 1f);
pcssIntegration.SetVRChatParameter("PCSS_Quality", 0.75f); // High quality
```

#### マテリアル管理
```csharp
// マテリアル追加
pcssIntegration.AddTargetMaterial(newMaterial);

// マテリアル削除
pcssIntegration.RemoveTargetMaterial(oldMaterial);

// 自動検出
pcssIntegration.AutoDetectPoiyomiMaterials();
```

## 🎨 カスタマイズ

### カスタム品質プリセット
```csharp
// カスタム品質設定の作成
var customQuality = new PCSSQualityData(
    sampleCount: 24,
    blockerSearchRadius: 0.012f,
    filterRadius: 0.012f,
    lightSize: 0.12f,
    shadowBias: 0.0012f
);
```

### エクスプレッションメニューのカスタマイズ
```csharp
// カスタムパラメータプレフィックス
settings.parameterPrefix = "MyAvatar_PCSS";

// カスタムメニュー作成
VRChatExpressionMenuCreator.CreatePCSSExpressionMenu(
    targetMaterial, 
    useBaseLayer: true, 
    parameterPrefix: "CustomPCSS"
);
```

## 📋 チェックリスト

### セットアップ完了チェック
- [ ] Poiyomi Toon Shaderがインストール済み
- [ ] VRChat SDKがインストール済み
- [ ] マテリアルシェーダーがPCSS Extension版に変更済み
- [ ] PoiyomiPCSSIntegrationコンポーネントが追加済み
- [ ] VRChatエクスプレッションメニューが作成済み
- [ ] アニメーターコントローラーが設定済み
- [ ] パラメータが正しく設定済み

### 動作確認チェック
- [ ] エディタでPCSSオン/オフが動作する
- [ ] 品質変更が正しく反映される
- [ ] VRChatでエクスプレッションメニューが表示される
- [ ] メニューからPCSS制御が可能
- [ ] パフォーマンスが許容範囲内

## 📞 サポート

### 問題報告
- GitHub Issues: [リポジトリURL]
- Discord: [サーバー招待URL]
- Email: [サポートメール]

### 更新情報
- リリースノート: [URL]
- 開発ブログ: [URL]

---

**lilToon PCSS Extension v1.1.0**  
Poiyomi & VRChat Expression Support  
© 2025 lilToon PCSS Extension Team 