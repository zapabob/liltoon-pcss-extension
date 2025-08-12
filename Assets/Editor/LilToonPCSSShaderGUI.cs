using UnityEditor;
using UnityEngine;

namespace lilToon.PCSS.Editor
{
    public class LilToonPCSSShaderGUI : ShaderGUI
    {
        private static bool foldMain = true;
        private static bool foldShadows = true;
        private static bool foldPCSS = true;
        private static bool foldShadowMask = false;
        private static bool foldSDF = false;
        private static bool foldLTCGI = false;
        private static bool foldBacklight = false;
        private static bool foldLightDirOverride = false;
        private static bool foldVRCLightVolumes = false;
        private static bool foldFlipbook = false;
        private static bool foldRendering = false;
        private static bool foldStencil = false;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            var mat = materialEditor.target as Material;
            if (mat == null) return;

            EditorGUI.BeginChangeCheck();

            DrawMain(materialEditor, properties);
            DrawShadows(materialEditor, properties, mat);
            DrawPCSS(materialEditor, properties, mat);
            DrawShadowMask(materialEditor, properties, mat);
            DrawSdfFaceShadow(materialEditor, properties, mat);
            DrawLTCGI(materialEditor, properties, mat);
            DrawBacklight(materialEditor, properties, mat);
            DrawLightDirectionOverride(materialEditor, properties, mat);
            DrawVRCLightVolumes(materialEditor, properties, mat);
            DrawFlipbook(materialEditor, properties, mat);
            DrawRendering(materialEditor, properties, mat);
            DrawStencil(materialEditor, properties);

            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in materialEditor.targets)
                {
                    var m = obj as Material;
                    if (m == null) continue;
                    EditorUtility.SetDirty(m);
                }
            }
        }

        private static void ToggleKeyword(Material mat, bool on, string keyword)
        {
            if (on) mat.EnableKeyword(keyword); else mat.DisableKeyword(keyword);
        }

        private static void DrawMain(MaterialEditor me, MaterialProperty[] props)
        {
            foldMain = EditorGUILayout.BeginFoldoutHeaderGroup(foldMain, "Main");
            if (foldMain)
            {
                me.TexturePropertySingleLine(new GUIContent("Main Texture"), FindProperty("_MainTex", props), FindProperty("_Color", props));
                me.ShaderProperty(FindProperty("_Cutoff", props), "Alpha Cutoff");
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private static void DrawShadows(MaterialEditor me, MaterialProperty[] props, Material mat)
        {
            foldShadows = EditorGUILayout.BeginFoldoutHeaderGroup(foldShadows, "Shadows");
            if (foldShadows)
            {
                var useShadow = FindProperty("_UseShadow", props);
                me.ShaderProperty(useShadow, "Use Shadow");
                ToggleKeyword(mat, useShadow.floatValue > 0.5f, "_USESHADOW_ON");
                if (useShadow.floatValue > 0.5f)
                {
                    me.TexturePropertySingleLine(new GUIContent("Shadow Color"), FindProperty("_ShadowColorTex", props));
                    me.ShaderProperty(FindProperty("_ShadowBorder", props), "Shadow Border");
                    me.ShaderProperty(FindProperty("_ShadowBlur", props), "Shadow Blur");
                }

                var useShadow2 = FindProperty("_UseShadow2", props);
                me.ShaderProperty(useShadow2, "Use 2nd Shadow");
                ToggleKeyword(mat, useShadow2.floatValue > 0.5f, "_USESHADOW2_ON");
                if (useShadow2.floatValue > 0.5f)
                {
                    me.TexturePropertySingleLine(new GUIContent("2nd Shadow Color"), FindProperty("_Shadow2ColorTex", props));
                    me.ShaderProperty(FindProperty("_Shadow2Border", props), "2nd Border");
                    me.ShaderProperty(FindProperty("_Shadow2Blur", props), "2nd Blur");
                }

                var useShadow3 = FindProperty("_UseShadow3", props);
                me.ShaderProperty(useShadow3, "Use 3rd Shadow");
                ToggleKeyword(mat, useShadow3.floatValue > 0.5f, "_USESHADOW3_ON");
                if (useShadow3.floatValue > 0.5f)
                {
                    me.TexturePropertySingleLine(new GUIContent("3rd Shadow Color"), FindProperty("_Shadow3ColorTex", props));
                    me.ShaderProperty(FindProperty("_Shadow3Border", props), "3rd Border");
                    me.ShaderProperty(FindProperty("_Shadow3Blur", props), "3rd Blur");
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private static void DrawPCSS(MaterialEditor me, MaterialProperty[] props, Material mat)
        {
            foldPCSS = EditorGUILayout.BeginFoldoutHeaderGroup(foldPCSS, "PCSS");
            if (foldPCSS)
            {
                var use = FindProperty("_UsePCSS", props);
                me.ShaderProperty(use, "Use PCSS");
                ToggleKeyword(mat, use.floatValue > 0.5f, "_USEPCSS_ON");

                me.ShaderProperty(FindProperty("_PCSSPresetMode", props), "Preset");
                me.ShaderProperty(FindProperty("_PCSSQualityLevel", props), "Quality");
                me.ShaderProperty(FindProperty("_LocalPCSSSamples", props), "Samples");
                me.ShaderProperty(FindProperty("_LocalPCSSFilterRadius", props), "Filter Radius");
                me.ShaderProperty(FindProperty("_LocalPCSSLightSize", props), "Light Size");
                me.ShaderProperty(FindProperty("_LocalPCSSBias", props), "Bias");
                me.ShaderProperty(FindProperty("_PCSSIntensity", props), "Intensity");

                bool optimized = mat.IsKeywordEnabled("_USE_OPTIMIZED_PCSS_ON");
                bool newOptimized = EditorGUILayout.Toggle("Optimized PCSS", optimized);
                if (newOptimized != optimized)
                {
                    ToggleKeyword(mat, newOptimized, "_USE_OPTIMIZED_PCSS_ON");
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private static void DrawShadowMask(MaterialEditor me, MaterialProperty[] props, Material mat)
        {
            foldShadowMask = EditorGUILayout.BeginFoldoutHeaderGroup(foldShadowMask, "Shadow Mask");
            if (foldShadowMask)
            {
                var use = FindProperty("_UseShadowMask", props);
                me.ShaderProperty(use, "Use Shadow Mask");
                ToggleKeyword(mat, use.floatValue > 0.5f, "_USESHADOWMASK_ON");
                if (use.floatValue > 0.5f)
                {
                    me.TexturePropertySingleLine(new GUIContent("Mask (R:Cast G:Receive)"), FindProperty("_ShadowMaskTex", props));
                    me.ShaderProperty(FindProperty("_ShadowMaskStrength", props), "Strength");
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private static void DrawSdfFaceShadow(MaterialEditor me, MaterialProperty[] props, Material mat)
        {
            foldSDF = EditorGUILayout.BeginFoldoutHeaderGroup(foldSDF, "SDF Face Shadow");
            if (foldSDF)
            {
                var use = FindProperty("_UseSDFFaceShadow", props);
                me.ShaderProperty(use, "Use SDF Face Shadow");
                ToggleKeyword(mat, use.floatValue > 0.5f, "_USESDFFACESHADOW_ON");
                if (use.floatValue > 0.5f)
                {
                    me.TexturePropertySingleLine(new GUIContent("SDF Texture"), FindProperty("_SDFFaceShadowTex", props));
                    me.ShaderProperty(FindProperty("_SDFFaceShadowIntensity", props), "Intensity");
                    me.ShaderProperty(FindProperty("_SDFFaceShadowSoftness", props), "Softness");
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private static void DrawLTCGI(MaterialEditor me, MaterialProperty[] props, Material mat)
        {
            foldLTCGI = EditorGUILayout.BeginFoldoutHeaderGroup(foldLTCGI, "LTCGI");
            if (foldLTCGI)
            {
                var use = FindProperty("_UseLTCGI", props);
                me.ShaderProperty(use, "Use LTCGI");
                ToggleKeyword(mat, use.floatValue > 0.5f, "_USELTCGI_ON");
                if (use.floatValue > 0.5f)
                {
                    me.ShaderProperty(FindProperty("_LTCGIIntensity", props), "Intensity");
                    me.ShaderProperty(FindProperty("_LTCGISamples", props), "Samples");
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private static void DrawBacklight(MaterialEditor me, MaterialProperty[] props, Material mat)
        {
            foldBacklight = EditorGUILayout.BeginFoldoutHeaderGroup(foldBacklight, "Backlight");
            if (foldBacklight)
            {
                var use = FindProperty("_UseBacklight", props);
                me.ShaderProperty(use, "Use Backlight");
                ToggleKeyword(mat, use.floatValue > 0.5f, "_USEBACKLIGHT_ON");
                if (use.floatValue > 0.5f)
                {
                    me.ColorProperty(FindProperty("_BacklightColor", props), "Color");
                    me.ShaderProperty(FindProperty("_BacklightIntensity", props), "Intensity");
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private static void DrawLightDirectionOverride(MaterialEditor me, MaterialProperty[] props, Material mat)
        {
            foldLightDirOverride = EditorGUILayout.BeginFoldoutHeaderGroup(foldLightDirOverride, "Light Direction Override");
            if (foldLightDirOverride)
            {
                var use = FindProperty("_UseLightDirectionOverride", props);
                me.ShaderProperty(use, "Use Override");
                ToggleKeyword(mat, use.floatValue > 0.5f, "_USELIGHTDIRECTIONOVERRIDE_ON");
                if (use.floatValue > 0.5f)
                {
                    me.VectorProperty(FindProperty("_LightDirectionOverride", props), "Direction");
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private static void DrawVRCLightVolumes(MaterialEditor me, MaterialProperty[] props, Material mat)
        {
            foldVRCLightVolumes = EditorGUILayout.BeginFoldoutHeaderGroup(foldVRCLightVolumes, "VRC Light Volumes");
            if (foldVRCLightVolumes)
            {
                var use = FindProperty("_UseVRCLightVolumes", props);
                me.ShaderProperty(use, "Use VRC Light Volumes");
                ToggleKeyword(mat, use.floatValue > 0.5f, "_USEVRCLIGHT_VOLUMES_ON");
                if (use.floatValue > 0.5f)
                {
                    me.ShaderProperty(FindProperty("_VRCLightVolumeIntensity", props), "Intensity");
                    me.ColorProperty(FindProperty("_VRCLightVolumeTint", props), "Tint");
                    me.ShaderProperty(FindProperty("_VRCLightVolumeDistanceFactor", props), "Distance Factor");
                    me.ShaderProperty(FindProperty("_EnvRimBorder", props), "Rim Border");
                    me.ShaderProperty(FindProperty("_EnvRimBlur", props), "Rim Blur");
                    me.ShaderProperty(FindProperty("_UseVRCLVRimLight", props), "Use Rim Light");
                    me.ShaderProperty(FindProperty("_VRCLVRimLightIntensity", props), "Rim Intensity");
                    me.ColorProperty(FindProperty("_VRCLVRimLightColor", props), "Rim Color");
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private static void DrawFlipbook(MaterialEditor me, MaterialProperty[] props, Material mat)
        {
            foldFlipbook = EditorGUILayout.BeginFoldoutHeaderGroup(foldFlipbook, "Flipbook");
            if (foldFlipbook)
            {
                var use = FindProperty("_UseFlipbook", props);
                me.ShaderProperty(use, "Use Flipbook");
                ToggleKeyword(mat, use.floatValue > 0.5f, "_USEFLIPBOOK_ON");
                if (use.floatValue > 0.5f)
                {
                    me.TexturePropertySingleLine(new GUIContent("Texture"), FindProperty("_FlipbookTex", props));
                    me.ShaderProperty(FindProperty("_FlipbookDivisionsX", props), "Divisions X");
                    me.ShaderProperty(FindProperty("_FlipbookDivisionsY", props), "Divisions Y");
                    me.ShaderProperty(FindProperty("_FlipbookSpeed", props), "Speed");
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private static void DrawRendering(MaterialEditor me, MaterialProperty[] props, Material mat)
        {
            foldRendering = EditorGUILayout.BeginFoldoutHeaderGroup(foldRendering, "Rendering");
            if (foldRendering)
            {
                me.ShaderProperty(FindProperty("_Cull", props), "Cull Mode");
                me.ShaderProperty(FindProperty("_ZWrite", props), "ZWrite");
                me.ShaderProperty(FindProperty("_ZTest", props), "ZTest");
                me.ShaderProperty(FindProperty("_SrcBlend", props), "Src Blend");
                me.ShaderProperty(FindProperty("_DstBlend", props), "Dst Blend");

                bool alphaTest = mat.IsKeywordEnabled("_ALPHATEST_ON");
                bool newAlphaTest = EditorGUILayout.Toggle("Alpha Test", alphaTest);
                if (newAlphaTest != alphaTest)
                {
                    if (newAlphaTest) mat.EnableKeyword("_ALPHATEST_ON"); else mat.DisableKeyword("_ALPHATEST_ON");
                }
                me.ShaderProperty(FindProperty("_Cutoff", props), "Alpha Cutoff");
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private static void DrawStencil(MaterialEditor me, MaterialProperty[] props)
        {
            foldStencil = EditorGUILayout.BeginFoldoutHeaderGroup(foldStencil, "Stencil");
            if (foldStencil)
            {
                me.ShaderProperty(FindProperty("_StencilRef", props), "Ref");
                me.ShaderProperty(FindProperty("_StencilReadMask", props), "ReadMask");
                me.ShaderProperty(FindProperty("_StencilWriteMask", props), "WriteMask");
                me.ShaderProperty(FindProperty("_StencilComp", props), "Comp");
                me.ShaderProperty(FindProperty("_StencilPass", props), "Pass");
                me.ShaderProperty(FindProperty("_StencilFail", props), "Fail");
                me.ShaderProperty(FindProperty("_StencilZFail", props), "ZFail");
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}


