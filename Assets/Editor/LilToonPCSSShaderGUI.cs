using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using lilToon;
namespace lilToon.PCSS.Editor
{
    public class LilToonPCSSMaterialUpgrader
    {
        private const string LILTOON_SHADER_NAME = "lilToon";
        private const string PCSS_EXTENSION_SHADER_NAME = "lilToon/PCSS Extension";

        // 繝励Ο繝代ユ繧｣縺ｮ繝槭ャ繝斐Φ繧ｰ螳夂ｾｩ・・ilToon 竊・PCSS Extension・・
        private static readonly Dictionary<string, string> PropertyMapping = new Dictionary<string, string>()
        {
            // 蝓ｺ譛ｬ繝励Ο繝代ユ繧｣
            { "_MainTex", "_MainTex" },
            { "_Color", "_Color" },
            { "_Cutoff", "_Cutoff" },
            // 繧ｷ繝｣繝峨え髢｢騾｣
            { "_UseShadow", "_UseShadow" },
            { "_ShadowColorTex", "_ShadowColorTex" },
            { "_ShadowBorder", "_ShadowBorder" },
            { "_ShadowBlur", "_ShadowBlur" },
            // 繝ｬ繝ｳ繝繝ｪ繝ｳ繧ｰ險ｭ螳・
            { "_Cull", "_Cull" },
            { "_ZWrite", "_ZWrite" },
            { "_ZTest", "_ZTest" },
            { "_SrcBlend", "_SrcBlend" },
            { "_DstBlend", "_DstBlend" },
            // 繧ｹ繝・Φ繧ｷ繝ｫ險ｭ螳・
            { "_StencilRef", "_StencilRef" },
            { "_StencilReadMask", "_StencilReadMask" },
            { "_StencilWriteMask", "_StencilWriteMask" },
            { "_StencilComp", "_StencilComp" },
            { "_StencilPass", "_StencilPass" },
            { "_StencilFail", "_StencilFail" },
            { "_StencilZFail", "_StencilZFail" },
        };

        // 繝・け繧ｹ繝√Ε繝励Ο繝代ユ繧｣縺ｮ繝ｪ繧ｹ繝・
        private static readonly HashSet<string> TextureProperties = new HashSet<string>()
        {
            "_MainTex",
            "_ShadowColorTex"
        };

        [MenuItem("Tools/lilToon PCSS Extension/Utilities/lilToon/Upgrade Selected Materials to PCSS Extension", true)]
        private static bool ValidateUpgradeMaterials()
        {
            return Selection.GetFiltered<Material>(SelectionMode.Assets).Any(m => m.shader != null && m.shader.name.Contains(LILTOON_SHADER_NAME) && m.shader.name != PCSS_EXTENSION_SHADER_NAME);
        }

        [MenuItem("Tools/lilToon PCSS Extension/Utilities/lilToon/Upgrade Selected Materials to PCSS Extension")]
        private static void UpgradeMaterials()
        {
            Shader pcssShader = Shader.Find(PCSS_EXTENSION_SHADER_NAME);
            if (pcssShader == null)
            {
                EditorUtility.DisplayDialog("繧ｨ繝ｩ繝ｼ", $"繧ｷ繧ｧ繝ｼ繝繝ｼ '{PCSS_EXTENSION_SHADER_NAME}' 縺瑚ｦ九▽縺九ｊ縺ｾ縺帙ｓ縲１CSS Extension縺梧ｭ｣縺励￥繧､繝ｳ繧ｹ繝医・繝ｫ縺輔ｌ縺ｦ縺・ｋ縺狗｢ｺ隱阪＠縺ｦ縺上□縺輔＞縲・, "OK");
                return;
            }

            var materialsToUpgrade = Selection.GetFiltered<Material>(SelectionMode.Assets)
                                              .Where(m => m.shader != null && m.shader.name.Contains(LILTOON_SHADER_NAME) && m.shader.name != PCSS_EXTENSION_SHADER_NAME)
                                              .ToList();

            if (materialsToUpgrade.Count == 0)
            {
                EditorUtility.DisplayDialog("繧｢繝・・繧ｰ繝ｬ繝ｼ繝牙ｯｾ雎｡縺ｪ縺・, "驕ｸ謚槭＆繧後◆繝槭ユ繝ｪ繧｢繝ｫ縺ｮ荳ｭ縺ｫlilToon繧ｷ繧ｧ繝ｼ繝繝ｼ繧剃ｽｿ逕ｨ縺励※縺・ｋ繧ゅ・縺後≠繧翫∪縺帙ｓ縲・, "OK");
                return;
            }
            
            Undo.RecordObjects(materialsToUpgrade.ToArray(), "Upgrade to PCSS Material");

            int upgradedCount = 0;
            foreach (var material in materialsToUpgrade)
            {
                Debug.Log($"繝槭ユ繝ｪ繧｢繝ｫ '{material.name}' 繧・'{material.shader.name}' 縺九ｉ '{PCSS_EXTENSION_SHADER_NAME}' 縺ｫ繧｢繝・・繧ｰ繝ｬ繝ｼ繝峨＠縺ｾ縺吶・);
                
                // 蜈・・繝励Ο繝代ユ繧｣蛟､繧剃ｿ晏ｭ・
                Dictionary<string, object> savedProperties = new Dictionary<string, object>();
                foreach (var mapping in PropertyMapping)
                {
                    string originalProperty = mapping.Key;
                    if (material.HasProperty(originalProperty))
                    {
                        if (TextureProperties.Contains(originalProperty))
                        {
                            savedProperties[originalProperty] = material.GetTexture(originalProperty);
                        }
                        else if (originalProperty == "_Color")
                        {
                            savedProperties[originalProperty] = material.GetColor(originalProperty);
                        }
                        else
                        {
                            savedProperties[originalProperty] = material.GetFloat(originalProperty);
                        }
                    }
                }
                
                // 繧ｷ繧ｧ繝ｼ繝繝ｼ螟画峩
                material.shader = pcssShader;
                
                // 繝・ヵ繧ｩ繝ｫ繝亥､縺ｮ險ｭ螳・
                material.SetFloat("_UsePCSS", 1.0f);
                material.SetFloat("_PCSSPresetMode", 1.0f); // Anime繝励Μ繧ｻ繝・ヨ
                material.SetFloat("_LocalPCSSFilterRadius", 0.01f);
                material.SetFloat("_LocalPCSSLightSize", 0.1f);
                material.SetFloat("_PCSSBias", 0.001f);
                material.SetFloat("_PCSSIntensity", 1.0f);
                material.SetFloat("_PCSSQuality", 1.0f); // Medium
                material.SetFloat("_LocalPCSSSamples", 16.0f);
                material.SetFloat("_UseShadowClamp", 0.0f);
                material.SetFloat("_ShadowClamp", 0.5f);
                material.SetFloat("_Translucency", 0.5f);
                material.SetFloat("_UseVRCLightVolumes", 0.0f);
                material.SetFloat("_VRCLightVolumeIntensity", 1.0f);
                material.SetColor("_VRCLightVolumeTint", Color.white);
                material.SetFloat("_VRCLightVolumeDistanceFactor", 0.1f);
                
                // 菫晏ｭ倥＠縺溘・繝ｭ繝代ユ繧｣蛟､繧貞ｾｩ蜈・
                foreach (var mapping in PropertyMapping)
                {
                    string originalProperty = mapping.Key;
                    string targetProperty = mapping.Value;
                    
                    if (savedProperties.ContainsKey(originalProperty) && material.HasProperty(targetProperty))
                    {
                        if (TextureProperties.Contains(originalProperty))
                        {
                            material.SetTexture(targetProperty, (Texture)savedProperties[originalProperty]);
                        }
                        else if (originalProperty == "_Color")
                        {
                            material.SetColor(targetProperty, (Color)savedProperties[originalProperty]);
                        }
                        else
                        {
                            material.SetFloat(targetProperty, (float)savedProperties[originalProperty]);
                        }
                    }
                }
                
                // 繧ｷ繧ｧ繝ｼ繝繝ｼ繧ｭ繝ｼ繝ｯ繝ｼ繝峨・險ｭ螳・
                material.EnableKeyword("_USEPCSS_ON");
                if (material.HasProperty("_UseShadow") && material.GetFloat("_UseShadow") > 0.5f)
                    material.EnableKeyword("_USESHADOW_ON");
                else
                    material.DisableKeyword("_USESHADOW_ON");
                
                upgradedCount++;
                EditorUtility.SetDirty(material);
            }
            
            AssetDatabase.SaveAssets();
            
            EditorUtility.DisplayDialog("繧｢繝・・繧ｰ繝ｬ繝ｼ繝牙ｮ御ｺ・, $"{upgradedCount}蛟九・繝槭ユ繝ｪ繧｢繝ｫ縺畦ilToon PCSS Extension繧ｷ繧ｧ繝ｼ繝繝ｼ縺ｫ繧｢繝・・繧ｰ繝ｬ繝ｼ繝峨＆繧後∪縺励◆縲・, "OK");
        }
    }
} 
