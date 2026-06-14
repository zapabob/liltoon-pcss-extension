using UnityEditor;
using UnityEngine;

namespace lilToon.PCSS.Editor
{
    public class LilToonPCSSShaderGUI : ShaderGUI
    {
        private MaterialProperty mainTex;
        private MaterialProperty color;
        private MaterialProperty cutoff;

        private MaterialProperty useShadow;
        private MaterialProperty shadowColorTex;
        private MaterialProperty shadowBorder;
        private MaterialProperty shadowBlur;
        private MaterialProperty useShadow2;
        private MaterialProperty shadow2ColorTex;
        private MaterialProperty shadow2Border;
        private MaterialProperty shadow2Blur;
        private MaterialProperty useShadow3;
        private MaterialProperty shadow3ColorTex;
        private MaterialProperty shadow3Border;
        private MaterialProperty shadow3Blur;

        private MaterialProperty useSDFFaceShadow;
        private MaterialProperty sdfFaceShadowTex;
        private MaterialProperty sdfFaceShadowIntensity;
        private MaterialProperty sdfFaceShadowSoftness;

        private MaterialProperty usePCSS;
        private MaterialProperty pcssPresetMode;
        private MaterialProperty localPCSSFilterRadius;
        private MaterialProperty localPCSSLightSize;
        private MaterialProperty localPCSSBias;
        private MaterialProperty pcssIntensity;
        private MaterialProperty pcssQualityLevel;
        private MaterialProperty localPCSSSamples;
        private MaterialProperty useShadowMask;
        private MaterialProperty shadowMaskTex;
        private MaterialProperty shadowMaskStrength;
        private MaterialProperty usePCSSOptimization;
        private MaterialProperty pcssOptimizationLevel;
        private MaterialProperty usePCSSMobileOptimization;

        private MaterialProperty useVRChatPerformanceGate;
        private MaterialProperty pcssMaxDistance;
        private MaterialProperty pcssDistanceFade;
        private MaterialProperty useNoLightPCSSBoost;
        private MaterialProperty noLightPCSSBoostStrength;
        private MaterialProperty noLightPCSSBoostSoftness;
        private MaterialProperty noLightPCSSBoostRim;
        private MaterialProperty noLightPCSSHighlightTint;

        private MaterialProperty useGlossShadowCoherence;
        private MaterialProperty glossShadowCoherence;
        private MaterialProperty glossShadowBoost;
        private MaterialProperty glossShadowSuppression;
        private MaterialProperty glossRimStrength;
        private MaterialProperty glossSmoothness;

        private MaterialProperty useSoftFlush;
        private MaterialProperty softFlushColor;
        private MaterialProperty softFlushStrength;
        private MaterialProperty softFlushWidth;
        private MaterialProperty softFlushVerticalBias;
        private MaterialProperty useExcitedTone;
        private MaterialProperty excitedToneColor;
        private MaterialProperty excitedToneStrength;
        private MaterialProperty excitedToneBreath;
        private MaterialProperty excitedToneUpperBias;

        private MaterialProperty useVRCLightVolumes;
        private MaterialProperty vrcLightVolumeIntensity;
        private MaterialProperty vrcLightVolumeTint;
        private MaterialProperty vrcLightVolumeDistanceFactor;
        private MaterialProperty useVRCLVOptimization;
        private MaterialProperty vrcLVOptimizationLevel;

        private MaterialProperty useFur;
        private MaterialProperty furTex;
        private MaterialProperty furLength;
        private MaterialProperty furDensity;
        private MaterialProperty furSubdivision;
        private MaterialProperty furGravity;
        private MaterialProperty furWind;
        private MaterialProperty furWindSpeed;
        private MaterialProperty furAO;
        private MaterialProperty furShadow;
        private MaterialProperty useFurOptimization;
        private MaterialProperty furOptimizationLevel;

        private MaterialProperty useRealisticShadow;
        private MaterialProperty realisticShadowColor;
        private MaterialProperty realisticShadowIntensity;
        private MaterialProperty realisticShadowSoftness;
        private MaterialProperty realisticShadowOffset;
        private MaterialProperty realisticShadowScale;

        private MaterialProperty useRimShade;
        private MaterialProperty rimShadeColor;
        private MaterialProperty rimShadeIntensity;
        private MaterialProperty rimShadeWidth;

        private MaterialProperty cull;
        private MaterialProperty zWrite;
        private MaterialProperty zTest;
        private MaterialProperty srcBlend;
        private MaterialProperty dstBlend;
        private MaterialProperty stencilRef;
        private MaterialProperty stencilReadMask;
        private MaterialProperty stencilWriteMask;
        private MaterialProperty stencilComp;
        private MaterialProperty stencilPass;
        private MaterialProperty stencilFail;
        private MaterialProperty stencilZFail;

        private bool showMainSettings = true;
        private bool showShadowSettings = true;
        private bool showPCSSSettings = true;
        private bool showRealisticShadowSettings = true;
        private bool showGlossSettings = true;
        private bool showSoftFlushSettings = true;
        private bool showExcitedToneSettings = true;
        private bool showVRCLightVolumesSettings;
        private bool showFurSettings;
        private bool showRenderingSettings;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            Material material = materialEditor.target as Material;
            FindProperties(properties);

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("lilToon PCSS Extension 2.8", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "VRChat SDK 3.10.3 / lilToon 2.3.2 profile. PCSS is kept PC-first; Android and distant-view cases are faded down instead of adding always-on light cost.",
                MessageType.Info);

            DrawMain(materialEditor);
            DrawRealisticShadow(materialEditor, material);
            DrawPCSS(materialEditor, material);
            DrawGloss(materialEditor, material);
            DrawSoftFlush(materialEditor, material);
            DrawExcitedTone(materialEditor, material);
            DrawShadow(materialEditor);
            DrawVRCLightVolumes(materialEditor);
            DrawFur(materialEditor);
            DrawRendering(materialEditor);

            if (material != null)
            {
                UpdateKeywords(material);
            }
        }

        private void DrawMain(MaterialEditor materialEditor)
        {
            showMainSettings = EditorGUILayout.Foldout(showMainSettings, "Main", true);
            if (!showMainSettings) return;

            EditorGUI.indentLevel++;
            if (mainTex != null) materialEditor.TexturePropertySingleLine(new GUIContent("Main Texture"), mainTex);
            if (color != null) materialEditor.ShaderProperty(color, "Color");
            if (cutoff != null) materialEditor.ShaderProperty(cutoff, "Alpha Cutoff");
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(6);
        }

        private void DrawRealisticShadow(MaterialEditor materialEditor, Material material)
        {
            showRealisticShadowSettings = EditorGUILayout.Foldout(showRealisticShadowSettings, "Realistic Shadow", true);
            if (!showRealisticShadowSettings) return;

            EditorGUI.indentLevel++;
            if (useRealisticShadow != null) materialEditor.ShaderProperty(useRealisticShadow, "Use Realistic Shadow");
            if (Enabled(useRealisticShadow))
            {
                if (realisticShadowColor != null) materialEditor.ShaderProperty(realisticShadowColor, "Shadow Color");
                if (realisticShadowIntensity != null) materialEditor.ShaderProperty(realisticShadowIntensity, "Intensity");
                if (realisticShadowSoftness != null) materialEditor.ShaderProperty(realisticShadowSoftness, "Softness");
                if (realisticShadowOffset != null) materialEditor.ShaderProperty(realisticShadowOffset, "Caster Offset");
                if (realisticShadowScale != null) materialEditor.ShaderProperty(realisticShadowScale, "Caster Scale");
            }

            if (useRimShade != null) materialEditor.ShaderProperty(useRimShade, "Use Rim Shade");
            if (Enabled(useRimShade))
            {
                if (rimShadeColor != null) materialEditor.ShaderProperty(rimShadeColor, "Rim Shade Color");
                if (rimShadeIntensity != null) materialEditor.ShaderProperty(rimShadeIntensity, "Rim Shade Intensity");
                if (rimShadeWidth != null) materialEditor.ShaderProperty(rimShadeWidth, "Rim Shade Width");
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Hair Shadow"))
            {
                ApplyHairShadowPreset(material);
            }

            if (GUILayout.Button("Face Shadow"))
            {
                ApplyFaceShadowPreset(material);
            }

            if (GUILayout.Button("Portrait Gloss"))
            {
                ApplyPortraitGlossPreset(material);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(6);
        }

        private void DrawPCSS(MaterialEditor materialEditor, Material material)
        {
            showPCSSSettings = EditorGUILayout.Foldout(showPCSSSettings, "PCSS", true);
            if (!showPCSSSettings) return;

            EditorGUI.indentLevel++;
            if (usePCSS != null) materialEditor.ShaderProperty(usePCSS, "Use PCSS");
            if (Enabled(usePCSS))
            {
                if (pcssPresetMode != null) materialEditor.ShaderProperty(pcssPresetMode, "Preset");
                if (pcssQualityLevel != null) materialEditor.ShaderProperty(pcssQualityLevel, "Quality");
                if (localPCSSSamples != null) materialEditor.ShaderProperty(localPCSSSamples, "Samples");
                if (localPCSSFilterRadius != null) materialEditor.ShaderProperty(localPCSSFilterRadius, "Filter Radius");
                if (localPCSSLightSize != null) materialEditor.ShaderProperty(localPCSSLightSize, "Light Size");
                if (localPCSSBias != null) materialEditor.ShaderProperty(localPCSSBias, "Bias");
                if (pcssIntensity != null) materialEditor.ShaderProperty(pcssIntensity, "Intensity");

                EditorGUILayout.Space(4);
                if (usePCSSOptimization != null) materialEditor.ShaderProperty(usePCSSOptimization, "Use PCSS Optimization");
                if (Enabled(usePCSSOptimization) && pcssOptimizationLevel != null)
                {
                    materialEditor.ShaderProperty(pcssOptimizationLevel, "Optimization Level");
                }
                if (usePCSSMobileOptimization != null) materialEditor.ShaderProperty(usePCSSMobileOptimization, "Mobile Fallback");

                EditorGUILayout.Space(4);
                if (useVRChatPerformanceGate != null) materialEditor.ShaderProperty(useVRChatPerformanceGate, "VRChat Performance Gate");
                if (Enabled(useVRChatPerformanceGate))
                {
                    if (pcssMaxDistance != null) materialEditor.ShaderProperty(pcssMaxDistance, "Max Realtime Distance");
                    if (pcssDistanceFade != null) materialEditor.ShaderProperty(pcssDistanceFade, "Distance Fade");
                }
                if (useNoLightPCSSBoost != null) materialEditor.ShaderProperty(useNoLightPCSSBoost, "No-Light PCSS Boost");
                if (Enabled(useNoLightPCSSBoost))
                {
                    if (noLightPCSSBoostStrength != null) materialEditor.ShaderProperty(noLightPCSSBoostStrength, "No-Light Strength");
                    if (noLightPCSSBoostSoftness != null) materialEditor.ShaderProperty(noLightPCSSBoostSoftness, "No-Light Softness");
                    if (noLightPCSSBoostRim != null) materialEditor.ShaderProperty(noLightPCSSBoostRim, "No-Light Rim");
                    if (noLightPCSSHighlightTint != null) materialEditor.ShaderProperty(noLightPCSSHighlightTint, "No-Light Highlight Tint");
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("VRChat Safe"))
                {
                    ApplyVrchatSafePreset(material);
                }
                if (GUILayout.Button("nHaruka Plus"))
                {
                    ApplyNHarukaPlusPreset(material);
                }
                if (GUILayout.Button("Dewy Skin"))
                {
                    ApplyDewySkinGlossPreset(material);
                }
                if (GUILayout.Button("Soft Flush"))
                {
                    ApplySoftFlushSkinPreset(material);
                }
                if (GUILayout.Button("Excited Tone"))
                {
                    ApplyExcitedTonePreset(material);
                }
                if (GUILayout.Button("Studio Boost"))
                {
                    ApplyStudioBoostPreset(material);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(4);
            if (useShadowMask != null) materialEditor.ShaderProperty(useShadowMask, "Use Shadow Mask");
            if (Enabled(useShadowMask))
            {
                if (shadowMaskTex != null) materialEditor.TexturePropertySingleLine(new GUIContent("Shadow Mask"), shadowMaskTex);
                if (shadowMaskStrength != null) materialEditor.ShaderProperty(shadowMaskStrength, "Mask Strength");
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(6);
        }

        private void DrawGloss(MaterialEditor materialEditor, Material material)
        {
            showGlossSettings = EditorGUILayout.Foldout(showGlossSettings, "Gloss Shadow Coherence", true);
            if (!showGlossSettings) return;

            EditorGUI.indentLevel++;
            if (useGlossShadowCoherence != null) materialEditor.ShaderProperty(useGlossShadowCoherence, "Use Gloss Coherence");
            if (Enabled(useGlossShadowCoherence))
            {
                if (glossShadowCoherence != null) materialEditor.ShaderProperty(glossShadowCoherence, "Coherence");
                if (glossShadowBoost != null) materialEditor.ShaderProperty(glossShadowBoost, "Light Highlight");
                if (glossShadowSuppression != null) materialEditor.ShaderProperty(glossShadowSuppression, "Shadow Suppression");
                if (glossRimStrength != null) materialEditor.ShaderProperty(glossRimStrength, "Rim Highlight");
                if (glossSmoothness != null) materialEditor.ShaderProperty(glossSmoothness, "Smoothness");
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Skin Gloss"))
            {
                ApplySkinGlossPreset(material);
            }
            if (GUILayout.Button("Dewy Skin"))
            {
                ApplyDewySkinGlossPreset(material);
            }
            if (GUILayout.Button("Soft Flush"))
            {
                ApplySoftFlushSkinPreset(material);
            }
            if (GUILayout.Button("Excited"))
            {
                ApplyExcitedTonePreset(material);
            }
            if (GUILayout.Button("Studio"))
            {
                ApplyStudioBoostPreset(material);
            }
            if (GUILayout.Button("Latex Gloss"))
            {
                ApplyLatexGlossPreset(material);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(6);
        }

        private void DrawExcitedTone(MaterialEditor materialEditor, Material material)
        {
            showExcitedToneSettings = EditorGUILayout.Foldout(showExcitedToneSettings, "Excited Tone", true);
            if (!showExcitedToneSettings) return;

            EditorGUI.indentLevel++;
            if (useExcitedTone != null) materialEditor.ShaderProperty(useExcitedTone, "Use Excited Tone");
            if (Enabled(useExcitedTone))
            {
                if (excitedToneColor != null) materialEditor.ShaderProperty(excitedToneColor, "Tone Color");
                if (excitedToneStrength != null) materialEditor.ShaderProperty(excitedToneStrength, "Strength");
                if (excitedToneBreath != null) materialEditor.ShaderProperty(excitedToneBreath, "Breath");
                if (excitedToneUpperBias != null) materialEditor.ShaderProperty(excitedToneUpperBias, "Upper Bias");
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Excited Tone"))
            {
                ApplyExcitedTonePreset(material);
            }
            if (GUILayout.Button("Clear Tone"))
            {
                ClearExcitedTone(material);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(6);
        }

        private void DrawSoftFlush(MaterialEditor materialEditor, Material material)
        {
            showSoftFlushSettings = EditorGUILayout.Foldout(showSoftFlushSettings, "Soft Flush Skin", true);
            if (!showSoftFlushSettings) return;

            EditorGUI.indentLevel++;
            if (useSoftFlush != null) materialEditor.ShaderProperty(useSoftFlush, "Use Soft Flush");
            if (Enabled(useSoftFlush))
            {
                if (softFlushColor != null) materialEditor.ShaderProperty(softFlushColor, "Flush Color");
                if (softFlushStrength != null) materialEditor.ShaderProperty(softFlushStrength, "Strength");
                if (softFlushWidth != null) materialEditor.ShaderProperty(softFlushWidth, "Cheek Width");
                if (softFlushVerticalBias != null) materialEditor.ShaderProperty(softFlushVerticalBias, "Vertical Position");
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Soft Flush Skin"))
            {
                ApplySoftFlushSkinPreset(material);
            }
            if (GUILayout.Button("Clear Flush"))
            {
                ClearSoftFlush(material);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(6);
        }

        private void DrawShadow(MaterialEditor materialEditor)
        {
            showShadowSettings = EditorGUILayout.Foldout(showShadowSettings, "lilToon Shadow Layers", true);
            if (!showShadowSettings) return;

            EditorGUI.indentLevel++;
            DrawShadowLayer(materialEditor, useShadow, shadowColorTex, shadowBorder, shadowBlur, "1st Shadow");
            DrawShadowLayer(materialEditor, useShadow2, shadow2ColorTex, shadow2Border, shadow2Blur, "2nd Shadow");
            DrawShadowLayer(materialEditor, useShadow3, shadow3ColorTex, shadow3Border, shadow3Blur, "3rd Shadow");

            EditorGUILayout.Space(4);
            if (useSDFFaceShadow != null) materialEditor.ShaderProperty(useSDFFaceShadow, "Use SDF Face Shadow");
            if (Enabled(useSDFFaceShadow))
            {
                if (sdfFaceShadowTex != null) materialEditor.TexturePropertySingleLine(new GUIContent("SDF Face Shadow"), sdfFaceShadowTex);
                if (sdfFaceShadowIntensity != null) materialEditor.ShaderProperty(sdfFaceShadowIntensity, "SDF Intensity");
                if (sdfFaceShadowSoftness != null) materialEditor.ShaderProperty(sdfFaceShadowSoftness, "SDF Softness");
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(6);
        }

        private void DrawVRCLightVolumes(MaterialEditor materialEditor)
        {
            showVRCLightVolumesSettings = EditorGUILayout.Foldout(showVRCLightVolumesSettings, "VRC Light Volumes", true);
            if (!showVRCLightVolumesSettings) return;

            EditorGUI.indentLevel++;
            if (useVRCLightVolumes != null) materialEditor.ShaderProperty(useVRCLightVolumes, "Use VRC Light Volumes");
            if (Enabled(useVRCLightVolumes))
            {
                if (vrcLightVolumeIntensity != null) materialEditor.ShaderProperty(vrcLightVolumeIntensity, "Intensity");
                if (vrcLightVolumeTint != null) materialEditor.ShaderProperty(vrcLightVolumeTint, "Tint");
                if (vrcLightVolumeDistanceFactor != null) materialEditor.ShaderProperty(vrcLightVolumeDistanceFactor, "Distance Factor");
                if (useVRCLVOptimization != null) materialEditor.ShaderProperty(useVRCLVOptimization, "Use LV Optimization");
                if (Enabled(useVRCLVOptimization) && vrcLVOptimizationLevel != null)
                {
                    materialEditor.ShaderProperty(vrcLVOptimizationLevel, "LV Optimization Level");
                }
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(6);
        }

        private void DrawFur(MaterialEditor materialEditor)
        {
            showFurSettings = EditorGUILayout.Foldout(showFurSettings, "Fur", true);
            if (!showFurSettings) return;

            EditorGUI.indentLevel++;
            if (useFur != null) materialEditor.ShaderProperty(useFur, "Use Fur");
            if (Enabled(useFur))
            {
                if (furTex != null) materialEditor.TexturePropertySingleLine(new GUIContent("Fur Texture"), furTex);
                if (furLength != null) materialEditor.ShaderProperty(furLength, "Length");
                if (furDensity != null) materialEditor.ShaderProperty(furDensity, "Density");
                if (furSubdivision != null) materialEditor.ShaderProperty(furSubdivision, "Subdivision");
                if (furGravity != null) materialEditor.ShaderProperty(furGravity, "Gravity");
                if (furWind != null) materialEditor.ShaderProperty(furWind, "Wind");
                if (furWindSpeed != null) materialEditor.ShaderProperty(furWindSpeed, "Wind Speed");
                if (furAO != null) materialEditor.ShaderProperty(furAO, "Ambient Occlusion");
                if (furShadow != null) materialEditor.ShaderProperty(furShadow, "Shadow");
                if (useFurOptimization != null) materialEditor.ShaderProperty(useFurOptimization, "Use Fur Optimization");
                if (Enabled(useFurOptimization) && furOptimizationLevel != null)
                {
                    materialEditor.ShaderProperty(furOptimizationLevel, "Fur Optimization Level");
                }
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(6);
        }

        private void DrawRendering(MaterialEditor materialEditor)
        {
            showRenderingSettings = EditorGUILayout.Foldout(showRenderingSettings, "Rendering", true);
            if (!showRenderingSettings) return;

            EditorGUI.indentLevel++;
            if (cull != null) materialEditor.ShaderProperty(cull, "Cull Mode");
            if (zWrite != null) materialEditor.ShaderProperty(zWrite, "ZWrite");
            if (zTest != null) materialEditor.ShaderProperty(zTest, "ZTest");
            if (srcBlend != null) materialEditor.ShaderProperty(srcBlend, "Src Blend");
            if (dstBlend != null) materialEditor.ShaderProperty(dstBlend, "Dst Blend");
            if (stencilRef != null) materialEditor.ShaderProperty(stencilRef, "Stencil Reference");
            if (stencilReadMask != null) materialEditor.ShaderProperty(stencilReadMask, "Stencil Read Mask");
            if (stencilWriteMask != null) materialEditor.ShaderProperty(stencilWriteMask, "Stencil Write Mask");
            if (stencilComp != null) materialEditor.ShaderProperty(stencilComp, "Stencil Compare");
            if (stencilPass != null) materialEditor.ShaderProperty(stencilPass, "Stencil Pass");
            if (stencilFail != null) materialEditor.ShaderProperty(stencilFail, "Stencil Fail");
            if (stencilZFail != null) materialEditor.ShaderProperty(stencilZFail, "Stencil ZFail");
            EditorGUI.indentLevel--;
        }

        private void DrawShadowLayer(
            MaterialEditor materialEditor,
            MaterialProperty toggle,
            MaterialProperty texture,
            MaterialProperty border,
            MaterialProperty blur,
            string label)
        {
            if (toggle != null) materialEditor.ShaderProperty(toggle, "Use " + label);
            if (!Enabled(toggle)) return;
            if (texture != null) materialEditor.TexturePropertySingleLine(new GUIContent(label + " Color"), texture);
            if (border != null) materialEditor.ShaderProperty(border, label + " Border");
            if (blur != null) materialEditor.ShaderProperty(blur, label + " Blur");
        }

        private void FindProperties(MaterialProperty[] properties)
        {
            mainTex = FindProperty("_MainTex", properties, false) ?? FindProperty("_BaseMap", properties, false);
            color = FindProperty("_Color", properties, false) ?? FindProperty("_BaseColor", properties, false);
            cutoff = FindProperty("_Cutoff", properties, false) ?? FindProperty("_AlphaClip", properties, false);

            useShadow = FindProperty("_UseShadow", properties, false) ?? FindProperty("_ShadowReceive", properties, false);
            shadowColorTex = FindProperty("_ShadowColorTex", properties, false) ?? FindProperty("_ShadowColor", properties, false);
            shadowBorder = FindProperty("_ShadowBorder", properties, false) ?? FindProperty("_ShadowBorderRange", properties, false);
            shadowBlur = FindProperty("_ShadowBlur", properties, false) ?? FindProperty("_ShadowBlurMask", properties, false);
            useShadow2 = FindProperty("_UseShadow2", properties, false) ?? FindProperty("_Shadow2ndReceive", properties, false);
            shadow2ColorTex = FindProperty("_Shadow2ColorTex", properties, false) ?? FindProperty("_Shadow2ndColor", properties, false);
            shadow2Border = FindProperty("_Shadow2Border", properties, false) ?? FindProperty("_Shadow2ndBorderRange", properties, false);
            shadow2Blur = FindProperty("_Shadow2Blur", properties, false) ?? FindProperty("_Shadow2ndBlurMask", properties, false);
            useShadow3 = FindProperty("_UseShadow3", properties, false) ?? FindProperty("_Shadow3rdReceive", properties, false);
            shadow3ColorTex = FindProperty("_Shadow3ColorTex", properties, false) ?? FindProperty("_Shadow3rdColor", properties, false);
            shadow3Border = FindProperty("_Shadow3Border", properties, false) ?? FindProperty("_Shadow3rdBorderRange", properties, false);
            shadow3Blur = FindProperty("_Shadow3Blur", properties, false) ?? FindProperty("_Shadow3rdBlurMask", properties, false);

            useSDFFaceShadow = FindProperty("_UseSDFFaceShadow", properties, false);
            sdfFaceShadowTex = FindProperty("_SDFFaceShadowTex", properties, false);
            sdfFaceShadowIntensity = FindProperty("_SDFFaceShadowIntensity", properties, false);
            sdfFaceShadowSoftness = FindProperty("_SDFFaceShadowSoftness", properties, false);

            usePCSS = FindProperty("_UsePCSS", properties, false);
            pcssPresetMode = FindProperty("_PCSSPresetMode", properties, false);
            localPCSSFilterRadius = FindProperty("_LocalPCSSFilterRadius", properties, false) ?? FindProperty("_PCSSFilterRadius", properties, false);
            localPCSSLightSize = FindProperty("_LocalPCSSLightSize", properties, false) ?? FindProperty("_PCSSLightSize", properties, false);
            localPCSSBias = FindProperty("_LocalPCSSBias", properties, false) ?? FindProperty("_PCSSBias", properties, false);
            pcssIntensity = FindProperty("_PCSSIntensity", properties, false);
            pcssQualityLevel = FindProperty("_PCSSQualityLevel", properties, false) ?? FindProperty("_PCSSQuality", properties, false);
            localPCSSSamples = FindProperty("_LocalPCSSSamples", properties, false) ?? FindProperty("_PCSSSampleCount", properties, false);
            useShadowMask = FindProperty("_UseShadowMask", properties, false);
            shadowMaskTex = FindProperty("_ShadowMaskTex", properties, false);
            shadowMaskStrength = FindProperty("_ShadowMaskStrength", properties, false);
            usePCSSOptimization = FindProperty("_UsePCSSOptimization", properties, false);
            pcssOptimizationLevel = FindProperty("_PCSSOptimizationLevel", properties, false);
            usePCSSMobileOptimization = FindProperty("_UsePCSSMobileOptimization", properties, false);

            useVRChatPerformanceGate = FindProperty("_UseVRChatPerformanceGate", properties, false);
            pcssMaxDistance = FindProperty("_PCSSMaxDistance", properties, false);
            pcssDistanceFade = FindProperty("_PCSSDistanceFade", properties, false);
            useNoLightPCSSBoost = FindProperty("_UseNoLightPCSSBoost", properties, false);
            noLightPCSSBoostStrength = FindProperty("_NoLightPCSSBoostStrength", properties, false);
            noLightPCSSBoostSoftness = FindProperty("_NoLightPCSSBoostSoftness", properties, false);
            noLightPCSSBoostRim = FindProperty("_NoLightPCSSBoostRim", properties, false);
            noLightPCSSHighlightTint = FindProperty("_NoLightPCSSHighlightTint", properties, false);

            useGlossShadowCoherence = FindProperty("_UseGlossShadowCoherence", properties, false);
            glossShadowCoherence = FindProperty("_GlossShadowCoherence", properties, false);
            glossShadowBoost = FindProperty("_GlossShadowBoost", properties, false);
            glossShadowSuppression = FindProperty("_GlossShadowSuppression", properties, false);
            glossRimStrength = FindProperty("_GlossRimStrength", properties, false);
            glossSmoothness = FindProperty("_GlossSmoothness", properties, false);

            useSoftFlush = FindProperty("_UseSoftFlush", properties, false);
            softFlushColor = FindProperty("_SoftFlushColor", properties, false);
            softFlushStrength = FindProperty("_SoftFlushStrength", properties, false);
            softFlushWidth = FindProperty("_SoftFlushWidth", properties, false);
            softFlushVerticalBias = FindProperty("_SoftFlushVerticalBias", properties, false);
            useExcitedTone = FindProperty("_UseExcitedTone", properties, false);
            excitedToneColor = FindProperty("_ExcitedToneColor", properties, false);
            excitedToneStrength = FindProperty("_ExcitedToneStrength", properties, false);
            excitedToneBreath = FindProperty("_ExcitedToneBreath", properties, false);
            excitedToneUpperBias = FindProperty("_ExcitedToneUpperBias", properties, false);

            useVRCLightVolumes = FindProperty("_UseVRCLightVolumes", properties, false) ?? FindProperty("_VRCLightVolumesEnabled", properties, false);
            vrcLightVolumeIntensity = FindProperty("_VRCLightVolumeIntensity", properties, false) ?? FindProperty("_VRCLVIntensity", properties, false);
            vrcLightVolumeTint = FindProperty("_VRCLightVolumeTint", properties, false) ?? FindProperty("_VRCLVTintColor", properties, false);
            vrcLightVolumeDistanceFactor = FindProperty("_VRCLightVolumeDistanceFactor", properties, false) ?? FindProperty("_VRCLVDistanceAttenuation", properties, false);
            useVRCLVOptimization = FindProperty("_UseVRCLVOptimization", properties, false) ?? FindProperty("_VRCLVOptimizationEnabled", properties, false);
            vrcLVOptimizationLevel = FindProperty("_VRCLVOptimizationLevel", properties, false) ?? FindProperty("_VRCLVOptimizationMode", properties, false);

            useFur = FindProperty("_UseFur", properties, false) ?? FindProperty("_FurEnabled", properties, false);
            furTex = FindProperty("_FurTex", properties, false) ?? FindProperty("_FurNoiseMask", properties, false);
            furLength = FindProperty("_FurLength", properties, false) ?? FindProperty("_FurLengthMask", properties, false);
            furDensity = FindProperty("_FurDensity", properties, false) ?? FindProperty("_FurRandomize", properties, false);
            furSubdivision = FindProperty("_FurSubdivision", properties, false) ?? FindProperty("_FurLayerNum", properties, false);
            furGravity = FindProperty("_FurGravity", properties, false) ?? FindProperty("_FurGravityMask", properties, false);
            furWind = FindProperty("_FurWind", properties, false) ?? FindProperty("_FurWindMask", properties, false);
            furWindSpeed = FindProperty("_FurWindSpeed", properties, false) ?? FindProperty("_FurWindFreq", properties, false);
            furAO = FindProperty("_FurAO", properties, false) ?? FindProperty("_FurAOMask", properties, false);
            furShadow = FindProperty("_FurShadow", properties, false) ?? FindProperty("_FurMesh", properties, false);
            useFurOptimization = FindProperty("_UseFurOptimization", properties, false) ?? FindProperty("_FurOptimizationEnabled", properties, false);
            furOptimizationLevel = FindProperty("_FurOptimizationLevel", properties, false) ?? FindProperty("_FurOptimizationMode", properties, false);

            useRealisticShadow = FindProperty("_UseRealisticShadow", properties, false);
            realisticShadowColor = FindProperty("_RealisticShadowColor", properties, false);
            realisticShadowIntensity = FindProperty("_RealisticShadowIntensity", properties, false);
            realisticShadowSoftness = FindProperty("_RealisticShadowSoftness", properties, false);
            realisticShadowOffset = FindProperty("_RealisticShadowOffset", properties, false);
            realisticShadowScale = FindProperty("_RealisticShadowScale", properties, false);

            useRimShade = FindProperty("_UseRimShade", properties, false);
            rimShadeColor = FindProperty("_RimShadeColor", properties, false);
            rimShadeIntensity = FindProperty("_RimShadeIntensity", properties, false);
            rimShadeWidth = FindProperty("_RimShadeWidth", properties, false);

            cull = FindProperty("_Cull", properties, false);
            zWrite = FindProperty("_ZWrite", properties, false);
            zTest = FindProperty("_ZTest", properties, false);
            srcBlend = FindProperty("_SrcBlend", properties, false);
            dstBlend = FindProperty("_DstBlend", properties, false);
            stencilRef = FindProperty("_StencilRef", properties, false);
            stencilReadMask = FindProperty("_StencilReadMask", properties, false);
            stencilWriteMask = FindProperty("_StencilWriteMask", properties, false);
            stencilComp = FindProperty("_StencilComp", properties, false);
            stencilPass = FindProperty("_StencilPass", properties, false);
            stencilFail = FindProperty("_StencilFail", properties, false);
            stencilZFail = FindProperty("_StencilZFail", properties, false);
        }

        private static bool Enabled(MaterialProperty property)
        {
            return property != null && property.floatValue > 0.5f;
        }

        private static void ApplyHairShadowPreset(Material material)
        {
            if (material == null) return;
            SetFloat(material, "_UseRealisticShadow", 1.0f);
            SetColor(material, "_RealisticShadowColor", new Color(0.16f, 0.14f, 0.13f, 0.82f));
            SetFloat(material, "_RealisticShadowIntensity", 0.68f);
            SetFloat(material, "_RealisticShadowSoftness", 0.36f);
            SetVector(material, "_RealisticShadowOffset", new Vector4(0f, -0.018f, 0f, 0f));
            SetVector(material, "_RealisticShadowScale", new Vector4(1.045f, 1.045f, 1.045f, 1f));
            SetFloat(material, "_UseRimShade", 1.0f);
            SetFloat(material, "_RimShadeIntensity", 0.28f);
            SetFloat(material, "_RimShadeWidth", 0.58f);
            UpdateKeywords(material);
        }

        private static void ApplyFaceShadowPreset(Material material)
        {
            if (material == null) return;
            SetFloat(material, "_UseRealisticShadow", 1.0f);
            SetColor(material, "_RealisticShadowColor", new Color(0.13f, 0.12f, 0.12f, 0.64f));
            SetFloat(material, "_RealisticShadowIntensity", 0.42f);
            SetFloat(material, "_RealisticShadowSoftness", 0.62f);
            SetVector(material, "_RealisticShadowOffset", new Vector4(0f, -0.010f, 0f, 0f));
            SetVector(material, "_RealisticShadowScale", new Vector4(1.020f, 1.020f, 1.020f, 1f));
            SetFloat(material, "_UseRimShade", 1.0f);
            SetFloat(material, "_RimShadeIntensity", 0.18f);
            SetFloat(material, "_RimShadeWidth", 0.72f);
            UpdateKeywords(material);
        }

        private static void ApplyPortraitGlossPreset(Material material)
        {
            if (material == null) return;
            ApplyNHarukaPlusPreset(material);
            ApplySkinGlossPreset(material);
        }

        private static void ApplySkinGlossPreset(Material material)
        {
            if (material == null) return;
            SetFloat(material, "_UseGlossShadowCoherence", 1.0f);
            SetFloat(material, "_GlossShadowCoherence", 0.48f);
            SetFloat(material, "_GlossShadowBoost", 0.32f);
            SetFloat(material, "_GlossShadowSuppression", 0.56f);
            SetFloat(material, "_GlossRimStrength", 0.24f);
            SetFloat(material, "_GlossSmoothness", 0.62f);
            UpdateKeywords(material);
        }

        private static void ApplyLatexGlossPreset(Material material)
        {
            if (material == null) return;
            SetFloat(material, "_UseGlossShadowCoherence", 1.0f);
            SetFloat(material, "_GlossShadowCoherence", 0.80f);
            SetFloat(material, "_GlossShadowBoost", 0.72f);
            SetFloat(material, "_GlossShadowSuppression", 0.34f);
            SetFloat(material, "_GlossRimStrength", 0.58f);
            SetFloat(material, "_GlossSmoothness", 0.88f);
            UpdateKeywords(material);
        }

        private static void ApplyDewySkinGlossPreset(Material material)
        {
            if (material == null) return;
            SetFloat(material, "_UsePCSS", 1.0f);
            SetFloat(material, "_UsePCSSOptimization", 1.0f);
            SetFloat(material, "_UseVRChatPerformanceGate", 1.0f);
            SetFloat(material, "_PCSSPresetMode", 4.0f);
            SetFloat(material, "_PCSSQualityLevel", 2.0f);
            SetFloat(material, "_LocalPCSSSamples", 12.0f);
            SetFloat(material, "_LocalPCSSFilterRadius", 0.009f);
            SetFloat(material, "_LocalPCSSLightSize", 0.095f);
            SetFloat(material, "_LocalPCSSBias", 0.0008f);
            SetFloat(material, "_PCSSIntensity", 0.96f);
            SetFloat(material, "_PCSSMaxDistance", 10.0f);
            SetFloat(material, "_PCSSDistanceFade", 3.0f);
            SetFloat(material, "_UseRealisticShadow", 1.0f);
            SetFloat(material, "_RealisticShadowIntensity", 0.62f);
            SetFloat(material, "_RealisticShadowSoftness", 0.68f);
            SetFloat(material, "_UseGlossShadowCoherence", 1.0f);
            SetFloat(material, "_GlossShadowCoherence", 0.76f);
            SetFloat(material, "_GlossShadowBoost", 0.66f);
            SetFloat(material, "_GlossShadowSuppression", 0.40f);
            SetFloat(material, "_GlossRimStrength", 0.48f);
            SetFloat(material, "_GlossSmoothness", 0.88f);
            SetFloat(material, "_Translucency", 0.55f);
            SetFloat(material, "_UseLightDirectionOverride", 1.0f);
            SetVector(material, "_LightDirectionOverride", new Vector4(0.22f, 0.86f, 0.46f, 0.0f));
            SetFloat(material, "_UseNoLightPCSSBoost", 1.0f);
            SetFloat(material, "_NoLightPCSSBoostStrength", 0.48f);
            SetFloat(material, "_NoLightPCSSBoostSoftness", 0.66f);
            SetFloat(material, "_NoLightPCSSBoostRim", 0.42f);
            SetColor(material, "_NoLightPCSSHighlightTint", new Color(0.58f, 0.52f, 0.50f, 1.0f));
            UpdateKeywords(material);
        }

        private static void ApplySoftFlushSkinPreset(Material material)
        {
            if (material == null) return;
            SetFloat(material, "_UsePCSS", 1.0f);
            SetFloat(material, "_UsePCSSOptimization", 1.0f);
            SetFloat(material, "_UseVRChatPerformanceGate", 1.0f);
            SetFloat(material, "_PCSSPresetMode", 5.0f);
            SetFloat(material, "_PCSSQualityLevel", 2.0f);
            SetFloat(material, "_LocalPCSSSamples", 12.0f);
            SetFloat(material, "_LocalPCSSFilterRadius", 0.0105f);
            SetFloat(material, "_LocalPCSSLightSize", 0.105f);
            SetFloat(material, "_LocalPCSSBias", 0.0009f);
            SetFloat(material, "_PCSSIntensity", 0.93f);
            SetFloat(material, "_PCSSMaxDistance", 10.0f);
            SetFloat(material, "_PCSSDistanceFade", 2.8f);
            SetFloat(material, "_UseRealisticShadow", 1.0f);
            SetColor(material, "_RealisticShadowColor", new Color(0.23f, 0.13f, 0.14f, 0.76f));
            SetFloat(material, "_RealisticShadowIntensity", 0.58f);
            SetFloat(material, "_RealisticShadowSoftness", 0.74f);
            SetFloat(material, "_UseGlossShadowCoherence", 1.0f);
            SetFloat(material, "_GlossShadowCoherence", 0.68f);
            SetFloat(material, "_GlossShadowBoost", 0.42f);
            SetFloat(material, "_GlossShadowSuppression", 0.46f);
            SetFloat(material, "_GlossRimStrength", 0.34f);
            SetFloat(material, "_GlossSmoothness", 0.78f);
            SetFloat(material, "_Translucency", 0.50f);
            SetFloat(material, "_UseSoftFlush", 1.0f);
            SetColor(material, "_SoftFlushColor", new Color(1.0f, 0.40f, 0.36f, 1.0f));
            SetFloat(material, "_SoftFlushStrength", 0.42f);
            SetFloat(material, "_SoftFlushWidth", 0.56f);
            SetFloat(material, "_SoftFlushVerticalBias", 0.46f);
            SetFloat(material, "_UseRimShade", 1.0f);
            SetColor(material, "_RimShadeColor", new Color(1.0f, 0.45f, 0.40f, 1.0f));
            SetFloat(material, "_RimShadeIntensity", 0.08f);
            SetFloat(material, "_RimShadeWidth", 0.78f);
            SetFloat(material, "_UseLightDirectionOverride", 1.0f);
            SetVector(material, "_LightDirectionOverride", new Vector4(0.18f, 0.88f, 0.42f, 0.0f));
            SetFloat(material, "_UseNoLightPCSSBoost", 1.0f);
            SetFloat(material, "_NoLightPCSSBoostStrength", 0.52f);
            SetFloat(material, "_NoLightPCSSBoostSoftness", 0.66f);
            SetFloat(material, "_NoLightPCSSBoostRim", 0.32f);
            SetColor(material, "_NoLightPCSSHighlightTint", new Color(0.58f, 0.52f, 0.50f, 1.0f));
            UpdateKeywords(material);
        }

        private static void ApplyExcitedTonePreset(Material material)
        {
            if (material == null) return;
            SetFloat(material, "_UsePCSS", 1.0f);
            SetFloat(material, "_UsePCSSOptimization", 1.0f);
            SetFloat(material, "_UseVRChatPerformanceGate", 1.0f);
            SetFloat(material, "_PCSSPresetMode", 7.0f);
            SetFloat(material, "_PCSSQualityLevel", 2.0f);
            SetFloat(material, "_LocalPCSSSamples", 14.0f);
            SetFloat(material, "_LocalPCSSFilterRadius", 0.0110f);
            SetFloat(material, "_LocalPCSSLightSize", 0.115f);
            SetFloat(material, "_LocalPCSSBias", 0.00085f);
            SetFloat(material, "_PCSSIntensity", 1.02f);
            SetFloat(material, "_PCSSMaxDistance", 10.0f);
            SetFloat(material, "_PCSSDistanceFade", 3.0f);
            SetFloat(material, "_UseRealisticShadow", 1.0f);
            SetColor(material, "_RealisticShadowColor", new Color(0.24f, 0.12f, 0.12f, 0.74f));
            SetFloat(material, "_RealisticShadowIntensity", 0.60f);
            SetFloat(material, "_RealisticShadowSoftness", 0.72f);
            SetFloat(material, "_UseGlossShadowCoherence", 1.0f);
            SetFloat(material, "_GlossShadowCoherence", 0.72f);
            SetFloat(material, "_GlossShadowBoost", 0.50f);
            SetFloat(material, "_GlossShadowSuppression", 0.42f);
            SetFloat(material, "_GlossRimStrength", 0.42f);
            SetFloat(material, "_GlossSmoothness", 0.82f);
            SetFloat(material, "_Translucency", 0.52f);
            SetFloat(material, "_UseSoftFlush", 1.0f);
            SetColor(material, "_SoftFlushColor", new Color(1.0f, 0.40f, 0.34f, 1.0f));
            SetFloat(material, "_SoftFlushStrength", 0.26f);
            SetFloat(material, "_SoftFlushWidth", 0.60f);
            SetFloat(material, "_SoftFlushVerticalBias", 0.48f);
            SetFloat(material, "_UseExcitedTone", 1.0f);
            SetColor(material, "_ExcitedToneColor", new Color(1.0f, 0.48f, 0.34f, 1.0f));
            SetFloat(material, "_ExcitedToneStrength", 0.28f);
            SetFloat(material, "_ExcitedToneBreath", 0.0f);
            SetFloat(material, "_ExcitedToneUpperBias", 0.58f);
            SetFloat(material, "_UseRimShade", 1.0f);
            SetColor(material, "_RimShadeColor", new Color(1.0f, 0.52f, 0.42f, 1.0f));
            SetFloat(material, "_RimShadeIntensity", 0.10f);
            SetFloat(material, "_RimShadeWidth", 0.74f);
            SetFloat(material, "_UseLightDirectionOverride", 1.0f);
            SetVector(material, "_LightDirectionOverride", new Vector4(0.20f, 0.86f, 0.44f, 0.0f));
            SetFloat(material, "_UseNoLightPCSSBoost", 1.0f);
            SetFloat(material, "_NoLightPCSSBoostStrength", 0.56f);
            SetFloat(material, "_NoLightPCSSBoostSoftness", 0.66f);
            SetFloat(material, "_NoLightPCSSBoostRim", 0.40f);
            SetColor(material, "_NoLightPCSSHighlightTint", new Color(0.64f, 0.50f, 0.46f, 1.0f));
            UpdateKeywords(material);
        }

        private static void ApplyStudioBoostPreset(Material material)
        {
            if (material == null) return;
            SetFloat(material, "_UsePCSS", 1.0f);
            SetFloat(material, "_UsePCSSOptimization", 1.0f);
            SetFloat(material, "_PCSSOptimizationLevel", 0.0f);
            SetFloat(material, "_UseVRChatPerformanceGate", 1.0f);
            SetFloat(material, "_PCSSPresetMode", 6.0f);
            SetFloat(material, "_PCSSQualityLevel", 2.0f);
            SetFloat(material, "_LocalPCSSSamples", 16.0f);
            SetFloat(material, "_LocalPCSSFilterRadius", 0.0125f);
            SetFloat(material, "_LocalPCSSLightSize", 0.160f);
            SetFloat(material, "_LocalPCSSBias", 0.00065f);
            SetFloat(material, "_PCSSIntensity", 1.30f);
            SetFloat(material, "_PCSSMaxDistance", 10.0f);
            SetFloat(material, "_PCSSDistanceFade", 3.2f);
            SetFloat(material, "_UseRealisticShadow", 1.0f);
            SetFloat(material, "_RealisticShadowIntensity", 0.78f);
            SetFloat(material, "_RealisticShadowSoftness", 0.62f);
            SetFloat(material, "_UseGlossShadowCoherence", 1.0f);
            SetFloat(material, "_GlossShadowCoherence", 0.86f);
            SetFloat(material, "_GlossShadowBoost", 0.88f);
            SetFloat(material, "_GlossShadowSuppression", 0.30f);
            SetFloat(material, "_GlossRimStrength", 0.72f);
            SetFloat(material, "_GlossSmoothness", 0.92f);
            SetFloat(material, "_Translucency", 0.48f);
            SetFloat(material, "_UseRimShade", 1.0f);
            SetColor(material, "_RimShadeColor", new Color(0.95f, 0.92f, 1.0f, 1.0f));
            SetFloat(material, "_RimShadeIntensity", 0.16f);
            SetFloat(material, "_RimShadeWidth", 0.64f);
            SetFloat(material, "_UseLightDirectionOverride", 1.0f);
            SetVector(material, "_LightDirectionOverride", new Vector4(0.28f, 0.82f, 0.50f, 0.0f));
            SetFloat(material, "_UseNoLightPCSSBoost", 1.0f);
            SetFloat(material, "_NoLightPCSSBoostStrength", 0.72f);
            SetFloat(material, "_NoLightPCSSBoostSoftness", 0.70f);
            SetFloat(material, "_NoLightPCSSBoostRim", 0.55f);
            SetColor(material, "_NoLightPCSSHighlightTint", new Color(0.72f, 0.70f, 0.76f, 1.0f));
            UpdateKeywords(material);
        }

        private static void ClearSoftFlush(Material material)
        {
            if (material == null) return;
            SetFloat(material, "_UseSoftFlush", 0.0f);
            SetFloat(material, "_SoftFlushStrength", 0.0f);
            UpdateKeywords(material);
        }

        private static void ClearExcitedTone(Material material)
        {
            if (material == null) return;
            SetFloat(material, "_UseExcitedTone", 0.0f);
            SetFloat(material, "_ExcitedToneStrength", 0.0f);
            SetFloat(material, "_ExcitedToneBreath", 0.0f);
            UpdateKeywords(material);
        }

        private static void ApplyVrchatSafePreset(Material material)
        {
            if (material == null) return;
            SetFloat(material, "_UsePCSS", 1.0f);
            SetFloat(material, "_UsePCSSOptimization", 1.0f);
            SetFloat(material, "_UseVRChatPerformanceGate", 1.0f);
            SetFloat(material, "_PCSSPresetMode", 1.0f);
            SetFloat(material, "_PCSSQualityLevel", 1.0f);
            SetFloat(material, "_LocalPCSSSamples", 12.0f);
            SetFloat(material, "_LocalPCSSFilterRadius", 0.012f);
            SetFloat(material, "_LocalPCSSLightSize", 0.085f);
            SetFloat(material, "_LocalPCSSBias", 0.0012f);
            SetFloat(material, "_PCSSIntensity", 0.88f);
            SetFloat(material, "_PCSSMaxDistance", 10.0f);
            SetFloat(material, "_PCSSDistanceFade", 3.0f);
            UpdateKeywords(material);
        }

        private static void ApplyNHarukaPlusPreset(Material material)
        {
            if (material == null) return;
            SetFloat(material, "_UsePCSS", 1.0f);
            SetFloat(material, "_UsePCSSOptimization", 1.0f);
            SetFloat(material, "_UseVRChatPerformanceGate", 1.0f);
            SetFloat(material, "_PCSSPresetMode", 0.0f);
            SetFloat(material, "_PCSSQualityLevel", 2.0f);
            SetFloat(material, "_LocalPCSSSamples", 16.0f);
            SetFloat(material, "_LocalPCSSFilterRadius", 0.0065f);
            SetFloat(material, "_LocalPCSSLightSize", 0.060f);
            SetFloat(material, "_LocalPCSSBias", 0.00055f);
            SetFloat(material, "_PCSSIntensity", 1.05f);
            SetFloat(material, "_PCSSMaxDistance", 10.0f);
            SetFloat(material, "_PCSSDistanceFade", 3.0f);
            SetFloat(material, "_UseRealisticShadow", 1.0f);
            SetFloat(material, "_RealisticShadowIntensity", 0.55f);
            SetFloat(material, "_RealisticShadowSoftness", 0.42f);
            SetFloat(material, "_UseGlossShadowCoherence", 1.0f);
            SetFloat(material, "_GlossShadowCoherence", 0.58f);
            UpdateKeywords(material);
        }

        private static void UpdateKeywords(Material material)
        {
            SetKeyword(material, "_USEPCSS_ON", GetFloat(material, "_UsePCSS") > 0.5f);
            SetKeyword(material, "_USESHADOW_ON", GetFloat(material, "_UseShadow") > 0.5f || GetFloat(material, "_ShadowReceive") > 0.5f);
            SetKeyword(material, "_USESHADOW2_ON", GetFloat(material, "_UseShadow2") > 0.5f || GetFloat(material, "_Shadow2ndReceive") > 0.5f);
            SetKeyword(material, "_USESHADOW3_ON", GetFloat(material, "_UseShadow3") > 0.5f || GetFloat(material, "_Shadow3rdReceive") > 0.5f);
            SetKeyword(material, "_USESDFFACESHADOW_ON", GetFloat(material, "_UseSDFFaceShadow") > 0.5f);
            SetKeyword(material, "_USESHADOWMASK_ON", GetFloat(material, "_UseShadowMask") > 0.5f);
            SetKeyword(material, "_USEPCSSOPTIMIZATION_ON", GetFloat(material, "_UsePCSSOptimization") > 0.5f);
            SetKeyword(material, "_USEPCSSMOBILEOPTIMIZATION_ON", GetFloat(material, "_UsePCSSMobileOptimization") > 0.5f);
            SetKeyword(material, "_USEVRCHATPERFORMANCEGATE_ON", GetFloat(material, "_UseVRChatPerformanceGate") > 0.5f);
            SetKeyword(material, "_USEGLOSSSHADOWCOHERENCE_ON", GetFloat(material, "_UseGlossShadowCoherence") > 0.5f);
            SetKeyword(material, "_USENOLIGHTPCSSBOOST_ON", GetFloat(material, "_UseNoLightPCSSBoost") > 0.5f);
            SetKeyword(material, "_USESOFTFLUSH_ON", GetFloat(material, "_UseSoftFlush") > 0.5f);
            SetKeyword(material, "_USEEXCITEDTONE_ON", GetFloat(material, "_UseExcitedTone") > 0.5f);
            SetKeyword(material, "_USEVRCLIGHT_VOLUMES_ON", GetFloat(material, "_UseVRCLightVolumes") > 0.5f || GetFloat(material, "_VRCLightVolumesEnabled") > 0.5f);
            SetKeyword(material, "_USEVRCLVOPTIMIZATION_ON", GetFloat(material, "_UseVRCLVOptimization") > 0.5f || GetFloat(material, "_VRCLVOptimizationEnabled") > 0.5f);
            SetKeyword(material, "_USEFUR_ON", GetFloat(material, "_UseFur") > 0.5f || GetFloat(material, "_FurEnabled") > 0.5f);
            SetKeyword(material, "_USEFUROPTIMIZATION_ON", GetFloat(material, "_UseFurOptimization") > 0.5f || GetFloat(material, "_FurOptimizationEnabled") > 0.5f);
            SetKeyword(material, "_USEREALISTICSHADOW_ON", GetFloat(material, "_UseRealisticShadow") > 0.5f);
            SetKeyword(material, "_USERIMSHADE_ON", GetFloat(material, "_UseRimShade") > 0.5f);
        }

        private static float GetFloat(Material material, string name)
        {
            return material != null && material.HasProperty(name) ? material.GetFloat(name) : 0.0f;
        }

        private static void SetFloat(Material material, string name, float value)
        {
            if (material != null && material.HasProperty(name))
            {
                material.SetFloat(name, value);
            }
        }

        private static void SetColor(Material material, string name, Color value)
        {
            if (material != null && material.HasProperty(name))
            {
                material.SetColor(name, value);
            }
        }

        private static void SetVector(Material material, string name, Vector4 value)
        {
            if (material != null && material.HasProperty(name))
            {
                material.SetVector(name, value);
            }
        }

        private static void SetKeyword(Material material, string keyword, bool enabled)
        {
            if (enabled)
            {
                material.EnableKeyword(keyword);
            }
            else
            {
                material.DisableKeyword(keyword);
            }
        }
    }
}
