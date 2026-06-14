using UnityEditor;
using UnityEngine;
using lilToon;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// VRC Light Volumes 2.0.0 Rim Light設定用カスタムエディタ
    /// </summary>
    public class VRCLVRimLightGUI : EditorWindow
    {
        private Material targetMaterial;
        private bool useVRCLightVolumes = false;
        private float envRimBorder = 0.85f;
        private float envRimBlur = 0.35f;
        private float vrcLightVolumeIntensity = 1.0f;
        private Color vrcLightVolumeTint = Color.white;
        private float vrcLightVolumeDistanceFactor = 0.1f;

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/VRC Light Volumes 2.0.0/Rim Light Settings")]
        public static void ShowWindow()
        {
            VRCLVRimLightGUI window = GetWindow<VRCLVRimLightGUI>("VRCLV Rim Light");
            window.minSize = new Vector2(400, 300);
        }

        private void OnEnable()
        {
            // 選択されたマテリアルを取得
            if (Selection.activeObject is Material)
            {
                targetMaterial = (Material)Selection.activeObject;
                LoadMaterialSettings();
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("VRC Light Volumes 2.0.0 Rim Light Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // マテリアル選択
            EditorGUI.BeginChangeCheck();
            targetMaterial = (Material)EditorGUILayout.ObjectField("Target Material", targetMaterial, typeof(Material), false);
            if (EditorGUI.EndChangeCheck() && targetMaterial != null)
            {
                LoadMaterialSettings();
            }

            if (targetMaterial == null)
            {
                EditorGUILayout.HelpBox("Please select a material with lilToon PCSS Extension shader.", MessageType.Info);
                return;
            }

            if (targetMaterial.shader.name != "lilToon/PCSS Extension")
            {
                EditorGUILayout.HelpBox("Selected material does not use lilToon PCSS Extension shader.", MessageType.Warning);
                return;
            }

            EditorGUILayout.Space(10);

            // VRC Light Volumes 基本設定
            EditorGUILayout.LabelField("VRC Light Volumes Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            useVRCLightVolumes = EditorGUILayout.Toggle("Use VRC Light Volumes", useVRCLightVolumes);
            if (useVRCLightVolumes)
            {
                EditorGUI.indentLevel++;
                vrcLightVolumeIntensity = EditorGUILayout.Slider("Intensity", vrcLightVolumeIntensity, 0.0f, 2.0f);
                vrcLightVolumeTint = EditorGUILayout.ColorField("Tint", vrcLightVolumeTint);
                vrcLightVolumeDistanceFactor = EditorGUILayout.Slider("Distance Factor", vrcLightVolumeDistanceFactor, 0.0f, 1.0f);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(10);

            // Rim Light設定（新機能）
            EditorGUILayout.LabelField("Rim Light Settings (2.0.0)", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUI.indentLevel++;
            envRimBorder = EditorGUILayout.Slider("Rim Border", envRimBorder, 0.0f, 1.0f);
            envRimBlur = EditorGUILayout.Slider("Rim Blur", envRimBlur, 0.0f, 1.0f);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(10);

            // プリセット設定
            EditorGUILayout.LabelField("Presets", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Default"))
            {
                SetDefaultSettings();
            }
            if (GUILayout.Button("Anime Style"))
            {
                SetAnimeStyleSettings();
            }
            if (GUILayout.Button("Realistic Style"))
            {
                SetRealisticStyleSettings();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // 適用ボタン
            if (GUILayout.Button("Apply Settings to Material"))
            {
                ApplySettingsToMaterial();
            }

            EditorGUILayout.Space(5);

            // 情報表示
            EditorGUILayout.HelpBox("VRC Light Volumes 2.0.0 features:\n• Per-pixel calculation for better performance\n• Direction-aware lighting\n• Enhanced rim light controls", MessageType.Info);
        }

        private void LoadMaterialSettings()
        {
            if (targetMaterial == null) return;

            useVRCLightVolumes = targetMaterial.GetFloat("_UseVRCLightVolumes") > 0.5f;
            vrcLightVolumeIntensity = targetMaterial.GetFloat("_VRCLightVolumeIntensity");
            vrcLightVolumeTint = targetMaterial.GetColor("_VRCLightVolumeTint");
            vrcLightVolumeDistanceFactor = targetMaterial.GetFloat("_VRCLightVolumeDistanceFactor");
            envRimBorder = targetMaterial.GetFloat("_EnvRimBorder");
            envRimBlur = targetMaterial.GetFloat("_EnvRimBlur");
        }

        private void ApplySettingsToMaterial()
        {
            if (targetMaterial == null) return;

            Undo.RecordObject(targetMaterial, "Apply VRCLV Rim Light Settings");

            targetMaterial.SetFloat("_UseVRCLightVolumes", useVRCLightVolumes ? 1.0f : 0.0f);
            targetMaterial.SetFloat("_VRCLightVolumeIntensity", vrcLightVolumeIntensity);
            targetMaterial.SetColor("_VRCLightVolumeTint", vrcLightVolumeTint);
            targetMaterial.SetFloat("_VRCLightVolumeDistanceFactor", vrcLightVolumeDistanceFactor);
            targetMaterial.SetFloat("_EnvRimBorder", envRimBorder);
            targetMaterial.SetFloat("_EnvRimBlur", envRimBlur);

            // シェーダーキーワード設定
            if (useVRCLightVolumes)
            {
                targetMaterial.EnableKeyword("_USEVRCLIGHT_VOLUMES_ON");
                targetMaterial.EnableKeyword("_USEVRCLV_RIMLIGHT_ON");
            }
            else
            {
                targetMaterial.DisableKeyword("_USEVRCLIGHT_VOLUMES_ON");
                targetMaterial.DisableKeyword("_USEVRCLV_RIMLIGHT_ON");
            }

            EditorUtility.SetDirty(targetMaterial);
            AssetDatabase.SaveAssets();

            Debug.Log($"VRCLV Rim Light settings applied to material: {targetMaterial.name}");
        }

        private void SetDefaultSettings()
        {
            useVRCLightVolumes = false;
            vrcLightVolumeIntensity = 1.0f;
            vrcLightVolumeTint = Color.white;
            vrcLightVolumeDistanceFactor = 0.1f;
            envRimBorder = 0.85f;
            envRimBlur = 0.35f;
        }

        private void SetAnimeStyleSettings()
        {
            useVRCLightVolumes = true;
            vrcLightVolumeIntensity = 1.2f;
            vrcLightVolumeTint = new Color(1.0f, 0.9f, 0.8f, 1.0f);
            vrcLightVolumeDistanceFactor = 0.05f;
            envRimBorder = 0.7f;
            envRimBlur = 0.2f;
        }

        private void SetRealisticStyleSettings()
        {
            useVRCLightVolumes = true;
            vrcLightVolumeIntensity = 0.8f;
            vrcLightVolumeTint = new Color(0.95f, 0.95f, 1.0f, 1.0f);
            vrcLightVolumeDistanceFactor = 0.15f;
            envRimBorder = 0.9f;
            envRimBlur = 0.5f;
        }
    }
} 