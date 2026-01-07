# VRChat AutoFix - ModularAvatarPCSSController問題解決

**実装日時**: 2025-08-17 10:03:24 JST  
**実装者**: AI Assistant  
**バージョン**: 2.7.0 AutoFix Safe

## 🚨 問題概要

**VRChatのAutoFixでModularAvatarPCSSControllerが弾かれる問題**
- カスタムランタイムコンポーネントがVRChatのホワイトリストに含まれていない
- ModularAvatarPCSSControllerがアバターに含まれているとAutoFixで削除される
- アップロード時にエラーが発生する

## 🔧 解決策実装

### 1. ModularAvatarPCSSSetupWizardをVRChat AutoFix対応版に変更

#### 主要変更点
- **カスタムランタイムコンポーネントを完全削除**
- **標準VRChatコンポーネントのみを使用**
- **AutoFixで弾かれない安全な実装**

#### 実装詳細

```csharp
/// <summary>
/// ModularAvatarPCSSSetupWizard - VRChat AutoFix対応版
/// カスタムランタイムコンポーネントを使用せず、標準VRChatコンポーネントのみでPCSS制御を実現
/// </summary>
public class ModularAvatarPCSSSetupWizard : EditorWindow
{
    // 最適化設定
    private bool enableOptimization = false; // AutoFix対応のため無効化
    
    // PhysBone制御（カスタムコンポーネント不使用）
    private int SetupPhysBoneControl()
    {
        // VRCPhysBoneを使用してエミッシブライトを制御
        // カスタムランタイムコンポーネントは使用しない
        var physBone = emissiveObject.AddComponent<VRCPhysBone>();
        physBone.parameter = "PB_Light";
        physBone.allowGrabbing = true;
        physBone.allowPosing = true;
    }
    
    // カスタムコンポーネント削除機能
    private void CleanupCustomComponents()
    {
        // カスタムランタイムコンポーネントを削除
        var customComponents = selectedAvatar.GetComponentsInChildren<Component>()
            .Where(c => c != null && c.GetType().Name.Contains("PCSSController"))
            .ToArray();
            
        foreach (var component in customComponents)
        {
            DestroyImmediate(component);
        }
    }
}
```

### 2. VRChatCleanupMenuにModularAvatarPCSSController削除機能を追加

#### 新機能追加

```csharp
[MenuItem("Tools/lilToon-PCSS-Extension/VRChat向けクリーンアップ/ModularAvatarPCSSController削除", false, 302)]
public static void RemoveModularAvatarPCSSController()
{
    // ModularAvatarPCSSControllerを検索して削除
    var allComponents = go.GetComponentsInChildren<Component>(true);
    foreach (var component in allComponents)
    {
        if (component != null && component.GetType().Name.Contains("ModularAvatarPCSSController"))
        {
            Undo.DestroyObjectImmediate(component);
            removedCount++;
        }
    }
}

[MenuItem("Tools/lilToon-PCSS-Extension/VRChat向けクリーンアップ/全カスタムPCSSコンポーネント削除", false, 303)]
public static void RemoveAllCustomPCSSComponents()
{
    // 全カスタムPCSSコンポーネントを検索して削除
    var allGameObjects = go.GetComponentsInChildren<Transform>(true);
    foreach (var transform in allGameObjects)
    {
        var components = transform.GetComponents<Component>();
        foreach (var component in components)
        {
            if (component != null && 
                (component.GetType().Name.Contains("PCSSController") ||
                 component.GetType().Name.Contains("ModularAvatarPCSS") ||
                 component.GetType().Name.Contains("PCSSLightController") ||
                 component.GetType().Name.Contains("PhysBoneLightController") ||
                 component.GetType().Name.Contains("VRCLightVolumesIntegration")))
            {
                Undo.DestroyObjectImmediate(component);
                removedCount++;
            }
        }
    }
}
```

## 🎯 解決策の特徴

### 1. VRChat AutoFix Safe
- **カスタムランタイムコンポーネントを完全削除**
- **標準VRChatコンポーネントのみを使用**
- **AutoFixで弾かれない安全な実装**

### 2. 標準VRChatコンポーネントによる制御
- **VRCPhysBone**: エミッシブライト制御
- **VRCExpressionParameters**: パラメータ管理
- **VRCExpressionsMenu**: メニュー制御
- **AnimatorController**: アニメーション制御

### 3. 自動クリーンアップ機能
- **ModularAvatarPCSSController削除**: 特定コンポーネントの削除
- **全カスタムPCSSコンポーネント削除**: 包括的なクリーンアップ
- **Missing Scripts削除**: 壊れたコンポーネントの削除

## 📋 使用方法

### 1. カスタムコンポーネント削除
```
Tools > lilToon-PCSS-Extension > VRChat向けクリーンアップ > ModularAvatarPCSSController削除
```

### 2. 全カスタムコンポーネント削除
```
Tools > lilToon-PCSS-Extension > VRChat向けクリーンアップ > 全カスタムPCSSコンポーネント削除
```

### 3. 安全なセットアップ
```
Tools > lilToon-PCSS-Extension > ModularAvatar PCSS Setup Wizard
```

## 🛡️ VRChat AutoFix対応

### 対応済み問題
1. ✅ **ModularAvatarPCSSController削除**: カスタムランタイムコンポーネントを完全削除
2. ✅ **標準VRChatコンポーネント使用**: AutoFixで弾かれない安全な実装
3. ✅ **Missing Scripts削除**: 壊れたコンポーネントの削除
4. ✅ **自動クリーンアップ**: 包括的なクリーンアップ機能

### 使用可能な標準コンポーネント
- **VRCPhysBone**: エミッシブライト制御
- **VRCPhysBoneCollider**: コライダー設定
- **VRCExpressionParameters**: パラメータ管理
- **VRCExpressionsMenu**: メニュー制御
- **AnimatorController**: アニメーション制御

## 📊 比較表

| 機能 | 従来版 | AutoFix対応版 |
|------|--------|--------------|
| カスタムコンポーネント | 使用 | **不使用** |
| AutoFix対応 | ❌ | **✅** |
| 標準VRChatコンポーネント | 部分的 | **完全対応** |
| 自動クリーンアップ | なし | **あり** |
| 安全性 | 低 | **高** |

## 🎉 実装完了

VRChatのAutoFixでModularAvatarPCSSControllerが弾かれる問題の解決策が完了しました！

### 主な成果
1. ✅ **ModularAvatarPCSSSetupWizard**: VRChat AutoFix対応版に変更
2. ✅ **VRChatCleanupMenu**: ModularAvatarPCSSController削除機能追加
3. ✅ **全カスタムPCSSコンポーネント削除**: 包括的なクリーンアップ機能
4. ✅ **標準VRChatコンポーネント使用**: AutoFixで弾かれない安全な実装
5. ✅ **自動クリーンアップ**: Missing Scripts削除機能

これでVRChatのAutoFixで弾かれる問題が解決されたで！なんｊ風にしゃべって、カスタムランタイムコンポーネントを完全に削除して、標準VRChatコンポーネントのみでPCSS制御を実現したぜ！
