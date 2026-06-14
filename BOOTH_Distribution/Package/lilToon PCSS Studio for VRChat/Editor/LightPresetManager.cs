#define MODULAR_AVATAR_EXISTS
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
// #if MODULAR_AVATAR_EXISTS
// using nadena.dev.modular_avatar.core;
// #endif

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// ライトプリセットマネージャー
    /// ライト設定の保存・読み込み・管理を行うシステム
    /// </summary>
    public class LightPresetManager : EditorWindow
    {
        private GameObject avatarRoot;
        private List<LightPreset> presets = new List<LightPreset>();
        private Vector2 scrollPos;
        private string newPresetName = "New Preset";
        private string presetCategory = "General";
        private LightPreset selectedPreset;
        private bool showAdvancedOptions = false;

        // デフォルトプリセット
        private readonly LightPreset[] defaultPresets = new LightPreset[]
        {
            new LightPreset { name = "Warm Light", category = "Indoor", color = new Color(1.0f, 0.8f, 0.6f), intensity = 1.2f, range = 5f },
            new LightPreset { name = "Cool Light", category = "Indoor", color = new Color(0.6f, 0.8f, 1.0f), intensity = 1.0f, range = 6f },
            new LightPreset { name = "Neon Blue", category = "Special", color = new Color(0.2f, 0.8f, 1.0f), intensity = 2.0f, range = 8f },
            new LightPreset { name = "Soft White", category = "General", color = new Color(1.0f, 0.98f, 0.95f), intensity = 0.8f, range = 4f },
            new LightPreset { name = "Sunset Orange", category = "Outdoor", color = new Color(1.0f, 0.5f, 0.2f), intensity = 1.5f, range = 7f },
            new LightPreset { name = "Moonlight", category = "Outdoor", color = new Color(0.8f, 0.9f, 1.0f), intensity = 0.6f, range = 10f }
        };

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Light Preset Manager")]
        public static void ShowWindow()
        {
            var window = GetWindow<LightPresetManager>("Light Preset Manager");
            window.minSize = new Vector2(400, 600);
            window.LoadPresets();
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Light Preset Manager", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("ライト設定のプリセットを管理・適用できます。", MessageType.Info);

            EditorGUILayout.Space(10);

            // アバター設定
            avatarRoot = (GameObject)EditorGUILayout.ObjectField("Avatar Root", avatarRoot, typeof(GameObject), true);

            EditorGUILayout.Space(20);

            // プリセット作成
            EditorGUILayout.LabelField("新しいプリセット作成", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            newPresetName = EditorGUILayout.TextField("Preset Name", newPresetName);
            presetCategory = EditorGUILayout.TextField("Category", presetCategory, GUILayout.Width(100));

            if (GUILayout.Button("Create from Current", GUILayout.Width(120)))
            {
                CreatePresetFromCurrent();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // デフォルトプリセット追加
            if (GUILayout.Button("Add Default Presets"))
            {
                AddDefaultPresets();
            }

            EditorGUILayout.Space(20);

            // プリセットリスト
            EditorGUILayout.LabelField("プリセット一覧", EditorStyles.boldLabel);

            var categories = presets.Select(p => p.category).Distinct().OrderBy(c => c);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));

            foreach (var category in categories)
            {
                EditorGUILayout.LabelField(category, EditorStyles.boldLabel);

                foreach (var preset in presets.Where(p => p.category == category))
                {
                    EditorGUILayout.BeginHorizontal();

                    GUI.color = selectedPreset == preset ? Color.cyan : Color.white;
                    if (GUILayout.Button(preset.name, EditorStyles.toolbarButton))
                    {
                        selectedPreset = preset;
                    }
                    GUI.color = Color.white;

                    if (GUILayout.Button("Apply", EditorStyles.miniButton, GUILayout.Width(50)))
                    {
                        ApplyPreset(preset);
                    }

                    if (GUILayout.Button("Update", EditorStyles.miniButton, GUILayout.Width(50)))
                    {
                        UpdatePresetFromCurrent(preset);
                    }

                    if (GUILayout.Button("Delete", EditorStyles.miniButton, GUILayout.Width(50)))
                    {
                        if (EditorUtility.DisplayDialog("Delete Preset",
                            $"Delete preset '{preset.name}'?", "Delete", "Cancel"))
                        {
                            presets.Remove(preset);
                            SavePresets();
                            if (selectedPreset == preset) selectedPreset = null;
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space(5);
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(10);

            // 選択されたプリセットの詳細
            if (selectedPreset != null)
            {
                EditorGUILayout.LabelField("プリセット詳細", EditorStyles.boldLabel);

                EditorGUI.BeginChangeCheck();

                selectedPreset.name = EditorGUILayout.TextField("Name", selectedPreset.name);
                selectedPreset.category = EditorGUILayout.TextField("Category", selectedPreset.category);
                selectedPreset.color = EditorGUILayout.ColorField("Color", selectedPreset.color);
                selectedPreset.intensity = EditorGUILayout.Slider("Intensity", selectedPreset.intensity, 0f, 5f);
                selectedPreset.range = EditorGUILayout.FloatField("Range", selectedPreset.range);
                selectedPreset.spotAngle = EditorGUILayout.Slider("Spot Angle", selectedPreset.spotAngle, 1f, 179f);
                selectedPreset.temperature = EditorGUILayout.Slider("Temperature", selectedPreset.temperature, 1500f, 20000f);

                showAdvancedOptions = EditorGUILayout.Foldout(showAdvancedOptions, "Advanced Options");
                if (showAdvancedOptions)
                {
                    EditorGUI.indentLevel++;
                    selectedPreset.shadowStrength = EditorGUILayout.Slider("Shadow Strength", selectedPreset.shadowStrength, 0f, 1f);
                    selectedPreset.bounceIntensity = EditorGUILayout.Slider("Bounce Intensity", selectedPreset.bounceIntensity, 0f, 8f);
                    selectedPreset.indirectMultiplier = EditorGUILayout.Slider("Indirect Multiplier", selectedPreset.indirectMultiplier, 0f, 5f);
                    EditorGUI.indentLevel--;
                }

                if (EditorGUI.EndChangeCheck())
                {
                    SavePresets();
                }

                EditorGUILayout.Space(10);

                // アクションボタン
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Apply to All Lights"))
                {
                    ApplyPresetToAllLights(selectedPreset);
                }

                if (GUILayout.Button("Copy Settings"))
                {
                    CopyPresetToClipboard(selectedPreset);
                }

                if (GUILayout.Button("Export"))
                {
                    ExportPreset(selectedPreset);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(10);

            // バルク操作
            EditorGUILayout.LabelField("バルク操作", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Apply Random Preset"))
            {
                if (presets.Count > 0)
                {
                    var randomPreset = presets[Random.Range(0, presets.Count)];
                    ApplyPreset(randomPreset);
                }
            }

            if (GUILayout.Button("Save All"))
            {
                SavePresets();
                EditorUtility.DisplayDialog("Save Complete", "All presets saved successfully!", "OK");
            }

            if (GUILayout.Button("Load All"))
            {
                LoadPresets();
                EditorUtility.DisplayDialog("Load Complete", "All presets loaded successfully!", "OK");
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Export All Presets"))
            {
                ExportAllPresets();
            }

            if (GUILayout.Button("Import Presets"))
            {
                ImportPresets();
            }
        }

        private void CreatePresetFromCurrent()
        {
            if (avatarRoot == null || string.IsNullOrEmpty(newPresetName)) return;

            var firstLight = avatarRoot.GetComponentInChildren<Light>(true);
            if (firstLight == null)
            {
                EditorUtility.DisplayDialog("Error", "No lights found in avatar!", "OK");
                return;
            }

            var preset = new LightPreset
            {
                name = newPresetName,
                category = presetCategory,
                color = firstLight.color,
                intensity = firstLight.intensity,
                range = firstLight.range,
                spotAngle = firstLight.spotAngle,
                temperature = firstLight.colorTemperature,
                shadowStrength = firstLight.shadowStrength,
                bounceIntensity = firstLight.bounceIntensity,
                indirectMultiplier = firstLight.bounceIntensity
            };

            presets.Add(preset);
            SavePresets();

            newPresetName = "New Preset";
            Debug.Log($"Created preset: {preset.name}");
        }

        private void UpdatePresetFromCurrent(LightPreset preset)
        {
            if (avatarRoot == null) return;

            var firstLight = avatarRoot.GetComponentInChildren<Light>(true);
            if (firstLight != null)
            {
                preset.color = firstLight.color;
                preset.intensity = firstLight.intensity;
                preset.range = firstLight.range;
                preset.spotAngle = firstLight.spotAngle;
                preset.temperature = firstLight.colorTemperature;
                preset.shadowStrength = firstLight.shadowStrength;
                preset.bounceIntensity = firstLight.bounceIntensity;
                preset.indirectMultiplier = firstLight.bounceIntensity;

                SavePresets();
                Debug.Log($"Updated preset: {preset.name}");
            }
        }

        private void ApplyPreset(LightPreset preset)
        {
            if (avatarRoot == null) return;

            var lights = avatarRoot.GetComponentsInChildren<Light>(true);

            foreach (var light in lights)
            {
                Undo.RecordObject(light, "Apply Light Preset");

                light.color = preset.color;
                light.intensity = preset.intensity;
                light.range = preset.range;
                light.spotAngle = preset.spotAngle;
                light.colorTemperature = preset.temperature;
                light.shadowStrength = preset.shadowStrength;
                light.bounceIntensity = preset.bounceIntensity;
            }

            EditorUtility.SetDirty(avatarRoot);
            Debug.Log($"Applied preset: {preset.name} to {lights.Length} lights");
        }

        private void ApplyPresetToAllLights(LightPreset preset)
        {
            ApplyPreset(preset);
            EditorUtility.DisplayDialog("Success",
                $"Preset '{preset.name}' applied to all lights in avatar!", "OK");
        }

        private void AddDefaultPresets()
        {
            int added = 0;
            foreach (var defaultPreset in defaultPresets)
            {
                if (!presets.Any(p => p.name == defaultPreset.name))
                {
                    presets.Add(defaultPreset.Clone());
                    added++;
                }
            }

            if (added > 0)
            {
                SavePresets();
                Debug.Log($"Added {added} default presets");
            }
        }

        private void SavePresets()
        {
            var saveData = new LightPresetSaveData
            {
                presets = presets.ToArray()
            };

            string json = JsonUtility.ToJson(saveData, true);
            string savePath = "Assets/_Generated/LightPresets.json";

            EnsureDirectoryExists(savePath);
            File.WriteAllText(savePath, json);
            AssetDatabase.Refresh();

            Debug.Log($"Saved {presets.Count} presets to {savePath}");
        }

        private void LoadPresets()
        {
            string loadPath = "Assets/_Generated/LightPresets.json";

            if (File.Exists(loadPath))
            {
                string json = File.ReadAllText(loadPath);
                var saveData = JsonUtility.FromJson<LightPresetSaveData>(json);

                if (saveData != null && saveData.presets != null)
                {
                    presets = saveData.presets.ToList();
                    Debug.Log($"Loaded {presets.Count} presets from {loadPath}");
                }
            }
        }

        private void ExportPreset(LightPreset preset)
        {
            string json = JsonUtility.ToJson(preset, true);
            string exportPath = EditorUtility.SaveFilePanel("Export Light Preset",
                "", $"{preset.name}_preset.json", "json");

            if (!string.IsNullOrEmpty(exportPath))
            {
                File.WriteAllText(exportPath, json);
                EditorUtility.DisplayDialog("Export Complete",
                    $"Preset exported to:\n{exportPath}", "OK");
            }
        }

        private void ExportAllPresets()
        {
            var saveData = new LightPresetSaveData { presets = presets.ToArray() };
            string json = JsonUtility.ToJson(saveData, true);

            string exportPath = EditorUtility.SaveFilePanel("Export All Light Presets",
                "", "light_presets.json", "json");

            if (!string.IsNullOrEmpty(exportPath))
            {
                File.WriteAllText(exportPath, json);
                EditorUtility.DisplayDialog("Export Complete",
                    $"All presets exported to:\n{exportPath}", "OK");
            }
        }

        private void ImportPresets()
        {
            string importPath = EditorUtility.OpenFilePanel("Import Light Presets",
                "", "json");

            if (!string.IsNullOrEmpty(importPath) && File.Exists(importPath))
            {
                string json = File.ReadAllText(importPath);
                var saveData = JsonUtility.FromJson<LightPresetSaveData>(json);

                if (saveData != null && saveData.presets != null)
                {
                    int imported = 0;
                    foreach (var preset in saveData.presets)
                    {
                        if (!presets.Any(p => p.name == preset.name))
                        {
                            presets.Add(preset.Clone());
                            imported++;
                        }
                    }

                    SavePresets();
                    EditorUtility.DisplayDialog("Import Complete",
                        $"Imported {imported} new presets from:\n{importPath}", "OK");
                }
            }
        }

        private void CopyPresetToClipboard(LightPreset preset)
        {
            string json = JsonUtility.ToJson(preset, true);
            EditorGUIUtility.systemCopyBuffer = json;
            EditorUtility.DisplayDialog("Copied",
                $"Preset '{preset.name}' copied to clipboard!", "OK");
        }

        private void EnsureDirectoryExists(string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private void OnEnable()
        {
            LoadPresets();
        }

        private void OnDisable()
        {
            SavePresets();
        }
    }

    [System.Serializable]
    public class LightPreset
    {
        public string name = "New Preset";
        public string category = "General";
        public Color color = Color.white;
        public float intensity = 1.0f;
        public float range = 5.0f;
        public float spotAngle = 30.0f;
        public float temperature = 6500.0f;
        public float shadowStrength = 1.0f;
        public float bounceIntensity = 1.0f;
        public float indirectMultiplier = 1.0f;

        public LightPreset Clone()
        {
            return new LightPreset
            {
                name = this.name,
                category = this.category,
                color = this.color,
                intensity = this.intensity,
                range = this.range,
                spotAngle = this.spotAngle,
                temperature = this.temperature,
                shadowStrength = this.shadowStrength,
                bounceIntensity = this.bounceIntensity,
                indirectMultiplier = this.indirectMultiplier
            };
        }
    }

    [System.Serializable]
    public class LightPresetSaveData
    {
        public LightPreset[] presets;
    }
}
