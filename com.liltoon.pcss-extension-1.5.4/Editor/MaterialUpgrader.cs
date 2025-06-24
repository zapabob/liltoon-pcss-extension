using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using lilToon
namespace lilToon.PCSS.Editor
{
    public class LilToonPCSSMaterialUpgrader
    {
        private const string LILTOON_SHADER_NAME = "lilToon";
        private const string PCSS_EXTENSION_SHADER_NAME = "lilToon/PCSS Extension";

        // プロパティのマッピング定義（lilToon → PCSS Extension）
        private static readonly Dictionary<string, string> PropertyMapping = new Dictionary<string, string>()
        {
            // 基本プロパティ
            { "_MainTex", "_MainTex" },
            { "_Color", "_Color" },
            { "_Cutoff", "_Cutoff" },
            // シャドウ関連
            { "_UseShadow", "_UseShadow" },
            { "_ShadowColorTex", "_ShadowColorTex" },
            { "_ShadowBorder", "_ShadowBorder" },
            { "_ShadowBlur", "_ShadowBlur" },
            // レンダリング設定
            { "_Cull", "_Cull" },
            { "_ZWrite", "_ZWrite" },
            { "_ZTest", "_ZTest" },
            { "_SrcBlend", "_SrcBlend" },
            { "_DstBlend", "_DstBlend" },
            // ステンシル設定
            { "_StencilRef", "_StencilRef" },
            { "_StencilReadMask", "_StencilReadMask" },
            { "_StencilWriteMask", "_StencilWriteMask" },
            { "_StencilComp", "_StencilComp" },
            { "_StencilPass", "_StencilPass" },
            { "_StencilFail", "_StencilFail" },
            { "_StencilZFail", "_StencilZFail" },
        };

        // テクスチャプロパティのリスト
        private static readonly HashSet<string> TextureProperties = new HashSet<string>()
        {
            "_MainTex",
            "_ShadowColorTex"
        };

        [MenuItem("Assets/lilToon/Upgrade Selected Materials to PCSS Extension", true)]
        private static bool ValidateUpgradeMaterials()
        {
            return Selection.GetFiltered<Material>(SelectionMode.Assets).Any(m => m.shader != null && m.shader.name.Contains(LILTOON_SHADER_NAME) && m.shader.name != PCSS_EXTENSION_SHADER_NAME);
        }

        [MenuItem("Assets/lilToon/Upgrade Selected Materials to PCSS Extension")]
        private static void UpgradeMaterials()
        {
            Shader pcssShader = Shader.Find(PCSS_EXTENSION_SHADER_NAME);
            if (pcssShader == null)
            {
                EditorUtility.DisplayDialog("エラー", $"シェーダー '{PCSS_EXTENSION_SHADER_NAME}' が見つかりません。PCSS Extensionが正しくインストールされているか確認してください。", "OK");
                return;
            }

            var materialsToUpgrade = Selection.GetFiltered<Material>(SelectionMode.Assets)
                                              .Where(m => m.shader != null && m.shader.name.Contains(LILTOON_SHADER_NAME) && m.shader.name != PCSS_EXTENSION_SHADER_NAME)
                                              .ToList();

            if (materialsToUpgrade.Count == 0)
            {
                EditorUtility.DisplayDialog("アップグレード対象なし", "選択されたマテリアルの中にlilToonシェーダーを使用しているものがありません。", "OK");
                return;
            }
            
            Undo.RecordObjects(materialsToUpgrade.ToArray(), "Upgrade to PCSS Material");

            int upgradedCount = 0;
            foreach (var material in materialsToUpgrade)
            {
                Debug.Log($"マテリアル '{material.name}' を '{material.shader.name}' から '{PCSS_EXTENSION_SHADER_NAME}' にアップグレードします。");
                
                // 元のプロパティ値を保存
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
                
                // シェーダー変更
                material.shader = pcssShader;
                
                // デフォルト値の設定
                material.SetFloat("_UsePCSS", 1.0f);
                material.SetFloat("_PCSSPresetMode", 1.0f); // Animeプリセット
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
                
                // 保存したプロパティ値を復元
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
                
                // シェーダーキーワードの設定
                material.EnableKeyword("_USEPCSS_ON");
                if (material.HasProperty("_UseShadow") && material.GetFloat("_UseShadow") > 0.5f)
                    material.EnableKeyword("_USESHADOW_ON");
                else
                    material.DisableKeyword("_USESHADOW_ON");
                
                upgradedCount++;
                EditorUtility.SetDirty(material);
            }
            
            AssetDatabase.SaveAssets();
            
            EditorUtility.DisplayDialog("アップグレード完了", $"{upgradedCount}個のマテリアルがlilToon PCSS Extensionシェーダーにアップグレードされました。", "OK");
        }
    }
} 