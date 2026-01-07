#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDK3.Dynamics.PhysBone.Components;
#endif

namespace lilToon.PCSS.Editor
{
    internal static class PCSSHubSetup
    {
        internal enum LightPreset
        {
            Anime = 0,
            Realistic = 1,
            Cinematic = 2
        }

        internal const string LightGroupName = "PCSS Hip Lights (MA)";
        internal const string HipAnchorName = "HipAnchor (MA Bone Proxy)";

        internal static bool TryGetHumanoidAnimator(GameObject avatarRoot, out Animator animator, out string error)
        {
            animator = null;
            error = string.Empty;
            if (avatarRoot == null)
            {
                error = "アバターのルートを選択してください。";
                return false;
            }

            animator = avatarRoot.GetComponent<Animator>();
            if (animator == null)
            {
                error = "Animator が見つかりません。Humanoid アバターを選択してください。";
                return false;
            }

            if (!animator.isHuman)
            {
                error = "Humanoid アバターのみ対応です。";
                return false;
            }

            return true;
        }

        internal static float EstimateAvatarHeight(Animator animator)
        {
            if (animator == null) return 0f;

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
                return Mathf.Max(0.3f, torso * 1.8f);
            }

            const float footSoleOffset = 0.05f;
            return Mathf.Max(0.3f, torso + leg + footSoleOffset);
        }

        internal static bool TryApplyPcssMaterials(GameObject avatarRoot, bool enableVrcLightVolumes, bool includeNonStandard, LightPreset preset, bool applyShadowPreset, out int updated, out int alreadyPcss, out int total, out string error)
        {
            updated = 0;
            alreadyPcss = 0;
            total = 0;
            error = string.Empty;

            if (avatarRoot == null)
            {
                error = "アバターが選択されていません。";
                return false;
            }

            Shader pcssShader = Shader.Find("lilToon/PCSS Extension");
            if (pcssShader == null)
            {
                error = "lilToon/PCSS Extension シェーダーが見つかりません。";
                return false;
            }

            Renderer[] renderers = avatarRoot.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                Material[] materials = renderer.sharedMaterials;
                foreach (Material material in materials)
                {
                    if (material == null || material.shader == null) continue;

                    string shaderName = material.shader.name;
                    if (!IsLilToonShader(shaderName)) continue;
                    if (!includeNonStandard && !IsLilToonStandardShader(shaderName)) continue;

                    total++;
                    bool isPcss = material.shader == pcssShader || shaderName.IndexOf("PCSS", StringComparison.OrdinalIgnoreCase) >= 0;
                    if (!isPcss)
                    {
                        material.shader = pcssShader;
                        updated++;
                    }
                    else
                    {
                        alreadyPcss++;
                    }

                    if (material.HasProperty("_UsePCSS"))
                    {
                        material.SetFloat("_UsePCSS", 1.0f);
                    }

                    if (applyShadowPreset)
                    {
                        ApplyShadowPreset(material, preset);
                    }

                    if (enableVrcLightVolumes)
                    {
                        EnableVrcLightVolumeProperties(material);
                    }

                    EditorUtility.SetDirty(material);
                }
            }

            return true;
        }

        internal static bool TryCreateHipLightRig(GameObject avatarRoot, Animator animator, float height, LightPreset preset, bool enablePhysBoneSway, bool addHandColliders, out List<Light> lights, out bool usedBoneProxy, out bool usedPhysBone, out string error)
        {
            lights = new List<Light>();
            usedBoneProxy = false;
            usedPhysBone = false;
            error = string.Empty;

            if (avatarRoot == null || animator == null)
            {
                error = "アバターまたはAnimatorが見つかりません。";
                return false;
            }

            Transform hips = animator.GetBoneTransform(HumanBodyBones.Hips);
            if (hips == null)
            {
                error = "Hips ボーンが見つかりません。";
                return false;
            }
            Transform head = animator.GetBoneTransform(HumanBodyBones.Head) ?? hips;

            Transform existing = avatarRoot.transform.Find(LightGroupName);
            if (existing != null)
            {
                Undo.DestroyObjectImmediate(existing.gameObject);
            }

            GameObject group = new GameObject(LightGroupName);
            Undo.RegisterCreatedObjectUndo(group, "Create PCSS Light Rig");
            group.transform.SetParent(avatarRoot.transform, false);
            group.transform.localPosition = Vector3.zero;
            group.transform.localRotation = Quaternion.identity;
            group.transform.localScale = Vector3.one;

            GameObject anchor = new GameObject(HipAnchorName);
            Undo.RegisterCreatedObjectUndo(anchor, "Create Hip Anchor");
            anchor.transform.SetParent(group.transform, false);
            anchor.transform.localPosition = Vector3.zero;
            anchor.transform.localRotation = Quaternion.identity;

            if (TryAttachModularAvatarBoneProxy(anchor, HumanBodyBones.Hips))
            {
                usedBoneProxy = true;
            }
            else
            {
                usedBoneProxy = false;
                group.transform.SetParent(hips, false);
                group.transform.localPosition = Vector3.zero;
                group.transform.localRotation = Quaternion.identity;
                group.transform.localScale = Vector3.one;
            }

            float heightScale = Mathf.Max(0.5f, height);
            LightPresetDefinition def = GetPresetDefinition(preset);

            float up = heightScale * def.UpMultiplier;
            float forward = heightScale * def.ForwardMultiplier;
            float side = heightScale * def.SideMultiplier;
            float back = heightScale * def.BackMultiplier;

            Transform lightParent = anchor.transform;
            if (enablePhysBoneSway)
            {
#if VRC_SDK_VRCSDK3
                GameObject swayRoot = new GameObject("PB_LightSway");
                Undo.RegisterCreatedObjectUndo(swayRoot, "Create PB Light Sway");
                swayRoot.transform.SetParent(anchor.transform, false);
                swayRoot.transform.localPosition = Vector3.zero;
                swayRoot.transform.localRotation = Quaternion.identity;

                GameObject tip = new GameObject("PB_LightTip");
                Undo.RegisterCreatedObjectUndo(tip, "Create PB Light Tip");
                tip.transform.SetParent(swayRoot.transform, false);
                tip.transform.localPosition = new Vector3(0f, heightScale * 0.10f, heightScale * 0.35f);

                var physBone = swayRoot.AddComponent<VRCPhysBone>();
                physBone.rootTransform = swayRoot.transform;
                physBone.pull = 0.8f;
                physBone.pullCurve = AnimationCurve.Linear(0f, 0.8f, 1f, 0.8f);
                physBone.spring = 0.2f;
                physBone.immobile = 0.0f;
                physBone.stretchMotion = 0.1f;
                physBone.allowGrabbing = CreateAdvancedBool(true);
                physBone.allowPosing = CreateAdvancedBool(true);
                physBone.parameter = "PB_Light";

                if (addHandColliders)
                {
                    var colliders = new List<VRC.Dynamics.VRCPhysBoneColliderBase>();
                    Transform leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                    Transform rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
                    TryEnsureHandCollider(leftHand, colliders);
                    TryEnsureHandCollider(rightHand, colliders);
                    if (colliders.Count > 0)
                    {
                        physBone.colliders = colliders;
                    }
                }

                var avatarDesc = avatarRoot.GetComponent<VRCAvatarDescriptor>();
                EnsureExpressionParameter(avatarDesc, "PB_Light", VRCExpressionParameters.ValueType.Float, saved: true);

                usedPhysBone = true;
                lightParent = tip.transform;
#else
                error = "PhysBone揺れはVRChat SDKが必要です。";
                return false;
#endif
            }

            Light key = CreateSpotLight(
                lightParent,
                "Key Light",
                new Vector3(heightScale * 0.35f, up, forward),
                head,
                new Vector3(10f, -25f, 0f),
                def.KeyIntensity,
                def.KeySpotAngle,
                heightScale * def.KeyRangeMultiplier,
                def.KeyColor,
                def.ShadowStrength);

            Light fill = CreateSpotLight(
                lightParent,
                "Fill Light",
                new Vector3(-side, up * 0.85f, forward * 0.8f),
                head,
                new Vector3(8f, -20f, 0f),
                def.FillIntensity,
                def.FillSpotAngle,
                heightScale * def.FillRangeMultiplier,
                def.FillColor,
                def.ShadowStrength);

            Light rim = CreateSpotLight(
                lightParent,
                "Rim Light",
                new Vector3(0.0f, up * def.RimUpMultiplier, -back),
                head,
                new Vector3(-6f, 180f, 0f),
                def.RimIntensity,
                def.RimSpotAngle,
                heightScale * def.RimRangeMultiplier,
                def.RimColor,
                def.ShadowStrength);

            if (key != null) lights.Add(key);
            if (fill != null) lights.Add(fill);
            if (rim != null) lights.Add(rim);

            return true;
        }

        private static Light CreateSpotLight(Transform parent, string name, Vector3 localPos, Transform lookAt, Vector3 fallbackEuler, float intensity, float spotAngle, float range, Color color, float shadowStrength)
        {
            GameObject go = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(go, "Create PCSS Light");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            if (lookAt != null)
            {
                go.transform.LookAt(lookAt.position, Vector3.up);
            }
            else
            {
                go.transform.localRotation = Quaternion.Euler(fallbackEuler);
            }

            Light light = go.AddComponent<Light>();
            light.type = LightType.Spot;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            light.spotAngle = spotAngle;
            light.shadows = LightShadows.Soft;
            light.shadowStrength = Mathf.Clamp01(shadowStrength);
            light.shadowNormalBias = 0.1f;
            return light;
        }

        private struct LightPresetDefinition
        {
            public float KeyIntensity;
            public float FillIntensity;
            public float RimIntensity;
            public float KeySpotAngle;
            public float FillSpotAngle;
            public float RimSpotAngle;
            public float KeyRangeMultiplier;
            public float FillRangeMultiplier;
            public float RimRangeMultiplier;
            public float UpMultiplier;
            public float ForwardMultiplier;
            public float SideMultiplier;
            public float BackMultiplier;
            public float RimUpMultiplier;
            public float ShadowStrength;
            public Color KeyColor;
            public Color FillColor;
            public Color RimColor;
        }

        private static LightPresetDefinition GetPresetDefinition(LightPreset preset)
        {
            switch (preset)
            {
                case LightPreset.Anime:
                    return new LightPresetDefinition
                    {
                        KeyIntensity = 1.2f,
                        FillIntensity = 1.0f,
                        RimIntensity = 0.9f,
                        KeySpotAngle = 75f,
                        FillSpotAngle = 85f,
                        RimSpotAngle = 80f,
                        KeyRangeMultiplier = 2.4f,
                        FillRangeMultiplier = 2.3f,
                        RimRangeMultiplier = 2.6f,
                        UpMultiplier = 0.55f,
                        ForwardMultiplier = 0.60f,
                        SideMultiplier = 0.35f,
                        BackMultiplier = 0.45f,
                        RimUpMultiplier = 0.85f,
                        ShadowStrength = 0.7f,
                        KeyColor = new Color(1.0f, 0.98f, 0.95f),
                        FillColor = Color.white,
                        RimColor = new Color(0.95f, 0.97f, 1.0f)
                    };
                case LightPreset.Cinematic:
                    return new LightPresetDefinition
                    {
                        KeyIntensity = 1.6f,
                        FillIntensity = 0.5f,
                        RimIntensity = 1.4f,
                        KeySpotAngle = 50f,
                        FillSpotAngle = 65f,
                        RimSpotAngle = 55f,
                        KeyRangeMultiplier = 3.2f,
                        FillRangeMultiplier = 2.8f,
                        RimRangeMultiplier = 3.4f,
                        UpMultiplier = 0.75f,
                        ForwardMultiplier = 0.90f,
                        SideMultiplier = 0.55f,
                        BackMultiplier = 0.80f,
                        RimUpMultiplier = 1.15f,
                        ShadowStrength = 0.95f,
                        KeyColor = new Color(1.0f, 0.95f, 0.90f),
                        FillColor = new Color(0.85f, 0.90f, 1.0f),
                        RimColor = new Color(1.0f, 0.98f, 0.90f)
                    };
                default:
                    return new LightPresetDefinition
                    {
                        KeyIntensity = 1.4f,
                        FillIntensity = 0.8f,
                        RimIntensity = 1.1f,
                        KeySpotAngle = 60f,
                        FillSpotAngle = 75f,
                        RimSpotAngle = 70f,
                        KeyRangeMultiplier = 2.8f,
                        FillRangeMultiplier = 2.6f,
                        RimRangeMultiplier = 3.0f,
                        UpMultiplier = 0.65f,
                        ForwardMultiplier = 0.75f,
                        SideMultiplier = 0.45f,
                        BackMultiplier = 0.60f,
                        RimUpMultiplier = 1.05f,
                        ShadowStrength = 0.9f,
                        KeyColor = Color.white,
                        FillColor = new Color(0.95f, 0.97f, 1.0f),
                        RimColor = new Color(1.0f, 0.98f, 0.95f)
                    };
            }
        }

        private static float EstimateLegLength(Animator animator)
        {
            Transform upper = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
            Transform lower = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
            Transform foot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            if (upper == null || lower == null || foot == null) return 0f;

            float thigh = Vector3.Distance(upper.position, lower.position);
            float shin = Vector3.Distance(lower.position, foot.position);
            const float ankleToSole = 0.07f;
            return Mathf.Max(0f, thigh + shin + ankleToSole);
        }

        private static bool TryAttachModularAvatarBoneProxy(GameObject target, HumanBodyBones bone)
        {
            try
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var proxyType = assemblies
                    .SelectMany(a => { try { return a.GetTypes(); } catch { return Array.Empty<Type>(); } })
                    .FirstOrDefault(t => t.FullName == "nadena.dev.modular_avatar.core.ModularAvatarBoneProxy");

                if (proxyType == null) return false;

                Component comp = target.AddComponent(proxyType);
                MemberInfo member = proxyType.GetMember("bone", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .FirstOrDefault();

                if (member is FieldInfo field && field.FieldType == typeof(HumanBodyBones))
                {
                    field.SetValue(comp, bone);
                }
                else if (member is PropertyInfo property && property.CanWrite && property.PropertyType == typeof(HumanBodyBones))
                {
                    property.SetValue(comp, bone);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsLilToonShader(string shaderName)
        {
            if (string.IsNullOrEmpty(shaderName)) return false;
            return shaderName.IndexOf("lilToon", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool IsLilToonStandardShader(string shaderName)
        {
            if (string.IsNullOrEmpty(shaderName)) return false;
            string lower = shaderName.ToLowerInvariant();
            if (!lower.Contains("liltoon")) return false;
            if (lower.Contains("outline") || lower.Contains("fur") || lower.Contains("tess") || lower.Contains("refraction") || lower.Contains("gem"))
            {
                return false;
            }
            return true;
        }

        private static void ApplyShadowPreset(Material material, LightPreset preset)
        {
            if (material == null) return;

            float filterRadius;
            float lightSize;
            float bias;
            float intensity;
            float samples;
            float qualityLevel;
            float presetMode;

            switch (preset)
            {
                case LightPreset.Anime:
                    filterRadius = 0.015f;
                    lightSize = 0.10f;
                    bias = 0.001f;
                    intensity = 0.85f;
                    samples = 12f;
                    qualityLevel = 1f;
                    presetMode = 1f;
                    break;
                case LightPreset.Cinematic:
                    filterRadius = 0.025f;
                    lightSize = 0.20f;
                    bias = 0.002f;
                    intensity = 1.20f;
                    samples = 24f;
                    qualityLevel = 3f;
                    presetMode = 2f;
                    break;
                default:
                    filterRadius = 0.005f;
                    lightSize = 0.05f;
                    bias = 0.0005f;
                    intensity = 1.0f;
                    samples = 16f;
                    qualityLevel = 2f;
                    presetMode = 0f;
                    break;
            }

            SetFloatIfExists(material, "_PCSSPresetMode", presetMode);
            SetFloatIfExists(material, "_PCSSQualityLevel", qualityLevel);
            SetFloatIfExists(material, "_LocalPCSSFilterRadius", filterRadius);
            SetFloatIfExists(material, "_LocalPCSSLightSize", lightSize);
            SetFloatIfExists(material, "_LocalPCSSBias", bias);
            SetFloatIfExists(material, "_LocalPCSSSamples", samples);
            SetFloatIfExists(material, "_PCSSIntensity", intensity);
        }

        private static void EnableVrcLightVolumeProperties(Material material)
        {
            if (material == null) return;
            SetFloatIfExists(material, "_UseVRCLightVolumes", 1.0f);
            SetFloatIfExists(material, "_VRCLightVolumesEnabled", 1.0f);
        }

        private static void SetFloatIfExists(Material material, string name, float value)
        {
            if (material.HasProperty(name))
            {
                material.SetFloat(name, value);
            }
        }

#if VRC_SDK_VRCSDK3
        private static void EnsureExpressionParameter(VRCAvatarDescriptor avatarDesc, string name, VRCExpressionParameters.ValueType type, bool saved)
        {
            if (avatarDesc == null) return;
            var ep = avatarDesc.expressionParameters;
            if (ep == null)
            {
                ep = ScriptableObject.CreateInstance<VRCExpressionParameters>();
                EnsureExpressionAssetFolder();
                AssetDatabase.CreateAsset(ep, "Assets/PCSS/Controllers/PCSS_Hub_ExpressionParameters.asset");
                avatarDesc.expressionParameters = ep;
                EditorUtility.SetDirty(avatarDesc);
            }

            var list = new List<VRCExpressionParameters.Parameter>();
            if (ep.parameters != null) list.AddRange(ep.parameters);
            if (!list.Exists(p => p != null && p.name == name))
            {
                list.Add(new VRCExpressionParameters.Parameter
                {
                    name = name,
                    valueType = type,
                    saved = saved
                });
                ep.parameters = list.ToArray();
                EditorUtility.SetDirty(ep);
                AssetDatabase.SaveAssets();
            }
        }

        private static void EnsureExpressionAssetFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/PCSS"))
            {
                AssetDatabase.CreateFolder("Assets", "PCSS");
            }
            if (!AssetDatabase.IsValidFolder("Assets/PCSS/Controllers"))
            {
                AssetDatabase.CreateFolder("Assets/PCSS", "Controllers");
            }
        }

        private static void TryEnsureHandCollider(Transform hand, List<VRC.Dynamics.VRCPhysBoneColliderBase> colliders)
        {
            if (hand == null) return;
            var collider = hand.GetComponent<VRCPhysBoneCollider>();
            if (collider == null)
            {
                collider = hand.gameObject.AddComponent<VRCPhysBoneCollider>();
                collider.shapeType = VRCPhysBoneCollider.ShapeType.Sphere;
                collider.radius = 0.06f;
            }
            colliders.Add(collider);
        }

        private static VRC.Dynamics.VRCPhysBoneBase.AdvancedBool CreateAdvancedBool(bool defaultValue)
        {
            var adv = new VRC.Dynamics.VRCPhysBoneBase.AdvancedBool();
            try
            {
                var field = typeof(VRC.Dynamics.VRCPhysBoneBase.AdvancedBool).GetField("value");
                if (field != null && field.FieldType == typeof(bool))
                {
                    field.SetValue(adv, defaultValue);
                }
            }
            catch
            {
                // Ignore if the SDK shape changed; default struct value is acceptable.
            }
            return adv;
        }
#endif
    }
}
#endif
