using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using lilToon;

namespace lilToon.PCSS.Editor
{
    // liltoon本体のShaderGUIを継承することで、互換性を確保しマテリアル消失を防ぐ
    public class LilToonPCSSShaderGUI : lilToonShaderGUI
    {
        // PCSS用のプロパティを保持
        private MaterialProperty _UsePCSS;
        private MaterialProperty _PCSSPresetMode;
        private MaterialProperty _LocalPCSSFilterRadius;
        private MaterialProperty _LocalPCSSLightSize;
        private MaterialProperty _PCSSBias;
        private MaterialProperty _PCSSIntensity;
        private MaterialProperty _PCSSQuality;
        private MaterialProperty _LocalPCSSSamples;
        private MaterialProperty _UseShadowClamp;
        private MaterialProperty _ShadowClamp;
        private MaterialProperty _Translucency;
        private MaterialProperty _UseVRCLightVolumes;
        private MaterialProperty _VRCLightVolumeIntensity;
        private MaterialProperty _VRCLightVolumeTint;
        private MaterialProperty _VRCLightVolumeDistanceFactor;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            // まずliltoon本体のGUIを描画
            base.OnGUI(materialEditor, properties);

            // マテリアルの基本プロパティを確保
            Material material = materialEditor.target as Material;
            if (material != null)
            {
                // 基本テクスチャとカラーの確保
                if (material.HasProperty("_MainTex") && material.GetTexture("_MainTex") == null)
                    material.SetTexture("_MainTex", Texture2D.whiteTexture);
                if (material.HasProperty("_Color") && material.GetColor("_Color") == default)
                    material.SetColor("_Color", Color.white);
                
                // シャドウ関連プロパティの確保
                if (material.HasProperty("_ShadowColorTex") && material.GetTexture("_ShadowColorTex") == null)
                    material.SetTexture("_ShadowColorTex", Texture2D.blackTexture);
                
                // シェーダーキーワードの設定
                SetKeyword(material, "_USEPCSS_ON", material.HasProperty("_UsePCSS") && material.GetFloat("_UsePCSS") > 0.5f);
                SetKeyword(material, "_USESHADOW_ON", material.HasProperty("_UseShadow") && material.GetFloat("_UseShadow") > 0.5f);
                SetKeyword(material, "_USESHADOWCLAMP_ON", material.HasProperty("_UseShadowClamp") && material.GetFloat("_UseShadowClamp") > 0.5f);
                SetKeyword(material, "_USEVRCLIGHT_VOLUMES_ON", material.HasProperty("_UseVRCLightVolumes") && material.GetFloat("_UseVRCLightVolumes") > 0.5f);
            }

            // PCSS拡張機能のUIを描画
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("PCSS Extension Settings", EditorStyles.boldLabel);
            
            // プロパティを取得
            _UsePCSS = FindProperty("_UsePCSS", properties, false);
            _PCSSPresetMode = FindProperty("_PCSSPresetMode", properties, false);
            _LocalPCSSFilterRadius = FindProperty("_LocalPCSSFilterRadius", properties, false);
            _LocalPCSSLightSize = FindProperty("_LocalPCSSLightSize", properties, false);
            _PCSSBias = FindProperty("_PCSSBias", properties, false);
            _PCSSIntensity = FindProperty("_PCSSIntensity", properties, false);
            _PCSSQuality = FindProperty("_PCSSQuality", properties, false);
            _LocalPCSSSamples = FindProperty("_LocalPCSSSamples", properties, false);
            _UseShadowClamp = FindProperty("_UseShadowClamp", properties, false);
            _ShadowClamp = FindProperty("_ShadowClamp", properties, false);
            _Translucency = FindProperty("_Translucency", properties, false);
            _UseVRCLightVolumes = FindProperty("_UseVRCLightVolumes", properties, false);
            _VRCLightVolumeIntensity = FindProperty("_VRCLightVolumeIntensity", properties, false);
            _VRCLightVolumeTint = FindProperty("_VRCLightVolumeTint", properties, false);
            _VRCLightVolumeDistanceFactor = FindProperty("_VRCLightVolumeDistanceFactor", properties, false);

            if (_UsePCSS != null)
            {
                materialEditor.ShaderProperty(_UsePCSS, "Use PCSS");

                if (_UsePCSS.floatValue > 0.5f)
                {
                    EditorGUI.indentLevel++;
                    
                    if (_PCSSPresetMode != null)
                    {
                        materialEditor.ShaderProperty(_PCSSPresetMode, "PCSS Preset");
                        
                        // プリセットモードに応じて自動設定
                        if (material != null && _PCSSPresetMode.floatValue < 3)
                        {
                            EditorGUI.BeginChangeCheck();
                            float presetMode = _PCSSPresetMode.floatValue;
                            
                            if (presetMode == 0) // Realistic
                            {
                                if (_LocalPCSSFilterRadius != null) material.SetFloat("_LocalPCSSFilterRadius", 0.005f);
                                if (_LocalPCSSLightSize != null) material.SetFloat("_LocalPCSSLightSize", 0.05f);
                                if (_PCSSBias != null) material.SetFloat("_PCSSBias", 0.0005f);
                                if (_PCSSIntensity != null) material.SetFloat("_PCSSIntensity", 1.0f);
                            }
                            else if (presetMode == 1) // Anime
                            {
                                if (_LocalPCSSFilterRadius != null) material.SetFloat("_LocalPCSSFilterRadius", 0.01f);
                                if (_LocalPCSSLightSize != null) material.SetFloat("_LocalPCSSLightSize", 0.1f);
                                if (_PCSSBias != null) material.SetFloat("_PCSSBias", 0.001f);
                                if (_PCSSIntensity != null) material.SetFloat("_PCSSIntensity", 1.0f);
                            }
                            else if (presetMode == 2) // Cinematic
                            {
                                if (_LocalPCSSFilterRadius != null) material.SetFloat("_LocalPCSSFilterRadius", 0.02f);
                                if (_LocalPCSSLightSize != null) material.SetFloat("_LocalPCSSLightSize", 0.2f);
                                if (_PCSSBias != null) material.SetFloat("_PCSSBias", 0.0008f);
                                if (_PCSSIntensity != null) material.SetFloat("_PCSSIntensity", 1.2f);
                            }
                            
                            if (EditorGUI.EndChangeCheck())
                            {
                                materialEditor.PropertiesChanged();
                            }
                        }
                    }
                    
                    // カスタムモードの場合は詳細設定を表示
                    if (_PCSSPresetMode == null || _PCSSPresetMode.floatValue == 3)
                    {
                        if (_LocalPCSSFilterRadius != null)
                            materialEditor.ShaderProperty(_LocalPCSSFilterRadius, "PCSS Filter Radius");
                        if (_LocalPCSSLightSize != null)
                            materialEditor.ShaderProperty(_LocalPCSSLightSize, "PCSS Light Size");
                        if (_PCSSBias != null)
                            materialEditor.ShaderProperty(_PCSSBias, "PCSS Bias");
                        if (_PCSSIntensity != null)
                            materialEditor.ShaderProperty(_PCSSIntensity, "PCSS Intensity");
                    }
                    
                    if (_PCSSQuality != null)
                        materialEditor.ShaderProperty(_PCSSQuality, "PCSS Quality");
                    
                    if (_LocalPCSSSamples != null)
                        materialEditor.ShaderProperty(_LocalPCSSSamples, "PCSS Samples");
                    
                    EditorGUI.indentLevel--;
                }
            }
            
            // シャドウクランプ設定
            if (_UseShadowClamp != null)
            {
                EditorGUILayout.Space();
                materialEditor.ShaderProperty(_UseShadowClamp, "Use Shadow Clamp (Anime Style)");
                
                if (_UseShadowClamp.floatValue > 0.5f && _ShadowClamp != null)
                {
                    EditorGUI.indentLevel++;
                    materialEditor.ShaderProperty(_ShadowClamp, "Shadow Clamp");
                    EditorGUI.indentLevel--;
                }
            }
            
            // 半透明設定
            if (_Translucency != null)
            {
                EditorGUILayout.Space();
                materialEditor.ShaderProperty(_Translucency, "Translucency");
            }
            
            // VRCライトボリューム設定
            if (_UseVRCLightVolumes != null)
            {
                EditorGUILayout.Space();
                materialEditor.ShaderProperty(_UseVRCLightVolumes, "Use VRC Light Volumes");
                
                if (_UseVRCLightVolumes.floatValue > 0.5f)
                {
                    EditorGUI.indentLevel++;
                    if (_VRCLightVolumeIntensity != null)
                        materialEditor.ShaderProperty(_VRCLightVolumeIntensity, "VRC Light Volume Intensity");
                    if (_VRCLightVolumeTint != null)
                        materialEditor.ShaderProperty(_VRCLightVolumeTint, "VRC Light Volume Tint");
                    if (_VRCLightVolumeDistanceFactor != null)
                        materialEditor.ShaderProperty(_VRCLightVolumeDistanceFactor, "VRC Light Volume Distance Factor");
                    EditorGUI.indentLevel--;
                }
            }
        }
        
        // シェーダーキーワードを設定するヘルパーメソッド
        private void SetKeyword(Material material, string keyword, bool state)
        {
            if (state)
                material.EnableKeyword(keyword);
            else
                material.DisableKeyword(keyword);
        }
    }
} 