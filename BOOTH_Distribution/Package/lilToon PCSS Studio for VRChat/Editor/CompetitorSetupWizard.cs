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
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Competitor Setup Wizard")]
        public static void ShowWindow()
        {
            GetWindow<CompetitorSetupWizard>("PCSS Setup Wizard");
        }

        void OnGUI()
        {
            GUILayout.Label("PCSS Setup Wizard", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Applies PCSS lighting and material settings to the selected avatar or GameObject.", MessageType.Info);
            
            EditorGUILayout.Space();

            #if VRC_SDK_VRCSDK3
            avatar = (VRCAvatarDescriptor)EditorGUILayout.ObjectField("Avatar", avatar, typeof(VRCAvatarDescriptor), true);
            #else
            avatarObject = (GameObject)EditorGUILayout.ObjectField("Avatar Object", avatarObject, typeof(GameObject), true);
            #endif

            EditorGUILayout.Space();

            GUILayout.Label("Preset", EditorStyles.centeredGreyMiniLabel);
            selectedPreset = (Preset)GUILayout.Toolbar((int)selectedPreset, new[] { "Toon", "Realistic", "Dark" });

            EditorGUILayout.Space();

            useShadowMask = EditorGUILayout.Toggle(new GUIContent("Use Shadow Mask", "Enable shadow color masking on compatible lilToon PCSS materials."), useShadowMask);

            EditorGUILayout.Space();

            useExternalLight = EditorGUILayout.Toggle(new GUIContent("Use External Light", "Use an existing scene light instead of creating a PCSS_Light object."), useExternalLight);
            if (useExternalLight)
            {
                externalLightObject = (Light)EditorGUILayout.ObjectField("External Light", externalLightObject, typeof(Light), true);
            }

            EditorGUILayout.Space();

            GUI.backgroundColor = new Color(0.5f, 1f, 0.5f);
            if (GUILayout.Button("Apply PCSS Setup", GUILayout.Height(40)))
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
                EditorUtility.DisplayDialog("Avatar Required", "Please select an avatar before applying PCSS settings.", "OK");
                return;
            }
            GameObject targetObject = avatar.gameObject;
            #else
            if (avatarObject == null)
            {
                EditorUtility.DisplayDialog("Avatar Object Required", "Please select an avatar GameObject before applying PCSS settings.", "OK");
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
                    EditorUtility.DisplayDialog("External Light Required", "Please assign an external Light before applying PCSS settings.", "OK");
                    return;
                }
                pcssLight = externalLightObject;

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
                Transform lightTransform = targetObject.transform.Find("PCSS_Light");
                if (lightTransform == null)
                {
                    GameObject lightObj = new GameObject("PCSS_Light");
                    lightTransform = lightObj.transform;
                    lightTransform.SetParent(targetObject.transform, false);
                }

                // Configure the light component.
                pcssLight = lightTransform.GetComponent<Light>();
                if (pcssLight == null) pcssLight = lightTransform.gameObject.AddComponent<Light>();

                // Configure PhysBoneLightController.
                controller = lightTransform.GetComponent<PhysBoneLightController>();
                if (controller == null) controller = lightTransform.gameObject.AddComponent<PhysBoneLightController>();
                controller.externalLight = null; // Indicates that this is not external-light mode.
            }
            
            pcssLight.type = LightType.Directional;
            pcssLight.shadows = LightShadows.Soft;
            pcssLight.shadowStrength = 0.8f;
            pcssLight.shadowNormalBias = 0.05f;

            // Apply light values for the selected preset.
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

            // Initialize PhysBoneLightController.
            controller.Initialize();

            // Apply material settings.
            ApplyMaterialSettings(targetObject, selectedPreset, useShadowMask);

            EditorUtility.DisplayDialog(
                "Success",
                $"Applied PCSS light settings to {targetObject.name}.\nPreset: {selectedPreset}",
                "OK");
        }

        private void ApplyMaterialSettings(GameObject target, Preset preset, bool useShadowMask)
        {
            var materials = FindMaterials(target);
            if (materials.Count == 0)
            {
                Debug.LogWarning($"[CompetitorSetupWizard] No material using lilToon/PCSS Extension was found on {target.name}.");
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
                // var filter = renderer.gameObject.GetComponent<MAPlatformFilter>();
                // if (filter == null) filter = renderer.gameObject.AddComponent<MAPlatformFilter>();
                // filter.Platform = MAPlatformFilter.PlatformType.Quest;
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
            Debug.Log($"[CompetitorSetupWizard] Applied PCSS material settings to {materials.Count} material(s).");
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
} 
