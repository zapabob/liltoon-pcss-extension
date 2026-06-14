#define MODULAR_AVATAR_EXISTS
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
// #if MODULAR_AVATAR_EXISTS
// using nadena.dev.modular_avatar.core;
// #endif

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// リアルタイムライトプレビューシステム
    /// エディタ上でライト制御の効果を即座に確認できるシステム
    /// </summary>
    public class LightPreviewSystem : EditorWindow
    {
        private GameObject avatarRoot;
        private Dictionary<GameObject, Light> originalLightStates = new Dictionary<GameObject, Light>();
        private bool isPreviewActive = false;
        private Vector2 scrollPos;

        // プレビュー制御用変数
        private bool previewToggle = true;
        private float previewIntensity = 1.0f;
        private Color previewColor = Color.white;
        private float previewHue = 0.5f;
        private float previewSaturation = 0.8f;
        private float previewBrightness = 0.8f;

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Light Preview System")]
        public static void ShowWindow()
        {
            var window = GetWindow<LightPreviewSystem>("Light Preview");
            window.minSize = new Vector2(350, 500);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Light Preview System", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("エディタ上でライト制御の効果をリアルタイムに確認できます。", MessageType.Info);

            EditorGUILayout.Space(10);

            // アバター設定
            avatarRoot = (GameObject)EditorGUILayout.ObjectField("Avatar Root", avatarRoot, typeof(GameObject), true);

            if (avatarRoot != null)
            {
                var descriptor = avatarRoot.GetComponent<VRCAvatarDescriptor>();
                if (descriptor == null)
                {
                    EditorGUILayout.HelpBox("VRCAvatarDescriptorが見つかりません。", MessageType.Warning);
                }
            }

            EditorGUILayout.Space(20);

            // プレビューステータス
            EditorGUI.BeginDisabledGroup(avatarRoot == null);
            EditorGUILayout.BeginHorizontal();
            GUI.color = isPreviewActive ? Color.green : Color.red;
            EditorGUILayout.LabelField("Preview Status:", isPreviewActive ? "Active" : "Inactive");
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            if (GUILayout.Button(isPreviewActive ? "Stop Preview" : "Start Preview"))
            {
                TogglePreview();
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(20);

            if (isPreviewActive)
            {
                // プレビュー制御
                EditorGUILayout.LabelField("リアルタイム制御", EditorStyles.boldLabel);

                EditorGUI.BeginChangeCheck();

                previewToggle = EditorGUILayout.Toggle("Light Toggle", previewToggle);
                previewIntensity = EditorGUILayout.Slider("Intensity", previewIntensity, 0f, 3.0f);
                previewColor = EditorGUILayout.ColorField("Color", previewColor);

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("HSV Control", EditorStyles.boldLabel);
                previewHue = EditorGUILayout.Slider("Hue", previewHue, 0f, 1f);
                previewSaturation = EditorGUILayout.Slider("Saturation", previewSaturation, 0f, 1f);
                previewBrightness = EditorGUILayout.Slider("Brightness", previewBrightness, 0f, 1f);

                if (EditorGUI.EndChangeCheck())
                {
                    UpdatePreview();
                }

                EditorGUILayout.Space(10);

                // プリセットボタン
                EditorGUILayout.LabelField("クイックプリセット", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Warm"))
                {
                    ApplyPreset(Color.yellow, 1.0f);
                }

                if (GUILayout.Button("Cool"))
                {
                    ApplyPreset(new Color(0.3f, 0.8f, 1.0f), 1.2f);
                }

                if (GUILayout.Button("Neon"))
                {
                    ApplyPreset(Color.magenta, 2.0f);
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Soft"))
                {
                    ApplyPreset(new Color(1.0f, 0.95f, 0.9f), 0.8f);
                }

                if (GUILayout.Button("Bright"))
                {
                    ApplyPreset(Color.white, 3.0f);
                }

                if (GUILayout.Button("Off"))
                {
                    previewToggle = false;
                    previewIntensity = 0f;
                    UpdatePreview();
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(20);

                // ライト情報
                if (avatarRoot != null)
                {
                    var lights = avatarRoot.GetComponentsInChildren<Light>(true);
                    var modularAvatarParametersType = Type.GetType("nadena.dev.modular_avatar.core.ModularAvatarParameters, nadena.dev.modular-avatar");
                    var maLights = lights.Where(l => modularAvatarParametersType != null && l.gameObject.GetComponent(modularAvatarParametersType) != null);

                    EditorGUILayout.LabelField("ライト情報", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"Total Lights: {lights.Count()}");
                    EditorGUILayout.LabelField($"MA Controlled: {maLights.Count()}");

                    scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(150));

                    foreach (var light in lights)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(light.name, GUILayout.Width(120));
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.FloatField(light.intensity, GUILayout.Width(60));
                        EditorGUILayout.ColorField(light.color, GUILayout.Width(100));
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndScrollView();
                }
            }

            EditorGUILayout.Space(10);

            if (GUI.changed)
            {
                Repaint();
            }
        }

        private void TogglePreview()
        {
            if (isPreviewActive)
            {
                StopPreview();
            }
            else
            {
                StartPreview();
            }
        }

        private void StartPreview()
        {
            if (avatarRoot == null) return;

            // 元のライト状態を保存
            originalLightStates.Clear();
            var lights = avatarRoot.GetComponentsInChildren<Light>(true);

            foreach (var light in lights)
            {
                                 var originalLight = UnityEngine.Object.Instantiate(light);
                originalLight.name = light.name + "_original";
                originalLight.transform.SetParent(light.transform.parent);
                originalLight.gameObject.SetActive(false);
                originalLightStates[light.gameObject] = originalLight;
            }

            isPreviewActive = true;
            Debug.Log("Light preview started");
        }

        private void StopPreview()
        {
            if (avatarRoot == null) return;

            // 元の状態を復元
            var lights = avatarRoot.GetComponentsInChildren<Light>(true);

            foreach (var light in lights)
            {
                if (originalLightStates.TryGetValue(light.gameObject, out var originalLight))
                {
                    light.intensity = originalLight.intensity;
                    light.color = originalLight.color;
                    light.enabled = originalLight.enabled;
                                         UnityEngine.Object.DestroyImmediate(originalLight.gameObject);
                }
            }

            originalLightStates.Clear();
            isPreviewActive = false;
            Debug.Log("Light preview stopped");
        }

        private void UpdatePreview()
        {
            if (!isPreviewActive || avatarRoot == null) return;

            var lights = avatarRoot.GetComponentsInChildren<Light>(true);

            foreach (var light in lights)
            {
                // HSVからRGBに変換
                Color hsvColor = Color.HSVToRGB(previewHue, previewSaturation, previewBrightness);

                // トグル状態を適用
                light.enabled = previewToggle;

                // 強度を適用
                light.intensity = previewToggle ? previewIntensity : 0f;

                // 色を適用（HSV優先、ColorFieldとの同期）
                if (previewColor != hsvColor)
                {
                    // ColorFieldが変更された場合はそれを使用
                    light.color = previewColor;
                    // HSVスライダーを同期
                    Color.RGBToHSV(previewColor, out previewHue, out previewSaturation, out previewBrightness);
                }
                else
                {
                    // HSVスライダーからの変更を適用
                    light.color = hsvColor;
                }
            }

            // シーンを更新
            SceneView.RepaintAll();
            EditorApplication.QueuePlayerLoopUpdate();
        }

        private void ApplyPreset(Color color, float intensity)
        {
            previewColor = color;
            previewIntensity = intensity;
            previewToggle = true;

            // HSVに変換
            Color.RGBToHSV(color, out previewHue, out previewSaturation, out previewBrightness);

            UpdatePreview();
        }

        private void OnDestroy()
        {
            if (isPreviewActive)
            {
                StopPreview();
            }
        }

        private void OnDisable()
        {
            if (isPreviewActive)
            {
                StopPreview();
            }
        }
    }

    /// <summary>
    /// ライトプレビューのためのシーンGUI拡張
    /// </summary>
    [InitializeOnLoad]
    public static class LightPreviewSceneGUI
    {
        private static LightPreviewSystem previewWindow;

        static LightPreviewSceneGUI()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (previewWindow == null)
            {
                var windows = Resources.FindObjectsOfTypeAll<LightPreviewSystem>();
                if (windows.Length > 0)
                {
                    previewWindow = windows[0];
                }
            }

            if (previewWindow != null && previewWindow.IsPreviewActive())
            {
                DrawPreviewInfo(sceneView);
            }
        }

        private static void DrawPreviewInfo(SceneView sceneView)
        {
            Handles.BeginGUI();

            var style = new GUIStyle(GUI.skin.label)
            {
                normal = { textColor = Color.green },
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };

            var rect = new Rect(10, 10, 200, 40);
            GUI.Label(rect, "🟢 Light Preview Active", style);

            Handles.EndGUI();
        }
    }

    // LightPreviewSystemの拡張メソッド
    public static class LightPreviewSystemExtensions
    {
        public static bool IsPreviewActive(this LightPreviewSystem system)
        {
            // リフレクションでプライベートフィールドにアクセス
            var field = typeof(LightPreviewSystem).GetField("isPreviewActive",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return field != null && (bool)field.GetValue(system);
        }
    }
}
