using UnityEngine;
using UnityEditor;
#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.Components;
#endif
using System.Collections.Generic;
using System.Linq;
#if MODULAR_AVATAR_AVAILABLE
using nadena.dev.modular_avatar.core;
#endif

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// 競合製品セットアップを模倣し、ワンクリックで設定を適用するウィザード
    /// </summary>
    public class CompetitorSetupWizard : EditorWindow
    {
        private enum Preset
        {
            Toon,
            Realistic,
            Dark
        }

#if VRC_SDK_VRCSDK3
        private VRCAvatarDescriptor avatar;
#else
        private GameObject avatarObject;
#endif
        private Preset selectedPreset = Preset.Toon;
        private bool useShadowMask = true;
        private bool useExternalLight = false;
        private Light externalLightObject;

        private const string MenuRoot = "Tools/lilToon PCSS Extension/";
        private const string CompatibilityMenu = MenuRoot + "Compatibility/";
        private const string MenuPath = CompatibilityMenu + "Competitor Setup Wizard";

        [MenuItem(MenuPath)]
        public static void ShowWindow()
        {
            GetWindow<CompetitorSetupWizard>("競合互換セットアップウィザード");
        }

        void OnGUI()
        {
            GUILayout.Label("競合製品互換セットアップ", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("このウィザードは、主要な競合製品の機能を参考にし、ワンクリックでアバターに高品質な影設定を適用します。", MessageType.Info);
            EditorGUILayout.Space();
#if VRC_SDK_VRCSDK3
            avatar = (VRCAvatarDescriptor)EditorGUILayout.ObjectField("対象アバター", avatar, typeof(VRCAvatarDescriptor), true);
#else
            avatarObject = (GameObject)EditorGUILayout.ObjectField("対象オブジェクト", avatarObject, typeof(GameObject), true);
#endif
            EditorGUILayout.Space();
            GUILayout.Label("プリセット選択", EditorStyles.centeredGreyMiniLabel);
            selectedPreset = (Preset)GUILayout.Toolbar((int)selectedPreset, new[] { "トゥーン", "リアル", "ダーク" });
            EditorGUILayout.Space();
            useShadowMask = EditorGUILayout.Toggle(new GUIContent("シャドウマスクを有効にする", "マテリアルのシャドウマスク機能を有効化し、影の落ち方をより詳細に制御できるようにします。"), useShadowMask);
            EditorGUILayout.Space();
            useExternalLight = EditorGUILayout.Toggle(new GUIContent("外部ライトを使用する", "シーンに既にあるライトを制御対象とします。"), useExternalLight);
            if (useExternalLight)
            {
                externalLightObject = (Light)EditorGUILayout.ObjectField("対象ライト", externalLightObject, typeof(Light), true);
            }
            EditorGUILayout.Space();
            GUI.backgroundColor = new Color(0.5f, 1f, 0.5f);
            if (GUILayout.Button("設定を適用", GUILayout.Height(40)))
            {
                ApplySettings();
            }
            GUI.backgroundColor = Color.white;
        }

        private void ApplySettings()
        {
#if VRC_SDK_VRCSDK3
            if (avatar == null)
            {
                EditorUtility.DisplayDialog("エラー", "対象のアバターが選択されていません。", "OK");
                return;
            }
            GameObject targetObject = avatar.gameObject;
#else
            if (avatarObject == null)
            {
                EditorUtility.DisplayDialog("エラー", "対象のオブジェクトが選択されていません。", "OK");
                return;
            }
            GameObject targetObject = avatarObject;
#endif
            // エミッシブ球体生成・再利用
            Transform sphereTransform = targetObject.transform.Find("PCSS_Emissive");
            GameObject sphereObj;
            if (sphereTransform != null)
            {
                sphereObj = sphereTransform.gameObject;
            }
            else
            {
                sphereObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphereObj.name = "PCSS_Emissive";
                sphereObj.transform.SetParent(targetObject.transform, false);
            }
            sphereObj.transform.localScale = Vector3.one * 0.15f;
            // Emissionマテリアル自動割り当て
            Renderer rend = sphereObj.GetComponent<Renderer>();
            if (rend == null) rend = sphereObj.AddComponent<MeshRenderer>();
            Material mat = new Material(Shader.Find("Standard"));
            mat.EnableKeyword("_EMISSION");
            Color emissionColor = Color.white;
            float emissionStrength = 1.0f;
            Vector3 presetPos = Vector3.zero;
            switch(selectedPreset)
            {
                case Preset.Toon:
                    emissionColor = new Color(1.0f, 0.9f, 0.6f);
                    emissionStrength = 2.0f;
                    presetPos = new Vector3(0, 1.5f, 0.2f);
                    break;
                case Preset.Realistic:
                    emissionColor = new Color(1.0f, 0.8f, 0.4f);
                    emissionStrength = 4.0f;
                    presetPos = new Vector3(0, 1.3f, 0.1f);
                    break;
                case Preset.Dark:
                    emissionColor = new Color(0.5f, 0.5f, 1.0f);
                    emissionStrength = 0.7f;
                    presetPos = new Vector3(0, 1.0f, 0.0f);
                    break;
            }
            mat.SetColor("_EmissionColor", emissionColor * emissionStrength);
            rend.sharedMaterial = mat;
            sphereObj.transform.localPosition = presetPos;
            // PhysBoneLightController自動アタッチ・設定
            var controller = sphereObj.GetComponent<PhysBoneLightController>();
            if (controller == null) controller = sphereObj.AddComponent<PhysBoneLightController>();
            controller.externalLight = externalLightObject;
            controller.lightIntensity = emissionStrength;
            controller.lightColor = emissionColor;
            controller.Initialize();
#if VRC_SDK_VRCSDK3
            if (sphereObj.GetComponent<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone>() == null)
            {
                sphereObj.AddComponent<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone>();
            }
#endif
            // マテリアル設定・自動適用
            ApplyMaterialSettings(targetObject, selectedPreset, useShadowMask);
            EditorUtility.DisplayDialog("完了", "PhysBoneエミッシブ球体セットアップが完了しました。", "OK");
        }

        private void ApplyMaterialSettings(GameObject target, Preset preset, bool useShadowMask)
        {
            var materials = FindMaterials(target);
            if (materials.Count == 0)
            {
                Debug.LogWarning($"[CompetitorSetupWizard] 対象オブジェクト「{target.name}」に lilToon/PCSS Extension シェーダーを使用しているマテリアルが見つかりませんでした。");
                return;
            }
            var renderers = target.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
#if MODULAR_AVATAR_AVAILABLE
                var swap = renderer.gameObject.GetComponent<MAMaterialSwap>();
                if (swap == null) swap = renderer.gameObject.AddComponent<MAMaterialSwap>();
                swap.Renderer = renderer;
                swap.Materials = new List<Material>(renderer.sharedMaterials);
                foreach (var mat in swap.Materials)
                {
                    mat.EnableKeyword("_USEPCSS_ON");
                    mat.EnableKeyword("_USESHADOW_ON");
                    switch (preset)
                    {
                        case Preset.Toon:
                            mat.SetFloat("_UseShadowClamp", 1);
                            mat.SetFloat("_ShadowClamp", 0.75f);
                            mat.SetFloat("_Translucency", 0.1f);
                            mat.SetFloat("_ShadowBlur", 0.05f);
                            break;
                        case Preset.Realistic:
                            mat.SetFloat("_UseShadowClamp", 0);
                            mat.SetFloat("_Translucency", 0.3f);
                            mat.SetFloat("_ShadowBlur", 0.2f);
                            break;
                        case Preset.Dark:
                            mat.SetFloat("_UseShadowClamp", 1);
                            mat.SetFloat("_ShadowClamp", 0.4f);
                            mat.SetFloat("_Translucency", 0.6f);
                            mat.SetFloat("_ShadowBlur", 0.3f);
                            break;
                    }
                    mat.SetFloat("_UseShadowColorMask", useShadowMask ? 1 : 0);
                }
#endif
            }
            foreach (var mat in materials)
            {
                mat.EnableKeyword("_USEPCSS_ON");
                mat.EnableKeyword("_USESHADOW_ON");
                switch (preset)
                {
                    case Preset.Toon:
                        mat.SetFloat("_UseShadowClamp", 1);
                        mat.SetFloat("_ShadowClamp", 0.75f);
                        mat.SetFloat("_Translucency", 0.1f);
                        mat.SetFloat("_ShadowBlur", 0.05f);
                        break;
                    case Preset.Realistic:
                        mat.SetFloat("_UseShadowClamp", 0);
                        mat.SetFloat("_Translucency", 0.3f);
                        mat.SetFloat("_ShadowBlur", 0.2f);
                        break;
                    case Preset.Dark:
                        mat.SetFloat("_UseShadowClamp", 1);
                        mat.SetFloat("_ShadowClamp", 0.4f);
                        mat.SetFloat("_Translucency", 0.6f);
                        mat.SetFloat("_ShadowBlur", 0.3f);
                        break;
                }
                mat.SetFloat("_UseShadowColorMask", useShadowMask ? 1 : 0);
            }
            Debug.Log($"[CompetitorSetupWizard] {materials.Count}個のマテリアルに設定を適用しました。");
        }

        private List<Material> FindMaterials(GameObject target)
        {
            var renderers = target.GetComponentsInChildren<Renderer>(true);
            var materials = new List<Material>();
            Shader pcssShader = Shader.Find("lilToon/PCSS Extension");
            foreach (var renderer in renderers)
            {
                foreach (var mat in renderer.sharedMaterials)
                {
                    if (mat == null) continue;
                    bool needsFix = false;
                    if (mat.shader == null)
                    {
                        needsFix = true;
                    }
                    else if (mat.shader.name != "lilToon/PCSS Extension")
                    {
                        if (mat.shader.name.Contains("lilToon"))
                            needsFix = true;
                    }
                    else if (mat.shader.name == "lilToon/PCSS Extension")
                    {
                        needsFix = true;
                    }
                    if (needsFix && pcssShader != null)
                    {
                        mat.shader = pcssShader;
                        lilToon.PCSS.Editor.LilToonPCSSExtensionInitializer.EnsureRequiredProperties(mat);
                        lilToon.PCSS.Editor.LilToonPCSSExtensionInitializer.SetupShaderKeywords(mat);
                        EditorUtility.SetDirty(mat);
                        var go = renderer.gameObject;
                        if (UnityEditor.PrefabUtility.IsPartOfPrefabInstance(go))
                        {
                            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(renderer);
                        }
                    }
                    if (!materials.Contains(mat))
                    {
                        materials.Add(mat);
                    }
                }
            }
            return materials;
        }
    }

    public class PhysBoneLightController : MonoBehaviour
    {
        [Header("Light Settings")]
        public Light externalLight;
        [Header("PhysBone Integration")]
        public bool autoDetectPhysBones = true;
        public float lightIntensity = 1.0f;
        public Color lightColor = Color.white;
        private List<Component> physBoneComponents;
        private bool isInitialized = false;
        public void Initialize()
        {
            if (isInitialized)
            {
                Debug.LogWarning("PhysBoneLightController is already initialized.");
                return;
            }
            try
            {
                if (autoDetectPhysBones)
                {
                    DetectPhysBoneComponents();
                }
                SetupExternalLight();
                isInitialized = true;
                Debug.Log("PhysBoneLightController initialized successfully.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to initialize PhysBoneLightController: {e.Message}");
            }
        }
        private void DetectPhysBoneComponents()
        {
            physBoneComponents = new List<Component>();
            var physBones = GetComponentsInChildren<Component>()
                .Where(comp => comp.GetType().Name.Contains("PhysBone"))
                .ToList();
            physBoneComponents.AddRange(physBones);
            Debug.Log($"Detected {physBoneComponents.Count} PhysBone components.");
        }
        private void SetupExternalLight()
        {
            if (externalLight == null)
            {
                Debug.LogWarning("No external light assigned. Creating default light.");
                CreateDefaultLight();
                return;
            }
            externalLight.intensity = lightIntensity;
            externalLight.color = lightColor;
            externalLight.enabled = true;
            Debug.Log("External light configured successfully.");
        }
        private void CreateDefaultLight()
        {
            GameObject lightObject = new GameObject("PhysBoneLight_Default");
            externalLight = lightObject.AddComponent<Light>();
            externalLight.type = LightType.Directional;
            externalLight.intensity = lightIntensity;
            externalLight.color = lightColor;
            externalLight.shadows = LightShadows.Soft;
            lightObject.transform.SetParent(transform);
            lightObject.transform.localPosition = Vector3.up * 2f;
            lightObject.transform.localRotation = Quaternion.Euler(45f, 0f, 0f);
        }
        public void ResetToDefaults()
        {
            isInitialized = false;
            physBoneComponents?.Clear();
            if (externalLight != null)
            {
                externalLight.intensity = 1.0f;
                externalLight.color = Color.white;
            }
        }
        private void OnDestroy()
        {
            physBoneComponents?.Clear();
        }
    }
}

