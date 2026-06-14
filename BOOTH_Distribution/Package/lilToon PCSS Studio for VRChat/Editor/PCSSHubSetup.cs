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
            Realistic = 0,
            Anime = 1,
            Cinematic = 2,
            DewySkin = 4,
            SoftFlushSkin = 5,
            StudioBoost = 6,
            ExcitedTone = 7,
            PCVRPerformance = 8
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

            if (animator.avatar == null || !animator.isHuman)
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

                    SetFloatIfExists(material, "_UsePCSS", 1.0f);
                    SetFloatIfExists(material, "_PCSSEnabled", 1.0f);

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
            return TryCreateHipLightRig(avatarRoot, animator, height, preset, enablePhysBoneSway, addHandColliders, disperseLightPositions: true, avatarOnlyCullingMask: true, hideLightHelpersInSceneView: true, out lights, out usedBoneProxy, out usedPhysBone, out error);
        }

        internal static bool TryCreateHipLightRig(GameObject avatarRoot, Animator animator, float height, LightPreset preset, bool enablePhysBoneSway, bool addHandColliders, bool disperseLightPositions, bool avatarOnlyCullingMask, bool hideLightHelpersInSceneView, out List<Light> lights, out bool usedBoneProxy, out bool usedPhysBone, out string error)
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

            if (TryAttachModularAvatarBoneProxy(anchor, hips, HumanBodyBones.Hips))
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

            Vector3 keyPosition = new Vector3(heightScale * 0.35f, up, forward);
            Vector3 fillPosition = new Vector3(-side, up * 0.85f, forward * 0.8f);
            Vector3 rimPosition = new Vector3(0.0f, up * def.RimUpMultiplier, -back);
            if (disperseLightPositions)
            {
                keyPosition = new Vector3(heightScale * 0.26f, up * 1.05f, forward * 0.88f);
                fillPosition = new Vector3(-side * 1.22f, up * 0.76f, forward * 0.54f);
                rimPosition = new Vector3(side * 0.44f, up * def.RimUpMultiplier, -back * 1.08f);
            }

            Light key = CreateSpotLight(
                lightParent,
                "PCSS Key Front-Right",
                keyPosition,
                head,
                new Vector3(10f, -25f, 0f),
                def.KeyIntensity,
                def.KeySpotAngle,
                heightScale * def.KeyRangeMultiplier,
                def.KeyColor,
                def.ShadowStrength);

            Light fill = CreateSpotLight(
                lightParent,
                "PCSS Fill Front-Left",
                fillPosition,
                head,
                new Vector3(8f, -20f, 0f),
                def.FillIntensity,
                def.FillSpotAngle,
                heightScale * def.FillRangeMultiplier,
                def.FillColor,
                def.ShadowStrength);

            Light rim = CreateSpotLight(
                lightParent,
                "PCSS Rim Back-Right",
                rimPosition,
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

            ConfigureGeneratedLights(avatarRoot, lights, avatarOnlyCullingMask, hideLightHelpersInSceneView);

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
            light.shadows = shadowStrength > 0.01f ? LightShadows.Soft : LightShadows.None;
            light.shadowStrength = Mathf.Clamp01(shadowStrength);
            light.shadowNormalBias = 0.1f;
            light.renderMode = shadowStrength > 0.01f ? LightRenderMode.Auto : LightRenderMode.ForceVertex;
            return light;
        }

        private static void ConfigureGeneratedLights(GameObject avatarRoot, IEnumerable<Light> lights, bool avatarOnlyCullingMask, bool hideLightHelpersInSceneView)
        {
            if (avatarRoot == null || lights == null) return;

            int avatarMask = BuildAvatarRendererLayerMask(avatarRoot);
            foreach (Light light in lights)
            {
                if (light == null) continue;
                if (avatarOnlyCullingMask)
                {
                    light.cullingMask = avatarMask;
                }

                EditorUtility.SetDirty(light);
                if (hideLightHelpersInSceneView)
                {
                    TryHideInSceneView(light.gameObject);
                    SetGeneratedLightEditorVisibility(light.gameObject, hidden: true);
                }
                else
                {
                    SetGeneratedLightEditorVisibility(light.gameObject, hidden: false);
                }
            }
        }

        private static int BuildAvatarRendererLayerMask(GameObject avatarRoot)
        {
            int mask = 0;
            Renderer[] renderers = avatarRoot.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null) continue;
                mask |= 1 << renderer.gameObject.layer;
            }

            if (mask == 0)
            {
                mask = 1 << avatarRoot.layer;
            }

            return mask;
        }

        private static void TryHideInSceneView(GameObject gameObject)
        {
            if (gameObject == null) return;
            try
            {
                SceneVisibilityManager.instance.Hide(gameObject, true);
            }
            catch
            {
                // Scene visibility is an editor convenience; light setup should still succeed if unavailable.
            }
        }

        private static void SetGeneratedLightEditorVisibility(GameObject gameObject, bool hidden)
        {
            if (gameObject == null) return;
            gameObject.hideFlags = hidden ? HideFlags.HideInHierarchy | HideFlags.NotEditable : HideFlags.None;
            EditorUtility.SetDirty(gameObject);
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
                case LightPreset.PCVRPerformance:
                    return new LightPresetDefinition
                    {
                        KeyIntensity = 0.42f,
                        FillIntensity = 0.14f,
                        RimIntensity = 0.24f,
                        KeySpotAngle = 42f,
                        FillSpotAngle = 52f,
                        RimSpotAngle = 46f,
                        KeyRangeMultiplier = 1.05f,
                        FillRangeMultiplier = 0.95f,
                        RimRangeMultiplier = 1.15f,
                        UpMultiplier = 0.54f,
                        ForwardMultiplier = 0.50f,
                        SideMultiplier = 0.34f,
                        BackMultiplier = 0.42f,
                        RimUpMultiplier = 0.94f,
                        ShadowStrength = 0.0f,
                        KeyColor = new Color(1.0f, 0.96f, 0.92f),
                        FillColor = new Color(0.92f, 0.96f, 1.0f),
                        RimColor = new Color(1.0f, 0.94f, 0.88f)
                    };
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
                case LightPreset.DewySkin:
                    return new LightPresetDefinition
                    {
                        KeyIntensity = 1.15f,
                        FillIntensity = 0.75f,
                        RimIntensity = 1.05f,
                        KeySpotAngle = 68f,
                        FillSpotAngle = 82f,
                        RimSpotAngle = 76f,
                        KeyRangeMultiplier = 2.7f,
                        FillRangeMultiplier = 2.5f,
                        RimRangeMultiplier = 3.0f,
                        UpMultiplier = 0.62f,
                        ForwardMultiplier = 0.72f,
                        SideMultiplier = 0.42f,
                        BackMultiplier = 0.62f,
                        RimUpMultiplier = 1.10f,
                        ShadowStrength = 0.72f,
                        KeyColor = new Color(1.0f, 0.96f, 0.92f),
                        FillColor = new Color(0.94f, 0.98f, 1.0f),
                        RimColor = new Color(1.0f, 1.0f, 0.96f)
                    };
                case LightPreset.SoftFlushSkin:
                    return new LightPresetDefinition
                    {
                        KeyIntensity = 1.05f,
                        FillIntensity = 0.82f,
                        RimIntensity = 0.78f,
                        KeySpotAngle = 72f,
                        FillSpotAngle = 86f,
                        RimSpotAngle = 82f,
                        KeyRangeMultiplier = 2.55f,
                        FillRangeMultiplier = 2.45f,
                        RimRangeMultiplier = 2.75f,
                        UpMultiplier = 0.60f,
                        ForwardMultiplier = 0.68f,
                        SideMultiplier = 0.38f,
                        BackMultiplier = 0.56f,
                        RimUpMultiplier = 1.02f,
                        ShadowStrength = 0.62f,
                        KeyColor = new Color(1.0f, 0.93f, 0.90f),
                        FillColor = new Color(1.0f, 0.96f, 0.94f),
                        RimColor = new Color(1.0f, 0.88f, 0.86f)
                    };
                case LightPreset.ExcitedTone:
                    return new LightPresetDefinition
                    {
                        KeyIntensity = 1.12f,
                        FillIntensity = 0.86f,
                        RimIntensity = 0.92f,
                        KeySpotAngle = 70f,
                        FillSpotAngle = 84f,
                        RimSpotAngle = 78f,
                        KeyRangeMultiplier = 2.65f,
                        FillRangeMultiplier = 2.45f,
                        RimRangeMultiplier = 2.90f,
                        UpMultiplier = 0.62f,
                        ForwardMultiplier = 0.70f,
                        SideMultiplier = 0.40f,
                        BackMultiplier = 0.58f,
                        RimUpMultiplier = 1.06f,
                        ShadowStrength = 0.66f,
                        KeyColor = new Color(1.0f, 0.94f, 0.90f),
                        FillColor = new Color(1.0f, 0.96f, 0.92f),
                        RimColor = new Color(1.0f, 0.88f, 0.82f)
                    };
                case LightPreset.StudioBoost:
                    return new LightPresetDefinition
                    {
                        KeyIntensity = 1.85f,
                        FillIntensity = 0.68f,
                        RimIntensity = 1.55f,
                        KeySpotAngle = 56f,
                        FillSpotAngle = 72f,
                        RimSpotAngle = 62f,
                        KeyRangeMultiplier = 3.15f,
                        FillRangeMultiplier = 2.75f,
                        RimRangeMultiplier = 3.35f,
                        UpMultiplier = 0.70f,
                        ForwardMultiplier = 0.82f,
                        SideMultiplier = 0.52f,
                        BackMultiplier = 0.72f,
                        RimUpMultiplier = 1.12f,
                        ShadowStrength = 0.96f,
                        KeyColor = new Color(1.0f, 0.96f, 0.92f),
                        FillColor = new Color(0.88f, 0.92f, 1.0f),
                        RimColor = new Color(0.96f, 0.94f, 1.0f)
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

        private static bool TryAttachModularAvatarBoneProxy(GameObject target, Transform targetBone, HumanBodyBones bone)
        {
            try
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var proxyType = assemblies
                    .SelectMany(a => { try { return a.GetTypes(); } catch { return Array.Empty<Type>(); } })
                    .FirstOrDefault(t => t.FullName == "nadena.dev.modular_avatar.core.ModularAvatarBoneProxy");

                if (proxyType == null) return false;

                Component comp = target.AddComponent(proxyType);
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
            catch
            {
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

            UnityEngine.Object unityObject = component as UnityEngine.Object;
            if (unityObject == null) return false;

            SerializedObject serializedObject = new SerializedObject(unityObject);
            SerializedProperty serializedProperty = serializedObject.FindProperty(memberName);
            if (serializedProperty == null) return false;

            if (serializedProperty.propertyType == SerializedPropertyType.ObjectReference && value is UnityEngine.Object objectValue)
            {
                serializedProperty.objectReferenceValue = objectValue;
                serializedObject.ApplyModifiedProperties();
                return true;
            }

            if (serializedProperty.propertyType == SerializedPropertyType.Enum && value != null && value.GetType().IsEnum)
            {
                serializedProperty.enumValueIndex = Convert.ToInt32(value);
                serializedObject.ApplyModifiedProperties();
                return true;
            }

            return false;
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
            float maxDistance;
            float distanceFade;
            float glossCoherence;
            float glossBoost;
            float glossSuppression;
            float glossRim;
            float glossSmoothness;
            float realisticIntensity;
            float realisticSoftness;

            switch (preset)
            {
                case LightPreset.PCVRPerformance:
                    filterRadius = 0.0100f;
                    lightSize = 0.105f;
                    bias = 0.0009f;
                    intensity = 0.92f;
                    samples = 6f;
                    qualityLevel = 1f;
                    presetMode = 6f;
                    maxDistance = 10.0f;
                    distanceFade = 3.0f;
                    glossCoherence = 0.68f;
                    glossBoost = 0.52f;
                    glossSuppression = 0.42f;
                    glossRim = 0.38f;
                    glossSmoothness = 0.78f;
                    realisticIntensity = 0.56f;
                    realisticSoftness = 0.70f;
                    if (!IsLikelySkinMaterial(material))
                    {
                        glossCoherence = 0.48f;
                        glossBoost = 0.20f;
                        glossSuppression = 0.56f;
                        glossRim = 0.18f;
                        glossSmoothness = 0.62f;
                        realisticIntensity = 0.66f;
                        realisticSoftness = 0.54f;
                    }
                    break;
                case LightPreset.Anime:
                    filterRadius = 0.015f;
                    lightSize = 0.10f;
                    bias = 0.001f;
                    intensity = 0.85f;
                    samples = 10f;
                    qualityLevel = 1f;
                    presetMode = 1f;
                    maxDistance = 9f;
                    distanceFade = 2.0f;
                    glossCoherence = 0.48f;
                    glossBoost = 0.25f;
                    glossSuppression = 0.35f;
                    glossRim = 0.25f;
                    glossSmoothness = 0.64f;
                    realisticIntensity = 0.78f;
                    realisticSoftness = 0.48f;
                    break;
                case LightPreset.Cinematic:
                    filterRadius = 0.020f;
                    lightSize = 0.18f;
                    bias = 0.0015f;
                    intensity = 1.15f;
                    samples = 16f;
                    qualityLevel = 2f;
                    presetMode = 2f;
                    maxDistance = 14f;
                    distanceFade = 3.0f;
                    glossCoherence = 0.72f;
                    glossBoost = 0.55f;
                    glossSuppression = 0.55f;
                    glossRim = 0.55f;
                    glossSmoothness = 0.82f;
                    realisticIntensity = 0.95f;
                    realisticSoftness = 0.70f;
                    break;
                case LightPreset.DewySkin:
                    filterRadius = 0.009f;
                    lightSize = 0.095f;
                    bias = 0.0008f;
                    intensity = 0.96f;
                    samples = 12f;
                    qualityLevel = 2f;
                    presetMode = 4f;
                    maxDistance = 8.5f;
                    distanceFade = 2.75f;
                    glossCoherence = 0.76f;
                    glossBoost = 0.66f;
                    glossSuppression = 0.40f;
                    glossRim = 0.48f;
                    glossSmoothness = 0.88f;
                    realisticIntensity = 0.62f;
                    realisticSoftness = 0.68f;
                    if (!IsLikelySkinMaterial(material))
                    {
                        glossCoherence = 0.56f;
                        glossBoost = 0.30f;
                        glossSuppression = 0.54f;
                        glossRim = 0.26f;
                        glossSmoothness = 0.70f;
                        realisticIntensity = 0.74f;
                        realisticSoftness = 0.54f;
                    }
                    break;
                case LightPreset.SoftFlushSkin:
                    filterRadius = 0.0105f;
                    lightSize = 0.105f;
                    bias = 0.0009f;
                    intensity = 0.93f;
                    samples = 12f;
                    qualityLevel = 2f;
                    presetMode = 5f;
                    maxDistance = 8.0f;
                    distanceFade = 2.6f;
                    glossCoherence = 0.68f;
                    glossBoost = 0.42f;
                    glossSuppression = 0.46f;
                    glossRim = 0.34f;
                    glossSmoothness = 0.78f;
                    realisticIntensity = 0.58f;
                    realisticSoftness = 0.74f;
                    if (!IsLikelySkinMaterial(material))
                    {
                        glossCoherence = 0.46f;
                        glossBoost = 0.18f;
                        glossSuppression = 0.58f;
                        glossRim = 0.18f;
                        glossSmoothness = 0.66f;
                        realisticIntensity = 0.72f;
                        realisticSoftness = 0.56f;
                    }
                    break;
                case LightPreset.ExcitedTone:
                    filterRadius = 0.0110f;
                    lightSize = 0.115f;
                    bias = 0.00085f;
                    intensity = 1.02f;
                    samples = 14f;
                    qualityLevel = 2f;
                    presetMode = 7f;
                    maxDistance = 8.5f;
                    distanceFade = 2.8f;
                    glossCoherence = 0.72f;
                    glossBoost = 0.50f;
                    glossSuppression = 0.42f;
                    glossRim = 0.42f;
                    glossSmoothness = 0.82f;
                    realisticIntensity = 0.60f;
                    realisticSoftness = 0.72f;
                    if (!IsLikelySkinMaterial(material))
                    {
                        glossCoherence = 0.50f;
                        glossBoost = 0.22f;
                        glossSuppression = 0.56f;
                        glossRim = 0.20f;
                        glossSmoothness = 0.68f;
                        realisticIntensity = 0.72f;
                        realisticSoftness = 0.56f;
                    }
                    break;
                case LightPreset.StudioBoost:
                    filterRadius = 0.0125f;
                    lightSize = 0.160f;
                    bias = 0.00065f;
                    intensity = 1.30f;
                    samples = 16f;
                    qualityLevel = 2f;
                    presetMode = 6f;
                    maxDistance = 12.0f;
                    distanceFade = 3.2f;
                    glossCoherence = 0.86f;
                    glossBoost = 0.88f;
                    glossSuppression = 0.30f;
                    glossRim = 0.72f;
                    glossSmoothness = 0.92f;
                    realisticIntensity = 0.78f;
                    realisticSoftness = 0.62f;
                    if (!IsLikelySkinMaterial(material))
                    {
                        glossCoherence = 0.70f;
                        glossBoost = 0.52f;
                        glossSuppression = 0.38f;
                        glossRim = 0.46f;
                        glossSmoothness = 0.82f;
                        realisticIntensity = 0.88f;
                        realisticSoftness = 0.54f;
                    }
                    break;
                default:
                    filterRadius = 0.0065f;
                    lightSize = 0.060f;
                    bias = 0.00055f;
                    intensity = 1.05f;
                    samples = 12f;
                    qualityLevel = 2f;
                    presetMode = 0f;
                    maxDistance = 10f;
                    distanceFade = 2.5f;
                    glossCoherence = 0.55f;
                    glossBoost = 0.35f;
                    glossSuppression = 0.45f;
                    glossRim = 0.35f;
                    glossSmoothness = 0.72f;
                    realisticIntensity = 0.92f;
                    realisticSoftness = 0.55f;
                    break;
            }

            maxDistance = 10.0f;
            distanceFade = Mathf.Clamp(distanceFade, 0.5f, 10.0f);

            SetFloatIfExists(material, "_PCSSPresetMode", presetMode);
            SetFloatIfExists(material, "_PCSSQualityLevel", qualityLevel);
            SetFloatIfExists(material, "_LocalPCSSFilterRadius", filterRadius);
            SetFloatIfExists(material, "_PCSSFilterRadius", filterRadius);
            SetFloatIfExists(material, "_LocalPCSSLightSize", lightSize);
            SetFloatIfExists(material, "_PCSSLightSize", lightSize);
            SetFloatIfExists(material, "_LocalPCSSBias", bias);
            SetFloatIfExists(material, "_PCSSBias", bias);
            SetFloatIfExists(material, "_LocalPCSSSamples", samples);
            SetFloatIfExists(material, "_PCSSSamples", samples);
            SetFloatIfExists(material, "_PCSSIntensity", intensity);
            SetFloatIfExists(material, "_UsePCSS", 1.0f);
            SetFloatIfExists(material, "_PCSSEnabled", 1.0f);
            SetFloatIfExists(material, "_UsePCSSOptimization", 1.0f);
            SetFloatIfExists(material, "_PCSSOptimizationLevel", preset == LightPreset.StudioBoost ? 0.0f : 1.0f);
            SetFloatIfExists(material, "_UseVRChatPerformanceGate", 1.0f);
            SetFloatIfExists(material, "_PCSSMaxDistance", maxDistance);
            SetFloatIfExists(material, "_PCSSDistanceFade", distanceFade);
            SetFloatIfExists(material, "_UseGlossShadowCoherence", 1.0f);
            SetFloatIfExists(material, "_GlossShadowCoherence", glossCoherence);
            SetFloatIfExists(material, "_GlossShadowBoost", glossBoost);
            SetFloatIfExists(material, "_GlossShadowSuppression", glossSuppression);
            SetFloatIfExists(material, "_GlossRimStrength", glossRim);
            SetFloatIfExists(material, "_GlossSmoothness", glossSmoothness);
            SetFloatIfExists(material, "_UseRealisticShadow", 1.0f);
            SetFloatIfExists(material, "_RealisticShadowIntensity", realisticIntensity);
            SetFloatIfExists(material, "_RealisticShadowSoftness", realisticSoftness);
            SetFloatIfExists(material, "_UseLightDirectionOverride", 1.0f);
            SetVectorIfExists(material, "_LightDirectionOverride", preset == LightPreset.PCVRPerformance
                ? new Vector4(0.24f, 0.84f, 0.48f, 0.0f)
                : preset == LightPreset.DewySkin
                ? new Vector4(0.22f, 0.86f, 0.46f, 0.0f)
                : preset == LightPreset.SoftFlushSkin
                    ? new Vector4(0.18f, 0.88f, 0.42f, 0.0f)
                    : preset == LightPreset.ExcitedTone
                        ? new Vector4(0.20f, 0.86f, 0.44f, 0.0f)
                    : preset == LightPreset.StudioBoost
                        ? new Vector4(0.28f, 0.82f, 0.50f, 0.0f)
                        : new Vector4(0.35f, 0.75f, 0.55f, 0.0f));
            SetFloatIfExists(material, "_UseNoLightPCSSBoost", preset == LightPreset.Realistic || preset == LightPreset.Anime || preset == LightPreset.Cinematic ? 0.0f : 1.0f);
            SetFloatIfExists(material, "_NoLightPCSSBoostStrength", preset == LightPreset.StudioBoost ? 0.72f : preset == LightPreset.PCVRPerformance ? 0.62f : preset == LightPreset.ExcitedTone ? 0.56f : preset == LightPreset.SoftFlushSkin ? 0.52f : 0.48f);
            SetFloatIfExists(material, "_NoLightPCSSBoostSoftness", preset == LightPreset.StudioBoost ? 0.70f : preset == LightPreset.PCVRPerformance ? 0.68f : 0.66f);
            SetFloatIfExists(material, "_NoLightPCSSBoostRim", preset == LightPreset.StudioBoost ? 0.55f : preset == LightPreset.PCVRPerformance ? 0.44f : preset == LightPreset.ExcitedTone ? 0.40f : preset == LightPreset.DewySkin ? 0.42f : 0.32f);
            SetColorIfExists(material, "_NoLightPCSSHighlightTint", preset == LightPreset.StudioBoost
                ? new Color(0.72f, 0.70f, 0.76f, 1.0f)
                : preset == LightPreset.PCVRPerformance
                    ? new Color(0.66f, 0.60f, 0.58f, 1.0f)
                : preset == LightPreset.ExcitedTone
                    ? new Color(0.64f, 0.50f, 0.46f, 1.0f)
                : new Color(0.58f, 0.52f, 0.50f, 1.0f));
            SetFloatIfExists(material, "_UseShadow", 1.0f);
            SetFloatIfExists(material, "_UseShadow2", 1.0f);
            SetFloatIfExists(material, "_UseShadow3", preset == LightPreset.Anime ? 0.0f : 1.0f);
            if (preset == LightPreset.DewySkin && IsLikelySkinMaterial(material))
            {
                SetFloatIfExists(material, "_Translucency", 0.55f);
            }
            if (preset == LightPreset.SoftFlushSkin)
            {
                bool isFace = IsLikelyFaceMaterial(material);
                bool isSkin = IsLikelySkinMaterial(material);
                SetFloatIfExists(material, "_UseSoftFlush", isFace ? 1.0f : 0.0f);
                SetColorIfExists(material, "_SoftFlushColor", new Color(1.0f, 0.40f, 0.36f, 1.0f));
                SetFloatIfExists(material, "_SoftFlushStrength", isFace ? 0.42f : 0.0f);
                SetFloatIfExists(material, "_SoftFlushWidth", 0.56f);
                SetFloatIfExists(material, "_SoftFlushVerticalBias", 0.46f);
                SetColorIfExists(material, "_RealisticShadowColor", isSkin
                    ? new Color(0.23f, 0.13f, 0.14f, 0.76f)
                    : new Color(0.14f, 0.13f, 0.15f, 0.76f));
                SetFloatIfExists(material, "_UseRimShade", isSkin ? 1.0f : 0.0f);
                SetColorIfExists(material, "_RimShadeColor", new Color(1.0f, 0.45f, 0.40f, 1.0f));
                SetFloatIfExists(material, "_RimShadeIntensity", isFace ? 0.08f : isSkin ? 0.035f : 0.0f);
                SetFloatIfExists(material, "_RimShadeWidth", 0.78f);
                SetFloatIfExists(material, "_Translucency", isSkin ? 0.50f : 0.42f);
            }
            if (preset == LightPreset.ExcitedTone)
            {
                bool isFace = IsLikelyFaceMaterial(material);
                bool isSkin = IsLikelySkinMaterial(material);
                SetFloatIfExists(material, "_UseSoftFlush", isFace ? 1.0f : 0.0f);
                SetColorIfExists(material, "_SoftFlushColor", new Color(1.0f, 0.40f, 0.34f, 1.0f));
                SetFloatIfExists(material, "_SoftFlushStrength", isFace ? 0.30f : 0.0f);
                SetFloatIfExists(material, "_SoftFlushWidth", 0.60f);
                SetFloatIfExists(material, "_SoftFlushVerticalBias", 0.48f);
                SetFloatIfExists(material, "_UseExcitedTone", isSkin ? 1.0f : 0.0f);
                SetColorIfExists(material, "_ExcitedToneColor", new Color(1.0f, 0.48f, 0.34f, 1.0f));
                SetFloatIfExists(material, "_ExcitedToneStrength", isFace ? 0.30f : isSkin ? 0.18f : 0.0f);
                SetFloatIfExists(material, "_ExcitedToneBreath", 0.0f);
                SetFloatIfExists(material, "_ExcitedToneUpperBias", 0.58f);
                SetColorIfExists(material, "_RealisticShadowColor", isSkin
                    ? new Color(0.24f, 0.12f, 0.12f, 0.74f)
                    : new Color(0.14f, 0.13f, 0.15f, 0.76f));
                SetFloatIfExists(material, "_UseRimShade", isSkin ? 1.0f : 0.0f);
                SetColorIfExists(material, "_RimShadeColor", new Color(1.0f, 0.52f, 0.42f, 1.0f));
                SetFloatIfExists(material, "_RimShadeIntensity", isFace ? 0.10f : isSkin ? 0.045f : 0.0f);
                SetFloatIfExists(material, "_RimShadeWidth", 0.74f);
                SetFloatIfExists(material, "_Translucency", isSkin ? 0.52f : 0.42f);
            }
            if (preset == LightPreset.StudioBoost)
            {
                bool isSkin = IsLikelySkinMaterial(material);
                SetFloatIfExists(material, "_Translucency", isSkin ? 0.48f : 0.40f);
                SetFloatIfExists(material, "_UseRimShade", 1.0f);
                SetColorIfExists(material, "_RimShadeColor", isSkin
                    ? new Color(0.95f, 0.92f, 1.0f, 1.0f)
                    : new Color(0.78f, 0.80f, 0.92f, 1.0f));
                SetFloatIfExists(material, "_RimShadeIntensity", isSkin ? 0.16f : 0.10f);
                SetFloatIfExists(material, "_RimShadeWidth", 0.64f);
            }
            if (preset == LightPreset.PCVRPerformance)
            {
                bool isSkin = IsLikelySkinMaterial(material);
                SetFloatIfExists(material, "_Translucency", isSkin ? 0.42f : 0.34f);
                SetFloatIfExists(material, "_UseRimShade", isSkin ? 1.0f : 0.0f);
                SetColorIfExists(material, "_RimShadeColor", new Color(0.95f, 0.88f, 0.82f, 1.0f));
                SetFloatIfExists(material, "_RimShadeIntensity", isSkin ? 0.08f : 0.0f);
                SetFloatIfExists(material, "_RimShadeWidth", 0.66f);
            }

            SetKeyword(material, "_USEPCSS_ON", true);
            SetKeyword(material, "_USEPCSSOPTIMIZATION_ON", true);
            SetKeyword(material, "_USEVRCHATPERFORMANCEGATE_ON", true);
            SetKeyword(material, "_USEGLOSSSHADOWCOHERENCE_ON", true);
            SetKeyword(material, "_USEREALISTICSHADOW_ON", true);
            SetKeyword(material, "_USELIGHTDIRECTIONOVERRIDE_ON", true);
            SetKeyword(material, "_USENOLIGHTPCSSBOOST_ON", GetFloatIfExists(material, "_UseNoLightPCSSBoost") > 0.5f);
            SetKeyword(material, "_USESOFTFLUSH_ON", (preset == LightPreset.SoftFlushSkin || preset == LightPreset.ExcitedTone) && IsLikelyFaceMaterial(material));
            SetKeyword(material, "_USEEXCITEDTONE_ON", preset == LightPreset.ExcitedTone && IsLikelySkinMaterial(material));
            SetKeyword(material, "_USERIMSHADE_ON", GetFloatIfExists(material, "_UseRimShade") > 0.5f);
            SetKeyword(material, "_USESHADOW_ON", true);
            SetKeyword(material, "_USESHADOW2_ON", true);
            SetKeyword(material, "_USESHADOW3_ON", preset != LightPreset.Anime);
        }

        private static bool IsLikelyFaceMaterial(Material material)
        {
            if (material == null) return false;
            string name = (material.name ?? string.Empty).ToLowerInvariant();
            return name.Contains("face") ||
                   name.Contains("head") ||
                   name.Contains("cheek") ||
                   name.Contains("blush") ||
                   name.Contains("makeup") ||
                   name.Contains("make");
        }

        private static bool IsLikelySkinMaterial(Material material)
        {
            if (material == null) return false;
            string name = (material.name ?? string.Empty).ToLowerInvariant();
            return name.Contains("skin") ||
                   name.Contains("body") ||
                   name.Contains("face") ||
                   name.Contains("head") ||
                   name.Contains("hand") ||
                   name.Contains("arm") ||
                   name.Contains("leg") ||
                   name.Contains("torso") ||
                   name.Contains("hada") ||
                   name.Contains("肌") ||
                   name.Contains("顔") ||
                   name.Contains("体") ||
                   name.Contains("素体");
        }

        private static void EnableVrcLightVolumeProperties(Material material)
        {
            if (material == null) return;
            SetFloatIfExists(material, "_UseVRCLightVolumes", 1.0f);
            SetFloatIfExists(material, "_VRCLightVolumesEnabled", 1.0f);
            SetFloatIfExists(material, "_UseVRCLVOptimization", 1.0f);
            SetFloatIfExists(material, "_VRCLVOptimizationEnabled", 1.0f);
            SetKeyword(material, "_USEVRCLIGHT_VOLUMES_ON", true);
            SetKeyword(material, "_USEVRCLVOPTIMIZATION_ON", true);
        }

        private static void SetFloatIfExists(Material material, string name, float value)
        {
            if (material.HasProperty(name))
            {
                material.SetFloat(name, value);
            }
        }

        private static void SetColorIfExists(Material material, string name, Color value)
        {
            if (material.HasProperty(name))
            {
                material.SetColor(name, value);
            }
        }

        private static float GetFloatIfExists(Material material, string name)
        {
            return material != null && material.HasProperty(name) ? material.GetFloat(name) : 0.0f;
        }

        private static void SetVectorIfExists(Material material, string name, Vector4 value)
        {
            if (material.HasProperty(name))
            {
                material.SetVector(name, value);
            }
        }

        private static void SetKeyword(Material material, string keyword, bool enabled)
        {
            if (material == null) return;

            if (enabled)
            {
                material.EnableKeyword(keyword);
            }
            else
            {
                material.DisableKeyword(keyword);
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
