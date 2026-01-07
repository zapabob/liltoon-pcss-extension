using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal; // keep for other editor internals, but avoid its Animator API
using Anim = UnityEditor.Animations;
using UnityEngine;

namespace lilToon.PCSS.Editor
{
    public static class ModularAvatarLightToggleBuilder
    {
        private const string GroupHip = "PCSS Hip Lights (MA)";
        private const string GroupAuto = "PCSS External Lights (Auto)";
        private const string RootName = "PCSS Light Controls (MA)";
        private const string ToggleGOName = "PCSS Lights Toggle";
        private const string ParamName = PCSSConstants.ParamLightOn;
        private const string ParamIntensity = PCSSConstants.ParamLightIntensity;
        private const string ParamSway = PCSSConstants.ParamLightSway;
        private const string ControllerDir = "Assets/PCSS/Controllers";
        private const string ControllerPath = "Assets/PCSS/Controllers/PCSS_LightControl.controller";

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Modular Avatar/外部光源トグルを作成", false, 205)]
        public static void Build()
        {
            var avatar = Selection.activeGameObject;
            if (avatar == null)
            {
                EditorUtility.DisplayDialog("PCSS", "アバターのルートを選択してください。", "OK");
                return;
            }

            if (!BuildForAvatar(avatar, null, true, false, out string error))
            {
                EditorUtility.DisplayDialog("PCSS", error, "OK");
                return;
            }

            EditorUtility.DisplayDialog("PCSS", "Modular Avatarベースの外部光源トグルを作成しました。", "OK");
        }

        public static bool BuildForAvatar(GameObject avatar, List<Light> lights, bool includeIntensity, out string error)
        {
            return BuildForAvatar(avatar, lights, includeIntensity, false, out error);
        }

        public static bool BuildForAvatar(GameObject avatar, List<Light> lights, bool includeIntensity, bool includeSway, out string error)
        {
            error = string.Empty;
            if (avatar == null)
            {
                error = "アバターのルートを選択してください。";
                return false;
            }

            // 必要なMA型があるかチェック
            var tMenuItem = FindType("nadena.dev.modular_avatar.core.ModularAvatarMenuItem");
            var tInstaller = FindType("nadena.dev.modular_avatar.core.ModularAvatarMenuInstaller");
            var tObjectToggle = FindType("nadena.dev.modular_avatar.core.ModularAvatarObjectToggle");
            var tParameter = FindType("nadena.dev.modular_avatar.core.ModularAvatarParameter");
            if (tMenuItem == null || tInstaller == null || tObjectToggle == null)
            {
                error = "Modular Avatar が見つかりません。導入後に再実行してください。";
                return false;
            }

            Undo.RegisterFullObjectHierarchyUndo(avatar, "Build MA Light Toggle");

            // 既存のコントロールルートを再作成
            var existing = avatar.transform.Find(RootName);
            if (existing != null) Undo.DestroyObjectImmediate(existing.gameObject);

            var root = new GameObject(RootName);
            Undo.RegisterCreatedObjectUndo(root, "Create MA Light Controls Root");
            root.transform.SetParent(avatar.transform, false);

            // トグル用オブジェクト
            var toggleGO = new GameObject(ToggleGOName);
            Undo.RegisterCreatedObjectUndo(toggleGO, "Create MA Light Toggle GO");
            toggleGO.transform.SetParent(root.transform, false);

            // MenuItem (Toggle)
            var mi = toggleGO.AddComponent(tMenuItem);
            TrySet(mi, "name", "PCSS Lights");
            // ControlType = Toggle
            TrySetEnum(mi, "type", "Toggle");
            // パラメータ名
            TrySet(mi, "parameter", ParamName);

            // Parameter (Bool) - あれば
            if (tParameter != null)
            {
                var paramGO = new GameObject("Parameter: " + ParamName);
                Undo.RegisterCreatedObjectUndo(paramGO, "Create MA Parameter");
                paramGO.transform.SetParent(root.transform, false);
                var p = paramGO.AddComponent(tParameter);
                // よくあるフィールド名に対応
                TrySet(p, "name", ParamName);
                TrySetEnum(p, "parameterType", "Bool");
                TrySet(p, "defaultValue", 1f);
                TrySet(p, "saved", true);

                if (includeIntensity)
                {
                    // Intensity Float parameter 0..1
                    var paramGO2 = new GameObject("Parameter: " + ParamIntensity);
                    Undo.RegisterCreatedObjectUndo(paramGO2, "Create MA Parameter(Float)");
                    paramGO2.transform.SetParent(root.transform, false);
                    var p2 = paramGO2.AddComponent(tParameter);
                    TrySet(p2, "name", ParamIntensity);
                    TrySetEnum(p2, "parameterType", "Float");
                    TrySet(p2, "defaultValue", 1f);
                    TrySet(p2, "saved", true);
                }

                if (includeSway)
                {
                    var paramGO3 = new GameObject("Parameter: " + ParamSway);
                    Undo.RegisterCreatedObjectUndo(paramGO3, "Create MA Parameter(Float)");
                    paramGO3.transform.SetParent(root.transform, false);
                    var p3 = paramGO3.AddComponent(tParameter);
                    TrySet(p3, "name", ParamSway);
                    TrySetEnum(p3, "parameterType", "Float");
                    TrySet(p3, "defaultValue", 1f);
                    TrySet(p3, "saved", true);
                }
            }

            // ObjectToggle (対象ライトを全部突っ込む)
            var ot = toggleGO.AddComponent(tObjectToggle);
            var targetLights = lights ?? CollectExternalLights(avatar);
            if (targetLights.Count == 0)
            {
                error = "外部光源(Light)が見つかりませんでした。先に光源を作成してください。";
                return false;
            }
            // 代表プロパティ candidates: objects, targets, objectToToggle, targetObject
            if (!TrySetObjectList(ot, new[] { "objects", "targets" }, targetLights.Select(l => l.gameObject).ToList()))
            {
                // フォールバック: 先頭のみ
                TrySet(ot, "targetObject", targetLights[0].gameObject);
                TrySet(ot, "objectToToggle", targetLights[0].gameObject);
            }
            // 初期状態ON
            TrySet(ot, "defaultValue", true);
            TrySet(ot, "objectActive", true);

            Transform swayTarget = null;
            if (includeSway)
            {
                swayTarget = FindTransformByName(avatar.transform, "PB_LightTip");
                if (swayTarget == null)
                {
                    error = "PB_LightTip が見つかりませんでした。PhysBone揺れを作成してから実行してください。";
                    return false;
                }
            }

            if (includeIntensity)
            {
                // Intensity Puppet (Radial 等価) 用メニュー
                var puppetGO = new GameObject("PCSS Lights Intensity");
                Undo.RegisterCreatedObjectUndo(puppetGO, "Create MA Light Puppet");
                puppetGO.transform.SetParent(root.transform, false);
                var puppet = puppetGO.AddComponent(tMenuItem);
                TrySet(puppet, "name", "Intensity");
                TrySetEnum(puppet, "type", "Puppet");
                TrySet(puppet, "parameter", ParamIntensity);

                // MergeAnimator で強度制御のAnimatorをFXに統合
                EnsureControllerAndBlendTree(avatar, targetLights, swayTarget, includeSway);
                var tMergeAnimator = FindType("nadena.dev.modular_avatar.core.ModularAvatarMergeAnimator");
                if (tMergeAnimator != null)
                {
                    var merge = avatar.AddComponent(tMergeAnimator);
                    var ctrl = AssetDatabase.LoadAssetAtPath<Anim.AnimatorController>(ControllerPath);
                    TrySet(merge, "animator", ctrl);
                    TrySet(merge, "Animator", ctrl);
                    TrySetEnum(merge, "layerType", "FX");
                    TrySetEnum(merge, "pathMode", "Absolute");
                }
            }
            else if (includeSway)
            {
                EnsureControllerAndBlendTree(avatar, targetLights, swayTarget, includeSway);
                var tMergeAnimator = FindType("nadena.dev.modular_avatar.core.ModularAvatarMergeAnimator");
                if (tMergeAnimator != null)
                {
                    var merge = avatar.AddComponent(tMergeAnimator);
                    var ctrl = AssetDatabase.LoadAssetAtPath<Anim.AnimatorController>(ControllerPath);
                    TrySet(merge, "animator", ctrl);
                    TrySet(merge, "Animator", ctrl);
                    TrySetEnum(merge, "layerType", "FX");
                    TrySetEnum(merge, "pathMode", "Absolute");
                }
            }

            if (includeSway)
            {
                var swayGO = new GameObject("PCSS Lights Sway Strength");
                Undo.RegisterCreatedObjectUndo(swayGO, "Create MA Light Sway Puppet");
                swayGO.transform.SetParent(root.transform, false);
                var sway = swayGO.AddComponent(tMenuItem);
                TrySet(sway, "name", "Sway Strength");
                TrySetEnum(sway, "type", "Puppet");
                TrySet(sway, "parameter", ParamSway);
            }

            // MenuInstaller（ルートに付与）
            avatar.AddComponent(tInstaller);

            Selection.activeObject = root;
            EditorGUIUtility.PingObject(root);
            return true;
        }

        private static void EnsureControllerAndBlendTree(GameObject avatar, List<Light> lights, Transform swayTarget = null, bool includeSway = false)
        {
            if (lights == null || lights.Count == 0) return;
            EnsureFolders();
            var controller = AssetDatabase.LoadAssetAtPath<Anim.AnimatorController>(ControllerPath);
            if (controller == null)
            {
                controller = Anim.AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            }

            // パラメータ整備
            if (controller.parameters.All(p => p.name != ParamIntensity))
                controller.AddParameter(ParamIntensity, UnityEngine.AnimatorControllerParameterType.Float);
            if (includeSway && controller.parameters.All(p => p.name != ParamSway))
                controller.AddParameter(ParamSway, UnityEngine.AnimatorControllerParameterType.Float);

            // レイヤを1つにまとめて再構築
            var layerName = "PCSS_LightControl";
            Anim.AnimatorControllerLayer layer = controller.layers.FirstOrDefault(l => l.name == layerName);
            if (layer.stateMachine == null)
            {
                // 既存が無い場合は新規作成して追加
                var newLayer = new Anim.AnimatorControllerLayer
                {
                    name = layerName,
                    defaultWeight = 1f,
                    stateMachine = new Anim.AnimatorStateMachine { name = layerName }
                };
                controller.AddLayer(newLayer);
                // 追加直後に正しい参照を再取得
                layer = controller.layers.FirstOrDefault(l => l.name == layerName);
            }
            else
            {
                // クリア
                var sm = layer.stateMachine;
                if (sm != null)
                {
                    for (int i = sm.states.Length - 1; i >= 0; i--) sm.RemoveState(sm.states[i].state);
                }
            }

            // 段階クリップ生成（0,0.25,0.5,0.75,1.0）
            var thresholds = new[] { 0f, 0.25f, 0.5f, 0.75f, 1f };
            var clips = new List<AnimationClip>();
            foreach (var t in thresholds)
            {
                var clip = new AnimationClip { name = $"PCSS_LightIntensity_{Mathf.RoundToInt(t*100)}" };
                foreach (var light in lights)
                {
                    if (light == null) continue;
                    var relPath = AnimationUtility.CalculateTransformPath(light.transform, avatar.transform);
                    var binding = new EditorCurveBinding
                    {
                        path = relPath,
                        type = typeof(Light),
                        propertyName = "m_Intensity"
                    };
                    var baseIntensity = Mathf.Max(0f, light.intensity);
                    var curve = AnimationCurve.Linear(0, baseIntensity * t, 1f / 60f, baseIntensity * t);
                    AnimationUtility.SetEditorCurve(clip, binding, curve);
                }
                AssetDatabase.AddObjectToAsset(clip, controller);
                clips.Add(clip);
            }
            AssetDatabase.SaveAssets();

            // BlendTree 構築
            var tree = new Anim.BlendTree { name = "PCSS_IntensityTree", hideFlags = HideFlags.HideInHierarchy };
            tree.blendType = Anim.BlendTreeType.Simple1D;
            tree.blendParameter = ParamIntensity;
            var motions = new Anim.ChildMotion[clips.Count];
            for (int i = 0; i < clips.Count; i++)
            {
                motions[i] = new Anim.ChildMotion { motion = clips[i], threshold = thresholds[i] };
            }
            tree.children = motions;
            AssetDatabase.AddObjectToAsset(tree, controller);
            AssetDatabase.SaveAssets();

            // 念のため再参照を取得し、StateMachineがnullなら作成
            layer = controller.layers.FirstOrDefault(l => l.name == layerName);
            if (layer.stateMachine == null)
            {
                layer.stateMachine = new Anim.AnimatorStateMachine { name = layerName };
            }
            var newState = layer.stateMachine.AddState("PCSS_Intensity");
            newState.motion = tree;
            layer.stateMachine.defaultState = newState;
            // 上書き反映
            for (int i = 0; i < controller.layers.Length; i++)
            {
                if (controller.layers[i].name == layerName)
                {
                    controller.layers[i] = layer;
                    break;
                }
            }
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();

            if (includeSway && swayTarget != null)
            {
                EnsureSwayLayer(controller, avatar, swayTarget);
            }
        }

        private static void EnsureSwayLayer(Anim.AnimatorController controller, GameObject avatar, Transform swayTarget)
        {
            var layerName = "PCSS_LightSway";
            Anim.AnimatorControllerLayer layer = controller.layers.FirstOrDefault(l => l.name == layerName);
            if (layer.stateMachine == null)
            {
                var newLayer = new Anim.AnimatorControllerLayer
                {
                    name = layerName,
                    defaultWeight = 1f,
                    stateMachine = new Anim.AnimatorStateMachine { name = layerName }
                };
                controller.AddLayer(newLayer);
                layer = controller.layers.FirstOrDefault(l => l.name == layerName);
            }
            else
            {
                var sm = layer.stateMachine;
                if (sm != null)
                {
                    for (int i = sm.states.Length - 1; i >= 0; i--) sm.RemoveState(sm.states[i].state);
                }
            }

            Vector3 basePos = swayTarget.localPosition;
            if (basePos == Vector3.zero)
            {
                basePos = new Vector3(0f, 0.1f, 0.2f);
            }

            var clipLow = CreateSwayClip(controller, "PCSS_LightSway_30", avatar, swayTarget, basePos * 0.3f);
            var clipHigh = CreateSwayClip(controller, "PCSS_LightSway_100", avatar, swayTarget, basePos);

            var tree = new Anim.BlendTree { name = "PCSS_SwayTree", hideFlags = HideFlags.HideInHierarchy };
            tree.blendType = Anim.BlendTreeType.Simple1D;
            tree.blendParameter = ParamSway;
            tree.useAutomaticThresholds = false;
            tree.AddChild(clipLow, 0f);
            tree.AddChild(clipHigh, 1f);
            AssetDatabase.AddObjectToAsset(tree, controller);
            AssetDatabase.SaveAssets();

            var state = layer.stateMachine.AddState("PCSS_Sway");
            state.motion = tree;
            layer.stateMachine.defaultState = state;
            for (int i = 0; i < controller.layers.Length; i++)
            {
                if (controller.layers[i].name == layerName)
                {
                    controller.layers[i] = layer;
                    break;
                }
            }
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
        }

        private static AnimationClip CreateSwayClip(Anim.AnimatorController controller, string name, GameObject avatar, Transform swayTarget, Vector3 localPos)
        {
            var clip = new AnimationClip { name = name };
            var path = AnimationUtility.CalculateTransformPath(swayTarget, avatar.transform);
            var bindingX = new EditorCurveBinding { path = path, type = typeof(Transform), propertyName = "m_LocalPosition.x" };
            var bindingY = new EditorCurveBinding { path = path, type = typeof(Transform), propertyName = "m_LocalPosition.y" };
            var bindingZ = new EditorCurveBinding { path = path, type = typeof(Transform), propertyName = "m_LocalPosition.z" };

            AnimationUtility.SetEditorCurve(clip, bindingX, AnimationCurve.Linear(0f, localPos.x, 1f / 60f, localPos.x));
            AnimationUtility.SetEditorCurve(clip, bindingY, AnimationCurve.Linear(0f, localPos.y, 1f / 60f, localPos.y));
            AnimationUtility.SetEditorCurve(clip, bindingZ, AnimationCurve.Linear(0f, localPos.z, 1f / 60f, localPos.z));

            AssetDatabase.AddObjectToAsset(clip, controller);
            return clip;
        }

        private static Transform FindTransformByName(Transform root, string name)
        {
            if (root == null) return null;
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
            {
                if (t != null && t.name == name)
                {
                    return t;
                }
            }
            return null;
        }

        private static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/PCSS")) AssetDatabase.CreateFolder("Assets", "PCSS");
            if (!AssetDatabase.IsValidFolder("Assets/PCSS/Controllers")) AssetDatabase.CreateFolder("Assets/PCSS", "Controllers");
        }

        private static List<Light> CollectExternalLights(GameObject avatar)
        {
            var results = new List<Light>();
            var hip = avatar.transform.Find(GroupHip);
            var auto = avatar.transform.Find(GroupAuto);
            if (hip != null) results.AddRange(hip.GetComponentsInChildren<Light>(true));
            if (auto != null) results.AddRange(auto.GetComponentsInChildren<Light>(true));
            if (results.Count == 0)
            {
                // フォールバック: アバター配下の全Light
                results.AddRange(avatar.GetComponentsInChildren<Light>(true));
            }
            // 重複排除
            results = results.Where(l => l != null).Distinct().ToList();
            return results;
        }

        private static Type FindType(string fullName)
        {
            try
            {
                return AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => { try { return a.GetTypes(); } catch { return Type.EmptyTypes; } })
                    .FirstOrDefault(t => t.FullName == fullName);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[PCSS] 型 '{fullName}' の検索に失敗しました: {e.Message}");
                return null;
            }
        }

        private static void TrySet(object component, string memberName, object value)
        {
            if (component == null) return;
            var t = component.GetType();
            var fi = t.GetField(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi != null && (value == null || fi.FieldType.IsAssignableFrom(value.GetType())))
            {
                fi.SetValue(component, value);
                return;
            }
            var pi = t.GetProperty(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (pi != null && pi.CanWrite && (value == null || pi.PropertyType.IsAssignableFrom(value.GetType())))
            {
                pi.SetValue(component, value);
                return;
            }
            // Serialized fallback (for arrays etc.)
            var uo = component as UnityEngine.Object;
            if (uo != null)
            {
                var so = new SerializedObject(uo);
                var sp = so.FindProperty(memberName);
                if (sp != null)
                {
                    if (sp.propertyType == SerializedPropertyType.Boolean && value is bool b)
                    {
                        sp.boolValue = b; so.ApplyModifiedProperties();
                    }
                    else if (sp.propertyType == SerializedPropertyType.Float && value is float f)
                    {
                        sp.floatValue = f; so.ApplyModifiedProperties();
                    }
                    else if (sp.propertyType == SerializedPropertyType.String && value is string s)
                    {
                        sp.stringValue = s; so.ApplyModifiedProperties();
                    }
                    else if (sp.propertyType == SerializedPropertyType.ObjectReference && value is UnityEngine.Object obj)
                    {
                        sp.objectReferenceValue = obj; so.ApplyModifiedProperties();
                    }
                }
            }
        }

        private static bool TrySetObjectList(object component, IEnumerable<string> candidateNames, List<GameObject> values)
        {
            var uo = component as UnityEngine.Object;
            if (uo == null) return false;
            foreach (var name in candidateNames)
            {
                var so = new SerializedObject(uo);
                var sp = so.FindProperty(name);
                if (sp == null || !sp.isArray) continue;
                sp.ClearArray();
                for (int i = 0; i < values.Count; i++)
                {
                    sp.InsertArrayElementAtIndex(i);
                    var elem = sp.GetArrayElementAtIndex(i);
                    if (elem.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        elem.objectReferenceValue = values[i];
                    }
                }
                so.ApplyModifiedProperties();
                return true;
            }
            return false;
        }

        private static void TrySetEnum(object component, string memberName, string enumValueName)
        {
            if (component == null) return;
            var t = component.GetType();
            var fi = t.GetField(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi != null && fi.FieldType.IsEnum)
            {
                var val = EnumParseSafe(fi.FieldType, enumValueName);
                if (val != null) fi.SetValue(component, val);
                return;
            }
            var pi = t.GetProperty(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (pi != null && pi.PropertyType.IsEnum && pi.CanWrite)
            {
                var val = EnumParseSafe(pi.PropertyType, enumValueName);
                if (val != null) pi.SetValue(component, val);
            }
        }

        private static object EnumParseSafe(Type enumType, string name)
        {
            try
            {
                return Enum.Parse(enumType, name);
            }
            catch { return null; }
        }
    }
}
