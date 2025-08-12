using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace lilToon.PCSS.Editor
{
    public static class LilToonFacialSanitizer
    {
        [MenuItem("Tools/lilToon-PCSS-Extension/VRChat/顔材質(lilToon)安全化", false, 261)]
        public static void Sanitize()
        {
            var avatar = Selection.activeGameObject;
            if (avatar == null)
            {
                EditorUtility.DisplayDialog("PCSS", "アバターのルートを選択してください。", "OK");
                return;
            }

            var renderers = avatar.GetComponentsInChildren<SkinnedMeshRenderer>(true)
                .Where(r => r != null && IsFaceRendererName(r.name))
                .ToList();

            if (renderers.Count == 0)
            {
                EditorUtility.DisplayDialog("PCSS", "顔系のSkinnedMeshRendererが見つかりませんでした。(nameに Face/Head/顔 を含むもの)", "OK");
                return;
            }

            int matCount = 0;
            Undo.RecordObjects(renderers.ToArray(), "Sanitize lilToon Facial Materials");
            foreach (var r in renderers)
            {
                var mats = r.sharedMaterials;
                if (mats == null) continue;

                // 重複割り当ての注意喚起
                if (mats.Length >= 2 && mats[0] != null && mats[1] != null && mats[0] == mats[1])
                {
                    Debug.LogWarning($"[PCSS] {r.name}: Element0 と Element1 に同一マテリアルが割り当てられています。赤み/重なり表現に影響する場合があります。");
                }

                foreach (var m in mats)
                {
                    if (m == null || m.shader == null) continue;
                    if (!m.shader.name.Contains("lilToon")) continue;

                    SanitizeLilToonFaceMaterial(m);
                    matCount++;
                }
            }

            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("PCSS", $"lilToon顔材質を安全化: {matCount} 件", "OK");
        }

        private static bool IsFaceRendererName(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            name = name.ToLowerInvariant();
            return name.Contains("face") || name.Contains("head") || name.Contains("顔");
        }

        private static void SanitizeLilToonFaceMaterial(Material mat)
        {
            // 代表的な色関連の安全値に戻す（存在する場合のみ）
            TrySetColor(mat, "_Color", Color.white);
            TrySetColor(mat, "_ShadeColor", new Color(0.9f, 0.9f, 0.9f, 1f));
            TrySetColor(mat, "_EmissionColor", Color.black);
            // しきい値系があれば、過度な影響を避ける方向へ
            TrySetFloat(mat, "_EmissionStrength", 0f);
            TrySetFloat(mat, "_ColorAdjust", 0f);
            TrySetFloat(mat, "_Contrast", 1f);
            TrySetFloat(mat, "_Saturation", 1f);

            EditorUtility.SetDirty(mat);
        }

        private static void TrySetColor(Material mat, string prop, Color val)
        {
            if (mat.HasProperty(prop)) mat.SetColor(prop, val);
        }

        private static void TrySetFloat(Material mat, string prop, float val)
        {
            if (mat.HasProperty(prop)) mat.SetFloat(prop, val);
        }
    }
}


