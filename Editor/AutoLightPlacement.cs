using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// アバターのHips/Headを基準に身長を推定し、Hip追従アンカー配下に外部ライトを自動配置
    /// </summary>
    public static class AutoLightPlacement
    {
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/外部ライト自動配置", false, 25)]
        public static void AutoPlaceExternalLights()
        {
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject == null)
            {
                EditorUtility.DisplayDialog("エラー", "アバターを選択してください。", "OK");
                return;
            }

            // アバターの身長を計算
            float avatarHeight = CalculateAvatarHeight(selectedObject);
            if (avatarHeight <= 0)
            {
                EditorUtility.DisplayDialog("エラー", "アバターの身長を計算できませんでした。", "OK");
                return;
            }

            Debug.Log($"[PCSS] アバター身長: {avatarHeight:F2}m");

            // 最適なライト位置を計算
            Vector3[] optimalPositions = CalculateOptimalLightPositions(avatarHeight);

            // ライトを配置
            CreateExternalLights(selectedObject, optimalPositions);

            EditorUtility.DisplayDialog("完了", $"アバター身長 {avatarHeight:F2}m に基づいて外部ライトを配置しました。", "OK");
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/外部ライト自動配置", true)]
        public static bool ValidateAutoPlaceExternalLights()
        {
            return Selection.activeGameObject != null;
        }

        private static float CalculateAvatarHeight(GameObject avatar)
        {
            var animator = avatar.GetComponent<Animator>();
            if (animator == null)
            {
                return 0f;
            }

            Transform head = animator.GetBoneTransform(HumanBodyBones.Head);
            Transform hips = animator.GetBoneTransform(HumanBodyBones.Hips);
            if (head == null || hips == null)
            {
                return 0f;
            }

            float torso = Mathf.Abs(head.position.y - hips.position.y);
            float leg = EstimateLegLength(animator);
            if (leg <= 0f)
            {
                // 胴長からの近似（安定係数）
                return Mathf.Max(0.3f, torso * 1.8f);
            }

            const float footSoleOffset = 0.05f; // 足底の厚み推定
            return Mathf.Max(0.3f, torso + leg + footSoleOffset);
        }

        private static Vector3[] CalculateOptimalLightPositions(float avatarHeight)
        {
            // アバター身長に応じたシンプルな3点＋リムの配置
            float baseDistance = avatarHeight * 2.5f;   // 前後左右距離
            float heightOffset = avatarHeight * 0.8f;   // 高さ

            return new[]
            {
                new Vector3(0, heightOffset,  baseDistance),          // Main
                new Vector3(0, heightOffset, -baseDistance * 0.8f),    // Rim
                new Vector3(-baseDistance * 0.7f, heightOffset, 0),    // Fill L
                new Vector3( baseDistance * 0.7f, heightOffset, 0),    // Fill R
            };
        }

        private static void CreateExternalLights(GameObject avatar, Vector3[] positions)
        {
            const string parentGroupName = "PCSS External Lights (Auto)";

            // 既存グループがあれば削除
            var existingGroup = avatar.transform.Find(parentGroupName);
            if (existingGroup != null)
            {
                Undo.DestroyObjectImmediate(existingGroup.gameObject);
            }

            // グループ作成
            var group = new GameObject(parentGroupName);
            Undo.RegisterCreatedObjectUndo(group, $"Create {parentGroupName}");
            group.transform.SetParent(avatar.transform, false);

            // Hip 追従アンカー（MA Bone Proxy を使えれば付与）
            var anchor = new GameObject("HipAnchor (MA Bone Proxy)");
            Undo.RegisterCreatedObjectUndo(anchor, "Create Hip Anchor");
            anchor.transform.SetParent(group.transform, false);
            anchor.transform.localPosition = Vector3.zero;
            anchor.transform.localRotation = Quaternion.identity;
            TryAttachModularAvatarBoneProxy(anchor, HumanBodyBones.Hips);

            string[] lightNames = { "Main Light", "Rim Light", "Fill Light Left", "Fill Light Right" };
            Color[] lightColors = { Color.white, new Color(0.8f, 0.8f, 1f), new Color(1f, 0.9f, 0.8f), new Color(0.8f, 1f, 0.9f) };
            float[] intensities = { 1.2f, 0.8f, 0.6f, 0.6f };
            LightType[] lightTypes = { LightType.Spot, LightType.Spot, LightType.Spot, LightType.Spot };

            for (int i = 0; i < positions.Length; i++)
            {
                var lightObject = new GameObject($"PCSS External {lightNames[i]}");
                Undo.RegisterCreatedObjectUndo(lightObject, $"Create {lightNames[i]}");

                // アンカー配下に配置
                lightObject.transform.SetParent(anchor.transform, false);
                lightObject.transform.localPosition = positions[i];
                lightObject.transform.LookAt(anchor.transform.position + Vector3.up * positions[i].y);

                // Light
                var light = lightObject.AddComponent<Light>();
                light.type = lightTypes[i];
                light.color = lightColors[i];
                light.intensity = intensities[i];
                light.range = avatar.transform.localScale.magnitude * 5f;
                light.spotAngle = 45f;
                light.shadows = LightShadows.Soft;
                light.shadowStrength = 0.8f;
                light.shadowNormalBias = 0.1f;
                light.cullingMask = 1; // Default layer only

                // ランタイムPCSS制御（存在すれば）
                try
                {
                    var ctrlType = Type.GetType("lilToon.PCSS.Runtime.ModularAvatarPCSSController, Assembly-CSharp");
                    if (ctrlType == null)
                    {
                        // もう一つのasmdef配置の可能性も考慮
                        ctrlType = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(a => { try { return a.GetTypes(); } catch { return Type.EmptyTypes; } })
                            .FirstOrDefault(t => t.FullName == "lilToon.PCSS.Runtime.ModularAvatarPCSSController");
                    }
                    if (ctrlType != null)
                    {
                        var comp = lightObject.AddComponent(ctrlType);
                        // よくあるプロパティ名にアタッチ（存在しない場合は無視）
                        TrySet(comp, "RealtimeQuality", Mathf.Clamp01(intensities[i] / 2.0f));
                        TrySet(comp, "AutoLightManagement", true);
                        TrySet(comp, "EnableLOD", true);
                    }
                }
                catch { /* ignore */ }

                Debug.Log($"[PCSS] 外部ライト '{lightNames[i]}' を配置: {positions[i]}");
            }

            Selection.activeGameObject = group;
        }

        // ---- Helpers ----
        private static float EstimateLegLength(Animator animator)
        {
            var upper = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
            var lower = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
            var foot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            if (upper == null || lower == null || foot == null) return 0f;

            float thigh = Vector3.Distance(upper.position, lower.position);
            float shin = Vector3.Distance(lower.position, foot.position);
            const float ankleToSole = 0.07f; // 足首→足底の推定
            return Mathf.Max(0f, thigh + shin + ankleToSole);
        }

        private static bool TryAttachModularAvatarBoneProxy(GameObject target, HumanBodyBones bone)
        {
            try
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var proxyType = assemblies
                    .SelectMany(a => { try { return a.GetTypes(); } catch { return Type.EmptyTypes; } })
                    .FirstOrDefault(t => t.FullName == "nadena.dev.modular_avatar.core.ModularAvatarBoneProxy");
                if (proxyType == null) return false;

                var comp = target.AddComponent(proxyType);
                var member = proxyType.GetMember("bone", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                      .FirstOrDefault();
                if (member is FieldInfo fi && fi.FieldType == typeof(HumanBodyBones))
                {
                    fi.SetValue(comp, bone);
                }
                else if (member is PropertyInfo pi && pi.CanWrite && pi.PropertyType == typeof(HumanBodyBones))
                {
                    pi.SetValue(comp, bone);
                }
                return true;
            }
            catch
            {
                return false;
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
        }
    }
}
