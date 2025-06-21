# lilToon PCSS Extension - シェーダーエラートラブルシューティングガイド

## 🚨 よくあるシェーダーエラーと解決方法

### **エラー1: `Couldn't open include file 'lil_pcss_shadows.hlsl'`**

**エラーメッセージ例**:
```
Shader error in 'Hidden/ltspass_opaque': Couldn't open include file 'lil_pcss_shadows.hlsl'. 
at /Users/downl/AppData/Local/VRChatCreatorCompanion/VRChatProjects/New Project2/Packages/jp.lilxyzw.liltoon/Shader/Includes/lil_common_frag.hlsl(978)
```

#### 🔧 **即座に実行する解決手順**

##### **手順1: lilToon PCSS Extensionのインストール確認**
```
1. VRChat Creator Companion (VCC) を開く
2. プロジェクトを選択
3. Manage Project → Packages タブ
4. "lilToon PCSS Extension" がインストールされているか確認
```

##### **手順2: VPMリポジトリからの再インストール**
```
1. VCC → Settings → Packages → Add Repository
2. URL: https://zapabob.github.io/liltoon-pcss-extension/index.json
3. プロジェクトで "lilToon PCSS Extension - All-in-One Edition" を選択
4. Install ボタンをクリック
```

##### **手順3: Unity Editor での強制再インポート**
```
1. Unity Editor を開く
2. Assets → Reimport All を実行
3. Window → lilToon PCSS Extension → Setup Wizard を開く
4. "Auto Setup" ボタンをクリック
```

##### **手順4: 手動ファイル配置（緊急時）**
不足しているインクルードファイルを手動で配置：

**配置場所**: `Assets/Shaders/Includes/`
- `lil_pcss_shadows.hlsl` - PCSS影計算メイン
- `lil_pcss_common.hlsl` - 共通定義・設定

---

### **エラー2: `PCSS_ENABLED` 未定義エラー**

**エラーメッセージ例**:
```
Shader error: undeclared identifier 'PCSS_ENABLED'
```

#### 🔧 **解決方法**
```csharp
// シェーダーファイルの先頭に追加
#include "lil_pcss_common.hlsl"
#include "lil_pcss_shadows.hlsl"
```

---

### **エラー3: VRChat Quest互換性エラー**

**エラーメッセージ例**:
```
Shader compile error: Too many texture samples for mobile platform
```

#### 🔧 **解決方法**
1. **品質設定を調整**:
   - VCC Setup Wizard → Quality Settings
   - "Mobile Optimized" を選択

2. **手動設定**:
   ```csharp
   #define LIL_PCSS_MOBILE_PLATFORM
   #define LIL_PCSS_REDUCED_SAMPLES
   ```

---

### **エラー4: Bakery Lightmapper競合エラー**

**エラーメッセージ例**:
```
Shader error: redefinition of 'BAKERY_ENABLED'
```

#### 🔧 **解決方法**
1. **Bakery統合を有効化**:
   - Window → lilToon PCSS Extension → Bakery Integration
   - "Enable Bakery Support" をチェック

2. **競合回避**:
   ```csharp
   #ifndef BAKERY_ENABLED
   #define BAKERY_ENABLED
   #endif
   ```

---

## 🛠️ 高度なトラブルシューティング

### **シェーダーキャッシュクリア**
```
1. Unity Editor を閉じる
2. プロジェクトフォルダ/Library/ShaderCache を削除
3. Unity Editor を再起動
4. Assets → Reimport All を実行
```

### **アセンブリ定義修復**
```
1. Window → lilToon PCSS Extension → Advanced Tools
2. "Repair Assembly Definitions" をクリック
3. Unity Editor を再起動
```

### **完全リセット手順**
```
1. Unity Editor を閉じる
2. プロジェクトフォルダ/Library 全体を削除
3. Unity Hub でプロジェクトを再度開く
4. VCC Setup Wizard を再実行
```

---

## 🎯 プラットフォーム別最適化

### **PC (Windows/Mac/Linux)**
- **推奨設定**: High Quality PCSS
- **サンプル数**: 16-32
- **パフォーマンス**: 良好

### **VRChat Quest (Android)**
- **推奨設定**: Mobile Optimized
- **サンプル数**: 4-8
- **制限事項**: 一部エフェクト無効

### **VRChat Quest Pro**
- **推奨設定**: Balanced Quality
- **サンプル数**: 8-16
- **パフォーマンス**: 中程度

---

## 📊 パフォーマンス診断

### **フレームレート低下の場合**
1. **品質設定を下げる**:
   - PCSS Sample Count: 16 → 8
   - Blocker Search Samples: 16 → 8

2. **VRChat最適化**:
   - Window → lilToon PCSS Extension → Performance Optimizer
   - "VRChat Optimized" プリセットを適用

### **メモリ使用量が多い場合**
1. **テクスチャサイズ削減**:
   - Shadow Map Resolution: 2048 → 1024
   - Normal Map Compression: 有効

---

## 🔄 バージョン互換性

### **lilToon v1.10.3+**
- ✅ **完全対応**
- ✅ **全機能利用可能**

### **lilToon v1.9.x**
- ⚠️ **部分対応**
- ⚠️ **一部機能制限**

### **lilToon v1.8.x以下**
- ❌ **非対応**
- ❌ **アップグレード必須**

---

## 📞 サポート・お問い合わせ

### **GitHub Issues**
- URL: https://github.com/zapabob/liltoon-pcss-extension/issues
- 新しいバグ報告・機能要望

### **Discord サポート**
- VRChat技術サポートコミュニティ
- リアルタイムサポート

### **メールサポート**
- r.minegishi1987@gmail.com
- 詳細な技術的問題

---

## 🎯 予防策

### **定期メンテナンス**
1. **月1回**: VCC Package Update確認
2. **週1回**: Unity Project Library クリア
3. **毎回**: ビルド前のShader Compilation確認

### **ベストプラクティス**
- ✅ 常に最新版のlilToon PCSS Extensionを使用
- ✅ VCC Setup Wizardを定期実行
- ✅ プロジェクトバックアップを定期取得
- ✅ テスト環境での事前確認

---

**最終更新**: 2025-06-22  
**対応バージョン**: lilToon PCSS Extension v1.2.1+  
**VRChat SDK**: 3.7.0+ 