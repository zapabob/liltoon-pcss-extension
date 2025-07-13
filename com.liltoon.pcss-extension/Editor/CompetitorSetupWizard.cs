using UnityEngine;
using UnityEditor;
#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.Components;
#endif
using System.Collections.Generic;
using System.Linq;
using nadena.dev.modular_avatar.core;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// 遶ｶ蜷郁｣ｽ蜩√・繧ｻ繝・ヨ繧｢繝・・繧呈ｨ｡蛟｣縺励√Ρ繝ｳ繧ｯ繝ｪ繝・け縺ｧ險ｭ螳壹ｒ驕ｩ逕ｨ縺吶ｋ繧ｦ繧｣繧ｶ繝ｼ繝峨・
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
                // PhysBone繧ｳ繝ｳ繝昴・繝阪Φ繝医・閾ｪ蜍墓､懷・
                if (autoDetectPhysBones)
                {
                    DetectPhysBoneComponents();
                }

                // 螟夜Κ繝ｩ繧､繝医・險ｭ螳・
                SetupExternalLight();

                // 蛻晄悄蛹門ｮ御ｺ・ヵ繝ｩ繧ｰ繧定ｨｭ螳・
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
            
            // VRCPhysBone繧ｳ繝ｳ繝昴・繝阪Φ繝医ｒ讀懃ｴ｢
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

            // 繝ｩ繧､繝医・蝓ｺ譛ｬ險ｭ螳壹ｒ驕ｩ逕ｨ
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
            
            // 繧｢繝舌ち繝ｼ縺ｮ蟄舌が繝悶ず繧ｧ繧ｯ繝医→縺励※驟咲ｽｮ
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
            // 繧ｯ繝ｪ繝ｼ繝ｳ繧｢繝・・蜃ｦ逅・
            physBoneComponents?.Clear();
        }
    }
        [MenuItem("Tools/lilToon PCSS/遶ｶ蜷井ｺ呈鋤繧ｻ繝・ヨ繧｢繝・・繧ｦ繧｣繧ｶ繝ｼ繝・)]
        public static void ShowWindow()
        {
            GetWindow<CompetitorSetupWizard>("遶ｶ蜷井ｺ呈鋤繧ｻ繝・ヨ繧｢繝・・");
        }

        void OnGUI()
        {
            GUILayout.Label("遶ｶ蜷郁｣ｽ蜩∽ｺ呈鋤繧ｻ繝・ヨ繧｢繝・・", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("縺薙・繧ｦ繧｣繧ｶ繝ｼ繝峨・縲∽ｸｻ隕√↑遶ｶ蜷郁｣ｽ蜩√・讖溯・繧貞盾閠・↓縲√Ρ繝ｳ繧ｯ繝ｪ繝・け縺ｧ繧｢繝舌ち繝ｼ縺ｫ鬮伜刀雉ｪ縺ｪ蠖ｱ險ｭ螳壹ｒ驕ｩ逕ｨ縺励∪縺吶・, MessageType.Info);
            
            EditorGUILayout.Space();

            #if VRC_SDK_VRCSDK3
            avatar = (VRCAvatarDescriptor)EditorGUILayout.ObjectField("蟇ｾ雎｡繧｢繝舌ち繝ｼ", avatar, typeof(VRCAvatarDescriptor), true);
            #else
            avatarObject = (GameObject)EditorGUILayout.ObjectField("蟇ｾ雎｡繧ｪ繝悶ず繧ｧ繧ｯ繝・, avatarObject, typeof(GameObject), true);
            #endif

            EditorGUILayout.Space();

            GUILayout.Label("繝励Μ繧ｻ繝・ヨ驕ｸ謚・, EditorStyles.centeredGreyMiniLabel);
            selectedPreset = (Preset)GUILayout.Toolbar((int)selectedPreset, new[] { "繝医ぇ繝ｼ繝ｳ", "繝ｪ繧｢繝ｫ", "繝繝ｼ繧ｯ" });

            EditorGUILayout.Space();

            useShadowMask = EditorGUILayout.Toggle(new GUIContent("繧ｷ繝｣繝峨え繝槭せ繧ｯ繧呈怏蜉ｹ縺ｫ縺吶ｋ", "繝槭ユ繝ｪ繧｢繝ｫ縺ｮ繧ｷ繝｣繝峨え繝槭せ繧ｯ讖溯・繧呈怏蜉ｹ蛹悶＠縲∝ｽｱ縺ｮ關ｽ縺｡譁ｹ繧偵ｈ繧願ｩｳ邏ｰ縺ｫ蛻ｶ蠕｡縺ｧ縺阪ｋ繧医≧縺ｫ縺励∪縺吶・), useShadowMask);

            EditorGUILayout.Space();

            useExternalLight = EditorGUILayout.Toggle(new GUIContent("螟夜Κ繝ｩ繧､繝医ｒ菴ｿ逕ｨ縺吶ｋ", "繧ｷ繝ｼ繝ｳ縺ｫ譌｢縺ｫ縺ゅｋ繝ｩ繧､繝医ｒ蛻ｶ蠕｡蟇ｾ雎｡縺ｨ縺励∪縺吶・), useExternalLight);
            if (useExternalLight)
            {
                externalLightObject = (Light)EditorGUILayout.ObjectField("蟇ｾ雎｡繝ｩ繧､繝・, externalLightObject, typeof(Light), true);
            }

            EditorGUILayout.Space();

            GUI.backgroundColor = new Color(0.5f, 1f, 0.5f);
            if (GUILayout.Button("險ｭ螳壹ｒ驕ｩ逕ｨ", GUILayout.Height(40)))
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
                EditorUtility.DisplayDialog("繧ｨ繝ｩ繝ｼ", "蟇ｾ雎｡縺ｮ繧｢繝舌ち繝ｼ縺碁∈謚槭＆繧後※縺・∪縺帙ｓ縲・, "OK");
                return;
            }
            GameObject targetObject = avatar.gameObject;
#else
            if (avatarObject == null)
            {
                EditorUtility.DisplayDialog("繧ｨ繝ｩ繝ｼ", "蟇ｾ雎｡縺ｮ繧ｪ繝悶ず繧ｧ繧ｯ繝医′驕ｸ謚槭＆繧後※縺・∪縺帙ｓ縲・, "OK");
                return;
            }
            GameObject targetObject = avatarObject;
#endif

            // 繧ｨ繝溘ャ繧ｷ繝也帥菴鍋函謌舌・蜀榊茜逕ｨ
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

            // Emission繝槭ユ繝ｪ繧｢繝ｫ閾ｪ蜍募牡繧雁ｽ薙※
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

            // PhysBoneLightController閾ｪ蜍輔い繧ｿ繝・メ繝ｻ險ｭ螳・
            var controller = sphereObj.GetComponent<PhysBoneLightController>();
            if (controller == null) controller = sphereObj.AddComponent<PhysBoneLightController>();
            controller.emissiveRenderer = rend;
            controller.baseEmission = emissionColor;
            controller.emissionStrength = emissionStrength;
            controller.smoothing = 5.0f;
            controller.enableDistanceAttenuation = true;
            controller.maxEffectDistance = 10.0f;

            // VRCPhysBone閾ｪ蜍戊ｿｽ蜉・亥ｭ伜惠縺励↑縺代ｌ縺ｰ・・
#if VRC_SDK_VRCSDK3
            if (sphereObj.GetComponent<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone>() == null)
            {
                sphereObj.AddComponent<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone>();
            }
#endif
            // 繝槭ユ繝ｪ繧｢繝ｫ險ｭ螳壹・閾ｪ蜍暮←逕ｨ
            ApplyMaterialSettings(targetObject, selectedPreset, useShadowMask);
            EditorUtility.DisplayDialog("螳御ｺ・, "PhysBone繧ｨ繝溘ャ繧ｷ繝也帥菴薙そ繝・ヨ繧｢繝・・縺悟ｮ御ｺ・＠縺ｾ縺励◆縲・, "OK");
        }

        private void ApplyMaterialSettings(GameObject target, Preset preset, bool useShadowMask)
        {
            var materials = FindMaterials(target);
            if (materials.Count == 0)
            {
                Debug.LogWarning($"[CompetitorSetupWizard] 蟇ｾ雎｡繧ｪ繝悶ず繧ｧ繧ｯ繝医鶏target.name}縲阪↓ lilToon/PCSS Extension 繧ｷ繧ｧ繝ｼ繝繝ｼ繧剃ｽｿ逕ｨ縺励※縺・ｋ繝槭ユ繝ｪ繧｢繝ｫ縺瑚ｦ九▽縺九ｊ縺ｾ縺帙ｓ縺ｧ縺励◆縲・);
                return;
            }

            var renderers = target.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                // ModularAvatar縺悟ｰ主・縺輔ｌ縺ｦ縺・ｌ縺ｰMA Material Swap/Platform Filter繧定・蜍戊ｿｽ蜉
                #if MODULAR_AVATAR_AVAILABLE
                var swap = renderer.gameObject.GetComponent<MAMaterialSwap>();
                if (swap == null) swap = renderer.gameObject.AddComponent<MAMaterialSwap>();
                swap.Renderer = renderer;
                swap.Materials = new List<Material>(renderer.sharedMaterials);
                // 繝励Μ繧ｻ繝・ヨ縺斐→縺ｫ繝槭ユ繝ｪ繧｢繝ｫ繝励Ο繝代ユ繧｣繧定ｪｿ謨ｴ
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
                // Quest/PC蛻・ｲ舌′蠢・ｦ√↑繧窺APlatformFilter繧りｿｽ蜉萓・
                // var filter = renderer.gameObject.GetComponent<MAPlatformFilter>();
                // if (filter == null) filter = renderer.gameObject.AddComponent<MAPlatformFilter>();
                // filter.Platform = MAPlatformFilter.PlatformType.Quest;
                #endif
            }

            // 譌｢蟄倥・謇句虚繝槭ユ繝ｪ繧｢繝ｫ蛻・ｊ譖ｿ縺医ｂ谿九☆・・odularAvatar譛ｪ蟆主・譎ら畑・・
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
            Debug.Log($"[CompetitorSetupWizard] {materials.Count}蛟九・繝槭ユ繝ｪ繧｢繝ｫ縺ｫ險ｭ螳壹ｒ驕ｩ逕ｨ縺励∪縺励◆縲・);
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
                        // 譌｢縺ｫ豁｣縺励＞蝣ｴ蜷医ｂ繝ｪ繧ｹ繝医い繝・・
                        needsFix = true;
                    }
                    if (needsFix && pcssShader != null)
                    {
                        mat.shader = pcssShader;
                        // 蠢・ｦ√↑繝励Ο繝代ユ繧｣繝ｻ繧ｭ繝ｼ繝ｯ繝ｼ繝峨ｒ陬懷ｮ・
                        lilToon.PCSS.Editor.LilToonPCSSExtensionInitializer.EnsureRequiredProperties(mat);
                        lilToon.PCSS.Editor.LilToonPCSSExtensionInitializer.SetupShaderKeywords(mat);
                        EditorUtility.SetDirty(mat);
                        // --- Prefab繧､繝ｳ繧ｹ繧ｿ繝ｳ繧ｹ蟇ｾ蠢・---
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
