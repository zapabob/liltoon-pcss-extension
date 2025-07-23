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
    /// 競合製品�EセチE��アチE�Eを模倣し、ワンクリチE��で設定を適用するウィザード、E
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
                // PhysBoneコンポ�Eネント�E自動検�E
                if (autoDetectPhysBones)
                {
                    DetectPhysBoneComponents();
                }

                // 外部ライト�E設宁E
                SetupExternalLight();

                // 初期化完亁E��ラグを設宁E
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
            
            // VRCPhysBoneコンポ�Eネントを検索
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

            // ライト�E基本設定を適用
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
            
            // アバターの子オブジェクトとして配置
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
            // クリーンアップ処理
            physBoneComponents?.Clear();
        }
    }
        [MenuItem("Tools/lilToon PCSS/競合互換セットアップウィザード")]
        public static void ShowWindow()
        {
            GetWindow<CompetitorSetupWizard>("競合互換セットアップ");
        }

        void OnGUI()
        {
            GUILayout.Label("競合製品互換セットアップウィザード", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("このウィザードで、主要な競合製品の機能を参考に、ワンクリックでアバターに高品質な影設定を適用します。", MessageType.Info);
            
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

            PhysBoneLightController controller;
            Light pcssLight;

            if (useExternalLight)
            {
                if (externalLightObject == null)
                {
                    EditorUtility.DisplayDialog("エラー", "外部ライトが設定されていません。", "OK");
                    return;
                }
                pcssLight = externalLightObject;

                // コントローラー用のオブジェクトをアバター直下に作成
                Transform existingController = targetObject.transform.Find("PCSS_Controller");
                GameObject controllerObj;
                if(existingController != null)
                {
                    controllerObj = existingController.gameObject;
                }
                else
                {
                    controllerObj = new GameObject("PCSS_Controller");
                    controllerObj.transform.SetParent(targetObject.transform, false);
                }
                
                controller = controllerObj.GetComponent<PhysBoneLightController>();
                if (controller == null) controller = controllerObj.AddComponent<PhysBoneLightController>();
                controller.externalLight = pcssLight;
            }
            else
            {
                // 既存のライトを探すか、新しく作成
                Transform lightTransform = targetObject.transform.Find("PCSS_Light");
                if (lightTransform == null)
                {
                    GameObject lightObj = new GameObject("PCSS_Light");
                    lightTransform = lightObj.transform;
                    lightTransform.SetParent(targetObject.transform, false);
                }

                // ライトコンポ�Eネント設宁E
                pcssLight = lightTransform.GetComponent<Light>();
                if (pcssLight == null) pcssLight = lightTransform.gameObject.AddComponent<Light>();

                // PhysBoneLightController設宁E
                controller = lightTransform.GetComponent<PhysBoneLightController>();
                if (controller == null) controller = lightTransform.gameObject.AddComponent<PhysBoneLightController>();
                controller.externalLight = null; // 外部ライトモードでなぁE��とを�E示
            }
            
            pcssLight.type = LightType.Directional;
            pcssLight.shadows = LightShadows.Soft;
            pcssLight.shadowStrength = 0.8f;
            pcssLight.shadowNormalBias = 0.05f;

            // プリセチE��に応じた設宁E
            switch (selectedPreset)
            {
                case Preset.Toon:
                    pcssLight.color = Color.white;
                    pcssLight.intensity = 1.2f;
                    pcssLight.shadowBias = 0.01f;
                    break;
                case Preset.Realistic:
                    pcssLight.color = new Color(1f, 0.95f, 0.84f); // Warm light
                    pcssLight.intensity = 1.5f;
                    pcssLight.shadowBias = 0.005f;
                    break;
                case Preset.Dark:
                    pcssLight.color = new Color(0.8f, 0.8f, 1f); // Cool light
                    pcssLight.intensity = 1.0f;
                    pcssLight.shadowBias = 0.02f;
                    break;
            }

            // PhysBoneLightController設宁E
            controller.Initialize();

            // マテリアルの設定変更処琁E
            ApplyMaterialSettings(targetObject, selectedPreset, useShadowMask);

            EditorUtility.DisplayDialog("成功", $"「{targetObject.name}」にPCSSライト�E設定を適用しました、EnプリセチE��: {selectedPreset}", "OK");
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
                // ModularAvatarが導入されていればMA Material Swap/Platform Filterを動的に追加
                #if MODULAR_AVATAR_AVAILABLE
                var swap = renderer.gameObject.GetComponent<MAMaterialSwap>();
                if (swap == null) swap = renderer.gameObject.AddComponent<MAMaterialSwap>();
                swap.Renderer = renderer;
                swap.Materials = new List<Material>(renderer.sharedMaterials);
                // プリセットごとにマテリアルプロパティを調整
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
                // Quest/PC分岐が必要ならMAPlatformFilterも追加
                // var filter = renderer.gameObject.GetComponent<MAPlatformFilter>();
                // if (filter == null) filter = renderer.gameObject.AddComponent<MAPlatformFilter>();
                // filter.Platform = MAPlatformFilter.PlatformType.Quest;
                #endif
            }

            // 既存の手動マテリアル置き換えも残す（ModularAvatar未導入時用）
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
                        // 既に正しい場合もリストアチE�E
                        needsFix = true;
                    }
                    if (needsFix && pcssShader != null)
                    {
                        mat.shader = pcssShader;
                        // 忁E��なプロパティ・キーワードを補宁E
                        lilToon.PCSS.Editor.LilToonPCSSExtensionInitializer.EnsureRequiredProperties(mat);
                        lilToon.PCSS.Editor.LilToonPCSSExtensionInitializer.SetupShaderKeywords(mat);
                        EditorUtility.SetDirty(mat);
                        // --- Prefabインスタンス対忁E---
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
} 
