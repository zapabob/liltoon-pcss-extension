#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace lilToon.PCSS.Editor
{
    internal static class PCSSPCVRUploadAudit
    {
        private const string HipLightGroupName = "PCSS Hip Lights (MA)";
        private const string AutoLightGroupName = "PCSS External Lights (Auto)";
        private const string LightControlsName = "PCSS Light Controls (MA)";
        private const string ControllerPath = "Assets/PCSS/Controllers/PCSS_LightControl.controller";
        private const string MenuPath = "Assets/PCSS/Controllers/PCSS_LightControls_Menu.asset";

        [MenuItem("Tools/lilToon-PCSS-Extension/PCSS Hub/Run PCVR Upload Audit", false, 46)]
        public static void AuditSelectedAvatar()
        {
            GameObject avatarRoot = ResolveAvatarRoot(Selection.activeGameObject);
            if (avatarRoot == null)
            {
                EditorUtility.DisplayDialog("PCSS PCVR Audit", "Select an avatar root or avatar child first.", "OK");
                return;
            }

            AuditReport report = AuditAvatar(avatarRoot);
            Debug.Log(report.ToDetailedString());
            EditorUtility.DisplayDialog("PCSS PCVR Audit", report.ToDialogString(), "OK");
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/PCSS Hub/Repair 0-Light Setup, Then Audit", false, 47)]
        public static void RepairThenAuditSelectedAvatar()
        {
            GameObject avatarRoot = ResolveAvatarRoot(Selection.activeGameObject);
            if (avatarRoot == null)
            {
                EditorUtility.DisplayDialog("PCSS PCVR Audit", "Select an avatar root or avatar child first.", "OK");
                return;
            }

            Undo.RegisterFullObjectHierarchyUndo(avatarRoot, "Repair PCSS PCVR Upload State");
            PCSSAvatarPerformanceGuard.Apply(avatarRoot, useUndo: true, removeGeneratedLightControls: true, tuneMaterials: true);
            PCSSNDMFAAOFixUtility.RepairAvatar(avatarRoot);
            EditorUtility.SetDirty(avatarRoot);

            AuditReport report = AuditAvatar(avatarRoot);
            Debug.Log(report.ToDetailedString());
            EditorUtility.DisplayDialog("PCSS PCVR Audit", report.ToDialogString(), "OK");
        }

        internal static AuditReport AuditAvatar(GameObject avatarRoot)
        {
            AuditReport report = new AuditReport
            {
                AvatarName = avatarRoot != null ? avatarRoot.name : string.Empty,
                Errors = new List<string>(),
                Warnings = new List<string>()
            };

            if (avatarRoot == null)
            {
                report.Errors.Add("Avatar root is missing.");
                return report;
            }

            AuditLights(avatarRoot, report);
            AuditAvatarDescriptor(avatarRoot, report);
            AuditMaterials(avatarRoot, report);
            AuditMenuAndAnimator(avatarRoot, report);
            AuditProjectSettings(report);
            return report;
        }

        private static void AuditAvatarDescriptor(GameObject avatarRoot, AuditReport report)
        {
            report.HasAvatarDescriptor = avatarRoot.GetComponent<VRCAvatarDescriptor>() != null;
            if (!report.HasAvatarDescriptor)
            {
                report.Errors.Add("VRCAvatarDescriptor is missing on the resolved avatar root. Modular Avatar outfit tools cannot detect it as an avatar.");
            }

            string sourcePath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(avatarRoot);
            report.SourceAssetPath = sourcePath ?? string.Empty;
            if (!string.IsNullOrEmpty(sourcePath) && sourcePath.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase))
            {
                report.Warnings.Add("The avatar root is an FBX instance. Use the avatar prefab or run Descriptor repair before Modular Avatar outfit fitting.");
            }
        }

        private static void AuditLights(GameObject avatarRoot, AuditReport report)
        {
            Renderer[] renderers = avatarRoot.GetComponentsInChildren<Renderer>(true);
            int avatarRendererMask = 0;
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null) continue;
                avatarRendererMask |= 1 << renderer.gameObject.layer;
            }

            foreach (Light light in avatarRoot.GetComponentsInChildren<Light>(true))
            {
                if (light == null) continue;

                report.TotalLights++;
                if (light.enabled && light.gameObject.activeInHierarchy)
                {
                    report.EnabledLights++;
                }

                bool generated = IsGeneratedPCSSLight(light.transform);
                if (generated)
                {
                    report.GeneratedPCSSLights++;
                    if ((light.gameObject.hideFlags & HideFlags.HideInHierarchy) == 0)
                    {
                        report.VisibleGeneratedLightHelpers++;
                    }

                    if (avatarRendererMask != 0 && (light.cullingMask & ~avatarRendererMask) != 0)
                    {
                        report.AvatarOnlyMaskIssues++;
                    }
                }
                else
                {
                    report.NonPCSSLights++;
                }
            }

            if (report.TotalLights > 0)
            {
                report.Errors.Add("Avatar Light count is not 0. VRChat PC Good/Medium budgets expect 0 avatar Lights.");
            }

            if (report.NonPCSSLights > 0)
            {
                report.Errors.Add("Non-PCSS avatar Lights remain under the avatar.");
            }

            if (report.VisibleGeneratedLightHelpers > 0)
            {
                report.Warnings.Add("Generated PCSS Light helpers are visible in the hierarchy.");
            }

            if (report.AvatarOnlyMaskIssues > 0)
            {
                report.Warnings.Add("Generated PCSS Lights include layers outside the avatar renderer layer set.");
            }
        }

        private static void AuditMaterials(GameObject avatarRoot, AuditReport report)
        {
            HashSet<Material> materials = new HashSet<Material>();
            foreach (Renderer renderer in avatarRoot.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null || renderer.sharedMaterials == null) continue;
                foreach (Material material in renderer.sharedMaterials)
                {
                    if (IsPCSSMaterial(material))
                    {
                        materials.Add(material);
                    }
                }
            }

            report.PCSSMaterials = materials.Count;
            foreach (Material material in materials)
            {
                if (HasFloatAbove(material, "_LocalPCSSSamples", 6f) ||
                    HasFloatAbove(material, "_PCSSSamples", 6f) ||
                    HasFloatAbove(material, "_PCSSQualityLevel", 1f) ||
                    HasFloatAbove(material, "_PCSSMaxDistance", 10f) ||
                    HasFloatAbove(material, "_PCSSDistanceFade", 3f))
                {
                    report.HeavyMaterialIssues++;
                    report.Warnings.Add("PCSS material exceeds the PCVR-safe RTX3060 profile: " + material.name);
                }

                if (HasFloatAbove(material, "_UseVRCLightVolumes", 0.5f) ||
                    HasFloatAbove(material, "_VRCLightVolumesEnabled", 0.5f))
                {
                    report.VRCLightVolumeMaterials++;
                    report.Warnings.Add("VRC Light Volumes remains enabled on PCSS material: " + material.name);
                }
            }
        }

        private static void AuditMenuAndAnimator(GameObject avatarRoot, AuditReport report)
        {
            Transform controls = avatarRoot.transform.Find(LightControlsName);
            report.LightControlRoots = controls != null ? 1 : 0;
            if (report.TotalLights == 0 && controls != null)
            {
                report.Errors.Add("PCSS Light Controls remain even though the avatar uses the 0-Light upload workflow.");
            }

            VRCExpressionsMenu menu = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(MenuPath);
            if (menu == null && report.TotalLights > 0)
            {
                report.Errors.Add("PCSS light menu asset is missing while optional PCSS Lights remain.");
            }
            else if (menu != null && menu.controls != null)
            {
                report.MenuControlCount = menu.controls.Count;
                bool hasAllToggle = menu.controls.Any(control => control != null && control.name == "All PCSS Lights" && Mathf.Approximately(control.value, 1f));
                bool labelsArePositioned = menu.controls
                    .Where(control => control != null && control.type == VRCExpressionsMenu.Control.ControlType.RadialPuppet)
                    .All(control => control.name.IndexOf("Max", StringComparison.OrdinalIgnoreCase) >= 0 &&
                                    (control.name.IndexOf("Front", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                     control.name.IndexOf("Back", StringComparison.OrdinalIgnoreCase) >= 0));

                if (report.TotalLights > 0 && !hasAllToggle)
                {
                    report.Errors.Add("PCSS light menu does not have a default-on All PCSS Lights toggle.");
                }

                if (report.TotalLights > 0 && !labelsArePositioned)
                {
                    report.Warnings.Add("PCSS light menu labels do not clearly include position and maximum intensity.");
                }
            }

            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            if (controller == null) return;

            foreach (AnimatorControllerLayer layer in controller.layers ?? Array.Empty<AnimatorControllerLayer>())
            {
                if (layer == null || layer.name == null || !layer.name.StartsWith("PCSS_", StringComparison.Ordinal)) continue;
                if (layer.stateMachine == null)
                {
                    report.AnimatorIssues++;
                    report.Errors.Add("PCSS animator layer has no state machine: " + layer.name);
                    continue;
                }

                foreach (ChildAnimatorState childState in layer.stateMachine.states ?? Array.Empty<ChildAnimatorState>())
                {
                    AnimatorState state = childState.state;
                    if (state == null || state.motion == null)
                    {
                        report.AnimatorIssues++;
                        report.Errors.Add("PCSS animator state has no motion in layer: " + layer.name);
                        continue;
                    }

                    BlendTree tree = state.motion as BlendTree;
                    if (tree == null) continue;
                    foreach (ChildMotion childMotion in tree.children)
                    {
                        if (childMotion.motion == null || childMotion.timeScale <= 0f)
                        {
                            report.AnimatorIssues++;
                            report.Errors.Add("PCSS BlendTree has a missing motion or invalid speed in layer: " + layer.name);
                        }
                    }
                }
            }
        }

        private static void AuditProjectSettings(AuditReport report)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "ProjectSettings", "lilToonSetting.json");
            if (!File.Exists(path))
            {
                report.Warnings.Add("ProjectSettings/lilToonSetting.json is missing.");
                return;
            }

            byte[] bytes = File.ReadAllBytes(path);
            if (bytes.Length == 0 || bytes.All(b => b == 0))
            {
                report.Errors.Add("ProjectSettings/lilToonSetting.json is empty or NUL-filled.");
                return;
            }

            string text = File.ReadAllText(path);
            if (!text.TrimStart().StartsWith("{", StringComparison.Ordinal))
            {
                report.Errors.Add("ProjectSettings/lilToonSetting.json does not start with a JSON object.");
            }
        }

        private static bool IsGeneratedPCSSLight(Transform transform)
        {
            if (transform == null) return false;
            return transform.name.IndexOf("PCSS", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   HasAncestor(transform, HipLightGroupName) ||
                   HasAncestor(transform, AutoLightGroupName);
        }

        private static bool HasAncestor(Transform transform, string name)
        {
            Transform current = transform;
            while (current != null)
            {
                if (current.name == name) return true;
                current = current.parent;
            }

            return false;
        }

        private static bool IsPCSSMaterial(Material material)
        {
            if (material == null || material.shader == null) return false;
            string shaderName = material.shader.name ?? string.Empty;
            return shaderName.IndexOf("PCSS", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   material.HasProperty("_UsePCSS") ||
                   material.HasProperty("_PCSSEnabled") ||
                   material.HasProperty("_LocalPCSSFilterRadius") ||
                   material.HasProperty("_PCSSFilterRadius");
        }

        private static bool HasFloatAbove(Material material, string propertyName, float limit)
        {
            return material != null &&
                   material.HasProperty(propertyName) &&
                   material.GetFloat(propertyName) > limit + 0.0001f;
        }

        private static GameObject ResolveAvatarRoot(GameObject selected)
        {
            return PCSSAvatarDescriptorGuard.ResolveAvatarRoot(selected);
        }

        internal struct AuditReport
        {
            public string AvatarName;
            public int TotalLights;
            public int EnabledLights;
            public int GeneratedPCSSLights;
            public int NonPCSSLights;
            public int VisibleGeneratedLightHelpers;
            public int AvatarOnlyMaskIssues;
            public int LightControlRoots;
            public int MenuControlCount;
            public int AnimatorIssues;
            public int PCSSMaterials;
            public int HeavyMaterialIssues;
            public int VRCLightVolumeMaterials;
            public bool HasAvatarDescriptor;
            public string SourceAssetPath;
            public List<string> Errors;
            public List<string> Warnings;

            public bool Passed => Errors.Count == 0;

            public string ToDialogString()
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine(Passed ? "PCVR upload audit passed." : "PCVR upload audit needs attention.");
                builder.AppendLine();
                builder.AppendLine("Avatar: " + AvatarName);
                builder.AppendLine("Avatar Descriptor: " + (HasAvatarDescriptor ? "present" : "missing"));
                builder.AppendLine("Avatar Lights: " + TotalLights + " (enabled " + EnabledLights + ")");
                builder.AppendLine("PCSS materials: " + PCSSMaterials);
                builder.AppendLine("Heavy material issues: " + HeavyMaterialIssues);
                builder.AppendLine("Animator issues: " + AnimatorIssues);
                AppendMessages(builder, "Errors", Errors);
                AppendMessages(builder, "Warnings", Warnings);
                return builder.ToString();
            }

            public string ToDetailedString()
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("[PCSS PCVR Audit] " + (Passed ? "PASS" : "FAIL"));
                builder.AppendLine("Avatar: " + AvatarName);
                builder.AppendLine("HasAvatarDescriptor: " + HasAvatarDescriptor);
                builder.AppendLine("SourceAssetPath: " + SourceAssetPath);
                builder.AppendLine("TotalLights: " + TotalLights);
                builder.AppendLine("EnabledLights: " + EnabledLights);
                builder.AppendLine("GeneratedPCSSLights: " + GeneratedPCSSLights);
                builder.AppendLine("NonPCSSLights: " + NonPCSSLights);
                builder.AppendLine("VisibleGeneratedLightHelpers: " + VisibleGeneratedLightHelpers);
                builder.AppendLine("AvatarOnlyMaskIssues: " + AvatarOnlyMaskIssues);
                builder.AppendLine("LightControlRoots: " + LightControlRoots);
                builder.AppendLine("MenuControlCount: " + MenuControlCount);
                builder.AppendLine("AnimatorIssues: " + AnimatorIssues);
                builder.AppendLine("PCSSMaterials: " + PCSSMaterials);
                builder.AppendLine("HeavyMaterialIssues: " + HeavyMaterialIssues);
                builder.AppendLine("VRCLightVolumeMaterials: " + VRCLightVolumeMaterials);
                AppendMessages(builder, "Errors", Errors);
                AppendMessages(builder, "Warnings", Warnings);
                return builder.ToString();
            }

            private static void AppendMessages(StringBuilder builder, string title, List<string> messages)
            {
                if (messages == null || messages.Count == 0) return;
                builder.AppendLine();
                builder.AppendLine(title + ":");
                foreach (string message in messages.Distinct())
                {
                    builder.AppendLine("- " + message);
                }
            }
        }
    }
}
#endif
