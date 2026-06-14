using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEditor;
using Anim = UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace lilToon.PCSS.Editor
{
    public static class ModularAvatarLightToggleBuilder
    {
        private const string GroupHip = "PCSS Hip Lights (MA)";
        private const string GroupAuto = "PCSS External Lights (Auto)";
        private const string RootName = "PCSS Light Controls (MA)";
        private const string ToggleGOName = "PCSS Lights Toggle";
        private const string ParamName = PCSSConstants.ParamLightOn;
        private const string ParamSway = PCSSConstants.ParamLightSway;
        private const string ControllerDir = "Assets/PCSS/Controllers";
        private const string ControllerPath = "Assets/PCSS/Controllers/PCSS_LightControl.controller";
        private const string MenuPath = "Assets/PCSS/Controllers/PCSS_LightControls_Menu.asset";

        private sealed class LightChannel
        {
            public Light Light;
            public string Label;
            public string DisplayLabel;
            public string Parameter;
            public string LayerName;
            public string ClipPrefix;
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/PCSS Hub/Rebuild Position-Labeled Light Menu", false, 45)]
        public static void Build()
        {
            GameObject avatar = Selection.activeGameObject;
            if (avatar == null)
            {
                EditorUtility.DisplayDialog("PCSS", "Select the avatar root first.", "OK");
                return;
            }

            if (!BuildForAvatar(avatar, null, includeIntensity: true, includeSway: false, out string error))
            {
                EditorUtility.DisplayDialog("PCSS", error, "OK");
                return;
            }

            EditorUtility.DisplayDialog("PCSS", "Created a Modular Avatar menu with position-labeled PCSS light controls.", "OK");
        }

        public static bool BuildForAvatar(GameObject avatar, List<Light> lights, bool includeIntensity, out string error)
        {
            return BuildForAvatar(avatar, lights, includeIntensity, includeSway: false, out error);
        }

        public static bool BuildForAvatar(GameObject avatar, List<Light> lights, bool includeIntensity, bool includeSway, out string error)
        {
            error = string.Empty;
            if (avatar == null)
            {
                error = "Select the avatar root first.";
                return false;
            }

            Type tMenuItem = FindType("nadena.dev.modular_avatar.core.ModularAvatarMenuItem");
            Type tInstaller = FindType("nadena.dev.modular_avatar.core.ModularAvatarMenuInstaller");
            Type tObjectToggle = FindType("nadena.dev.modular_avatar.core.ModularAvatarObjectToggle");
            Type tParameter = FindType("nadena.dev.modular_avatar.core.ModularAvatarParameter");
            if (tMenuItem == null || tInstaller == null || tObjectToggle == null)
            {
                error = "Modular Avatar was not found. Install Modular Avatar and run this again.";
                return false;
            }

            List<Light> targetLights = lights ?? CollectExternalLights(avatar);
            if (targetLights.Count == 0)
            {
                error = "No PCSS Light was found. Create the PCSS light rig first, or keep AAO Performance Safe mode enabled for the 0-Light workflow.";
                return false;
            }

            List<LightChannel> channels = BuildLightChannels(targetLights);
            Undo.RegisterFullObjectHierarchyUndo(avatar, "Build MA PCSS Light Menu");

            Transform existing = avatar.transform.Find(RootName);
            if (existing != null)
            {
                Undo.DestroyObjectImmediate(existing.gameObject);
            }
            RemoveEmptyRootInstallers(avatar, tInstaller);

            GameObject root = new GameObject(RootName);
            Undo.RegisterCreatedObjectUndo(root, "Create MA Light Controls Root");
            root.transform.SetParent(avatar.transform, false);

            CreateToggleObject(root.transform, tMenuItem, tObjectToggle, targetLights);
            CreateParameters(root.transform, tParameter, channels, includeIntensity, includeSway);
            if (includeIntensity)
            {
                CreatePerLightIntensityItems(root.transform, tMenuItem, channels);
            }

            Transform swayTarget = null;
            if (includeSway)
            {
                swayTarget = FindTransformByName(avatar.transform, "PB_LightTip");
                if (swayTarget == null)
                {
                    error = "PB_LightTip was not found. Create the PhysBone sway rig before adding the sway slider.";
                    return false;
                }
                CreateSwayItem(root.transform, tMenuItem);
            }

            if ((includeIntensity || includeSway) && !TryEnsureControllerAndBlendTree(avatar, channels, swayTarget, includeSway, out error))
            {
                return false;
            }
            if (includeIntensity || includeSway)
            {
                AddMergeAnimator(avatar);
            }

            Component installer = root.AddComponent(tInstaller);
            VRCExpressionsMenu menu = CreateOrUpdateLightMenu(includeIntensity, includeSway, channels);
            TrySet(installer, "menuToAppend", menu);
            TrySet(installer, "installTargetMenu", null);

            Selection.activeObject = root;
            EditorGUIUtility.PingObject(root);
            return true;
        }

        internal static VRCExpressionsMenu CreateOrUpdateLightMenu(bool includeIntensity, bool includeSway)
        {
            return CreateOrUpdateLightMenu(includeIntensity, includeSway, null);
        }

        private static VRCExpressionsMenu CreateOrUpdateLightMenu(bool includeIntensity, bool includeSway, List<LightChannel> channels)
        {
            EnsureFolders();

            VRCExpressionsMenu menu = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(MenuPath);
            if (menu == null)
            {
                menu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
                AssetDatabase.CreateAsset(menu, MenuPath);
            }

            menu.controls = new List<VRCExpressionsMenu.Control>
            {
                new VRCExpressionsMenu.Control
                {
                    name = "All PCSS Lights",
                    type = VRCExpressionsMenu.Control.ControlType.Toggle,
                    parameter = new VRCExpressionsMenu.Control.Parameter { name = ParamName },
                    value = 1f
                }
            };

            if (includeIntensity)
            {
                foreach (LightChannel channel in channels ?? CreateFallbackChannels())
                {
                    menu.controls.Add(new VRCExpressionsMenu.Control
                    {
                        name = channel.DisplayLabel,
                        type = VRCExpressionsMenu.Control.ControlType.RadialPuppet,
                        subParameters = new[]
                        {
                            new VRCExpressionsMenu.Control.Parameter { name = channel.Parameter }
                        }
                    });
                }
            }

            if (includeSway)
            {
                menu.controls.Add(new VRCExpressionsMenu.Control
                {
                    name = "Sway Strength",
                    type = VRCExpressionsMenu.Control.ControlType.RadialPuppet,
                    subParameters = new[]
                    {
                        new VRCExpressionsMenu.Control.Parameter { name = ParamSway }
                    }
                });
            }

            EditorUtility.SetDirty(menu);
            AssetDatabase.SaveAssets();
            return menu;
        }

        private static void CreateToggleObject(Transform root, Type menuItemType, Type objectToggleType, List<Light> targetLights)
        {
            GameObject toggleGO = new GameObject(ToggleGOName);
            Undo.RegisterCreatedObjectUndo(toggleGO, "Create MA Light Toggle");
            toggleGO.transform.SetParent(root, false);

            Component menuItem = toggleGO.AddComponent(menuItemType);
            TrySet(menuItem, "name", "All PCSS Lights");
            TrySet(menuItem, "menuItemName", "All PCSS Lights");
            TrySetEnum(menuItem, "type", "Toggle");
            TrySet(menuItem, "parameter", ParamName);

            Component objectToggle = toggleGO.AddComponent(objectToggleType);
            if (!TrySetObjectList(objectToggle, new[] { "objects", "targets" }, targetLights.Select(light => light.gameObject).ToList()))
            {
                TrySet(objectToggle, "targetObject", targetLights[0].gameObject);
                TrySet(objectToggle, "objectToToggle", targetLights[0].gameObject);
            }
            TrySet(objectToggle, "defaultValue", true);
            TrySet(objectToggle, "objectActive", true);
        }

        private static void CreateParameters(Transform root, Type parameterType, List<LightChannel> channels, bool includeIntensity, bool includeSway)
        {
            if (parameterType == null) return;

            CreateParameter(root, parameterType, ParamName, "Bool", 1f, saved: true);
            if (includeIntensity)
            {
                foreach (LightChannel channel in channels)
                {
                    CreateParameter(root, parameterType, channel.Parameter, "Float", 1f, saved: true);
                }
            }
            if (includeSway)
            {
                CreateParameter(root, parameterType, ParamSway, "Float", 1f, saved: true);
            }
        }

        private static void CreateParameter(Transform root, Type parameterType, string parameterName, string parameterKind, float defaultValue, bool saved)
        {
            GameObject parameterGO = new GameObject("Parameter: " + parameterName);
            Undo.RegisterCreatedObjectUndo(parameterGO, "Create MA Parameter");
            parameterGO.transform.SetParent(root, false);
            Component parameter = parameterGO.AddComponent(parameterType);
            TrySet(parameter, "name", parameterName);
            TrySet(parameter, "parameterName", parameterName);
            TrySetEnum(parameter, "parameterType", parameterKind);
            TrySet(parameter, "defaultValue", defaultValue);
            TrySet(parameter, "saved", saved);
        }

        private static void CreatePerLightIntensityItems(Transform root, Type menuItemType, List<LightChannel> channels)
        {
            foreach (LightChannel channel in channels)
            {
                GameObject puppetGO = new GameObject("PCSS Intensity - " + channel.DisplayLabel);
                Undo.RegisterCreatedObjectUndo(puppetGO, "Create MA Light Puppet");
                puppetGO.transform.SetParent(root, false);
                Component puppet = puppetGO.AddComponent(menuItemType);
                TrySet(puppet, "name", channel.DisplayLabel);
                TrySet(puppet, "menuItemName", channel.DisplayLabel);
                TrySetEnum(puppet, "type", "RadialPuppet");
                TrySet(puppet, "parameter", channel.Parameter);
            }
        }

        private static void CreateSwayItem(Transform root, Type menuItemType)
        {
            GameObject swayGO = new GameObject("PCSS Lights Sway Strength");
            Undo.RegisterCreatedObjectUndo(swayGO, "Create MA Light Sway Puppet");
            swayGO.transform.SetParent(root, false);
            Component sway = swayGO.AddComponent(menuItemType);
            TrySet(sway, "name", "Sway Strength");
            TrySet(sway, "menuItemName", "Sway Strength");
            TrySetEnum(sway, "type", "RadialPuppet");
            TrySet(sway, "parameter", ParamSway);
        }

        private static List<LightChannel> BuildLightChannels(List<Light> lights)
        {
            List<LightChannel> channels = new List<LightChannel>();
            for (int i = 0; i < lights.Count; i++)
            {
                Light light = lights[i];
                if (light == null) continue;

                string role = ResolveRole(light, i);
                string position = ResolvePosition(light);
                string label = $"{role} {position}";
                string intensityLabel = light.intensity.ToString("0.##", CultureInfo.InvariantCulture);
                string parameter = ResolveParameterName(role, channels.Count);
                string safeLabel = SanitizeIdentifier(label);
                channels.Add(new LightChannel
                {
                    Light = light,
                    Label = label,
                    DisplayLabel = $"{label} Max {intensityLabel}",
                    Parameter = parameter,
                    LayerName = "PCSS_Int_" + safeLabel,
                    ClipPrefix = "PCSS_" + safeLabel
                });
            }

            return channels;
        }

        private static List<LightChannel> CreateFallbackChannels()
        {
            return new List<LightChannel>
            {
                new LightChannel { Label = "Key Front-Right", DisplayLabel = "Key Front-Right Max 1", Parameter = PCSSConstants.ParamLightKeyIntensity },
                new LightChannel { Label = "Fill Front-Left", DisplayLabel = "Fill Front-Left Max 1", Parameter = PCSSConstants.ParamLightFillIntensity },
                new LightChannel { Label = "Rim Back-Right", DisplayLabel = "Rim Back-Right Max 1", Parameter = PCSSConstants.ParamLightRimIntensity }
            };
        }

        private static string ResolveRole(Light light, int index)
        {
            string name = (light.name ?? string.Empty).ToLowerInvariant();
            if (name.Contains("key") || name.Contains("main")) return "Key";
            if (name.Contains("fill")) return "Fill";
            if (name.Contains("rim") || name.Contains("back")) return "Rim";
            return index == 0 ? "Key" : index == 1 ? "Fill" : index == 2 ? "Rim" : $"Light{index + 1}";
        }

        private static string ResolvePosition(Light light)
        {
            Vector3 p = light.transform.localPosition;
            string depth = p.z >= 0f ? "Front" : "Back";
            string side = p.x > 0.05f ? "Right" : p.x < -0.05f ? "Left" : "Center";
            return depth + "-" + side;
        }

        private static string ResolveParameterName(string role, int index)
        {
            if (role == "Key") return PCSSConstants.ParamLightKeyIntensity;
            if (role == "Fill") return PCSSConstants.ParamLightFillIntensity;
            if (role == "Rim") return PCSSConstants.ParamLightRimIntensity;
            return $"PCSS_Light{index + 1}_Intensity";
        }

        private static string SanitizeIdentifier(string label)
        {
            char[] chars = label.Select(c => char.IsLetterOrDigit(c) ? c : '_').ToArray();
            string text = new string(chars);
            while (text.Contains("__"))
            {
                text = text.Replace("__", "_");
            }
            return text.Trim('_');
        }

        private static bool TryEnsureControllerAndBlendTree(GameObject avatar, List<LightChannel> channels, Transform swayTarget, bool includeSway, out string error)
        {
            try
            {
                EnsureControllerAndBlendTree(avatar, channels, swayTarget, includeSway);
                error = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                error = $"Failed to create PCSS light control animator: {ex.Message}";
                Debug.LogException(ex);
                return false;
            }
        }

        private static void EnsureControllerAndBlendTree(GameObject avatar, List<LightChannel> channels, Transform swayTarget = null, bool includeSway = false)
        {
            if (channels == null || channels.Count == 0) return;
            EnsureFolders();
            Anim.AnimatorController controller = AssetDatabase.LoadAssetAtPath<Anim.AnimatorController>(ControllerPath);
            if (controller == null)
            {
                controller = Anim.AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            }

            RemoveGeneratedIntensityLayers(controller);
            foreach (LightChannel channel in channels)
            {
                if (channel.Light == null) continue;
                EnsureFloatParameter(controller, channel.Parameter);
                EnsureSingleLightIntensityLayer(controller, avatar, channel);
            }

            if (includeSway && swayTarget != null)
            {
                EnsureFloatParameter(controller, ParamSway);
                EnsureSwayLayer(controller, avatar, swayTarget);
            }

            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
        }

        private static void EnsureSingleLightIntensityLayer(Anim.AnimatorController controller, GameObject avatar, LightChannel channel)
        {
            Anim.AnimatorControllerLayer layer = EnsureAnimatorLayer(controller, channel.LayerName);
            ClearStateMachine(layer.stateMachine);

            float[] thresholds = { 0f, 0.33f, 0.66f, 1f };
            List<AnimationClip> clips = new List<AnimationClip>();
            foreach (float threshold in thresholds)
            {
                AnimationClip clip = CreateLightIntensityClip(controller, channel, avatar, threshold);
                clips.Add(clip);
            }

            Anim.BlendTree tree = new Anim.BlendTree
            {
                name = channel.ClipPrefix + "_Tree",
                hideFlags = HideFlags.HideInHierarchy,
                blendType = Anim.BlendTreeType.Simple1D,
                blendParameter = channel.Parameter,
                useAutomaticThresholds = false
            };

            Anim.ChildMotion[] motions = new Anim.ChildMotion[clips.Count];
            for (int i = 0; i < clips.Count; i++)
            {
                motions[i] = new Anim.ChildMotion
                {
                    motion = clips[i],
                    threshold = thresholds[i],
                    timeScale = 1f
                };
            }
            tree.children = motions;
            AssetDatabase.AddObjectToAsset(tree, controller);

            Anim.AnimatorState state = layer.stateMachine.AddState(channel.LayerName);
            state.motion = tree;
            state.writeDefaultValues = false;
            layer.stateMachine.defaultState = state;
            CommitAnimatorLayer(controller, channel.LayerName, layer);
        }

        private static AnimationClip CreateLightIntensityClip(Anim.AnimatorController controller, LightChannel channel, GameObject avatar, float value)
        {
            AnimationClip clip = new AnimationClip { name = $"{channel.ClipPrefix}_{Mathf.RoundToInt(value * 100f)}" };
            string path = AnimationUtility.CalculateTransformPath(channel.Light.transform, avatar.transform);
            EditorCurveBinding binding = new EditorCurveBinding
            {
                path = path,
                type = typeof(Light),
                propertyName = "m_Intensity"
            };
            float baseIntensity = Mathf.Max(0f, channel.Light.intensity);
            AnimationCurve curve = AnimationCurve.Linear(0f, baseIntensity * value, 1f / 60f, baseIntensity * value);
            AnimationUtility.SetEditorCurve(clip, binding, curve);
            AssetDatabase.AddObjectToAsset(clip, controller);
            return clip;
        }

        private static void EnsureSwayLayer(Anim.AnimatorController controller, GameObject avatar, Transform swayTarget)
        {
            const string layerName = "PCSS_LightSway";
            Anim.AnimatorControllerLayer layer = EnsureAnimatorLayer(controller, layerName);
            ClearStateMachine(layer.stateMachine);

            Vector3 basePos = swayTarget.localPosition;
            if (basePos == Vector3.zero)
            {
                basePos = new Vector3(0f, 0.1f, 0.2f);
            }

            AnimationClip clipLow = CreateSwayClip(controller, "PCSS_LightSway_30", avatar, swayTarget, basePos * 0.3f);
            AnimationClip clipHigh = CreateSwayClip(controller, "PCSS_LightSway_100", avatar, swayTarget, basePos);

            Anim.BlendTree tree = new Anim.BlendTree
            {
                name = "PCSS_SwayTree",
                hideFlags = HideFlags.HideInHierarchy,
                blendType = Anim.BlendTreeType.Simple1D,
                blendParameter = ParamSway,
                useAutomaticThresholds = false
            };
            tree.AddChild(clipLow, 0f);
            tree.AddChild(clipHigh, 1f);
            AssetDatabase.AddObjectToAsset(tree, controller);

            Anim.AnimatorState state = layer.stateMachine.AddState("PCSS_Sway");
            state.motion = tree;
            state.writeDefaultValues = false;
            layer.stateMachine.defaultState = state;
            CommitAnimatorLayer(controller, layerName, layer);
        }

        private static void EnsureFloatParameter(Anim.AnimatorController controller, string parameter)
        {
            if (controller.parameters.All(p => p.name != parameter))
            {
                controller.AddParameter(parameter, AnimatorControllerParameterType.Float);
            }
        }

        private static void RemoveGeneratedIntensityLayers(Anim.AnimatorController controller)
        {
            Anim.AnimatorControllerLayer[] layers = controller.layers ?? Array.Empty<Anim.AnimatorControllerLayer>();
            controller.layers = layers
                .Where(layer => layer != null &&
                                layer.name != "PCSS_LightControl" &&
                                !layer.name.StartsWith("PCSS_Int_", StringComparison.Ordinal))
                .ToArray();
            EditorUtility.SetDirty(controller);
        }

        private static void ClearStateMachine(Anim.AnimatorStateMachine stateMachine)
        {
            if (stateMachine == null) return;
            for (int i = stateMachine.states.Length - 1; i >= 0; i--)
            {
                stateMachine.RemoveState(stateMachine.states[i].state);
            }
        }

        private static Anim.AnimatorControllerLayer EnsureAnimatorLayer(Anim.AnimatorController controller, string layerName)
        {
            Anim.AnimatorControllerLayer[] layers = controller.layers ?? Array.Empty<Anim.AnimatorControllerLayer>();
            Anim.AnimatorControllerLayer layer = layers.FirstOrDefault(candidate => candidate != null && candidate.name == layerName);
            if (layer == null)
            {
                controller.AddLayer(layerName);
                layers = controller.layers ?? Array.Empty<Anim.AnimatorControllerLayer>();
                layer = layers.FirstOrDefault(candidate => candidate != null && candidate.name == layerName);
            }

            if (layer == null)
            {
                layer = new Anim.AnimatorControllerLayer
                {
                    name = layerName,
                    defaultWeight = 1f
                };
                Array.Resize(ref layers, layers.Length + 1);
                layers[layers.Length - 1] = layer;
                controller.layers = layers;
            }

            if (layer.stateMachine == null)
            {
                Anim.AnimatorStateMachine stateMachine = new Anim.AnimatorStateMachine
                {
                    name = layerName,
                    hideFlags = HideFlags.HideInHierarchy
                };
                AssetDatabase.AddObjectToAsset(stateMachine, controller);
                layer.stateMachine = stateMachine;
            }

            layer.defaultWeight = 1f;
            CommitAnimatorLayer(controller, layerName, layer);
            return layer;
        }

        private static void CommitAnimatorLayer(Anim.AnimatorController controller, string layerName, Anim.AnimatorControllerLayer layer)
        {
            Anim.AnimatorControllerLayer[] layers = controller.layers ?? Array.Empty<Anim.AnimatorControllerLayer>();
            for (int i = 0; i < layers.Length; i++)
            {
                if (layers[i] != null && layers[i].name == layerName)
                {
                    layers[i] = layer;
                    controller.layers = layers;
                    EditorUtility.SetDirty(controller);
                    return;
                }
            }

            Array.Resize(ref layers, layers.Length + 1);
            layers[layers.Length - 1] = layer;
            controller.layers = layers;
            EditorUtility.SetDirty(controller);
        }

        private static AnimationClip CreateSwayClip(Anim.AnimatorController controller, string name, GameObject avatar, Transform swayTarget, Vector3 localPos)
        {
            AnimationClip clip = new AnimationClip { name = name };
            string path = AnimationUtility.CalculateTransformPath(swayTarget, avatar.transform);
            EditorCurveBinding bindingX = new EditorCurveBinding { path = path, type = typeof(Transform), propertyName = "m_LocalPosition.x" };
            EditorCurveBinding bindingY = new EditorCurveBinding { path = path, type = typeof(Transform), propertyName = "m_LocalPosition.y" };
            EditorCurveBinding bindingZ = new EditorCurveBinding { path = path, type = typeof(Transform), propertyName = "m_LocalPosition.z" };

            AnimationUtility.SetEditorCurve(clip, bindingX, AnimationCurve.Linear(0f, localPos.x, 1f / 60f, localPos.x));
            AnimationUtility.SetEditorCurve(clip, bindingY, AnimationCurve.Linear(0f, localPos.y, 1f / 60f, localPos.y));
            AnimationUtility.SetEditorCurve(clip, bindingZ, AnimationCurve.Linear(0f, localPos.z, 1f / 60f, localPos.z));

            AssetDatabase.AddObjectToAsset(clip, controller);
            return clip;
        }

        private static void AddMergeAnimator(GameObject avatar)
        {
            Type mergeAnimatorType = FindType("nadena.dev.modular_avatar.core.ModularAvatarMergeAnimator");
            if (mergeAnimatorType == null) return;

            RemoveGeneratedMergeAnimators(avatar, mergeAnimatorType);
            Component merge = avatar.AddComponent(mergeAnimatorType);
            Anim.AnimatorController controller = AssetDatabase.LoadAssetAtPath<Anim.AnimatorController>(ControllerPath);
            TrySet(merge, "animator", controller);
            TrySet(merge, "Animator", controller);
            TrySetEnum(merge, "layerType", "FX");
            TrySetEnum(merge, "pathMode", "Absolute");
            TrySet(merge, "matchAvatarWriteDefaults", true);
            TrySet(merge, "MatchAvatarWriteDefaults", true);
            TrySet(merge, "deleteAttachedAnimator", true);
            TrySet(merge, "DeleteAttachedAnimator", true);
            EditorUtility.SetDirty(merge);
        }

        private static void RemoveGeneratedMergeAnimators(GameObject avatar, Type mergeAnimatorType)
        {
            if (avatar == null || mergeAnimatorType == null) return;

            foreach (Component component in avatar.GetComponents(mergeAnimatorType))
            {
                if (component == null) continue;
                UnityEngine.Object animatorObject = (TryGetReflected(component, "animator") ?? TryGetReflected(component, "Animator")) as UnityEngine.Object;
                string assetPath = animatorObject != null ? AssetDatabase.GetAssetPath(animatorObject) : string.Empty;
                string objectName = animatorObject != null ? animatorObject.name : string.Empty;
                if (assetPath == ControllerPath || objectName.IndexOf("PCSS_LightControl", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Undo.DestroyObjectImmediate(component);
                }
            }
        }

        private static void RemoveEmptyRootInstallers(GameObject avatar, Type installerType)
        {
            if (avatar == null || installerType == null) return;

            Component[] installers = avatar.GetComponents(installerType);
            foreach (Component installer in installers)
            {
                if (installer == null) continue;
                if (TryGetReflected(installer, "menuToAppend") != null) continue;
                if (TryGetReflected(installer, "installTargetMenu") != null) continue;

                Undo.DestroyObjectImmediate(installer);
            }
        }

        private static Transform FindTransformByName(Transform root, string name)
        {
            if (root == null) return null;
            foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
            {
                if (transform != null && transform.name == name)
                {
                    return transform;
                }
            }
            return null;
        }

        private static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/PCSS")) AssetDatabase.CreateFolder("Assets", "PCSS");
            if (!AssetDatabase.IsValidFolder(ControllerDir)) AssetDatabase.CreateFolder("Assets/PCSS", "Controllers");
        }

        private static List<Light> CollectExternalLights(GameObject avatar)
        {
            List<Light> results = new List<Light>();
            Transform hip = avatar.transform.Find(GroupHip);
            Transform auto = avatar.transform.Find(GroupAuto);
            if (hip != null) results.AddRange(hip.GetComponentsInChildren<Light>(true));
            if (auto != null) results.AddRange(auto.GetComponentsInChildren<Light>(true));
            if (results.Count == 0)
            {
                results.AddRange(avatar.GetComponentsInChildren<Light>(true));
            }
            return results.Where(light => light != null).Distinct().ToList();
        }

        private static Type FindType(string fullName)
        {
            try
            {
                return AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly =>
                    {
                        try { return assembly.GetTypes(); }
                        catch { return Type.EmptyTypes; }
                    })
                    .FirstOrDefault(type => type.FullName == fullName);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[PCSS] Failed to search type '{fullName}': {e.Message}");
                return null;
            }
        }

        private static void TrySet(object component, string memberName, object value)
        {
            if (component == null) return;
            Type type = component.GetType();
            FieldInfo field = type.GetField(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null && (value == null || field.FieldType.IsAssignableFrom(value.GetType())))
            {
                field.SetValue(component, value);
                return;
            }

            PropertyInfo property = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property != null && property.CanWrite && (value == null || property.PropertyType.IsAssignableFrom(value.GetType())))
            {
                property.SetValue(component, value);
                return;
            }

            UnityEngine.Object unityObject = component as UnityEngine.Object;
            if (unityObject == null) return;

            SerializedObject serializedObject = new SerializedObject(unityObject);
            SerializedProperty serializedProperty = serializedObject.FindProperty(memberName);
            if (serializedProperty == null) return;

            if (serializedProperty.propertyType == SerializedPropertyType.Boolean && value is bool boolValue)
            {
                serializedProperty.boolValue = boolValue;
            }
            else if (serializedProperty.propertyType == SerializedPropertyType.Float && value is float floatValue)
            {
                serializedProperty.floatValue = floatValue;
            }
            else if (serializedProperty.propertyType == SerializedPropertyType.String && value is string stringValue)
            {
                serializedProperty.stringValue = stringValue;
            }
            else if (serializedProperty.propertyType == SerializedPropertyType.ObjectReference && value is UnityEngine.Object objectValue)
            {
                serializedProperty.objectReferenceValue = objectValue;
            }
            serializedObject.ApplyModifiedProperties();
        }

        private static object TryGetReflected(object component, string memberName)
        {
            if (component == null) return null;

            Type type = component.GetType();
            FieldInfo field = type.GetField(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                return field.GetValue(component);
            }

            PropertyInfo property = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property != null && property.CanRead)
            {
                return property.GetValue(component);
            }

            UnityEngine.Object unityObject = component as UnityEngine.Object;
            if (unityObject == null) return null;

            SerializedObject serializedObject = new SerializedObject(unityObject);
            SerializedProperty serializedProperty = serializedObject.FindProperty(memberName);
            if (serializedProperty != null && serializedProperty.propertyType == SerializedPropertyType.ObjectReference)
            {
                return serializedProperty.objectReferenceValue;
            }

            return null;
        }

        private static bool TrySetObjectList(object component, IEnumerable<string> candidateNames, List<GameObject> values)
        {
            UnityEngine.Object unityObject = component as UnityEngine.Object;
            if (unityObject == null) return false;

            foreach (string name in candidateNames)
            {
                SerializedObject serializedObject = new SerializedObject(unityObject);
                SerializedProperty serializedProperty = serializedObject.FindProperty(name);
                if (serializedProperty == null || !serializedProperty.isArray) continue;

                serializedProperty.ClearArray();
                for (int i = 0; i < values.Count; i++)
                {
                    serializedProperty.InsertArrayElementAtIndex(i);
                    SerializedProperty element = serializedProperty.GetArrayElementAtIndex(i);
                    if (element.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        element.objectReferenceValue = values[i];
                    }
                }
                serializedObject.ApplyModifiedProperties();
                return true;
            }

            return false;
        }

        private static void TrySetEnum(object component, string memberName, string enumValueName)
        {
            if (component == null) return;
            Type type = component.GetType();
            FieldInfo field = type.GetField(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null && field.FieldType.IsEnum)
            {
                object value = EnumParseSafe(field.FieldType, enumValueName);
                if (value != null) field.SetValue(component, value);
                return;
            }

            PropertyInfo property = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property != null && property.PropertyType.IsEnum && property.CanWrite)
            {
                object value = EnumParseSafe(property.PropertyType, enumValueName);
                if (value != null) property.SetValue(component, value);
            }
        }

        private static object EnumParseSafe(Type enumType, string name)
        {
            try
            {
                return Enum.Parse(enumType, name);
            }
            catch
            {
                return null;
            }
        }
    }
}
