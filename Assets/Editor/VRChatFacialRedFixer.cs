using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace lilToon.PCSS.Editor
{
    public static class VRChatFacialRedFixer
    {
        private static readonly string[] ProblemKeys =
        {
            "material._Color",
            "material._ShadeColor",
            "material._EmissionColor",
            "material._RimColor",
            "material._MainTex_ST",
        };

        [MenuItem("Tools/lilToon-PCSS-Extension/VRChat/顔赤みスキャナ＆修正", false, 260)]
        public static void ScanAndFix()
        {
            var avatar = Selection.activeGameObject;
            if (avatar == null)
            {
                EditorUtility.DisplayDialog("PCSS", "アバターのルートを選択してください。", "OK");
                return;
            }

            var controllers = FindAvatarControllers(avatar);
            if (controllers.Count == 0)
            {
                EditorUtility.DisplayDialog("PCSS", "AnimatorController(FX/Gesture) が見つかりませんでした。", "OK");
                return;
            }

            var hits = new List<(AnimationClip clip, EditorCurveBinding binding)>();
            foreach (var ctrl in controllers)
            {
                foreach (var clip in ctrl.animationClips.Distinct())
                {
                    if (clip == null) continue;
                    foreach (var binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
                    {
                        // テクスチャ差し替えはスキップ
                    }
                    foreach (var binding in AnimationUtility.GetCurveBindings(clip))
                    {
                        if (IsProblemBinding(binding))
                        {
                            hits.Add((clip, binding));
                        }
                    }
                }
            }

            if (hits.Count == 0)
            {
                EditorUtility.DisplayDialog("PCSS", "赤み誘発の疑いがある色系キーフレームは見つかりませんでした。", "OK");
                return;
            }

            if (!EditorUtility.DisplayDialog("PCSS", $"問題のキーを {hits.Count} 件検出。削除バックアップを作成して修正しますか？", "修正する", "キャンセル"))
            {
                return;
            }

            string backupDir = CreateBackupDir();
            int removed = 0;
            foreach (var g in hits.GroupBy(h => h.clip))
            {
                var clip = g.Key;
                BackupClip(clip, backupDir);
                foreach (var b in g.Select(x => x.binding))
                {
                    AnimationUtility.SetEditorCurve(clip, b, null);
                    removed++;
                }
                EditorUtility.SetDirty(clip);
            }
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("PCSS", $"修正完了: {removed} キー削除／バックアップ: {backupDir}", "OK");
        }

        private static bool IsProblemBinding(EditorCurveBinding binding)
        {
            if (binding.type != typeof(Renderer) && binding.type != typeof(SkinnedMeshRenderer)) return false;
            var name = binding.propertyName;
            if (string.IsNullOrEmpty(name)) return false;
            // material.Color (x/y/z/w) のような型もヒットさせる
            if (name.StartsWith("material."))
            {
                // ピクセルに影響の強い代表キーだけ対象
                return ProblemKeys.Any(k => name.StartsWith(k));
            }
            return false;
        }

        private static List<AnimatorController> FindAvatarControllers(GameObject avatar)
        {
            var list = new List<AnimatorController>();
            foreach (var animator in avatar.GetComponentsInChildren<Animator>(true))
            {
                var ctrl = animator.runtimeAnimatorController as AnimatorController;
                if (ctrl != null) list.Add(ctrl);
            }
            return list.Distinct().ToList();
        }

        private static string CreateBackupDir()
        {
            var dir = "Assets/PCSS/Backups/FacialRedFix";
            if (!AssetDatabase.IsValidFolder("Assets/PCSS")) AssetDatabase.CreateFolder("Assets", "PCSS");
            if (!AssetDatabase.IsValidFolder("Assets/PCSS/Backups")) AssetDatabase.CreateFolder("Assets/PCSS", "Backups");
            if (!AssetDatabase.IsValidFolder(dir)) AssetDatabase.CreateFolder("Assets/PCSS/Backups", "FacialRedFix");
            return dir;
        }

        private static void BackupClip(AnimationClip clip, string backupDir)
        {
            if (clip == null) return;
            var path = AssetDatabase.GetAssetPath(clip);
            if (string.IsNullOrEmpty(path)) return;
            var name = Path.GetFileNameWithoutExtension(path);
            var copy = UnityEngine.Object.Instantiate(clip);
            copy.name = name + "_backup";
            AssetDatabase.CreateAsset(copy, AssetDatabase.GenerateUniqueAssetPath($"{backupDir}/{copy.name}.anim"));
        }
    }
}


