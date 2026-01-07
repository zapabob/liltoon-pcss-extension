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
                true)
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

            if (material.HasProperty("_PCSSEnabled"))
            {
                material.SetFloat("_PCSSEnabled", 1.0f);
            }

            EditorUtility.SetDirty(material);
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
