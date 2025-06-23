using UnityEngine;
using UnityEditor;
#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.Components;
#endif
using System.Collections.Generic;
using System.Linq;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// 競合製品のセットアップを模倣し、ワンクリックで設定を適用するウィザード。
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

        [MenuItem("Tools/lilToon PCSS/競合互換セットアップウィザード")]
        public static void ShowWindow()
        {
            GetWindow<CompetitorSetupWizard>("競合互換セットアップ");
        }

        void OnGUI()
        {
            GUILayout.Label("競合製品互換セットアップ", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("このウィザードは、主要な競合製品の機能を参考に、ワンクリックでアバターに高品質な影設定を適用します。", MessageType.Info);
            
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

                // ライトコンポーネント設定
                pcssLight = lightTransform.GetComponent<Light>();
                if (pcssLight == null) pcssLight = lightTransform.gameObject.AddComponent<Light>();

                // PhysBoneLightController設定
                controller = lightTransform.GetComponent<PhysBoneLightController>();
                if (controller == null) controller = lightTransform.gameObject.AddComponent<PhysBoneLightController>();
                controller.externalLight = null; // 外部ライトモードでないことを明示
                controller.lightTransform = lightTransform; // 自身のTransformを明示的に設定
            }
            
            pcssLight.type = LightType.Directional;
            pcssLight.shadows = LightShadows.Soft;
            pcssLight.shadowStrength = 0.8f;
            pcssLight.shadowNormalBias = 0.05f;

            // プリセットに応じた設定
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

            // PhysBoneLightController設定
            controller.Initialize();

            // マテリアルの設定変更処理
            ApplyMaterialSettings(targetObject, selectedPreset, useShadowMask);

            EditorUtility.DisplayDialog("成功", $"「{targetObject.name}」にPCSSライトの設定を適用しました。\nプリセット: {selectedPreset}", "OK");
        }

        private void ApplyMaterialSettings(GameObject target, Preset preset, bool useShadowMask)
        {
            var materials = FindMaterials(target);
            if (materials.Count == 0)
            {
                Debug.LogWarning($"[CompetitorSetupWizard] 対象オブジェクト「{target.name}」に lilToon/PCSS Extension シェーダーを使用しているマテリアルが見つかりませんでした。");
                return;
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

                if (useShadowMask)
                {
                    mat.SetFloat("_UseShadowColorMask", 1);
                }
                else
                {
                    mat.SetFloat("_UseShadowColorMask", 0);
                }
            }
            Debug.Log($"[CompetitorSetupWizard] {materials.Count}個のマテリアルに設定を適用しました。");
        }

        private List<Material> FindMaterials(GameObject target)
        {
            var renderers = target.GetComponentsInChildren<Renderer>(true);
            var materials = new List<Material>();
            foreach (var renderer in renderers)
            {
                foreach (var mat in renderer.sharedMaterials)
                {
                    if (mat != null && mat.shader.name == "lilToon/PCSS Extension")
                    {
                        if (!materials.Contains(mat))
                        {
                            materials.Add(mat);
                        }
                    }
                }
            }
            return materials;
        }
    }
} 