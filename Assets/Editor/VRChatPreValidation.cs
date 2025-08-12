using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace lilToon.PCSS.Editor
{
    public static class VRChatPreValidation
    {
        [MenuItem("Tools/lilToon-PCSS-Extension/VRChat/事前バリデーション実行", false, 202)]
        public static void Run()
        {
            var selected = Selection.activeGameObject;
            if (selected != null)
            {
                int fixes = ProcessAvatar(selected);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("PCSS", fixes > 0 ? $"事前バリデーション修復を {fixes} 件適用しました。\n再度バリデータを実行してください。" : "特に修復は不要でした。", "OK");
                return;
            }

            // 選択が無い場合はシーン内の全アバターを対象
            var all = Object.FindObjectsOfType<VRCAvatarDescriptor>(true);
            int totalFixes = 0;
            foreach (var a in all)
            {
                totalFixes += ProcessAvatar(a.gameObject);
            }
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("PCSS", totalFixes > 0 ? $"{all.Length}体に対して合計 {totalFixes} 件修復しました。\n再度バリデータを実行してください。" : "全アバターで修復は不要でした。", "OK");
        }

        private static int ProcessAvatar(GameObject root)
        {
            if (root == null) return 0;
            int fixes = 0;
            var desc = root.GetComponent<VRCAvatarDescriptor>();
            if (desc == null) return 0;

            Undo.RegisterFullObjectHierarchyUndo(root, "VRChat PreValidation Fix");

            // Animator 確保
            var animator = root.GetComponent<Animator>();
            if (animator == null)
            {
                animator = root.AddComponent<Animator>();
                fixes++;
            }

            // Missing Scripts 除去
            fixes += RemoveMissingScripts(root);

            // Disallowed/開発用成分のクリーンアップ
            try { VRChatCleanupMenu.CleanupDisallowedComponents(); fixes++; } catch { /* ignore */ }

            // 各Playable Layerのコントローラ修復
            fixes += FixLayers(desc.baseAnimationLayers);
            fixes += FixLayers(desc.specialAnimationLayers);

            // Expression Parameters のNULL条項除去 + 上限/重複/無名の整形
            try
            {
                var @params = desc.expressionParameters;
                if (@params != null && @params.parameters != null)
                {
                    var list = @params.parameters
                        .Where(p => p != null && !string.IsNullOrEmpty(p.name))
                        .GroupBy(p => p.name)
                        .Select(g => g.First())
                        .ToList();

                    // 上限16に収める
                    const int MaxParams = 16;
                    if (list.Count > MaxParams) list = list.Take(MaxParams).ToList();

                    foreach (var p in list)
                    {
                        p.defaultValue = Mathf.Clamp01(p.defaultValue);
                    }

                    if (!Enumerable.SequenceEqual(list, @params.parameters))
                    {
                        @params.parameters = list.ToArray();
                        EditorUtility.SetDirty(@params);
                        fixes++;
                    }
                }
            }
            catch { /* ignore */ }

            return fixes;
        }

        private static int FixLayers(VRCAvatarDescriptor.CustomAnimLayer[] layers)
        {
            int fixes = 0;
            if (layers == null) return fixes;
            for (int idx = 0; idx < layers.Length; idx++)
            {
                var layer = layers[idx];
                // struct のため null 比較不可。デフォルト層 or コントローラ未設定はスキップ
                if (layer.isDefault)
                {
                    // デフォルト層はSDK内部のデフォルトでOK
                    continue;
                }

                // カスタム層だがコントローラ未設定なら空コントローラを自動生成して割り当て
                if (layer.animatorController == null)
                {
                    string dir = "Assets/PCSS/Controllers";
                    if (!AssetDatabase.IsValidFolder("Assets/PCSS")) AssetDatabase.CreateFolder("Assets", "PCSS");
                    if (!AssetDatabase.IsValidFolder(dir)) AssetDatabase.CreateFolder("Assets/PCSS", "Controllers");
                    string safeName = string.IsNullOrEmpty(layer.mask?.name) ? $"Layer_{idx}" : layer.mask.name.Replace('/', '_');
                    string path = AssetDatabase.GenerateUniqueAssetPath($"{dir}/Auto_{safeName}.controller");
                    var created = new AnimatorController { name = $"Auto_{safeName}" };
                    AssetDatabase.CreateAsset(created, path);
                    AssetDatabase.SaveAssets();
                    layer.animatorController = created;
                    layers[idx] = layer; // 構造体なので書き戻す
                    fixes++;
                }

                var ctrl = layer.animatorController as AnimatorController;
                if (ctrl == null) continue;

                if (ctrl.layers == null || ctrl.layers.Length == 0)
                {
                    var sm = new AnimatorStateMachine { name = "BaseSM" };
                    var newLayer = new AnimatorControllerLayer
                    {
                        name = string.IsNullOrEmpty(layer.mask?.name) ? "BaseLayer" : layer.mask.name,
                        stateMachine = sm,
                        defaultWeight = 1f
                    };
                    ctrl.AddLayer(newLayer);
                    fixes++;
                }

                for (int i = 0; i < ctrl.layers.Length; i++)
                {
                    var l = ctrl.layers[i];
                    if (l.stateMachine == null)
                    {
                        l.stateMachine = new AnimatorStateMachine { name = $"SM_{l.name}" };
                        ctrl.layers[i] = l;
                        fixes++;
                    }
                    // ステートマシンの完全性を再帰的に修復（モーションnull/BlendTree子ゼロを埋める）
                    if (l.stateMachine != null)
                    {
                        fixes += FixStateMachineDeep(l.stateMachine, ctrl);
                    }
                    if (l.stateMachine.states == null || l.stateMachine.states.Length == 0)
                    {
                        var idle = l.stateMachine.AddState("Idle");
                        // デフォルトステートと空モーションを強制付与
                        l.stateMachine.defaultState = idle;
                        var empty = EnsureEmbeddedEmptyClip(ctrl);
                        if (idle.motion == null && empty != null)
                        {
                            idle.motion = empty;
                        }
                        ctrl.layers[i] = l;
                        fixes++;
                    }
                    if (l.stateMachine.defaultState == null && l.stateMachine.states.Length > 0)
                    {
                        l.stateMachine.defaultState = l.stateMachine.states[0].state;
                        // defaultStateにモーションが無ければ空クリップを割り当て
                        var empty = EnsureEmbeddedEmptyClip(ctrl);
                        if (l.stateMachine.defaultState != null && l.stateMachine.defaultState.motion == null && empty != null)
                        {
                            l.stateMachine.defaultState.motion = empty;
                        }
                        ctrl.layers[i] = l;
                        fixes++;
                    }
                    // 既存各ステートにモーションが一切無い場合の保険
                    if (l.stateMachine != null && l.stateMachine.states != null && l.stateMachine.states.Length > 0)
                    {
                        bool anyMotion = false;
                        for (int s = 0; s < l.stateMachine.states.Length; s++)
                        {
                            var st = l.stateMachine.states[s].state;
                            if (st != null && st.motion != null) { anyMotion = true; break; }
                        }
                        if (!anyMotion)
                        {
                            var empty = EnsureEmbeddedEmptyClip(ctrl);
                            var target = l.stateMachine.defaultState ?? l.stateMachine.states[0].state;
                            if (target != null && target.motion == null && empty != null)
                            {
                                target.motion = empty;
                                ctrl.layers[i] = l;
                                fixes++;
                            }
                        }
                    }
                }
                EditorUtility.SetDirty(ctrl);
            }
            return fixes;
        }

        // ステートマシンを再帰的にめぐって、モーションnullやBlendTree子ゼロを修復
        private static int FixStateMachineDeep(AnimatorStateMachine sm, AnimatorController ctrl)
        {
            int fixes = 0;
            if (sm == null) return 0;
            var empty = EnsureEmbeddedEmptyClip(ctrl);

            // ステートのモーション保証
            if (sm.states != null)
            {
                foreach (var child in sm.states)
                {
                    var st = child.state;
                    if (st == null) continue;
                    if (st.motion == null && empty != null)
                    {
                        st.motion = empty; fixes++;
                    }
                    // BlendTreeの子ゼロ対応
                    var bt = st.motion as BlendTree;
                    if (bt != null)
                    {
                        if (bt.children == null || bt.children.Length == 0)
                        {
                            bt.blendType = BlendTreeType.Simple1D;
                            bt.blendParameter = string.IsNullOrEmpty(bt.blendParameter) ? "Blend" : bt.blendParameter;
                            bt.useAutomaticThresholds = false;
                            bt.children = new ChildMotion[]
                            {
                                new ChildMotion{ motion = empty, threshold = 0f }
                            };
                            fixes++;
                        }
                    }
                }
            }

            // サブステートマシンを再帰
            if (sm.stateMachines != null)
            {
                foreach (var sub in sm.stateMachines)
                {
                    if (sub.stateMachine != null)
                    {
                        fixes += FixStateMachineDeep(sub.stateMachine, ctrl);
                    }
                }
            }
            return fixes;
        }

        // コントローラ内に安全な空クリップを1つだけ内包し再利用
        private static AnimationClip EnsureEmbeddedEmptyClip(AnimatorController ctrl)
        {
            if (ctrl == null) return null;
            // 既存検索
            foreach (var obj in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(ctrl)))
            {
                var clip = obj as AnimationClip;
                if (clip != null && clip.name == "PCSS_EmptyClip") return clip;
            }
            // 新規作成
            var newClip = new AnimationClip { name = "PCSS_EmptyClip" };
            AssetDatabase.AddObjectToAsset(newClip, ctrl);
            EditorUtility.SetDirty(ctrl);
            AssetDatabase.SaveAssets();
            return newClip;
        }

        private static int RemoveMissingScripts(GameObject root)
        {
            int removed = 0;
            var transforms = root.GetComponentsInChildren<Transform>(true);
            foreach (var t in transforms)
            {
                removed += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);
            }
            return removed;
        }
    }
}


