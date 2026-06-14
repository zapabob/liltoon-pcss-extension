using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace lilToon.PCSS.Editor
{
    public static class HipBasedLightPlacement
    {
        private const string GroupName = "PCSS Hip Lights (MA)";
        private const string AnchorName = "HipAnchor (MA Bone Proxy)";

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/ライト/Hip基準ライト配置 (MA)", false, 41)]
        public static void CreateHipBasedLightsFromMenu()
        {
            var avatar = Selection.activeGameObject;
            if (avatar == null)
            {
                EditorUtility.DisplayDialog("PCSS", "アバターのルートを選択してください。", "OK");
                return;
            }
            CreateHipBasedLights(avatar);
        }

        public static void CreateHipBasedLights(GameObject avatarRoot)
        {
            if (avatarRoot == null) return;

            // 既存があれば削除して作り直す
            var existing = avatarRoot.transform.Find(GroupName);
            if (existing != null)
            {
                Undo.DestroyObjectImmediate(existing.gameObject);
            }

            var group = new GameObject(GroupName);
            Undo.RegisterCreatedObjectUndo(group, "Create Hip Lights Group");
            group.transform.SetParent(avatarRoot.transform, false);
            group.transform.localPosition = Vector3.zero;
            group.transform.localRotation = Quaternion.identity;
            group.transform.localScale = Vector3.one;

            // Hipに追従するアンカー（Modular Avatar Bone Proxy）
            var anchor = new GameObject(AnchorName);
            Undo.RegisterCreatedObjectUndo(anchor, "Create Hip Anchor");
            anchor.transform.SetParent(group.transform, false);
            anchor.transform.localPosition = Vector3.zero;
            anchor.transform.localRotation = Quaternion.identity;

            var animator = avatarRoot.GetComponent<Animator>();
            Transform hips = animator != null ? animator.GetBoneTransform(HumanBodyBones.Hips) : null;
            if (!TryAttachModularAvatarBoneProxy(anchor, hips, HumanBodyBones.Hips))
            {
                EditorUtility.DisplayDialog(
                    "PCSS",
                    "Modular Avatar が見つかりませんでした。\nHip追従には MA の導入が必要です。\n（アンカーは通常のTransformとして作成しました）",
                    "OK");
            }

            // 見栄え用ライト 3点（Key / Fill / Rim）。Hip基準で相対配置
            CreateSpotLight(anchor.transform, "Key Light",  new Vector3(0.35f, 0.65f, 0.65f), new Vector3(10f,  -25f, 0f), 1.4f, 60f, 8f, Color.white);
            CreateSpotLight(anchor.transform, "Fill Light", new Vector3(-0.45f, 0.55f, 0.55f), new Vector3(8f,  -20f, 0f), 0.8f, 75f, 7f,  new Color(0.95f, 0.97f, 1f));
            CreateSpotLight(anchor.transform, "Rim Light",  new Vector3(0.00f, 0.70f, -0.60f), new Vector3(-6f, 180f, 0f), 1.1f, 70f, 9f, new Color(1f, 0.98f, 0.95f));

            Selection.activeObject = group;
            EditorGUIUtility.PingObject(group);
            Debug.Log("[PCSS] Hip基準ライト配置 (MA) を作成しました。");
        }

        private static void CreateSpotLight(Transform parent, string name, Vector3 localPos, Vector3 localEuler, float intensity, float spotAngle, float range, Color color)
        {
            var go = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(go, "Create Spot Light");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localRotation = Quaternion.Euler(localEuler);

            var light = go.AddComponent<Light>();
            light.type = LightType.Spot;
            light.color = color;
            light.intensity = intensity; // 初期強度を設定
            light.range = range;
            light.spotAngle = spotAngle;
            light.shadows = LightShadows.Soft;

            // スケール連動のためにAnimatorにScaleFactorを渡す
            var animator = parent.root.GetComponent<Animator>();
            if (animator != null)
            {
                // パラメータが存在するか確認
                bool hasScaleFactor = animator.parameters.Any(p => p.name == "ScaleFactor");
                if (hasScaleFactor)
                {
                    // 何もしない（Animatorが制御する）
                }
            }
        }

        private static bool TryAttachModularAvatarBoneProxy(GameObject target, Transform targetBone, HumanBodyBones bone)
        {
            try
            {
                // リフレクションで MA Bone Proxy を探す（依存が無くてもコンパイル可にする）
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var proxyType = assemblies
                    .SelectMany(a =>
                    {
                        try { return a.GetTypes(); } catch { return Array.Empty<Type>(); }
                    })
                    .FirstOrDefault(t => t.FullName == "nadena.dev.modular_avatar.core.ModularAvatarBoneProxy");

                if (proxyType == null) return false;

                var comp = target.AddComponent(proxyType);
                bool assignedTarget =
                    TrySetReflected(comp, "target", targetBone) ||
                    TrySetReflected(comp, "targetObject", targetBone != null ? targetBone.gameObject : null) ||
                    TrySetReflected(comp, "targetTransform", targetBone);
                bool assignedBone =
                    TrySetReflected(comp, "boneReference", bone) ||
                    TrySetReflected(comp, "bone", bone);

                EditorUtility.SetDirty(comp);
                return assignedTarget || assignedBone;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[PCSS] Modular Avatar Bone Proxyの接続に失敗しました: {e.Message}");
                return false;
            }
        }

        private static bool TrySetReflected(object component, string memberName, object value)
        {
            if (component == null) return false;

            Type type = component.GetType();
            FieldInfo field = type.GetField(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null && (value == null || field.FieldType.IsAssignableFrom(value.GetType())))
            {
                field.SetValue(component, value);
                return true;
            }

            PropertyInfo property = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property != null && property.CanWrite && (value == null || property.PropertyType.IsAssignableFrom(value.GetType())))
            {
                property.SetValue(component, value);
                return true;
            }

            return false;
        }
    }
}
