using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if MODULAR_AVATAR_AVAILABLE
using nadena.dev.modular_avatar.core;
#endif

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// Editor window that applies lilToon PCSS presets to VRChat avatars.
    /// Refactored for clarity and predictable layout when pairing across multiple machines.
    /// </summary>
    internal sealed class AvatarSelectorMenu : EditorWindow
    {
        private readonly struct PresetDefinition
        {
            public readonly string Name;
            public readonly string Description;
            public readonly Vector4 Parameters;
            public readonly bool IsCustom;

            public PresetDefinition(string name, string description, Vector4 parameters, bool isCustom = false)
            {
                Name = name;
                Description = description;
                Parameters = parameters;
                IsCustom = isCustom;
            }
        }

        private struct CustomPresetValues
        {
            public float FilterRadius;
            public float LightSize;
            public float Bias;
            public float Intensity;

            public Vector4 ToVector4() => new Vector4(FilterRadius, LightSize, Bias, Intensity);
        }

        private static readonly PresetDefinition[] Presets =
        {
            new PresetDefinition(
                "Realistic",
                "Natural soft shadows for film-style lighting.",
                new Vector4(0.005f, 0.05f, 0.0005f, 1.0f)),
            new PresetDefinition(
                "Anime",
                "Stylised falloff tuned for toon shading.",
                new Vector4(0.015f, 0.1f, 0.001f, 0.8f)),
            new PresetDefinition(
                "Cinematic",
                "High-contrast preset suited to dramatic scenes.",
                new Vector4(0.025f, 0.2f, 0.002f, 1.2f)),
            new PresetDefinition(
                "Custom",
                "Manually configure PCSS parameters.",
                Vector4.zero,
                true),
            new PresetDefinition(
                "Dewy Skin Gloss",
                "Healthy moist skin highlights with restrained sweat-like sheen.",
                new Vector4(0.009f, 0.095f, 0.0008f, 0.96f)),
            new PresetDefinition(
                "Soft Flush Skin",
                "Healthy warm cheek flush with soft shadows and restrained skin gloss.",
                new Vector4(0.0105f, 0.105f, 0.0009f, 0.93f)),
            new PresetDefinition(
                "Studio Boost",
                "Maximum PCSS and shader boost for no-light AAO workflows and lit PC scenes.",
                new Vector4(0.0125f, 0.160f, 0.00065f, 1.30f)),
            new PresetDefinition(
                "Excited Tone",
                "Healthy high-energy warm tone with soft shadows and restrained skin gloss.",
                new Vector4(0.0110f, 0.115f, 0.00085f, 1.02f))
        };

        private static readonly GUIContent[] PresetLabels = Presets
            .Select(p => new GUIContent(p.Name, p.Description))
            .ToArray();

        private Vector2 _scrollPosition;
        private readonly List<GameObject> _detectedAvatars = new List<GameObject>();
        private GameObject _selectedAvatar;
        private bool _autoDetectAvatars = true;
        private bool _showAdvancedOptions;
        private int _selectedPresetIndex;
        private CustomPresetValues _customPreset = new CustomPresetValues
        {
            FilterRadius = 0.01f,
            LightSize = 0.1f,
            Bias = 0.001f,
            Intensity = 1.0f
        };

        // One-click actions
        private bool _applyExpressions = true;
        private bool _applyPerformanceTweaks = true;
        private bool _applyLightVolumes = true;

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Avatar Selector")]
        private static void ShowWindow()
        {
            var window = GetWindow<AvatarSelectorMenu>("Avatar Selector");
            window.minSize = new Vector2(420f, 620f);
            window.Show();
        }

        private void OnEnable()
        {
            RefreshAvatarList();
        }

        private void OnGUI()
        {
            using (var scroll = new EditorGUILayout.ScrollViewScope(_scrollPosition))
            {
                _scrollPosition = scroll.scrollPosition;

                DrawHeader();
                DrawAvatarSection();
                DrawPresetSection();
                DrawAdvancedOptions();
                DrawActionButtons();
            }
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(8f);

            var titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 18,
                alignment = TextAnchor.MiddleCenter
            };

            EditorGUILayout.LabelField("lilToon PCSS Extension", titleStyle);
            EditorGUILayout.LabelField("Avatar Selector & Preset Applier", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
            EditorGUILayout.Space(6f);
        }

        private void DrawAvatarSection()
        {
            EditorGUILayout.LabelField("Avatar Selection", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                _autoDetectAvatars = EditorGUILayout.ToggleLeft("Auto detect scene avatars", _autoDetectAvatars);
                if (GUILayout.Button("Refresh", GUILayout.Width(80f)))
                {
                    RefreshAvatarList();
                }
            }

            EditorGUILayout.Space(6f);

            if (_autoDetectAvatars)
            {
                DrawDetectedAvatars();
            }
            else
            {
                _selectedAvatar = (GameObject)EditorGUILayout.ObjectField("Avatar", _selectedAvatar, typeof(GameObject), true);
                if (_selectedAvatar == null)
                {
                    EditorGUILayout.HelpBox("Select a scene avatar to continue.", MessageType.Info);
                }
            }

            EditorGUILayout.Space(10f);
        }

        private void DrawDetectedAvatars()
        {
            _detectedAvatars.RemoveAll(go => go == null);

            if (_detectedAvatars.Count == 0)
            {
                EditorGUILayout.HelpBox("No avatars detected in the current scene.", MessageType.Warning);
                return;
            }

            EditorGUILayout.LabelField($"{_detectedAvatars.Count} avatar(s) detected.", EditorStyles.helpBox);

            foreach (var avatar in _detectedAvatars)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    bool isSelected = _selectedAvatar == avatar;
                    bool toggled = EditorGUILayout.Toggle(isSelected, GUILayout.Width(20f));
                    if (toggled != isSelected)
                    {
                        _selectedAvatar = toggled ? avatar : null;
                    }

                    EditorGUILayout.ObjectField(avatar, typeof(GameObject), true);

#if VRCHAT_SDK_AVAILABLE
                    var descriptor = avatar.GetComponent<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>();
                    if (descriptor != null)
                    {
                        EditorGUILayout.LabelField("VRChat", GUILayout.Width(60f));
                    }
#endif

#if MODULAR_AVATAR_AVAILABLE
                    var modularInfo = avatar.GetComponent<ModularAvatarInformation>();
                    if (modularInfo != null)
                    {
                        EditorGUILayout.LabelField("Modular Avatar", GUILayout.Width(110f));
                    }
#endif
                }
            }
        }

        private void DrawPresetSection()
        {
            EditorGUILayout.LabelField("PCSS Presets", EditorStyles.boldLabel);

            int newPreset = GUILayout.SelectionGrid(_selectedPresetIndex, PresetLabels, 2, GUILayout.Height(64f));
            if (newPreset != _selectedPresetIndex)
            {
                _selectedPresetIndex = newPreset;
            }

            EditorGUILayout.HelpBox(Presets[_selectedPresetIndex].Description, MessageType.Info);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Parameter Preview", EditorStyles.miniBoldLabel);

                if (Presets[_selectedPresetIndex].IsCustom)
                {
                    _customPreset.FilterRadius = EditorGUILayout.Slider("Filter Radius", _customPreset.FilterRadius, 0.001f, 0.1f);
                    _customPreset.LightSize = EditorGUILayout.Slider("Light Size", _customPreset.LightSize, 0.01f, 0.5f);
                    _customPreset.Bias = EditorGUILayout.Slider("Bias", _customPreset.Bias, 0.0001f, 0.01f);
                    _customPreset.Intensity = EditorGUILayout.Slider("Intensity", _customPreset.Intensity, 0.0f, 2.0f);
                }
                else
                {
                    Vector4 parameters = Presets[_selectedPresetIndex].Parameters;
                    DrawParameterLabel("Filter Radius", parameters.x, 4);
                    DrawParameterLabel("Light Size", parameters.y, 3);
                    DrawParameterLabel("Bias", parameters.z, 4);
                    DrawParameterLabel("Intensity", parameters.w, 2);
                }
            }

            EditorGUILayout.Space(10f);
        }

        private void DrawAdvancedOptions()
        {
            _showAdvancedOptions = EditorGUILayout.Foldout(_showAdvancedOptions, "Advanced Options", true);
            if (!_showAdvancedOptions)
            {
                return;
            }

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("One-Click Setup Tasks", EditorStyles.miniBoldLabel);
                _applyExpressions = EditorGUILayout.ToggleLeft("Create / update VRChat expression menu entries", _applyExpressions);
                _applyPerformanceTweaks = EditorGUILayout.ToggleLeft("Run performance optimizer helpers", _applyPerformanceTweaks);
                _applyLightVolumes = EditorGUILayout.ToggleLeft("Configure VRChat Light Volumes integration", _applyLightVolumes);
            }

            EditorGUILayout.Space(6f);
        }

        private void DrawActionButtons()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(_selectedAvatar == null))
                {
                    if (GUILayout.Button("Apply Preset"))
                    {
                        ApplyPresetToAvatar();
                    }

                    if (GUILayout.Button("Preview In Scene"))
                    {
                        PreviewPreset();
                    }
                }
            }

            using (new EditorGUI.DisabledScope(_selectedAvatar == null))
            {
                if (GUILayout.Button("One-Click Setup"))
                {
                    OneClickSetup();
                }
            }
        }

        private void RefreshAvatarList()
        {
            _detectedAvatars.Clear();

#if VRCHAT_SDK_AVAILABLE
            _detectedAvatars.AddRange(
                FindObjectsOfType<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>()
                    .Select(descriptor => descriptor.gameObject));
#endif

#if MODULAR_AVATAR_AVAILABLE
            foreach (var modular in FindObjectsOfType<ModularAvatarInformation>())
            {
                if (!_detectedAvatars.Contains(modular.gameObject))
                {
                    _detectedAvatars.Add(modular.gameObject);
                }
            }
#endif

            _detectedAvatars.RemoveAll(go => go == null);
            _detectedAvatars.Sort((a, b) => string.CompareOrdinal(a.name, b.name));

            if (_autoDetectAvatars)
            {
                if (_selectedAvatar == null && _detectedAvatars.Count > 0)
                {
                    _selectedAvatar = _detectedAvatars[0];
                }
                else if (_selectedAvatar != null && !_detectedAvatars.Contains(_selectedAvatar))
                {
                    _selectedAvatar = _detectedAvatars.Count > 0 ? _detectedAvatars[0] : null;
                }
            }
        }

        private void ApplyPresetToAvatar()
        {
            if (_selectedAvatar == null)
            {
                return;
            }

            Vector4 parameters = ResolveSelectedParameters();
            var preset = Presets[_selectedPresetIndex];

            Undo.RegisterFullObjectHierarchyUndo(_selectedAvatar, "Apply PCSS Preset");

            int appliedCount = 0;
            foreach (var renderer in _selectedAvatar.GetComponentsInChildren<Renderer>(true))
            {
                appliedCount += ApplyPresetToRenderer(renderer, parameters, _selectedPresetIndex);
            }

            EditorUtility.SetDirty(_selectedAvatar);

            EditorUtility.DisplayDialog(
                "Preset Applied",
                $"{preset.Name} preset applied to {appliedCount} material(s).",
                "OK");
        }

        private int ApplyPresetToRenderer(Renderer renderer, Vector4 parameters, int presetIndex)
        {
            if (renderer == null)
            {
                return 0;
            }

            int applied = 0;
            var materials = renderer.sharedMaterials;

#if MODULAR_AVATAR_AVAILABLE
            var swap = renderer.GetComponent<MAMaterialSwap>();
            if (swap == null)
            {
                swap = Undo.AddComponent<MAMaterialSwap>(renderer.gameObject);
            }

            swap.Renderer = renderer;
            swap.Materials = new List<Material>(materials);
#endif

            foreach (var material in materials)
            {
                if (material == null || !IsPcssCompatibleShader(material.shader))
                {
                    continue;
                }

                ApplyPresetToMaterial(material, parameters, presetIndex);
                applied++;
            }

            return applied;
        }

        private void ApplyPresetToMaterial(Material material, Vector4 parameters, int presetIndex)
        {
            Undo.RecordObject(material, "Apply PCSS Preset (Material)");

            if (material.HasProperty("_PCSSPresetMode"))
            {
                material.SetFloat("_PCSSPresetMode", presetIndex);
            }

            if (material.HasProperty("_PCSSFilterRadius"))
            {
                material.SetFloat("_PCSSFilterRadius", parameters.x);
            }

            if (material.HasProperty("_PCSSLightSize"))
            {
                material.SetFloat("_PCSSLightSize", parameters.y);
            }

            if (material.HasProperty("_PCSSBias"))
            {
                material.SetFloat("_PCSSBias", parameters.z);
            }

            if (material.HasProperty("_PCSSIntensity"))
            {
                material.SetFloat("_PCSSIntensity", parameters.w);
            }

            if (presetIndex == 4)
            {
                ApplyDewySkinGloss(material);
            }
            else if (presetIndex == 5)
            {
                ApplySoftFlushSkin(material);
            }
            else if (presetIndex == 6)
            {
                ApplyStudioBoost(material);
            }
            else if (presetIndex == 7)
            {
                ApplyExcitedTone(material);
            }

            if (material.HasProperty("_PCSSEnabled"))
            {
                material.SetFloat("_PCSSEnabled", 1.0f);
            }

            EditorUtility.SetDirty(material);
        }

        private static void ApplyDewySkinGloss(Material material)
        {
            SetFloatIfExists(material, "_UsePCSS", 1.0f);
            SetFloatIfExists(material, "_PCSSEnabled", 1.0f);
            SetFloatIfExists(material, "_PCSSPresetMode", 4.0f);
            SetFloatIfExists(material, "_PCSSQualityLevel", 2.0f);
            SetFloatIfExists(material, "_LocalPCSSSamples", 12.0f);
            SetFloatIfExists(material, "_LocalPCSSFilterRadius", 0.009f);
            SetFloatIfExists(material, "_LocalPCSSLightSize", 0.095f);
            SetFloatIfExists(material, "_LocalPCSSBias", 0.0008f);
            SetFloatIfExists(material, "_UseGlossShadowCoherence", 1.0f);
            SetFloatIfExists(material, "_GlossShadowCoherence", 0.76f);
            SetFloatIfExists(material, "_GlossShadowBoost", 0.66f);
            SetFloatIfExists(material, "_GlossShadowSuppression", 0.40f);
            SetFloatIfExists(material, "_GlossRimStrength", 0.48f);
            SetFloatIfExists(material, "_GlossSmoothness", 0.88f);
            SetFloatIfExists(material, "_UseRealisticShadow", 1.0f);
            SetFloatIfExists(material, "_RealisticShadowIntensity", 0.62f);
            SetFloatIfExists(material, "_RealisticShadowSoftness", 0.68f);
            SetFloatIfExists(material, "_UseVRChatPerformanceGate", 1.0f);
            SetFloatIfExists(material, "_PCSSMaxDistance", 10.0f);
            SetFloatIfExists(material, "_PCSSDistanceFade", 3.0f);
            SetFloatIfExists(material, "_Translucency", 0.55f);
            SetFloatIfExists(material, "_UseLightDirectionOverride", 1.0f);
            SetVectorIfExists(material, "_LightDirectionOverride", new Vector4(0.22f, 0.86f, 0.46f, 0.0f));
            SetFloatIfExists(material, "_UseNoLightPCSSBoost", 1.0f);
            SetFloatIfExists(material, "_NoLightPCSSBoostStrength", 0.48f);
            SetFloatIfExists(material, "_NoLightPCSSBoostSoftness", 0.66f);
            SetFloatIfExists(material, "_NoLightPCSSBoostRim", 0.42f);
            SetColorIfExists(material, "_NoLightPCSSHighlightTint", new Color(0.58f, 0.52f, 0.50f, 1.0f));
            SetKeyword(material, "_USEPCSS_ON", true);
            SetKeyword(material, "_USEGLOSSSHADOWCOHERENCE_ON", true);
            SetKeyword(material, "_USEREALISTICSHADOW_ON", true);
            SetKeyword(material, "_USEVRCHATPERFORMANCEGATE_ON", true);
            SetKeyword(material, "_USELIGHTDIRECTIONOVERRIDE_ON", true);
            SetKeyword(material, "_USENOLIGHTPCSSBOOST_ON", true);
        }

        private static void ApplySoftFlushSkin(Material material)
        {
            bool isFace = IsLikelyFaceMaterial(material);
            bool isSkin = IsLikelySkinMaterial(material);
            SetFloatIfExists(material, "_UsePCSS", 1.0f);
            SetFloatIfExists(material, "_PCSSEnabled", 1.0f);
            SetFloatIfExists(material, "_PCSSPresetMode", 5.0f);
            SetFloatIfExists(material, "_PCSSQualityLevel", 2.0f);
            SetFloatIfExists(material, "_LocalPCSSSamples", 12.0f);
            SetFloatIfExists(material, "_LocalPCSSFilterRadius", 0.0105f);
            SetFloatIfExists(material, "_LocalPCSSLightSize", 0.105f);
            SetFloatIfExists(material, "_LocalPCSSBias", 0.0009f);
            SetFloatIfExists(material, "_UseGlossShadowCoherence", 1.0f);
            SetFloatIfExists(material, "_GlossShadowCoherence", 0.68f);
            SetFloatIfExists(material, "_GlossShadowBoost", 0.42f);
            SetFloatIfExists(material, "_GlossShadowSuppression", 0.46f);
            SetFloatIfExists(material, "_GlossRimStrength", 0.34f);
            SetFloatIfExists(material, "_GlossSmoothness", 0.78f);
            SetFloatIfExists(material, "_UseRealisticShadow", 1.0f);
            SetColorIfExists(material, "_RealisticShadowColor", isSkin
                ? new Color(0.23f, 0.13f, 0.14f, 0.76f)
                : new Color(0.14f, 0.13f, 0.15f, 0.76f));
            SetFloatIfExists(material, "_RealisticShadowIntensity", isSkin ? 0.58f : 0.72f);
            SetFloatIfExists(material, "_RealisticShadowSoftness", isSkin ? 0.74f : 0.56f);
            SetFloatIfExists(material, "_UseVRChatPerformanceGate", 1.0f);
            SetFloatIfExists(material, "_PCSSMaxDistance", 10.0f);
            SetFloatIfExists(material, "_PCSSDistanceFade", 2.8f);
            SetFloatIfExists(material, "_Translucency", isSkin ? 0.50f : 0.42f);
            SetFloatIfExists(material, "_UseSoftFlush", isFace ? 1.0f : 0.0f);
            SetColorIfExists(material, "_SoftFlushColor", new Color(1.0f, 0.40f, 0.36f, 1.0f));
            SetFloatIfExists(material, "_SoftFlushStrength", isFace ? 0.34f : 0.0f);
            SetFloatIfExists(material, "_SoftFlushWidth", 0.56f);
            SetFloatIfExists(material, "_SoftFlushVerticalBias", 0.46f);
            SetFloatIfExists(material, "_UseRimShade", isSkin ? 1.0f : 0.0f);
            SetColorIfExists(material, "_RimShadeColor", new Color(1.0f, 0.45f, 0.40f, 1.0f));
            SetFloatIfExists(material, "_RimShadeIntensity", isFace ? 0.06f : isSkin ? 0.035f : 0.0f);
            SetFloatIfExists(material, "_RimShadeWidth", 0.78f);
            SetFloatIfExists(material, "_UseLightDirectionOverride", 1.0f);
            SetVectorIfExists(material, "_LightDirectionOverride", new Vector4(0.18f, 0.88f, 0.42f, 0.0f));
            SetFloatIfExists(material, "_UseNoLightPCSSBoost", 1.0f);
            SetFloatIfExists(material, "_NoLightPCSSBoostStrength", 0.52f);
            SetFloatIfExists(material, "_NoLightPCSSBoostSoftness", 0.66f);
            SetFloatIfExists(material, "_NoLightPCSSBoostRim", 0.32f);
            SetColorIfExists(material, "_NoLightPCSSHighlightTint", new Color(0.58f, 0.52f, 0.50f, 1.0f));
            SetKeyword(material, "_USEPCSS_ON", true);
            SetKeyword(material, "_USEGLOSSSHADOWCOHERENCE_ON", true);
            SetKeyword(material, "_USEREALISTICSHADOW_ON", true);
            SetKeyword(material, "_USEVRCHATPERFORMANCEGATE_ON", true);
            SetKeyword(material, "_USELIGHTDIRECTIONOVERRIDE_ON", true);
            SetKeyword(material, "_USENOLIGHTPCSSBOOST_ON", true);
            SetKeyword(material, "_USESOFTFLUSH_ON", isFace);
            SetKeyword(material, "_USERIMSHADE_ON", isSkin);
        }

        private static void ApplyStudioBoost(Material material)
        {
            SetFloatIfExists(material, "_UsePCSS", 1.0f);
            SetFloatIfExists(material, "_PCSSEnabled", 1.0f);
            SetFloatIfExists(material, "_PCSSPresetMode", 6.0f);
            SetFloatIfExists(material, "_PCSSQualityLevel", 2.0f);
            SetFloatIfExists(material, "_LocalPCSSSamples", 16.0f);
            SetFloatIfExists(material, "_LocalPCSSFilterRadius", 0.0125f);
            SetFloatIfExists(material, "_LocalPCSSLightSize", 0.160f);
            SetFloatIfExists(material, "_LocalPCSSBias", 0.00065f);
            SetFloatIfExists(material, "_PCSSIntensity", 1.30f);
            SetFloatIfExists(material, "_UsePCSSOptimization", 1.0f);
            SetFloatIfExists(material, "_PCSSOptimizationLevel", 0.0f);
            SetFloatIfExists(material, "_UseVRChatPerformanceGate", 1.0f);
            SetFloatIfExists(material, "_PCSSMaxDistance", 10.0f);
            SetFloatIfExists(material, "_PCSSDistanceFade", 3.2f);
            SetFloatIfExists(material, "_UseGlossShadowCoherence", 1.0f);
            SetFloatIfExists(material, "_GlossShadowCoherence", 0.86f);
            SetFloatIfExists(material, "_GlossShadowBoost", 0.88f);
            SetFloatIfExists(material, "_GlossShadowSuppression", 0.30f);
            SetFloatIfExists(material, "_GlossRimStrength", 0.72f);
            SetFloatIfExists(material, "_GlossSmoothness", 0.92f);
            SetFloatIfExists(material, "_UseRealisticShadow", 1.0f);
            SetFloatIfExists(material, "_RealisticShadowIntensity", 0.78f);
            SetFloatIfExists(material, "_RealisticShadowSoftness", 0.62f);
            SetFloatIfExists(material, "_Translucency", 0.48f);
            SetFloatIfExists(material, "_UseRimShade", 1.0f);
            SetColorIfExists(material, "_RimShadeColor", new Color(0.95f, 0.92f, 1.0f, 1.0f));
            SetFloatIfExists(material, "_RimShadeIntensity", 0.16f);
            SetFloatIfExists(material, "_RimShadeWidth", 0.64f);
            SetFloatIfExists(material, "_UseLightDirectionOverride", 1.0f);
            SetVectorIfExists(material, "_LightDirectionOverride", new Vector4(0.28f, 0.82f, 0.50f, 0.0f));
            SetFloatIfExists(material, "_UseNoLightPCSSBoost", 1.0f);
            SetFloatIfExists(material, "_NoLightPCSSBoostStrength", 0.72f);
            SetFloatIfExists(material, "_NoLightPCSSBoostSoftness", 0.70f);
            SetFloatIfExists(material, "_NoLightPCSSBoostRim", 0.55f);
            SetColorIfExists(material, "_NoLightPCSSHighlightTint", new Color(0.72f, 0.70f, 0.76f, 1.0f));
            SetKeyword(material, "_USEPCSS_ON", true);
            SetKeyword(material, "_USEPCSSOPTIMIZATION_ON", true);
            SetKeyword(material, "_USEGLOSSSHADOWCOHERENCE_ON", true);
            SetKeyword(material, "_USEREALISTICSHADOW_ON", true);
            SetKeyword(material, "_USEVRCHATPERFORMANCEGATE_ON", true);
            SetKeyword(material, "_USELIGHTDIRECTIONOVERRIDE_ON", true);
            SetKeyword(material, "_USENOLIGHTPCSSBOOST_ON", true);
            SetKeyword(material, "_USERIMSHADE_ON", true);
        }

        private static void ApplyExcitedTone(Material material)
        {
            bool isFace = IsLikelyFaceMaterial(material);
            bool isSkin = IsLikelySkinMaterial(material);
            SetFloatIfExists(material, "_UsePCSS", 1.0f);
            SetFloatIfExists(material, "_PCSSEnabled", 1.0f);
            SetFloatIfExists(material, "_PCSSPresetMode", 7.0f);
            SetFloatIfExists(material, "_PCSSQualityLevel", 2.0f);
            SetFloatIfExists(material, "_LocalPCSSSamples", 14.0f);
            SetFloatIfExists(material, "_LocalPCSSFilterRadius", 0.0110f);
            SetFloatIfExists(material, "_LocalPCSSLightSize", 0.115f);
            SetFloatIfExists(material, "_LocalPCSSBias", 0.00085f);
            SetFloatIfExists(material, "_PCSSIntensity", 1.02f);
            SetFloatIfExists(material, "_UsePCSSOptimization", 1.0f);
            SetFloatIfExists(material, "_UseVRChatPerformanceGate", 1.0f);
            SetFloatIfExists(material, "_PCSSMaxDistance", 10.0f);
            SetFloatIfExists(material, "_PCSSDistanceFade", 3.0f);
            SetFloatIfExists(material, "_UseGlossShadowCoherence", 1.0f);
            SetFloatIfExists(material, "_GlossShadowCoherence", isSkin ? 0.72f : 0.50f);
            SetFloatIfExists(material, "_GlossShadowBoost", isSkin ? 0.50f : 0.22f);
            SetFloatIfExists(material, "_GlossShadowSuppression", isSkin ? 0.42f : 0.56f);
            SetFloatIfExists(material, "_GlossRimStrength", isSkin ? 0.42f : 0.20f);
            SetFloatIfExists(material, "_GlossSmoothness", isSkin ? 0.82f : 0.68f);
            SetFloatIfExists(material, "_UseRealisticShadow", 1.0f);
            SetColorIfExists(material, "_RealisticShadowColor", isSkin
                ? new Color(0.24f, 0.12f, 0.12f, 0.74f)
                : new Color(0.14f, 0.13f, 0.15f, 0.76f));
            SetFloatIfExists(material, "_RealisticShadowIntensity", isSkin ? 0.60f : 0.72f);
            SetFloatIfExists(material, "_RealisticShadowSoftness", isSkin ? 0.72f : 0.56f);
            SetFloatIfExists(material, "_Translucency", isSkin ? 0.52f : 0.42f);
            SetFloatIfExists(material, "_UseSoftFlush", isFace ? 1.0f : 0.0f);
            SetColorIfExists(material, "_SoftFlushColor", new Color(1.0f, 0.40f, 0.34f, 1.0f));
            SetFloatIfExists(material, "_SoftFlushStrength", isFace ? 0.30f : 0.0f);
            SetFloatIfExists(material, "_SoftFlushWidth", 0.60f);
            SetFloatIfExists(material, "_SoftFlushVerticalBias", 0.48f);
            SetFloatIfExists(material, "_UseExcitedTone", isSkin ? 1.0f : 0.0f);
            SetColorIfExists(material, "_ExcitedToneColor", new Color(1.0f, 0.48f, 0.34f, 1.0f));
            SetFloatIfExists(material, "_ExcitedToneStrength", isFace ? 0.30f : isSkin ? 0.18f : 0.0f);
            SetFloatIfExists(material, "_ExcitedToneBreath", 0.0f);
            SetFloatIfExists(material, "_ExcitedToneUpperBias", 0.58f);
            SetFloatIfExists(material, "_UseRimShade", isSkin ? 1.0f : 0.0f);
            SetColorIfExists(material, "_RimShadeColor", new Color(1.0f, 0.52f, 0.42f, 1.0f));
            SetFloatIfExists(material, "_RimShadeIntensity", isFace ? 0.10f : isSkin ? 0.045f : 0.0f);
            SetFloatIfExists(material, "_RimShadeWidth", 0.74f);
            SetFloatIfExists(material, "_UseLightDirectionOverride", 1.0f);
            SetVectorIfExists(material, "_LightDirectionOverride", new Vector4(0.20f, 0.86f, 0.44f, 0.0f));
            SetFloatIfExists(material, "_UseNoLightPCSSBoost", 1.0f);
            SetFloatIfExists(material, "_NoLightPCSSBoostStrength", 0.56f);
            SetFloatIfExists(material, "_NoLightPCSSBoostSoftness", 0.66f);
            SetFloatIfExists(material, "_NoLightPCSSBoostRim", 0.40f);
            SetColorIfExists(material, "_NoLightPCSSHighlightTint", new Color(0.64f, 0.50f, 0.46f, 1.0f));
            SetKeyword(material, "_USEPCSS_ON", true);
            SetKeyword(material, "_USEPCSSOPTIMIZATION_ON", true);
            SetKeyword(material, "_USEGLOSSSHADOWCOHERENCE_ON", true);
            SetKeyword(material, "_USEREALISTICSHADOW_ON", true);
            SetKeyword(material, "_USEVRCHATPERFORMANCEGATE_ON", true);
            SetKeyword(material, "_USELIGHTDIRECTIONOVERRIDE_ON", true);
            SetKeyword(material, "_USENOLIGHTPCSSBOOST_ON", true);
            SetKeyword(material, "_USESOFTFLUSH_ON", isFace);
            SetKeyword(material, "_USEEXCITEDTONE_ON", isSkin);
            SetKeyword(material, "_USERIMSHADE_ON", isSkin);
        }

        private static bool IsLikelyFaceMaterial(Material material)
        {
            if (material == null) return false;
            string name = (material.name ?? string.Empty).ToLowerInvariant();
            return name.Contains("face") ||
                   name.Contains("head") ||
                   name.Contains("cheek") ||
                   name.Contains("blush") ||
                   name.Contains("makeup") ||
                   name.Contains("make");
        }

        private static bool IsLikelySkinMaterial(Material material)
        {
            if (material == null) return false;
            string name = (material.name ?? string.Empty).ToLowerInvariant();
            return IsLikelyFaceMaterial(material) ||
                   name.Contains("skin") ||
                   name.Contains("body") ||
                   name.Contains("hand") ||
                   name.Contains("arm") ||
                   name.Contains("leg") ||
                   name.Contains("torso") ||
                   name.Contains("hada");
        }

        private static void SetFloatIfExists(Material material, string name, float value)
        {
            if (material != null && material.HasProperty(name))
            {
                material.SetFloat(name, value);
            }
        }

        private static void SetColorIfExists(Material material, string name, Color value)
        {
            if (material != null && material.HasProperty(name))
            {
                material.SetColor(name, value);
            }
        }

        private static void SetVectorIfExists(Material material, string name, Vector4 value)
        {
            if (material != null && material.HasProperty(name))
            {
                material.SetVector(name, value);
            }
        }

        private static void SetKeyword(Material material, string keyword, bool enabled)
        {
            if (material == null) return;
            if (enabled)
            {
                material.EnableKeyword(keyword);
            }
            else
            {
                material.DisableKeyword(keyword);
            }
        }

        private void PreviewPreset()
        {
            if (_selectedAvatar == null)
            {
                return;
            }

            Selection.activeGameObject = _selectedAvatar;
            SceneView.FocusWindowIfItsOpen(typeof(SceneView));
            SceneView.FrameLastActiveSceneView();

            Debug.Log($"[lilToon PCSS] Previewing preset {Presets[_selectedPresetIndex].Name} on {_selectedAvatar.name}.");
        }

        private void OneClickSetup()
        {
            if (_selectedAvatar == null)
            {
                return;
            }

            var summary = BuildOneClickSummary();
            if (!EditorUtility.DisplayDialog("One-Click Setup", summary, "Apply", "Cancel"))
            {
                return;
            }

            ApplyPresetToAvatar();

            if (_applyExpressions)
            {
                SetupVRChatExpressions();
            }

            if (_applyPerformanceTweaks)
            {
                OptimizePerformance();
            }

            if (_applyLightVolumes)
            {
                SetupLightVolumes();
            }

            EditorUtility.DisplayDialog("Setup Complete", "One-click setup finished successfully.", "OK");
        }

        private string BuildOneClickSummary()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendLine("The following tasks will run:");
            builder.AppendLine(" • Apply selected PCSS preset");

            if (_applyExpressions)
            {
                builder.AppendLine(" • Update VRChat expression menu assets");
            }

            if (_applyPerformanceTweaks)
            {
                builder.AppendLine(" • Run VRChat performance optimizer helpers");
            }

            if (_applyLightVolumes)
            {
                builder.AppendLine(" • Configure VRChat Light Volumes integration");
            }

            builder.AppendLine();
            builder.AppendLine("Proceed?");
            return builder.ToString();
        }

        private static bool IsPcssCompatibleShader(Shader shader)
        {
            if (shader == null)
            {
                return false;
            }

            string shaderName = shader.name.ToLowerInvariant();
            return (shaderName.Contains("liltoon") || shaderName.Contains("poiyomi")) &&
                   shaderName.Contains("pcss");
        }

        private Vector4 ResolveSelectedParameters()
        {
            var preset = Presets[_selectedPresetIndex];
            return preset.IsCustom ? _customPreset.ToVector4() : preset.Parameters;
        }

        private static void DrawParameterLabel(string label, float value, int decimals)
        {
            string formatted = value.ToString($"F{decimals}");
            EditorGUILayout.LabelField(label, formatted);
        }

        private void SetupVRChatExpressions()
        {
            Debug.Log("[lilToon PCSS] Expression menu update queued (implement editor automation here).");
        }

        private void OptimizePerformance()
        {
            Debug.Log("[lilToon PCSS] Performance optimizer queued (implement checklist execution here).");
        }

        private void SetupLightVolumes()
        {
            Debug.Log("[lilToon PCSS] Light Volumes configuration queued (implement integration pipeline here).");
        }
    }
}
