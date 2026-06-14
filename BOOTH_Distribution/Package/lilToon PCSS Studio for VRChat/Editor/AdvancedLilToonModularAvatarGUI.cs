#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// Streamlined inspector for the lilToon PCSS extension when pairing with Modular Avatar.
    /// Aligns with lilToon 2.2.1 / VRChat Light Volumes 2.0.1 property naming guidance.
    /// </summary>
    public class AdvancedLilToonModularAvatarGUI : ShaderGUI
    {
        private const string AdvancedShaderName = "lilToon/Advanced Modular Avatar Integration";

        private enum FieldType
        {
            Shader,
            Toggle,
            Texture
        }

        private struct FieldDefinition
        {
            public FieldDefinition(string label, FieldType type, string keyword, string[] propertyNames)
            {
                Label = label;
                Type = type;
                Keyword = keyword;
                PropertyNames = propertyNames;
            }

            public string Label;
            public FieldType Type;
            public string Keyword;
            public string[] PropertyNames;

            public static FieldDefinition Shader(string label, params string[] propertyNames)
            {
                return new FieldDefinition(label, FieldType.Shader, string.Empty, propertyNames);
            }

            public static FieldDefinition Texture(string label, params string[] propertyNames)
            {
                return new FieldDefinition(label, FieldType.Texture, string.Empty, propertyNames);
            }

            public static FieldDefinition Toggle(string label, string keyword, params string[] propertyNames)
            {
                return new FieldDefinition(label, FieldType.Toggle, keyword, propertyNames);
            }
        }

        private struct SectionDefinition
        {
            public SectionDefinition(string label, string icon, bool defaultExpanded, FieldDefinition[] fields)
            {
                Label = label;
                Icon = icon;
                DefaultExpanded = defaultExpanded;
                Fields = fields;
            }

            public string Label;
            public string Icon;
            public bool DefaultExpanded;
            public FieldDefinition[] Fields;
        }

        private static readonly SectionDefinition[] Sections = new SectionDefinition[]
        {
            new SectionDefinition(
                "Basic Settings",
                "⚙️",
                true,
                new FieldDefinition[]
                {
                    FieldDefinition.Texture("Main Texture", "_MainTex", "_BaseMap"),
                    FieldDefinition.Shader("Base Color", "_Color", "_BaseColor"),
                    FieldDefinition.Shader("Alpha Cutoff", "_Cutoff", "_AlphaClip")
                }),
            new SectionDefinition(
                "PCSS Settings",
                "🌘",
                true,
                new FieldDefinition[]
                {
                    FieldDefinition.Toggle("Use PCSS", "_LIL_PCSS_ENABLED", "_UsePCSS"),
                    FieldDefinition.Shader("Preset Mode", "_PCSSPresetMode"),
                    FieldDefinition.Shader("Quality Level", "_PCSSQualityLevel"),
                    FieldDefinition.Shader("PCSS Samples", "_LocalPCSSSamples"),
                    FieldDefinition.Shader("Filter Radius", "_LocalPCSSFilterRadius"),
                    FieldDefinition.Shader("Light Size", "_LocalPCSSLightSize"),
                    FieldDefinition.Shader("Bias", "_LocalPCSSBias"),
                    FieldDefinition.Shader("Intensity", "_PCSSIntensity"),
                    FieldDefinition.Toggle("Use Shadow Mask", string.Empty, "_UseShadowMask"),
                    FieldDefinition.Texture("Shadow Mask", "_ShadowMaskTex"),
                    FieldDefinition.Shader("Shadow Mask Strength", "_ShadowMaskStrength"),
                    FieldDefinition.Toggle("Use PCSS Optimization", string.Empty, "_UsePCSSOptimization"),
                    FieldDefinition.Shader("Optimization Level", "_PCSSOptimizationLevel"),
                    FieldDefinition.Toggle("Enable Mobile Optimization", string.Empty, "_UsePCSSMobileOptimization")
                }),
            new SectionDefinition(
                "VRC Light Volumes",
                "💡",
                true,
                new FieldDefinition[]
                {
                    FieldDefinition.Toggle("Enable VRC Light Volumes", string.Empty, "_UseVRCLightVolumes", "_VRCLightVolumesEnabled"),
                    FieldDefinition.Shader("Intensity", "_VRCLightVolumeIntensity", "_VRCLVIntensity"),
                    FieldDefinition.Shader("Tint", "_VRCLightVolumeTint", "_VRCLVTintColor"),
                    FieldDefinition.Shader("Distance Factor", "_VRCLightVolumeDistanceFactor", "_VRCLVDistanceAttenuation"),
                    FieldDefinition.Toggle("Use Optimization", string.Empty, "_UseVRCLVOptimization", "_VRCLVOptimizationEnabled"),
                    FieldDefinition.Shader("Optimization Level", "_VRCLVOptimizationLevel")
                }),
            new SectionDefinition(
                "Realistic Shadow & RimShade",
                "🌓",
                false,
                new FieldDefinition[]
                {
                    FieldDefinition.Toggle("Use Realistic Shadow", string.Empty, "_UseRealisticShadow"),
                    FieldDefinition.Shader("Shadow Color", "_RealisticShadowColor"),
                    FieldDefinition.Shader("Shadow Intensity", "_RealisticShadowIntensity"),
                    FieldDefinition.Shader("Shadow Softness", "_RealisticShadowSoftness"),
                    FieldDefinition.Shader("Shadow Offset", "_RealisticShadowOffset"),
                    FieldDefinition.Shader("Shadow Scale", "_RealisticShadowScale"),
                    FieldDefinition.Toggle("Use RimShade", string.Empty, "_UseRimShade"),
                    FieldDefinition.Shader("RimShade Color", "_RimShadeColor"),
                    FieldDefinition.Shader("RimShade Intensity", "_RimShadeIntensity"),
                    FieldDefinition.Shader("RimShade Width", "_RimShadeWidth")
                }),
            new SectionDefinition(
                "Rendering & Stencil",
                "🛠️",
                false,
                new FieldDefinition[]
                {
                    FieldDefinition.Shader("Cull", "_Cull"),
                    FieldDefinition.Shader("ZWrite", "_ZWrite"),
                    FieldDefinition.Shader("ZTest", "_ZTest"),
                    FieldDefinition.Shader("Src Blend", "_SrcBlend"),
                    FieldDefinition.Shader("Dst Blend", "_DstBlend"),
                    FieldDefinition.Shader("Stencil Ref", "_StencilRef"),
                    FieldDefinition.Shader("Stencil Read Mask", "_StencilReadMask"),
                    FieldDefinition.Shader("Stencil Write Mask", "_StencilWriteMask"),
                    FieldDefinition.Shader("Stencil Compare", "_StencilComp"),
                    FieldDefinition.Shader("Stencil Pass", "_StencilPass"),
                    FieldDefinition.Shader("Stencil Fail", "_StencilFail"),
                    FieldDefinition.Shader("Stencil ZFail", "_StencilZFail")
                })
        };

        private readonly Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            Material material = materialEditor.target as Material;
            if (material == null)
            {
                return;
            }

            Dictionary<string, MaterialProperty> propertyMap = BuildPropertyMap(properties);

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("lilToon PCSS Modular Avatar Integration", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Inspector trimmed to the documented lilToon PCSS + VRChat Light Volumes workflow.", MessageType.Info);
            EditorGUILayout.Space(8);

            for (int i = 0; i < Sections.Length; i++)
            {
                SectionDefinition section = Sections[i];
                bool expanded = GetFoldoutState(section);
                expanded = EditorGUILayout.Foldout(expanded, section.Icon + " " + section.Label, true);
                foldoutStates[section.Label] = expanded;

                if (!expanded)
                {
                    continue;
                }

                EditorGUI.indentLevel++;
                DrawSection(section, materialEditor, material, propertyMap);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(6);
            }

            DrawActions(material);
        }

        private static Dictionary<string, MaterialProperty> BuildPropertyMap(MaterialProperty[] properties)
        {
            Dictionary<string, MaterialProperty> map = new Dictionary<string, MaterialProperty>(properties.Length);
            for (int i = 0; i < properties.Length; i++)
            {
                MaterialProperty property = properties[i];
                if (!map.ContainsKey(property.name))
                {
                    map.Add(property.name, property);
                }
            }

            return map;
        }

        private bool GetFoldoutState(SectionDefinition section)
        {
            bool state;
            if (!foldoutStates.TryGetValue(section.Label, out state))
            {
                state = section.DefaultExpanded;
            }

            return state;
        }

        private void DrawSection(SectionDefinition section, MaterialEditor materialEditor, Material material, Dictionary<string, MaterialProperty> propertyMap)
        {
            FieldDefinition[] fields = section.Fields;
            for (int i = 0; i < fields.Length; i++)
            {
                FieldDefinition field = fields[i];
                DrawField(field, materialEditor, material, propertyMap);
            }
        }

        private void DrawField(FieldDefinition field, MaterialEditor materialEditor, Material material, Dictionary<string, MaterialProperty> propertyMap)
        {
            switch (field.Type)
            {
                case FieldType.Texture:
                    DrawTextureField(field, materialEditor, material, propertyMap);
                    break;
                case FieldType.Toggle:
                    DrawToggleField(field, material, propertyMap);
                    break;
                default:
                    DrawShaderField(field, materialEditor, material, propertyMap);
                    break;
            }
        }

        private void DrawShaderField(FieldDefinition field, MaterialEditor materialEditor, Material material, Dictionary<string, MaterialProperty> propertyMap)
        {
            MaterialProperty property;
            if (!TryGetProperty(propertyMap, field.PropertyNames, out property))
            {
                return;
            }

            EditorGUI.BeginChangeCheck();
            materialEditor.ShaderProperty(property, field.Label);
            if (EditorGUI.EndChangeCheck())
            {
                SyncMaterialAliases(material, property, field.PropertyNames);
            }
        }

        private void DrawTextureField(FieldDefinition field, MaterialEditor materialEditor, Material material, Dictionary<string, MaterialProperty> propertyMap)
        {
            MaterialProperty property;
            if (!TryGetProperty(propertyMap, field.PropertyNames, out property))
            {
                return;
            }

            EditorGUI.BeginChangeCheck();
            materialEditor.TexturePropertySingleLine(new GUIContent(field.Label), property);
            if (EditorGUI.EndChangeCheck())
            {
                SyncMaterialAliases(material, property, field.PropertyNames);
            }
        }

        private void DrawToggleField(FieldDefinition field, Material material, Dictionary<string, MaterialProperty> propertyMap)
        {
            MaterialProperty property;
            if (!TryGetProperty(propertyMap, field.PropertyNames, out property))
            {
                return;
            }

            bool current = property.floatValue > 0.5f;
            EditorGUI.BeginChangeCheck();
            bool next = EditorGUILayout.Toggle(field.Label, current);
            if (!EditorGUI.EndChangeCheck())
            {
                return;
            }

            float value = next ? 1.0f : 0.0f;
            property.floatValue = value;

            if (material != null)
            {
                for (int i = 0; i < field.PropertyNames.Length; i++)
                {
                    string alias = field.PropertyNames[i];
                    if (material.HasProperty(alias))
                    {
                        material.SetFloat(alias, value);
                    }
                }

                if (!string.IsNullOrEmpty(field.Keyword))
                {
                    if (next)
                    {
                        material.EnableKeyword(field.Keyword);
                    }
                    else
                    {
                        material.DisableKeyword(field.Keyword);
                    }
                }
            }
        }

        private static bool TryGetProperty(Dictionary<string, MaterialProperty> propertyMap, string[] propertyNames, out MaterialProperty property)
        {
            for (int i = 0; i < propertyNames.Length; i++)
            {
                string name = propertyNames[i];
                if (propertyMap.TryGetValue(name, out property))
                {
                    return true;
                }
            }

            property = null;
            return false;
        }

        private static void SyncMaterialAliases(Material material, MaterialProperty source, string[] aliases)
        {
            if (material == null)
            {
                return;
            }

            for (int i = 0; i < aliases.Length; i++)
            {
                string alias = aliases[i];
                if (alias == source.name || !material.HasProperty(alias))
                {
                    continue;
                }

                switch (source.type)
                {
                    case MaterialProperty.PropType.Color:
                        material.SetColor(alias, source.colorValue);
                        break;
                    case MaterialProperty.PropType.Vector:
                        material.SetVector(alias, source.vectorValue);
                        break;
                    case MaterialProperty.PropType.Texture:
                        material.SetTexture(alias, source.textureValue);
                        Vector4 scaleOffset = source.textureScaleAndOffset;
                        Vector2 scale = new Vector2(scaleOffset.x, scaleOffset.y);
                        Vector2 offset = new Vector2(scaleOffset.z, scaleOffset.w);
                        material.SetTextureScale(alias, scale);
                        material.SetTextureOffset(alias, offset);
                        break;
                    case MaterialProperty.PropType.Range:
                    case MaterialProperty.PropType.Float:
                        material.SetFloat(alias, source.floatValue);
                        break;
                    default:
                        break;
                }
            }
        }

        private void DrawActions(Material material)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Actions", EditorStyles.miniBoldLabel);

            if (GUILayout.Button("Setup Modular Avatar Integration"))
            {
                SetupModularAvatarIntegration(material);
            }

            if (GUILayout.Button("Reset Recommended Values"))
            {
                SetDefaultValues(material);
            }
        }

        private void SetDefaultValues(Material material)
        {
            if (material == null)
            {
                return;
            }

            SetFloat(material, 1.0f, "_UsePCSS");
            SetFloat(material, 1.0f, "_PCSSPresetMode");
            SetFloat(material, 2.0f, "_PCSSQualityLevel");
            SetFloat(material, 16.0f, "_LocalPCSSSamples");
            SetFloat(material, 0.015f, "_LocalPCSSFilterRadius");
            SetFloat(material, 0.1f, "_LocalPCSSLightSize");
            SetFloat(material, 0.001f, "_LocalPCSSBias");
            SetFloat(material, 1.0f, "_PCSSIntensity");
            SetFloat(material, 0.0f, "_UseShadowMask");
            SetFloat(material, 1.0f, "_UsePCSSOptimization");
            SetFloat(material, 1.0f, "_PCSSOptimizationLevel");
            SetFloat(material, 0.0f, "_UsePCSSMobileOptimization");

            SetFloat(material, 1.0f, "_UseVRCLightVolumes", "_VRCLightVolumesEnabled");
            SetFloat(material, 1.0f, "_VRCLightVolumeIntensity", "_VRCLVIntensity");
            SetColor(material, Color.white, "_VRCLightVolumeTint", "_VRCLVTintColor");
            SetFloat(material, 0.15f, "_VRCLightVolumeDistanceFactor", "_VRCLVDistanceAttenuation");
            SetFloat(material, 1.0f, "_UseVRCLVOptimization", "_VRCLVOptimizationEnabled");
            SetFloat(material, 1.0f, "_VRCLVOptimizationLevel");

            SetFloat(material, 0.0f, "_UseRealisticShadow");
            SetColor(material, new Color(0.2f, 0.2f, 0.2f, 0.8f), "_RealisticShadowColor");
            SetFloat(material, 0.5f, "_RealisticShadowIntensity");
            SetFloat(material, 0.3f, "_RealisticShadowSoftness");
            SetVector(material, Vector4.zero, "_RealisticShadowOffset");
            SetVector(material, Vector4.one, "_RealisticShadowScale");

            SetFloat(material, 0.0f, "_UseRimShade");
            SetColor(material, new Color(0.3f, 0.3f, 0.3f, 1.0f), "_RimShadeColor");
            SetFloat(material, 0.4f, "_RimShadeIntensity");
            SetFloat(material, 0.5f, "_RimShadeWidth");

            EditorUtility.SetDirty(material);
        }

        private static void SetFloat(Material material, float value, params string[] propertyNames)
        {
            for (int i = 0; i < propertyNames.Length; i++)
            {
                string name = propertyNames[i];
                if (material.HasProperty(name))
                {
                    material.SetFloat(name, value);
                }
            }
        }

        private static void SetColor(Material material, Color value, params string[] propertyNames)
        {
            for (int i = 0; i < propertyNames.Length; i++)
            {
                string name = propertyNames[i];
                if (material.HasProperty(name))
                {
                    material.SetColor(name, value);
                }
            }
        }

        private static void SetVector(Material material, Vector4 value, params string[] propertyNames)
        {
            for (int i = 0; i < propertyNames.Length; i++)
            {
                string name = propertyNames[i];
                if (material.HasProperty(name))
                {
                    material.SetVector(name, value);
                }
            }
        }

        private void SetupModularAvatarIntegration(Material material)
        {
            GameObject avatarRoot = GetAvatarRoot();
            if (avatarRoot == null)
            {
                EditorUtility.DisplayDialog("Avatar Not Selected", "Select the avatar root object in the hierarchy before running the setup.", "OK");
                return;
            }

            Undo.RegisterFullObjectHierarchyUndo(avatarRoot, "Setup Modular Avatar PCSS");

            lilToon.PCSS.Runtime.ModularAvatarPCSSController controller = avatarRoot.GetComponent<lilToon.PCSS.Runtime.ModularAvatarPCSSController>();
            if (controller == null)
            {
                Undo.AddComponent<lilToon.PCSS.Runtime.ModularAvatarPCSSController>(avatarRoot);
            }

            SetFloat(material, 1.0f, "_UsePCSS");
            SetFloat(material, 1.0f, "_UseVRCLightVolumes", "_VRCLightVolumesEnabled");
            EditorUtility.SetDirty(material);

            EditorUtility.DisplayDialog("Setup Complete", "Modular Avatar PCSS controller added to the selected avatar.", "OK");
        }

        private static GameObject GetAvatarRoot()
        {
            return Selection.activeGameObject;
        }
    }
}
#endif
