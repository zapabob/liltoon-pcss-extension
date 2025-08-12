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
        private const string ParamName = "PCSS_LightOn";
        private const string ParamIntensity = "PCSS_LightIntensity";
        private const string ControllerDir = "Assets/PCSS/Controllers";
        private const string ControllerPath = "Assets/PCSS/Controllers/PCSS_LightControl.controller";

        [MenuItem("Tools/lilToon-PCSS-Extension/Modular Avatar/外部光源トグルを作成", false, 205)]
        public static void Build()
        {
            var avatar = Selection.activeGameObject;
            if (avatar == null)
            {
                EditorUtility.DisplayDialog("PCSS", "アバターのルートを選択してください。", "OK");
                return;
            }

            // 必要なMA型があるかチェック
            var tMenuItem = FindType("nadena.dev.modular_avatar.core.ModularAvatarMenuItem");
            var tInstaller = FindType("nadena.dev.modular_avatar.core.ModularAvatarMenuInstaller");
            var tObjectToggle = FindType("nadena.dev.modular_avatar.core.ModularAvatarObjectToggle");
            var tParameter = FindType("nadena.dev.modular_avatar.core.ModularAvatarParameter");
            if (tMenuItem == null || tInstaller == null || tObjectToggle == null)
            {
                EditorUtility.DisplayDialog("PCSS", "Modular Avatar が見つかりません。導入後に再実行してください。", "OK");
                return;
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
            // パラメータ名（存在すれば設定）
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

            // ObjectToggle (対象ライトを全部突っ込む)
            var ot = toggleGO.AddComponent(tObjectToggle);
            var lights = CollectExternalLights(avatar);
            if (lights.Count == 0)
            {
                EditorUtility.DisplayDialog("PCSS", "外部光源(Light)が見つかりませんでした。先に光源を作成してください。", "OK");
                return;
            }
            // 代表プロパティ candidates: objects, targets, objectToToggle, targetObject
            if (!TrySetObjectList(ot, new[] { "objects", "targets" }, lights.Select(l => l.gameObject).ToList()))
            {
                // フォールバック: 先頭のみ
                TrySet(ot, "targetObject", lights[0].gameObject);
                TrySet(ot, "objectToToggle", lights[0].gameObject);
            }
            // 初期状態ON
            TrySet(ot, "defaultValue", true);
            TrySet(ot, "objectActive", true);

            // Intensity Puppet (Radial 等価) 用メニュー
            var puppetGO = new GameObject("PCSS Lights Intensity");
            Undo.RegisterCreatedObjectUndo(puppetGO, "Create MA Light Puppet");
            puppetGO.transform.SetParent(root.transform, false);
            var puppet = puppetGO.AddComponent(tMenuItem);
            TrySet(puppet, "name", "Intensity");
            TrySetEnum(puppet, "type", "Puppet");
            TrySet(puppet, "parameter", ParamIntensity);

            // MergeAnimator で強度制御のAnimatorをFXに統合
            EnsureControllerAndBlendTree(avatar, CollectExternalLights(avatar));
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

            // MenuInstaller（ルートに付与）
            avatar.AddComponent(tInstaller);

            Selection.activeObject = root;
            EditorGUIUtility.PingObject(root);
            EditorUtility.DisplayDialog("PCSS", "Modular Avatarベースの外部光源トグルを作成しました。", "OK");
        }

        private static void EnsureControllerAndBlendTree(GameObject avatar, List<Light> lights)
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


