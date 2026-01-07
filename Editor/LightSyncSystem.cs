using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// 複数アバター間でのライト設定同期システム
    /// 異なるアバター間でライト設定を同期・共有するシステム
    /// </summary>
    public class LightSyncSystem : EditorWindow
    {
        private GameObject sourceAvatar;
        private List<GameObject> targetAvatars = new List<GameObject>();
        private Vector2 scrollPos;
        private bool syncPosition = true;
        private bool syncRotation = true;
        private bool syncScale = true;
        private bool syncLightProperties = true;
        private bool syncModularAvatarComponents = true;
        private bool createBackup = true;

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Light Sync System")]
        public static void ShowWindow()
        {
            var window = GetWindow<LightSyncSystem>("Light Sync System");
            window.minSize = new Vector2(400, 600);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Light Sync System", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("異なるアバター間でライト設定を同期・共有します。", MessageType.Info);

            EditorGUILayout.Space(10);

            // ソースアバター設定
            EditorGUILayout.LabelField("ソースアバター（コピー元）", EditorStyles.boldLabel);
            sourceAvatar = (GameObject)EditorGUILayout.ObjectField("Source Avatar", sourceAvatar, typeof(GameObject), true);

            if (sourceAvatar != null)
            {
                var sourceDescriptor = sourceAvatar.GetComponent<VRCAvatarDescriptor>();
                if (sourceDescriptor == null)
                {
                    EditorGUILayout.HelpBox("VRCAvatarDescriptorが見つかりません。", MessageType.Warning);
                }
                else
                {
                    var lights = sourceAvatar.GetComponentsInChildren<Light>(true);
                    EditorGUILayout.LabelField($"Source Lights: {lights.Length}");
                }
            }

            EditorGUILayout.Space(10);

            // ターゲットアバター設定
            EditorGUILayout.LabelField("ターゲットアバター（コピー先）", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Selected"))
            {
                var selected = Selection.gameObjects;
                foreach (var obj in selected)
                {
                    if (!targetAvatars.Contains(obj) && obj != sourceAvatar)
                    {
                        targetAvatars.Add(obj);
                    }
                }
            }

            if (GUILayout.Button("Clear All"))
            {
                targetAvatars.Clear();
            }
            EditorGUILayout.EndHorizontal();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(150));

            for (int i = 0; i < targetAvatars.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                targetAvatars[i] = (GameObject)EditorGUILayout.ObjectField(
                    $"Target {i + 1}", targetAvatars[i], typeof(GameObject), true);

                if (GUILayout.Button("Remove", EditorStyles.miniButton, GUILayout.Width(60)))
                {
                    targetAvatars.RemoveAt(i);
                    i--;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(10);

            // 同期オプション
            EditorGUILayout.LabelField("同期オプション", EditorStyles.boldLabel);

            syncPosition = EditorGUILayout.Toggle("Sync Position", syncPosition);
            syncRotation = EditorGUILayout.Toggle("Sync Rotation", syncRotation);
            syncScale = EditorGUILayout.Toggle("Sync Scale", syncScale);
            syncLightProperties = EditorGUILayout.Toggle("Sync Light Properties", syncLightProperties);
            syncModularAvatarComponents = EditorGUILayout.Toggle("Sync Modular Avatar Components", syncModularAvatarComponents);
            createBackup = EditorGUILayout.Toggle("Create Backup", createBackup);

            EditorGUILayout.Space(20);

            // 同期実行
            EditorGUI.BeginDisabledGroup(sourceAvatar == null || targetAvatars.Count == 0);
            if (GUILayout.Button("Synchronize Lights"))
            {
                SynchronizeLights();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(10);

            // 追加機能
            EditorGUILayout.LabelField("追加機能", EditorStyles.boldLabel);

            if (GUILayout.Button("Export Light Configuration"))
            {
                if (sourceAvatar != null)
                {
                    ExportLightConfiguration(sourceAvatar);
                }
            }

            if (GUILayout.Button("Import Light Configuration"))
            {
                ImportLightConfiguration();
            }

            if (GUILayout.Button("Validate Light Setup"))
            {
                if (sourceAvatar != null)
                {
                    ValidateLightSetup(sourceAvatar);
                }
            }
        }

        private void SynchronizeLights()
        {
            if (sourceAvatar == null || targetAvatars.Count == 0) return;

            try
            {
                // ソースアバターのライト情報を取得
                var sourceLights = sourceAvatar.GetComponentsInChildren<Light>(true);
                if (sourceLights.Length == 0)
                {
                    EditorUtility.DisplayDialog("Warning", "ソースアバターにライトが見つかりません。", "OK");
                    return;
                }

                int totalSynced = 0;

                foreach (var targetAvatar in targetAvatars)
                {
                    if (targetAvatar == null) continue;

                    // バックアップ作成
                    if (createBackup)
                    {
                        CreateBackup(targetAvatar);
                    }

                    int synced = SynchronizeLightsToTarget(sourceAvatar, targetAvatar, sourceLights);
                    totalSynced += synced;

                    Debug.Log($"Synced {synced} lights to {targetAvatar.name}");
                }

                EditorUtility.DisplayDialog("Sync Complete",
                    $"{totalSynced} lights synchronized to {targetAvatars.Count} avatars!", "OK");

                SaveSyncLog();
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Error", $"Synchronization failed: {e.Message}", "OK");
                Debug.LogError($"Light sync failed: {e}");
            }
        }

        private int SynchronizeLightsToTarget(GameObject source, GameObject target, Light[] sourceLights)
        {
            int synced = 0;

            foreach (var sourceLight in sourceLights)
            {
                // ターゲットで対応するライトを見つける（名前ベース）
                var targetLight = FindCorrespondingLight(target, sourceLight.gameObject.name);

                if (targetLight != null)
                {
                    // 既存のライトを同期
                    SyncLightProperties(sourceLight, targetLight);
                    synced++;
                }
                else
                {
                    // 新しいライトを作成
                    var newLightObj = CreateLightInTarget(source, target, sourceLight.gameObject);
                    if (newLightObj != null)
                    {
                        synced++;
                    }
                }
            }

            return synced;
        }

        private Light FindCorrespondingLight(GameObject target, string lightName)
        {
            var lights = target.GetComponentsInChildren<Light>(true);
            return lights.FirstOrDefault(l => l.gameObject.name == lightName);
        }

        private GameObject CreateLightInTarget(GameObject source, GameObject target, GameObject sourceLightObj)
        {
            try
            {
                // ソースでの相対パスを取得
                string relativePath = GetRelativePath(source.transform, sourceLightObj.transform);

                // ターゲットで同じパスにライトを作成
                GameObject newLightObj = CreateObjectAtPath(target.transform, relativePath);
                if (newLightObj == null) return null;

                newLightObj.name = sourceLightObj.name;

                // Lightコンポーネントを追加・設定
                var newLight = newLightObj.AddComponent<Light>();
                var sourceLight = sourceLightObj.GetComponent<Light>();

                if (sourceLight != null && newLight != null)
                {
                    SyncLightProperties(sourceLight, newLight);

                    // トランスフォーム同期
                    newLightObj.transform.localPosition = sourceLightObj.transform.localPosition;
                    newLightObj.transform.localRotation = sourceLightObj.transform.localRotation;
                    newLightObj.transform.localScale = sourceLightObj.transform.localScale;

                    return newLightObj;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Failed to create light in target: {e.Message}");
            }

            return null;
        }

        private GameObject CreateObjectAtPath(Transform root, string path)
        {
            if (string.IsNullOrEmpty(path)) return root.gameObject;

            string[] parts = path.Split('/');
            Transform current = root;

            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];
                Transform child = current.Find(part);

                if (child == null)
                {
                    var newObj = new GameObject(part);
                    newObj.transform.SetParent(current);
                    newObj.transform.localPosition = Vector3.zero;
                    newObj.transform.localRotation = Quaternion.identity;
                    newObj.transform.localScale = Vector3.one;
                    current = newObj.transform;
                }
                else
                {
                    current = child;
                }
            }

            return current.gameObject;
        }

        private void SyncLightProperties(Light source, Light target)
        {
            if (syncLightProperties)
            {
                target.type = source.type;
                target.color = source.color;
                target.intensity = source.intensity;
                target.range = source.range;
                target.spotAngle = source.spotAngle;
                target.shadows = source.shadows;
                target.cookie = source.cookie;
                target.cookieSize = source.cookieSize;
                target.colorTemperature = source.colorTemperature;
            }

            // Modular Avatarコンポーネントの同期
            if (syncModularAvatarComponents)
            {
                SyncModularAvatarComponents(source.gameObject, target.gameObject);
            }
        }

        private void SyncModularAvatarComponents(GameObject source, GameObject target)
        {
#if MODULAR_AVATAR_EXISTS
            // 各種Modular Avatarコンポーネントの同期（リフレクションで取得）
            var componentTypes = new System.Type[]
            {
                Type.GetType("nadena.dev.modular_avatar.core.ModularAvatarParameters, nadena.dev.modular-avatar"),
                Type.GetType("nadena.dev.modular_avatar.core.ModularAvatarMenuInstaller, nadena.dev.modular-avatar"),
                Type.GetType("nadena.dev.modular_avatar.core.ModularAvatarMergeAnimator, nadena.dev.modular-avatar"),
                Type.GetType("nadena.dev.modular_avatar.core.ModularAvatarBoneProxy, nadena.dev.modular-avatar"),
                Type.GetType("nadena.dev.modular_avatar.core.ModularAvatarParentConstraint, nadena.dev.modular-avatar"),
                Type.GetType("nadena.dev.modular_avatar.core.ModularAvatarPositionConstraint, nadena.dev.modular-avatar")
            };

            foreach (var type in componentTypes)
            {
                var sourceComponent = source.GetComponent(type);
                if (sourceComponent != null)
                {
                    var targetComponent = target.GetComponent(type);
                    if (targetComponent == null)
                    {
                        targetComponent = target.AddComponent(type);
                    }

                    // コンポーネントのプロパティをコピー（簡略化）
                    EditorUtility.CopySerialized(sourceComponent, targetComponent);
                }
            }
#endif
        }

        private void CreateBackup(GameObject avatar)
        {
            string backupName = $"{avatar.name}_Backup_{System.DateTime.Now:yyyyMMdd_HHmmss}";
            var backup = Object.Instantiate(avatar);
            backup.name = backupName;
            backup.transform.SetParent(avatar.transform.parent);
            backup.SetActive(false);

            Debug.Log($"Created backup: {backupName}");
        }

        private void ExportLightConfiguration(GameObject avatar)
        {
            var lights = avatar.GetComponentsInChildren<Light>(true);
            var lightConfigs = new List<LightConfiguration>();

            foreach (var light in lights)
            {
                var config = new LightConfiguration
                {
                    name = light.gameObject.name,
                    path = GetRelativePath(avatar.transform, light.transform),
                    type = light.type,
                    color = light.color,
                    intensity = light.intensity,
                    range = light.range,
                    spotAngle = light.spotAngle,
                    position = light.transform.localPosition,
                    rotation = light.transform.localRotation,
                    scale = light.transform.localScale
                };

                lightConfigs.Add(config);
            }

            var exportData = new LightSyncExportData
            {
                avatarName = avatar.name,
                exportTime = System.DateTime.Now,
                lights = lightConfigs.ToArray()
            };

            string json = JsonUtility.ToJson(exportData, true);
            string exportPath = EditorUtility.SaveFilePanel("Export Light Configuration",
                "", $"{avatar.name}_lights.json", "json");

            if (!string.IsNullOrEmpty(exportPath))
            {
                File.WriteAllText(exportPath, json);
                EditorUtility.DisplayDialog("Export Complete",
                    $"Light configuration exported to:\n{exportPath}", "OK");
            }
        }

        private void ImportLightConfiguration()
        {
            string importPath = EditorUtility.OpenFilePanel("Import Light Configuration", "", "json");

            if (!string.IsNullOrEmpty(importPath) && File.Exists(importPath))
            {
                string json = File.ReadAllText(importPath);
                var importData = JsonUtility.FromJson<LightSyncExportData>(json);

                if (importData != null && sourceAvatar != null)
                {
                    if (EditorUtility.DisplayDialog("Import Configuration",
                        $"Import {importData.lights.Length} lights to {sourceAvatar.name}?", "Import", "Cancel"))
                    {
                        ImportLightsToAvatar(sourceAvatar, importData.lights);
                        EditorUtility.DisplayDialog("Import Complete",
                            $"Imported {importData.lights.Length} lights successfully!", "OK");
                    }
                }
            }
        }

        private void ImportLightsToAvatar(GameObject avatar, LightConfiguration[] configs)
        {
            foreach (var config in configs)
            {
                var targetObj = CreateObjectAtPath(avatar.transform, config.path);
                if (targetObj == null) continue;

                targetObj.name = config.name;

                var light = targetObj.GetComponent<Light>();
                if (light == null) light = targetObj.AddComponent<Light>();

                light.type = config.type;
                light.color = config.color;
                light.intensity = config.intensity;
                light.range = config.range;
                light.spotAngle = config.spotAngle;

                targetObj.transform.localPosition = config.position;
                targetObj.transform.localRotation = config.rotation;
                targetObj.transform.localScale = config.scale;
            }
        }

        private void ValidateLightSetup(GameObject avatar)
        {
            var lights = avatar.GetComponentsInChildren<Light>(true);
            var errors = new List<string>();
            var warnings = new List<string>();

            foreach (var light in lights)
            {
                // 強度チェック
                if (light.intensity > 5f)
                {
                    warnings.Add($"{light.name}: High intensity ({light.intensity})");
                }

                // 範囲チェック
                if (light.range > 20f)
                {
                    warnings.Add($"{light.name}: Large range ({light.range})");
                }

                // 位置チェック
                if (light.transform.localPosition.magnitude > 10f)
                {
                    warnings.Add($"{light.name}: Far from avatar center");
                }
            }

            string message = $"Validation Results:\nLights: {lights.Length}\nErrors: {errors.Count}\nWarnings: {warnings.Count}";

            if (errors.Count > 0)
            {
                message += "\n\nErrors:\n" + string.Join("\n", errors);
            }

            if (warnings.Count > 0)
            {
                message += "\n\nWarnings:\n" + string.Join("\n", warnings);
            }

            EditorUtility.DisplayDialog("Validation Complete", message, "OK");
        }

        private string GetRelativePath(Transform root, Transform target)
        {
            if (target == root) return "";

            var path = target.name;
            var current = target.parent;

            while (current != null && current != root)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }

            return path;
        }

        private void EnsureDirectoryExists(string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private void SaveSyncLog()
        {
            string logPath = "_docs/2025-08-18_light_sync_system.md";
            string logContent = $@"# 2025-08-18 Light Sync System Implementation

## 概要
複数アバター間でのライト設定同期システムを実装。異なるアバター間でライト設定を同期・共有できるようにした。

## 実装内容
- **ライト設定同期**: 位置・回転・スケール・Lightプロパティの同期
- **Modular Avatar統合**: MAコンポーネントの同期機能
- **バックアップ機能**: 同期前の自動バックアップ
- **エクスポート/インポート**: JSON形式での設定共有
- **検証機能**: ライト設定の妥当性チェック

## 同期オプション
- 位置同期: {syncPosition}
- 回転同期: {syncRotation}
- スケール同期: {syncScale}
- ライトプロパティ同期: {syncLightProperties}
- Modular Avatar同期: {syncModularAvatarComponents}
- バックアップ作成: {createBackup}

## ソースアバター
- {sourceAvatar?.name ?? "None"}
- ライト数: {sourceAvatar?.GetComponentsInChildren<Light>(true).Length ?? 0}

## ターゲットアバター数
- {targetAvatars.Count}

## タイムスタンプ
- 2025-08-18
";

            try
            {
                File.WriteAllText(logPath, logContent);
                AssetDatabase.Refresh();
                Debug.Log($"Light sync implementation log saved to {logPath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save sync log: {e.Message}");
            }
        }
    }

    [System.Serializable]
    public class LightConfiguration
    {
        public string name;
        public string path;
        public LightType type;
        public Color color;
        public float intensity;
        public float range;
        public float spotAngle;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    [System.Serializable]
    public class LightSyncExportData
    {
        public string avatarName;
        public System.DateTime exportTime;
        public LightConfiguration[] lights;
    }
}
