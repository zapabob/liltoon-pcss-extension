using UnityEngine;
using UnityEditor;

public class OneClickSetupWizard : EditorWindow
{
    private GameObject avatarRoot;

    // Shadow Mask Settings
    private bool showShadowMaskSettings = false;
    private bool useShadowMask = false;
    private Texture2D shadowMaskTex;
    private float shadowMaskStrength = 1.0f;

    // Light Settings
    private bool showLightSettings = false;
    private float lightIntensity = 1.0f;
    private float lightRange = 10.0f;
    private float lightSpotAngle = 45.0f;
    private string lightPrefabPath = "Assets/Prefabs/PCSS_Light.prefab";

    [MenuItem("Tools/lilToon PCSS Extension/One-Click Setup Wizard")]
    public static void ShowWindow()
    {
        GetWindow<OneClickSetupWizard>("One-Click PCSS Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("One-Click PCSS Setup Wizard", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("アバターのルートオブジェクトをドラッグ＆ドロップして、セットアップを開始してください。", MessageType.Info);

        avatarRoot = (GameObject)EditorGUILayout.ObjectField("Avatar Root", avatarRoot, typeof(GameObject), true);

        EditorGUILayout.Space();

        // Shadow Mask Settings
        showShadowMaskSettings = EditorGUILayout.Foldout(showShadowMaskSettings, "Shadow Mask Settings", true);
        if (showShadowMaskSettings)
        {
            useShadowMask = EditorGUILayout.Toggle("Use Shadow Mask", useShadowMask);
            if (useShadowMask)
            {
                shadowMaskTex = (Texture2D)EditorGUILayout.ObjectField("Shadow Mask Texture", shadowMaskTex, typeof(Texture2D), false);
                shadowMaskStrength = EditorGUILayout.Slider("Shadow Mask Strength", shadowMaskStrength, 0.0f, 1.0f);
            }
        }

        EditorGUILayout.Space();

        // Light Settings
        showLightSettings = EditorGUILayout.Foldout(showLightSettings, "Light Settings", true);
        if (showLightSettings)
        {
            lightPrefabPath = EditorGUILayout.TextField("Light Prefab Path", lightPrefabPath);
            lightIntensity = EditorGUILayout.Slider("Light Intensity", lightIntensity, 0.0f, 5.0f);
            lightRange = EditorGUILayout.Slider("Light Range", lightRange, 1.0f, 50.0f);
            lightSpotAngle = EditorGUILayout.Slider("Light Spot Angle", lightSpotAngle, 1.0f, 179.0f);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Setup PCSS"))
        {
            if (ValidateAvatar())
            {
                PerformSetup();
            }
        }
    }

    private bool ValidateAvatar()
    {
        if (avatarRoot == null)
        {
            EditorUtility.DisplayDialog("エラー", "アバターのルートオブジェクトが指定されていません。", "OK");
            return false;
        }

        var animator = avatarRoot.GetComponent<Animator>();
        if (animator == null || !animator.isHuman)
        {
            EditorUtility.DisplayDialog("エラー", "有効なHumanoid Animatorコンポーネントが見つかりません。", "OK");
            return false;
        }

        // VRChat SDKとModular Avatarの必須コンポーネントチェック（リフレクション版）
        try
        {
            var vrcAvatarDescriptorType = System.Type.GetType("VRC.SDK3.Avatars.Components.VRCAvatarDescriptor, VRC.SDK3.Avatars");
            if (vrcAvatarDescriptorType == null || avatarRoot.GetComponent(vrcAvatarDescriptorType) == null)
            {
                EditorUtility.DisplayDialog("エラー", "VRCAvatarDescriptorが見つかりません。VRChat SDKが正しくインポートされているか確認してください。", "OK");
                return false;
            }

            var modularAvatarRootType = System.Type.GetType("nadena.dev.modular_avatar.core.ModularAvatarRoot, ModularAvatar.Core");
            // ModularAvatarは必須ではない場合もあるため、警告に留める
            if (modularAvatarRootType != null && avatarRoot.GetComponentInChildren(modularAvatarRootType) == null)
            {
                Debug.LogWarning("[PCSS] ModularAvatarRootが見つかりませんでした。Modular Avatarを使用しないセットアップとして続行します。");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[PCSS] 必須コンポーネントのチェック中にエラーが発生しました: {ex.Message}");
            return false;
        }

        return true;
    }

    private void PerformSetup()
    {
        var animator = avatarRoot.GetComponent<Animator>();
        var headBone = animator.GetBoneTransform(HumanBodyBones.Head);
        if (headBone == null)
        {
            EditorUtility.DisplayDialog("エラー", "Headボーンが見つかりませんでした。", "OK");
            return;
        }

        var lightPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(lightPrefabPath);
        if (lightPrefab == null)
        {
            EditorUtility.DisplayDialog("エラー", $"ライトのプレハブが見つかりません。パスを確認してください: {lightPrefabPath}", "OK");
            return;
        }

        Undo.RecordObject(avatarRoot, "Setup PCSS");

        var lightInstance = (GameObject)PrefabUtility.InstantiatePrefab(lightPrefab, headBone);
        lightInstance.transform.localPosition = Vector3.zero;
        lightInstance.transform.localRotation = Quaternion.identity;

        // Apply light settings
        Light lightComponent = lightInstance.GetComponent<Light>();
        if (lightComponent != null)
        {
            lightComponent.intensity = lightIntensity;
            lightComponent.range = lightRange;
            lightComponent.spotAngle = lightSpotAngle;
            EditorUtility.SetDirty(lightComponent);
        }

        // Apply shadow mask settings to all compatible materials
        if (useShadowMask)
        {
            var renderers = avatarRoot.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                foreach (var mat in renderer.sharedMaterials)
                {
                    if (mat != null && mat.shader != null && mat.shader.name == "lilToon/PCSS Extension")
                    {
                        mat.SetFloat("_UseShadowMask", 1.0f);
                        mat.SetTexture("_ShadowMaskTex", shadowMaskTex);
                        mat.SetFloat("_ShadowMaskStrength", shadowMaskStrength);
                        EditorUtility.SetDirty(mat);
                    }
                }
            }
        }

        if (avatarRoot.GetComponent<lilToon.PCSS.Runtime.ModularAvatarPCSSController>() == null)
        {
            avatarRoot.AddComponent<lilToon.PCSS.Runtime.ModularAvatarPCSSController>();
        }

        EditorUtility.DisplayDialog("成功", "PCSSのセットアップが完了しました！", "OK");
    }
}
