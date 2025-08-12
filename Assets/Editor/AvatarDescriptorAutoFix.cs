using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Animations;
using UnityEngine;

#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
#endif

namespace lilToon.PCSS.Editor
{
    public static class AvatarDescriptorAutoFix
    {
#if VRC_SDK_VRCSDK3
        [MenuItem("Tools/lilToon-PCSS-Extension/VRChat向けクリーンアップ/AvatarDescriptor: Custom Layer空設定を無効化", false, 300)]
        public static void FixEmptyCustomLayers()
        {
            var go = Selection.activeGameObject;
            if (go == null)
            {
                EditorUtility.DisplayDialog("PCSS", "アバターのルートを選択してください。", "OK");
                return;
            }
            var desc = go.GetComponent<VRCAvatarDescriptor>();
            if (desc == null)
            {
                EditorUtility.DisplayDialog("PCSS", "選択対象にVRCAvatarDescriptorが見つかりません。", "OK");
                return;
            }

            int changes = 0;

            // Base Layers (Standing/Seated/Action/FX/…)
            var baseLayers = desc.baseAnimationLayers;
            for (int i = 0; i < baseLayers.Length; i++)
            {
                var layer = baseLayers[i];
                if (layer.isDefault) continue; // default使用中はスキップ
                if (layer.animatorController == null)
                {
                    layer.isDefault = true; // 空ならデフォルトへ戻す
                    baseLayers[i] = layer;
                    changes++;
                    continue;
                }
                var ctrl = layer.animatorController as AnimatorController;
                if (ctrl != null && (ctrl.layers == null || ctrl.layers.Length == 0))
                {
                    // レイヤー0件はSDKが落ちやすい→ダミーレイヤーを1つ作成
                    var newLayer = new AnimatorControllerLayer
                    {
                        name = "Base",
                        defaultWeight = 1f,
                        stateMachine = new AnimatorStateMachine { name = "Base_SM" }
                    };
                    ctrl.AddLayer(newLayer);
                    EditorUtility.SetDirty(ctrl);
                    AssetDatabase.SaveAssets();
                    changes++;
                }
            }
            desc.baseAnimationLayers = baseLayers;

            // Special Layers (EyeLook, etc.) はnullならデフォルトに戻す
            var specialLayers = desc.specialAnimationLayers;
            for (int i = 0; i < specialLayers.Length; i++)
            {
                var layer = specialLayers[i];
                if (layer.isDefault) continue;
                if (layer.animatorController == null)
                {
                    layer.isDefault = true;
                    specialLayers[i] = layer;
                    changes++;
                }
            }
            desc.specialAnimationLayers = specialLayers;

            // Expression Parameters/Menu の最小整合
            if (desc.expressionParameters == null)
            {
                var ep = ScriptableObject.CreateInstance<VRCExpressionParameters>();
                AssetDatabase.CreateAsset(ep, "Assets/PCSS/Controllers/Auto_EP.asset");
                desc.expressionParameters = ep;
                changes++;
            }
            if (desc.expressionsMenu == null)
            {
                var menu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
                AssetDatabase.CreateAsset(menu, "Assets/PCSS/Controllers/Auto_Menu.asset");
                desc.expressionsMenu = menu;
                changes++;
            }

            if (changes > 0)
            {
                EditorUtility.SetDirty(desc);
                AssetDatabase.SaveAssets();
            }

            EditorUtility.DisplayDialog("PCSS", $"AvatarDescriptorの空カスタム設定を修正しました。変更点: {changes}", "OK");
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/VRChat向けクリーンアップ/PreValidation: Deep Fix Animator/Expressions", false, 301)]
        public static void DeepFixAnimatorAndExpressions()
        {
            var go = Selection.activeGameObject;
            if (go == null)
            {
                EditorUtility.DisplayDialog("PCSS", "アバターのルートを選択してください。", "OK");
                return;
            }
            var desc = go.GetComponent<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>();
            if (desc == null)
            {
                EditorUtility.DisplayDialog("PCSS", "選択対象にVRCAvatarDescriptorが見つかりません。", "OK");
                return;
            }

            int changes = 0;

            // Ensure Animator on root
            var animator = go.GetComponent<Animator>();
            if (animator == null)
            {
                animator = go.AddComponent<Animator>();
                EditorUtility.SetDirty(go);
                changes++;
            }

            // Sanitize Base Layers
            var bases = desc.baseAnimationLayers;
            for (int i = 0; i < bases.Length; i++)
            {
                var layer = bases[i];
                // 原則としてFX以外はデフォルトに戻す（不整合回避）
                if (layer.type != VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType.FX)
                {
                    if (!layer.isDefault)
                    {
                        layer.isDefault = true;
                        bases[i] = layer;
                        changes++;
                    }
                    continue;
                }

                if (!layer.isDefault)
                {
                    var ctrl = layer.animatorController as AnimatorController;
                    if (ctrl == null)
                    {
                        // fallback to default
                        layer.isDefault = true;
                        bases[i] = layer;
                        changes++;
                    }
                    else
                    {
                        changes += SanitizeAnimatorController(ctrl);
                    }
                }
            }
            desc.baseAnimationLayers = bases;

            // Sanitize Special Layers（全てデフォルトに）
            var specials = desc.specialAnimationLayers;
            for (int i = 0; i < specials.Length; i++)
            {
                var layer = specials[i];
                if (!layer.isDefault)
                {
                    var ctrl = layer.animatorController as AnimatorController;
                    // どのみち特別レイヤーはデフォルトに戻す
                    layer.isDefault = true;
                    specials[i] = layer;
                    changes++;
                }
            }
            desc.specialAnimationLayers = specials;

            // Expressions: ensure non-null, remove null entries
            if (desc.expressionParameters == null)
            {
                var ep = ScriptableObject.CreateInstance<VRCExpressionParameters>();
                EnsureControllersFolder();
                AssetDatabase.CreateAsset(ep, "Assets/PCSS/Controllers/Auto_EP.asset");
                desc.expressionParameters = ep;
                changes++;
            }
            else if (desc.expressionParameters.parameters == null)
            {
                desc.expressionParameters.parameters = new VRCExpressionParameters.Parameter[0];
                EditorUtility.SetDirty(desc.expressionParameters);
                changes++;
            }
            else
            {
                var filtered = new System.Collections.Generic.List<VRCExpressionParameters.Parameter>();
                foreach (var p in desc.expressionParameters.parameters)
                {
                    if (p != null && !string.IsNullOrEmpty(p.name)) filtered.Add(p);
                }
                if (filtered.Count != desc.expressionParameters.parameters.Length)
                {
                    desc.expressionParameters.parameters = filtered.ToArray();
                    EditorUtility.SetDirty(desc.expressionParameters);
                    changes++;
                }
            }

            if (desc.expressionsMenu == null)
            {
                var menu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
                EnsureControllersFolder();
                AssetDatabase.CreateAsset(menu, "Assets/PCSS/Controllers/Auto_Menu.asset");
                desc.expressionsMenu = menu;
                changes++;
            }
            else
            {
                changes += SanitizeExpressionMenu(desc.expressionsMenu, desc.expressionParameters);
            }

            if (changes > 0)
            {
                EditorUtility.SetDirty(desc);
                AssetDatabase.SaveAssets();
            }

            EditorUtility.DisplayDialog("PCSS", $"PreValidation 修復を実行しました。変更点: {changes}", "OK");
        }

        private static int SanitizeAnimatorController(AnimatorController ctrl)
        {
            int changes = 0;
            if (ctrl == null) return changes;

            // Ensure at least 1 layer
            if (ctrl.layers == null || ctrl.layers.Length == 0)
            {
                var newLayer = new AnimatorControllerLayer
                {
                    name = "Base",
                    defaultWeight = 1f,
                    stateMachine = new AnimatorStateMachine { name = "Base_SM" }
                };
                EnsureControllersFolder();
                AssetDatabase.AddObjectToAsset(newLayer.stateMachine, ctrl);
                ctrl.AddLayer(newLayer);
                changes++;
            }

            // Ensure each layer has at least 1 state
            for (int i = 0; i < ctrl.layers.Length; i++)
            {
                var layer = ctrl.layers[i];
                if (layer.stateMachine == null)
                {
                    layer.stateMachine = new AnimatorStateMachine { name = layer.name + "_SM" };
                    AssetDatabase.AddObjectToAsset(layer.stateMachine, ctrl);
                    ctrl.layers[i] = layer;
                    changes++;
                }
                if (layer.stateMachine.states == null || layer.stateMachine.states.Length == 0)
                {
                    var st = layer.stateMachine.AddState("Idle");
                    st.motion = EnsureDummyClip("Assets/PCSS/Controllers/Idle_Dummy.anim");
                    layer.stateMachine.defaultState = st;
                    changes++;
                }
            }

            EditorUtility.SetDirty(ctrl);
            return changes;
        }

        private static int SanitizeExpressionMenu(VRCExpressionsMenu menu, VRCExpressionParameters ep)
        {
            int changes = 0;
            if (menu == null) return changes;

            if (menu.controls == null)
            {
                menu.controls = new System.Collections.Generic.List<VRCExpressionsMenu.Control>();
                EditorUtility.SetDirty(menu);
                return 1;
            }

            // Remove null/empty name controls
            int before = menu.controls.Count;
            menu.controls.RemoveAll(c => c == null || string.IsNullOrEmpty(c.name));
            if (menu.controls.Count != before) changes++;

            foreach (var ctrl in menu.controls)
            {
                if (ctrl == null) continue;
                // Ensure parameter object exists when needed
                if (ctrl.parameter == null)
                {
                    ctrl.parameter = new VRCExpressionsMenu.Control.Parameter { name = ctrl.name };
                    changes++;
                }

                // Ensure subParameters length matches control type expectation
                int need = RequiredSubParamCount(ctrl.type);
                if (need > 0)
                {
                    if (ctrl.subParameters == null || ctrl.subParameters.Length != need)
                    {
                        var arr = new VRCExpressionsMenu.Control.Parameter[need];
                        for (int i = 0; i < need; i++)
                        {
                            string pname = (ctrl.parameter != null && !string.IsNullOrEmpty(ctrl.parameter.name))
                                ? ctrl.parameter.name
                                : ($"PCSS_Auto_{ctrl.name}_{i}");
                            arr[i] = new VRCExpressionsMenu.Control.Parameter { name = pname };
                        }
                        ctrl.subParameters = arr;
                        changes++;
                    }
                }

                // Recurse into submenu
                if (ctrl.type == VRCExpressionsMenu.Control.ControlType.SubMenu && ctrl.subMenu != null)
                {
                    changes += SanitizeExpressionMenu(ctrl.subMenu, ep);
                }
            }

            EditorUtility.SetDirty(menu);
            return changes;
        }

        private static int RequiredSubParamCount(VRCExpressionsMenu.Control.ControlType type)
        {
            switch (type)
            {
                case VRCExpressionsMenu.Control.ControlType.RadialPuppet:
                    return 1;
                case VRCExpressionsMenu.Control.ControlType.TwoAxisPuppet:
                    return 2;
                case VRCExpressionsMenu.Control.ControlType.FourAxisPuppet:
                    return 4;
                default:
                    return 0;
            }
        }

        private static AnimationClip EnsureDummyClip(string path)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null)
            {
                clip = new AnimationClip { name = System.IO.Path.GetFileNameWithoutExtension(path) };
                EnsureControllersFolder();
                AssetDatabase.CreateAsset(clip, path);
            }
            return clip;
        }

        private static void EnsureControllersFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/PCSS")) AssetDatabase.CreateFolder("Assets", "PCSS");
            if (!AssetDatabase.IsValidFolder("Assets/PCSS/Controllers")) AssetDatabase.CreateFolder("Assets/PCSS", "Controllers");
        }
#endif
    }
}


