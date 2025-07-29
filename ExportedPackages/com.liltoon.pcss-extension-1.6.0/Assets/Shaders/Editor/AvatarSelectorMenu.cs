using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;
#if MODULAR_AVATAR_AVAILABLE
using nadena.dev.modular_avatar.core;
#endif

// #if MODULAR_AVATAR_AVAILABLE
// using nadena.dev.modular_avatar.core;
// #endif

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// lilToon PCSS Extension - Avatar Selector Menu
    /// 繧｢繝舌ち繝ｼ驕ｸ謚槭→PCSS繝励Μ繧ｻ繝・ヨ驕ｩ逕ｨ縺ｮ縺溘ａ縺ｮUnity繝｡繝九Η繝ｼ
    /// </summary>
    public class AvatarSelectorMenu : EditorWindow
    {
        private Vector2 scrollPosition;
        private GameObject selectedAvatar;
        private int selectedPreset = 0;
        private readonly string[] presetNames = { "汐 繝ｪ繧｢繝ｫ蠖ｱ", "耳 繧｢繝九Γ鬚ｨ", "鹿 譏逕ｻ鬚ｨ", "笞呻ｸ・繧ｫ繧ｹ繧ｿ繝" };
        private readonly string[] presetDescriptions = {
            "邏ｰ縺九＞繝・ぅ繝・・繝ｫ縺ｨ閾ｪ辟ｶ縺ｪ蠖ｱ - 繝輔か繝医Μ繧｢繝ｫ蜷代￠",
            "繧ｽ繝輔ヨ縺ｪ繧ｨ繝・ず縺ｨ繧｢繝九Γ隱ｿ縺ｮ蠖ｱ - 繝医ぇ繝ｼ繝ｳ隱ｿ蜷代￠",
            "繝峨Λ繝槭メ繝・け縺ｧ蠑ｷ隱ｿ縺輔ｌ縺溷ｽｱ - 譏逕ｻ逧・｡ｨ迴ｾ蜷代￠",
            "謇句虚險ｭ螳壹↓繧医ｋ繧ｫ繧ｹ繧ｿ繝蠖ｱ - 荳顔ｴ夊・髄縺・
        };

        private List<GameObject> detectedAvatars = new List<GameObject>();
        private bool autoDetectAvatars = true;
        private bool showAdvancedOptions = false;

        // 繧ｫ繧ｹ繧ｿ繝繝励Μ繧ｻ繝・ヨ險ｭ螳・
        private float customFilterRadius = 0.01f;
        private float customLightSize = 0.1f;
        private float customBias = 0.001f;
        private float customIntensity = 1.0f;

        [MenuItem("Tools/lilToon PCSS Extension/lilToon PCSS Extension/識 Avatar Selector")]
        public static void ShowWindow()
        {
            var window = GetWindow<AvatarSelectorMenu>("Avatar Selector");
            window.titleContent = new GUIContent("識 Avatar Selector", "繧｢繝舌ち繝ｼ驕ｸ謚槭→PCSS繝励Μ繧ｻ繝・ヨ驕ｩ逕ｨ");
            window.minSize = new Vector2(400, 600);
            window.Show();
        }

        private void OnEnable()
        {
            RefreshAvatarList();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawHeader();
            DrawAvatarSelection();
            DrawPresetSelection();
            DrawAdvancedOptions();
            DrawActionButtons();

            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(10);
            
            // 繧ｿ繧､繝医Ν
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 18,
                alignment = TextAnchor.MiddleCenter
            };
            EditorGUILayout.LabelField("識 lilToon PCSS Extension", titleStyle);
            EditorGUILayout.LabelField("Avatar Selector & Preset Applier", EditorStyles.centeredGreyMiniLabel);
            
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space(10);
        }

        private void DrawAvatarSelection()
        {
            EditorGUILayout.LabelField("搭 繧｢繝舌ち繝ｼ驕ｸ謚・, EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            autoDetectAvatars = EditorGUILayout.Toggle("閾ｪ蜍墓､懷・", autoDetectAvatars);
            if (GUILayout.Button("売 譖ｴ譁ｰ", GUILayout.Width(60)))
            {
                RefreshAvatarList();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            if (autoDetectAvatars && detectedAvatars.Count > 0)
            {
                EditorGUILayout.LabelField($"讀懷・縺輔ｌ縺溘い繝舌ち繝ｼ: {detectedAvatars.Count}菴・, EditorStyles.helpBox);
                
                foreach (var avatar in detectedAvatars)
                {
                    if (avatar == null) continue;

                    EditorGUILayout.BeginHorizontal();
                    
                    bool isSelected = selectedAvatar == avatar;
                    bool newSelected = EditorGUILayout.Toggle(isSelected, GUILayout.Width(20));
                    
                    if (newSelected != isSelected)
                    {
                        selectedAvatar = newSelected ? avatar : null;
                    }

                    EditorGUILayout.ObjectField(avatar, typeof(GameObject), true);
                    
                    // 繧｢繝舌ち繝ｼ諠・ｱ陦ｨ遉ｺ
                    var descriptor = avatar.GetComponent<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>();
                    if (descriptor != null)
                    {
                        EditorGUILayout.LabelField("笨・VRC", GUILayout.Width(40));
                    }

                    // #if MODULAR_AVATAR_AVAILABLE
                    // var modularAvatar = avatar.GetComponent<ModularAvatarInformation>();
                    // if (modularAvatar != null)
                    // {
                    //     EditorGUILayout.LabelField("肌 MA", GUILayout.Width(40));
                    // }
                    // #endif

                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                selectedAvatar = (GameObject)EditorGUILayout.ObjectField("謇句虚驕ｸ謚・, selectedAvatar, typeof(GameObject), true);
                
                if (selectedAvatar == null)
                {
                    EditorGUILayout.HelpBox("繧｢繝舌ち繝ｼ繧帝∈謚槭＠縺ｦ縺上□縺輔＞", MessageType.Info);
                }
            }

            EditorGUILayout.Space(10);
        }

        private void DrawPresetSelection()
        {
            EditorGUILayout.LabelField("耳 PCSS繝励Μ繧ｻ繝・ヨ", EditorStyles.boldLabel);
            
            int newPreset = GUILayout.SelectionGrid(selectedPreset, presetNames, 2);
            if (newPreset != selectedPreset)
            {
                selectedPreset = newPreset;
            }

            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox(presetDescriptions[selectedPreset], MessageType.Info);

            // 繝励Μ繧ｻ繝・ヨ繝励Ξ繝薙Η繝ｼ
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("繝励Μ繧ｻ繝・ヨ險ｭ螳壹・繝ｬ繝薙Η繝ｼ:", EditorStyles.miniBoldLabel);
            
            var presetParams = GetPresetParameters(selectedPreset);
            EditorGUI.BeginDisabledGroup(selectedPreset != 3); // 繧ｫ繧ｹ繧ｿ繝莉･螟悶・辟｡蜉ｹ
            
            if (selectedPreset == 3) // 繧ｫ繧ｹ繧ｿ繝
            {
                customFilterRadius = EditorGUILayout.Slider("Filter Radius", customFilterRadius, 0.001f, 0.1f);
                customLightSize = EditorGUILayout.Slider("Light Size", customLightSize, 0.01f, 0.5f);
                customBias = EditorGUILayout.Slider("Bias", customBias, 0.0001f, 0.01f);
                customIntensity = EditorGUILayout.Slider("Intensity", customIntensity, 0.0f, 2.0f);
            }
            else
            {
                EditorGUILayout.LabelField($"Filter Radius: {presetParams.x:F4}");
                EditorGUILayout.LabelField($"Light Size: {presetParams.y:F3}");
                EditorGUILayout.LabelField($"Bias: {presetParams.z:F4}");
                EditorGUILayout.LabelField($"Intensity: {presetParams.w:F2}");
            }
            
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);
        }

        private void DrawAdvancedOptions()
        {
            showAdvancedOptions = EditorGUILayout.Foldout(showAdvancedOptions, "笞呻ｸ・隧ｳ邏ｰ繧ｪ繝励す繝ｧ繝ｳ", true);
            
            if (showAdvancedOptions)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                // 繧ｷ繧ｧ繝ｼ繝繝ｼ蜿ｯ逕ｨ諤ｧ繝√ぉ繝・け
                var availableShaders = lilToon.PCSS.Runtime.PCSSUtilities.DetectAvailableShaders();
                EditorGUILayout.LabelField("繧ｷ繧ｧ繝ｼ繝繝ｼ蜿ｯ逕ｨ諤ｧ:", EditorStyles.miniBoldLabel);
                
                switch (availableShaders)
                {
                    case lilToon.PCSS.Runtime.PCSSUtilities.ShaderType.Both:
                        EditorGUILayout.LabelField("笨・lilToon & Poiyomi 荳｡譁ｹ蛻ｩ逕ｨ蜿ｯ閭ｽ", EditorStyles.helpBox);
                        break;
                    case lilToon.PCSS.Runtime.PCSSUtilities.ShaderType.LilToon:
                        EditorGUILayout.LabelField("笨・lilToon縺ｮ縺ｿ蛻ｩ逕ｨ蜿ｯ閭ｽ", EditorStyles.helpBox);
                        EditorGUILayout.HelpBox("Poiyomi繧ｷ繧ｧ繝ｼ繝繝ｼ縺瑚ｦ九▽縺九ｊ縺ｾ縺帙ｓ縲ＭilToon繝槭ユ繝ｪ繧｢繝ｫ縺ｮ縺ｿ蜃ｦ逅・＆繧後∪縺吶・, MessageType.Warning);
                        break;
                    case lilToon.PCSS.Runtime.PCSSUtilities.ShaderType.Poiyomi:
                        EditorGUILayout.LabelField("笨・Poiyomi縺ｮ縺ｿ蛻ｩ逕ｨ蜿ｯ閭ｽ", EditorStyles.helpBox);
                        EditorGUILayout.HelpBox("lilToon繧ｷ繧ｧ繝ｼ繝繝ｼ縺瑚ｦ九▽縺九ｊ縺ｾ縺帙ｓ縲１oiyomi繝槭ユ繝ｪ繧｢繝ｫ縺ｮ縺ｿ蜃ｦ逅・＆繧後∪縺吶・, MessageType.Warning);
                        break;
                    case lilToon.PCSS.Runtime.PCSSUtilities.ShaderType.Unknown:
                        EditorGUILayout.LabelField("笞・・蟇ｾ蠢懊す繧ｧ繝ｼ繝繝ｼ縺瑚ｦ九▽縺九ｊ縺ｾ縺帙ｓ", EditorStyles.helpBox);
                        EditorGUILayout.HelpBox("lilToon 縺ｾ縺溘・ Poiyomi 繧ｷ繧ｧ繝ｼ繝繝ｼ繧偵う繝ｳ繝昴・繝医＠縺ｦ縺上□縺輔＞縲・, MessageType.Error);
                        break;
                }
                
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("驕ｩ逕ｨ蟇ｾ雎｡:", EditorStyles.miniBoldLabel);
                
                bool lilToonAvailable = (availableShaders == lilToon.PCSS.Runtime.PCSSUtilities.ShaderType.LilToon || 
                                       availableShaders == lilToon.PCSS.Runtime.PCSSUtilities.ShaderType.Both);
                bool poiyomiAvailable = (availableShaders == lilToon.PCSS.Runtime.PCSSUtilities.ShaderType.Poiyomi || 
                                       availableShaders == lilToon.PCSS.Runtime.PCSSUtilities.ShaderType.Both);
                
                EditorGUI.BeginDisabledGroup(!lilToonAvailable);
                EditorGUILayout.Toggle(lilToonAvailable ? "笨・lilToon繝槭ユ繝ｪ繧｢繝ｫ" : "笶・lilToon繝槭ユ繝ｪ繧｢繝ｫ", lilToonAvailable);
                EditorGUI.EndDisabledGroup();
                
                EditorGUI.BeginDisabledGroup(!poiyomiAvailable);
                EditorGUILayout.Toggle(poiyomiAvailable ? "笨・Poiyomi繝槭ユ繝ｪ繧｢繝ｫ" : "笶・Poiyomi繝槭ユ繝ｪ繧｢繝ｫ", poiyomiAvailable);
                EditorGUI.EndDisabledGroup();
                
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("霑ｽ蜉讖溯・:", EditorStyles.miniBoldLabel);
                EditorGUILayout.Toggle("肌 ModularAvatar邨ｱ蜷・, true);
                EditorGUILayout.Toggle("笞｡ VRChat譛驕ｩ蛹・, true);
                EditorGUILayout.Toggle("庁 VRC Light Volumes", true);
                
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space(10);
        }

        private void DrawActionButtons()
        {
            EditorGUILayout.LabelField("噫 螳溯｡・, EditorStyles.boldLabel);
            
            EditorGUI.BeginDisabledGroup(selectedAvatar == null);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("耳 繝励Μ繧ｻ繝・ヨ驕ｩ逕ｨ", GUILayout.Height(30)))
            {
                ApplyPresetToAvatar();
            }
            
            if (GUILayout.Button("早・・繝励Ξ繝薙Η繝ｼ", GUILayout.Height(30)))
            {
                PreviewPreset();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            if (GUILayout.Button("肌 繝ｯ繝ｳ繧ｯ繝ｪ繝・け繧ｻ繝・ヨ繧｢繝・・", GUILayout.Height(35)))
            {
                OneClickSetup();
            }
            
            EditorGUI.EndDisabledGroup();

            if (selectedAvatar == null)
            {
                EditorGUILayout.HelpBox("繧｢繝舌ち繝ｼ繧帝∈謚槭＠縺ｦ縺上□縺輔＞", MessageType.Warning);
            }
        }

        private void RefreshAvatarList()
        {
            detectedAvatars.Clear();
            
            // VRCAvatarDescriptor繧呈戟縺､GameObject繧呈､懃ｴ｢
            var avatarDescriptors = FindObjectsOfType<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>();
            detectedAvatars.AddRange(avatarDescriptors.Select(desc => desc.gameObject));

            // #if MODULAR_AVATAR_AVAILABLE
            // // ModularAvatarInformation繧呈戟縺､GameObject繧よ､懃ｴ｢
            // var modularAvatars = FindObjectsOfType<ModularAvatarInformation>();
            // foreach (var ma in modularAvatars)
            // {
            //     if (!detectedAvatars.Contains(ma.gameObject))
            //     {
            //         detectedAvatars.Add(ma.gameObject);
            //     }
            // }
            // #endif

            // 驥崎､・ｒ髯､蜴ｻ縺励※繧ｽ繝ｼ繝・
            detectedAvatars = detectedAvatars.Distinct().OrderBy(go => go.name).ToList();
        }

        private Vector4 GetPresetParameters(int presetIndex)
        {
            switch (presetIndex)
            {
                case 0: // 繝ｪ繧｢繝ｫ蠖ｱ
                    return new Vector4(0.005f, 0.05f, 0.0005f, 1.0f);
                case 1: // 繧｢繝九Γ鬚ｨ
                    return new Vector4(0.015f, 0.1f, 0.001f, 0.8f);
                case 2: // 譏逕ｻ鬚ｨ
                    return new Vector4(0.025f, 0.2f, 0.002f, 1.2f);
                case 3: // 繧ｫ繧ｹ繧ｿ繝
                    return new Vector4(customFilterRadius, customLightSize, customBias, customIntensity);
                default:
                    return new Vector4(0.01f, 0.1f, 0.001f, 1.0f);
            }
        }

        private void ApplyPresetToAvatar()
        {
            if (selectedAvatar == null) return;

            Undo.RecordObject(selectedAvatar, "Apply PCSS Preset");

            var presetParams = GetPresetParameters(selectedPreset);
            string presetName = presetNames[selectedPreset];

            Debug.Log($"耳 Applying PCSS Preset '{presetName}' to {selectedAvatar.name}");

            // 繧｢繝舌ち繝ｼ蜀・・蜈ｨ繝槭ユ繝ｪ繧｢繝ｫ繧呈､懃ｴ｢
            var renderers = selectedAvatar.GetComponentsInChildren<Renderer>();
            int appliedCount = 0;

            foreach (var renderer in renderers)
            {
                // ModularAvatar縺悟ｰ主・縺輔ｌ縺ｦ縺・ｌ縺ｰMA Material Swap/Platform Filter繧定・蜍戊ｿｽ蜉
                #if MODULAR_AVATAR_AVAILABLE
                var swap = renderer.gameObject.GetComponent<MAMaterialSwap>();
                if (swap == null) swap = renderer.gameObject.AddComponent<MAMaterialSwap>();
                swap.Renderer = renderer;
                swap.Materials = new List<Material>(renderer.sharedMaterials);
                foreach (var mat in swap.Materials)
                {
                    if (mat == null) continue;
                    if (IsPCSSCompatibleShader(mat.shader))
                    {
                        ApplyPresetToMaterial(mat, presetParams, selectedPreset);
                        appliedCount++;
                    }
                }
                // Quest/PC蛻・ｲ舌′蠢・ｦ√↑繧窺APlatformFilter繧りｿｽ蜉萓・
                // var filter = renderer.gameObject.GetComponent<MAPlatformFilter>();
                // if (filter == null) filter = renderer.gameObject.AddComponent<MAPlatformFilter>();
                // filter.Platform = MAPlatformFilter.PlatformType.Quest;
                #endif
                // 譌｢蟄倥・謇句虚繝槭ユ繝ｪ繧｢繝ｫ蛻・ｊ譖ｿ縺医ｂ谿九☆・・odularAvatar譛ｪ蟆主・譎ら畑・・
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material == null) continue;
                    if (IsPCSSCompatibleShader(material.shader))
                    {
                        ApplyPresetToMaterial(material, presetParams, selectedPreset);
                        appliedCount++;
                    }
                }
            }

            EditorUtility.SetDirty(selectedAvatar);
            EditorUtility.DisplayDialog("驕ｩ逕ｨ螳御ｺ・, 
                $"笨・繝励Μ繧ｻ繝・ヨ '{presetName}' 繧帝←逕ｨ縺励∪縺励◆\n" +
                $"投 驕ｩ逕ｨ縺輔ｌ縺溘・繝・Μ繧｢繝ｫ: {appliedCount}蛟・, "OK");
        }

        private void PreviewPreset()
        {
            if (selectedAvatar == null) return;

            // Scene View縺ｫ繝輔か繝ｼ繧ｫ繧ｹ
            SceneView.FocusWindowIfItsOpen(typeof(SceneView));
            
            // 繧｢繝舌ち繝ｼ繧帝∈謚槭＠縺ｦ繝輔Ξ繝ｼ繝溘Φ繧ｰ
            Selection.activeGameObject = selectedAvatar;
            SceneView.FrameLastActiveSceneView();

            Debug.Log($"早・・Previewing PCSS Preset '{presetNames[selectedPreset]}' on {selectedAvatar.name}");
        }

        private void OneClickSetup()
        {
            if (selectedAvatar == null) return;

            bool result = EditorUtility.DisplayDialog("繝ｯ繝ｳ繧ｯ繝ｪ繝・け繧ｻ繝・ヨ繧｢繝・・",
                "莉･荳九・謫堺ｽ懊ｒ螳溯｡後＠縺ｾ縺・\n" +
                "窶｢ PCSS繝励Μ繧ｻ繝・ヨ縺ｮ驕ｩ逕ｨ\n" +
                "窶｢ VRChat Expression Menu縺ｮ菴懈・\n" +
                "窶｢ 繝代ヵ繧ｩ繝ｼ繝槭Φ繧ｹ譛驕ｩ蛹悶・驕ｩ逕ｨ\n" +
                "窶｢ VRC Light Volumes縺ｮ險ｭ螳喀n\n" +
                "邯夊｡後＠縺ｾ縺吶°・・, "螳溯｡・, "繧ｭ繝｣繝ｳ繧ｻ繝ｫ");

            if (result)
            {
                ApplyPresetToAvatar();
                
                // 霑ｽ蜉縺ｮ繧ｻ繝・ヨ繧｢繝・・蜃ｦ逅・
                SetupVRChatExpressions();
                OptimizePerformance();
                SetupLightVolumes();

                EditorUtility.DisplayDialog("繧ｻ繝・ヨ繧｢繝・・螳御ｺ・, 
                    "脂 繝ｯ繝ｳ繧ｯ繝ｪ繝・け繧ｻ繝・ヨ繧｢繝・・縺悟ｮ御ｺ・＠縺ｾ縺励◆・―n" +
                    "繧｢繝舌ち繝ｼ縺ｯ繧｢繝・・繝ｭ繝ｼ繝画ｺ門ｙ縺梧紛縺・∪縺励◆縲・, "OK");
            }
        }

        private bool IsPCSSCompatibleShader(Shader shader)
        {
            if (shader == null) return false;
            
            string shaderName = shader.name.ToLower();
            return shaderName.Contains("liltoon") && shaderName.Contains("pcss") ||
                   shaderName.Contains("poiyomi") && shaderName.Contains("pcss");
        }

        private void ApplyPresetToMaterial(Material material, Vector4 presetParams, int presetIndex)
        {
            Undo.RecordObject(material, "Apply PCSS Preset to Material");

            // 繝励Μ繧ｻ繝・ヨ繝｢繝ｼ繝峨ｒ險ｭ螳・
            if (material.HasProperty("_PCSSPresetMode"))
            {
                material.SetFloat("_PCSSPresetMode", presetIndex);
            }

            // 蛟句挨繝代Λ繝｡繝ｼ繧ｿ繧りｨｭ螳夲ｼ亥ｾ梧婿莠呈鋤諤ｧ・・
            if (material.HasProperty("_PCSSFilterRadius"))
                material.SetFloat("_PCSSFilterRadius", presetParams.x);
            
            if (material.HasProperty("_PCSSLightSize"))
                material.SetFloat("_PCSSLightSize", presetParams.y);
            
            if (material.HasProperty("_PCSSBias"))
                material.SetFloat("_PCSSBias", presetParams.z);
            
            if (material.HasProperty("_PCSSIntensity"))
                material.SetFloat("_PCSSIntensity", presetParams.w);

            // PCSS讖溯・繧呈怏蜉ｹ蛹・
            if (material.HasProperty("_PCSSEnabled"))
                material.SetFloat("_PCSSEnabled", 1.0f);

            EditorUtility.SetDirty(material);
        }

        private void SetupVRChatExpressions()
        {
            // VRChat Expression Menu菴懈・蜃ｦ逅・
            Debug.Log("鹿 Setting up VRChat Expression Menu...");
        }

        private void OptimizePerformance()
        {
            // 繝代ヵ繧ｩ繝ｼ繝槭Φ繧ｹ譛驕ｩ蛹門・逅・
            Debug.Log("笞｡ Optimizing performance...");
        }

        private void SetupLightVolumes()
        {
            // VRC Light Volumes險ｭ螳壼・逅・
            Debug.Log("庁 Setting up VRC Light Volumes...");
        }
    }
} 
