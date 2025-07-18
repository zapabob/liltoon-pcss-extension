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
    /// ç«¶åè£½åãEã»ãEã¢ãEEãæ¨¡å£ããã¯ã³ã¯ãªãE¯ã§è¨­å®ãé©ç¨ããã¦ã£ã¶ã¼ããE
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
                // PhysBoneã³ã³ããEãã³ããEèªåæ¤åE
                if (autoDetectPhysBones)
                {
                    DetectPhysBoneComponents();
                }

                // å¤é¨ã©ã¤ããEè¨­å®E
                SetupExternalLight();

                // åæåå®äºEã©ã°ãè¨­å®E
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
            
            // VRCPhysBoneã³ã³ããEãã³ããæ¤ç´¢
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

            // ã©ã¤ããEåºæ¬è¨­å®ãé©ç¨
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
            
            // ã¢ãã¿ã¼ã®å­ãªãã¸ã§ã¯ãã¨ãã¦éç½®
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
            // ã¯ãªã¼ã³ã¢ãEEå¦çE
            physBoneComponents?.Clear();
        }
    }
        [MenuItem("Tools/lilToon PCSS/ç«¶åäºæã»ãEã¢ãEEã¦ã£ã¶ã¼ãE)]
        public static void ShowWindow()
        {
            GetWindow<CompetitorSetupWizard>("ç«¶åäºæã»ãEã¢ãEE");
        }

        void OnGUI()
        {
            GUILayout.Label("ç«¶åè£½åäºæã»ãEã¢ãEE", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("ããEã¦ã£ã¶ã¼ããEãä¸»è¦ãªç«¶åè£½åãEæ©èEãåèE«ãã¯ã³ã¯ãªãE¯ã§ã¢ãã¿ã¼ã«é«åè³ªãªå½±è¨­å®ãé©ç¨ãã¾ããE, MessageType.Info);
            
            EditorGUILayout.Space();

            #if VRC_SDK_VRCSDK3
            avatar = (VRCAvatarDescriptor)EditorGUILayout.ObjectField("å¯¾è±¡ã¢ãã¿ã¼", avatar, typeof(VRCAvatarDescriptor), true);
            #else
            avatarObject = (GameObject)EditorGUILayout.ObjectField("å¯¾è±¡ãªãã¸ã§ã¯ãE, avatarObject, typeof(GameObject), true);
            #endif

            EditorGUILayout.Space();

            GUILayout.Label("ããªã»ãEé¸æE, EditorStyles.centeredGreyMiniLabel);
            selectedPreset = (Preset)GUILayout.Toolbar((int)selectedPreset, new[] { "ãã¥ã¼ã³", "ãªã¢ã«", "ãã¼ã¯" });

            EditorGUILayout.Space();

            useShadowMask = EditorGUILayout.Toggle(new GUIContent("ã·ã£ãã¦ãã¹ã¯ãæå¹ã«ãã", "ãããªã¢ã«ã®ã·ã£ãã¦ãã¹ã¯æ©èEãæå¹åããå½±ã®è½ã¡æ¹ãããè©³ç´°ã«å¶å¾¡ã§ããããã«ãã¾ããE), useShadowMask);

            EditorGUILayout.Space();

            useExternalLight = EditorGUILayout.Toggle(new GUIContent("å¤é¨ã©ã¤ããä½¿ç¨ãã", "ã·ã¼ã³ã«æ¢ã«ããã©ã¤ããå¶å¾¡å¯¾è±¡ã¨ãã¾ããE), useExternalLight);
            if (useExternalLight)
            {
                externalLightObject = (Light)EditorGUILayout.ObjectField("å¯¾è±¡ã©ã¤ãE, externalLightObject, typeof(Light), true);
            }

            EditorGUILayout.Space();

            GUI.backgroundColor = new Color(0.5f, 1f, 0.5f);
            if (GUILayout.Button("è¨­å®ãé©ç¨", GUILayout.Height(40)))
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
                EditorUtility.DisplayDialog("ã¨ã©ã¼", "å¯¾è±¡ã®ã¢ãã¿ã¼ãé¸æããã¦ãE¾ãããE, "OK");
                return;
            }
            GameObject targetObject = avatar.gameObject;
            #else
            if (avatarObject == null)
            {
                EditorUtility.DisplayDialog("ã¨ã©ã¼", "å¯¾è±¡ã®ãªãã¸ã§ã¯ããé¸æããã¦ãE¾ãããE, "OK");
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
                    EditorUtility.DisplayDialog("ã¨ã©ã¼", "å¤é¨ã©ã¤ããè¨­å®ããã¦ãE¾ãããE, "OK");
                    return;
                }
                pcssLight = externalLightObject;

                // ã³ã³ãã­ã¼ã©ã¼ç¨ã®ãªãã¸ã§ã¯ããã¢ãã¿ã¼ç´ä¸ã«ä½æE
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
                // æ¢å­ãEã©ã¤ããæ¢ãããæ°ããä½æE
                Transform lightTransform = targetObject.transform.Find("PCSS_Light");
                if (lightTransform == null)
                {
                    GameObject lightObj = new GameObject("PCSS_Light");
                    lightTransform = lightObj.transform;
                    lightTransform.SetParent(targetObject.transform, false);
                }

                // ã©ã¤ãã³ã³ããEãã³ãè¨­å®E
                pcssLight = lightTransform.GetComponent<Light>();
                if (pcssLight == null) pcssLight = lightTransform.gameObject.AddComponent<Light>();

                // PhysBoneLightControllerè¨­å®E
                controller = lightTransform.GetComponent<PhysBoneLightController>();
                if (controller == null) controller = lightTransform.gameObject.AddComponent<PhysBoneLightController>();
                controller.externalLight = null; // å¤é¨ã©ã¤ãã¢ã¼ãã§ãªãEã¨ãæEç¤º
            }
            
            pcssLight.type = LightType.Directional;
            pcssLight.shadows = LightShadows.Soft;
            pcssLight.shadowStrength = 0.8f;
            pcssLight.shadowNormalBias = 0.05f;

            // ããªã»ãEã«å¿ããè¨­å®E
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

            // PhysBoneLightControllerè¨­å®E
            controller.Initialize();

            // ãããªã¢ã«ã®è¨­å®å¤æ´å¦çE
            ApplyMaterialSettings(targetObject, selectedPreset, useShadowMask);

            EditorUtility.DisplayDialog("æå", $"ã{targetObject.name}ãã«PCSSã©ã¤ããEè¨­å®ãé©ç¨ãã¾ãããEnããªã»ãE: {selectedPreset}", "OK");
        }

        private void ApplyMaterialSettings(GameObject target, Preset preset, bool useShadowMask)
        {
            var materials = FindMaterials(target);
            if (materials.Count == 0)
            {
                Debug.LogWarning($"[CompetitorSetupWizard] å¯¾è±¡ãªãã¸ã§ã¯ãã{target.name}ãã« lilToon/PCSS Extension ã·ã§ã¼ãã¼ãä½¿ç¨ãã¦ãEãããªã¢ã«ãè¦ã¤ããã¾ããã§ãããE);
                return;
            }

            var renderers = target.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                // ModularAvatarãå°åEããã¦ãEã°MA Material Swap/Platform FilterãèEåè¿½å 
                #if MODULAR_AVATAR_AVAILABLE
                var swap = renderer.gameObject.GetComponent<MAMaterialSwap>();
                if (swap == null) swap = renderer.gameObject.AddComponent<MAMaterialSwap>();
                swap.Renderer = renderer;
                swap.Materials = new List<Material>(renderer.sharedMaterials);
                // ããªã»ãEãã¨ã«ãããªã¢ã«ãã­ããã£ãèª¿æ´
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
                // Quest/PCåE²ãå¿E¦ãªãMAPlatformFilterãè¿½å ä¾E
                // var filter = renderer.gameObject.GetComponent<MAPlatformFilter>();
                // if (filter == null) filter = renderer.gameObject.AddComponent<MAPlatformFilter>();
                // filter.Platform = MAPlatformFilter.PlatformType.Quest;
                #endif
            }

            // æ¢å­ãEæåãããªã¢ã«åEæ¿ããæ®ãEEodularAvataræªå°åEæç¨EE
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
            Debug.Log($"[CompetitorSetupWizard] {materials.Count}åãEãããªã¢ã«ã«è¨­å®ãé©ç¨ãã¾ãããE);
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
                        // æ¢ã«æ­£ããå ´åããªã¹ãã¢ãEE
                        needsFix = true;
                    }
                    if (needsFix && pcssShader != null)
                    {
                        mat.shader = pcssShader;
                        // å¿E¦ãªãã­ããã£ã»ã­ã¼ã¯ã¼ããè£å®E
                        lilToon.PCSS.Editor.LilToonPCSSExtensionInitializer.EnsureRequiredProperties(mat);
                        lilToon.PCSS.Editor.LilToonPCSSExtensionInitializer.SetupShaderKeywords(mat);
                        EditorUtility.SetDirty(mat);
                        // --- Prefabã¤ã³ã¹ã¿ã³ã¹å¯¾å¿E---
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
