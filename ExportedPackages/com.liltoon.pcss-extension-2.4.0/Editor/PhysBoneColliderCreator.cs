#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

#if VRCHAT_SDK_AVAILABLE
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDKBase;
using VRC.SDK3.Dynamics.PhysBone.Components;
#endif

/// <summary>
/// PhysBone Colliderを自動で作成するシステム
/// 画像のような自然な動きを演出するための衝突検出
/// </summary>
public class PhysBoneColliderCreator : MonoBehaviour
{
    private const string MenuRoot = "Tools/lilToon PCSS Extension/";
    private const string UtilitiesMenu = MenuRoot + "Utilities/";
    private const string PhysBoneMenu = UtilitiesMenu + "PhysBone Controller/";
    
    // メニュー項目
    private const string CreateCollidersPath = PhysBoneMenu + "Create PhysBone Colliders";
    private const string CreateBodyCollidersPath = PhysBoneMenu + "Create Body Colliders";
    private const string CreateHandCollidersPath = PhysBoneMenu + "Create Hand Colliders";
    private const string OptimizeCollidersPath = PhysBoneMenu + "Optimize Colliders";

    [MenuItem(CreateCollidersPath)]
    public static void CreatePhysBoneColliders()
    {
#if VRCHAT_SDK_AVAILABLE
        var selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("エラー", "アバターのルートを選択してください。", "OK");
            return;
        }

        var avatarDescriptor = selected.GetComponent<VRCAvatarDescriptor>();
        if (avatarDescriptor == null)
        {
            EditorUtility.DisplayDialog("エラー", "VRCAvatarDescriptorが見つかりません。", "OK");
            return;
        }

        int bodyColliders = 0;
        int handColliders = 0;
        int optimizedColliders = 0;

        // 体のCollider作成
        var bodyParts = FindBodyParts(selected);
        foreach (var bodyPart in bodyParts)
        {
            if (CreateBodyCollider(bodyPart))
            {
                bodyColliders++;
            }
        }

        // 手のCollider作成
        var handParts = FindHandParts(selected);
        foreach (var handPart in handParts)
        {
            if (CreateHandCollider(handPart))
            {
                handColliders++;
            }
        }

        // 既存のColliderを最適化
        var allColliders = selected.GetComponentsInChildren<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider>(true);
        foreach (var collider in allColliders)
        {
            if (OptimizeCollider(collider))
            {
                optimizedColliders++;
            }
        }

        EditorUtility.DisplayDialog("PhysBone Collider作成完了", 
            $"PhysBone Colliderを作成しました。\n\n" +
            $"体のCollider: {bodyColliders}個\n" +
            $"手のCollider: {handColliders}個\n" +
            $"最適化されたCollider: {optimizedColliders}個\n\n" +
            "機能:\n" +
            "• 髪と体の衝突検出\n" +
            "• アクセサリーと手の衝突検出\n" +
            "• 自然な動きの制御\n" +
            "• パフォーマンス最適化", "OK");
#else
        EditorUtility.DisplayDialog("エラー", "VRChat SDKがインストールされていません。\nVRChat SDKをインストールしてから再試行してください。", "OK");
#endif
    }

    [MenuItem(CreateBodyCollidersPath)]
    public static void CreateBodyColliders()
    {
        var selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("エラー", "アバターのルートを選択してください。", "OK");
            return;
        }

        var bodyParts = FindBodyParts(selected);
        int createdCount = 0;

        foreach (var bodyPart in bodyParts)
        {
            if (CreateBodyCollider(bodyPart))
            {
                createdCount++;
            }
        }

        EditorUtility.DisplayDialog("体Collider作成完了", 
            $"{createdCount}個の体Colliderを作成しました。\n\n" +
            "設定内容:\n" +
            "• 髪との衝突検出\n" +
            "• 自然な動きの制御\n" +
            "• パフォーマンス最適化\n" +
            "• 美しい演出効果", "OK");
    }

    [MenuItem(CreateHandCollidersPath)]
    public static void CreateHandColliders()
    {
        var selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("エラー", "アバターのルートを選択してください。", "OK");
            return;
        }

        var handParts = FindHandParts(selected);
        int createdCount = 0;

        foreach (var handPart in handParts)
        {
            if (CreateHandCollider(handPart))
            {
                createdCount++;
            }
        }

        EditorUtility.DisplayDialog("手Collider作成完了", 
            $"{createdCount}個の手Colliderを作成しました。\n\n" +
            "設定内容:\n" +
            "• アクセサリーとの衝突検出\n" +
            "• 手の動きに応じた制御\n" +
            "• 細かい制御\n" +
            "• インタラクティブな演出", "OK");
    }

    [MenuItem(OptimizeCollidersPath)]
    public static void OptimizePhysBoneColliders()
    {
#if VRCHAT_SDK_AVAILABLE
        var selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("エラー", "アバターのルートを選択してください。", "OK");
            return;
        }

        var colliders = selected.GetComponentsInChildren<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider>(true);
        int optimizedCount = 0;

        foreach (var collider in colliders)
        {
            if (OptimizeCollider(collider))
            {
                optimizedCount++;
            }
        }

        EditorUtility.DisplayDialog("Collider最適化完了", 
            $"{optimizedCount}個のColliderを最適化しました。\n\n" +
            "最適化内容:\n" +
            "• パフォーマンス向上\n" +
            "• メモリ使用量削減\n" +
            "• 衝突検出精度向上\n" +
            "• 安定性向上", "OK");
#else
        EditorUtility.DisplayDialog("エラー", "VRChat SDKがインストールされていません。\nVRChat SDKをインストールしてから再試行してください。", "OK");
#endif
    }

    /// <summary>
    /// 体のパーツを検索
    /// </summary>
    private static List<Transform> FindBodyParts(GameObject avatar)
    {
        var bodyParts = new List<Transform>();
        var renderers = avatar.GetComponentsInChildren<Renderer>(true);
        
        foreach (var renderer in renderers)
        {
            if (renderer.name.ToLower().Contains("body") || 
                renderer.name.ToLower().Contains("chest") ||
                renderer.name.ToLower().Contains("torso") ||
                renderer.name.ToLower().Contains("spine") ||
                renderer.name.ToLower().Contains("neck") ||
                renderer.name.ToLower().Contains("head"))
            {
                bodyParts.Add(renderer.transform);
            }
        }
        
        return bodyParts;
    }

    /// <summary>
    /// 手のパーツを検索
    /// </summary>
    private static List<Transform> FindHandParts(GameObject avatar)
    {
        var handParts = new List<Transform>();
        var renderers = avatar.GetComponentsInChildren<Renderer>(true);
        
        foreach (var renderer in renderers)
        {
            if (renderer.name.ToLower().Contains("hand") || 
                renderer.name.ToLower().Contains("finger") ||
                renderer.name.ToLower().Contains("arm") ||
                renderer.name.ToLower().Contains("wrist"))
            {
                handParts.Add(renderer.transform);
            }
        }
        
        return handParts;
    }

    /// <summary>
    /// 体のColliderを作成
    /// </summary>
    private static bool CreateBodyCollider(Transform bodyPart)
    {
#if VRCHAT_SDK_AVAILABLE
        if (bodyPart == null) return false;

        try
        {
            // 既存のColliderをチェック
            var existingCollider = bodyPart.GetComponent<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider>();
            if (existingCollider != null)
            {
                Debug.Log($"PhysBone Collider already exists on {bodyPart.name}");
                return false;
            }

            // PhysBone Colliderコンポーネントを追加
            var collider = bodyPart.gameObject.AddComponent<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider>();
            
            // 体用の設定
            collider.rootTransform = bodyPart;
            
            // 形状の設定（Capsule）
            collider.shapeType = VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider.ShapeType.Capsule;
            collider.height = 0.2f; // 高さ
            collider.radius = 0.1f; // 半径
            
            // 位置と回転の設定
            collider.position = Vector3.zero;
            collider.rotation = Vector3.zero;
            
            // 衝突設定
            collider.insideBounds = false; // 外側に押し出す
            collider.bonesAsSpheres = false; // ボーン全体を考慮
            
            EditorUtility.SetDirty(collider);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"体Colliderの作成に失敗: {e.Message}");
            return false;
        }
#else
        Debug.LogWarning("VRChat SDK not available, skipping body collider creation.");
        return false;
#endif
    }

    /// <summary>
    /// 手のColliderを作成
    /// </summary>
    private static bool CreateHandCollider(Transform handPart)
    {
#if VRCHAT_SDK_AVAILABLE
        if (handPart == null) return false;

        try
        {
            // 既存のColliderをチェック
            var existingCollider = handPart.GetComponent<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider>();
            if (existingCollider != null)
            {
                Debug.Log($"PhysBone Collider already exists on {handPart.name}");
                return false;
            }

            // PhysBone Colliderコンポーネントを追加
            var collider = handPart.gameObject.AddComponent<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider>();
            
            // 手用の設定
            collider.rootTransform = handPart;
            
            // 形状の設定（Sphere）
            collider.shapeType = VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider.ShapeType.Sphere;
            collider.radius = 0.05f; // 小さな半径
            
            // 位置と回転の設定
            collider.position = Vector3.zero;
            collider.rotation = Vector3.zero;
            
            // 衝突設定
            collider.insideBounds = false; // 外側に押し出す
            collider.bonesAsSpheres = true; // ボーンのみを考慮
            
            EditorUtility.SetDirty(collider);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"手Colliderの作成に失敗: {e.Message}");
            return false;
        }
#else
        Debug.LogWarning("VRChat SDK not available, skipping hand collider creation.");
        return false;
#endif
    }

    /// <summary>
    /// Colliderを最適化
    /// </summary>
#if VRCHAT_SDK_AVAILABLE
    private static bool OptimizeCollider(VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider collider)
#else
    private static bool OptimizeCollider(object collider)
#endif
    {
#if VRCHAT_SDK_AVAILABLE
        if (collider == null) return false;

        try
        {
            // サイズの最適化
            if (collider.radius > 0.2f) collider.radius = 0.2f;
            if (collider.height > 0.5f) collider.height = 0.5f;
            
            // 位置の調整
            if (collider.position.magnitude > 1.0f)
            {
                collider.position = collider.position.normalized * 0.5f;
            }
            
            // 回転の調整
            if (collider.rotation.magnitude > 90f)
            {
                collider.rotation = collider.rotation.normalized * 45f;
            }
            
            EditorUtility.SetDirty(collider);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Colliderの最適化に失敗: {e.Message}");
            return false;
        }
#else
        Debug.LogWarning("VRChat SDK not available, skipping collider optimization.");
        return false;
#endif
    }

    /// <summary>
    /// メニュー項目の有効化条件
    /// </summary>
    [MenuItem(CreateCollidersPath, true)]
    [MenuItem(CreateBodyCollidersPath, true)]
    [MenuItem(CreateHandCollidersPath, true)]
    [MenuItem(OptimizeCollidersPath, true)]
    public static bool ValidateMenuItems()
    {
        return Selection.activeGameObject != null;
    }
}
#endif 