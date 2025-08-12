using System.Linq;
using UnityEditor;
using UnityEngine;

public static class VRChatLilToonApplier
{
    // PCSS優先の候補。存在しないものはスキップ
    private static readonly string[] ShaderCandidates = new[]
    {
        // PCSS Extension 系（仮称含む）
        "lilToon/lilToonPCSS",
        "lilToonPCSS/lilToon",
        "lilToon/lilToon (PCSS)",
        "lilToon/PCSS Extension",

        // 通常のlilToon系
        "lilToon/lilToon",
        "lilToon/lilToon (Outline)",
        "lilToon/lilToon (Cutout)",
        "lilToon/lilToon (Transparent)",
    };

    public static bool TryApplyLilToon(Material material, bool force, out string appliedShaderName)
    {
        appliedShaderName = null;
        if (material == null) return false;
        if (!force && material.shader != null && material.shader.name != null && material.shader.name.StartsWith("lilToon"))
        {
            return false;
        }

        var shader = FindAvailableShader(out appliedShaderName);
        if (shader == null) return false;

        material.shader = shader;
        AutoTuneRenderQueue(material);
        return true;
    }

    public static Shader FindAvailableShader(out string nameUsed)
    {
        foreach (var n in ShaderCandidates)
        {
            var s = Shader.Find(n);
            if (s != null)
            {
                nameUsed = n;
                return s;
            }
        }
        nameUsed = null;
        return null;
    }

    public static void AutoTuneRenderQueue(Material mat)
    {
        var shaderName = mat.shader != null ? mat.shader.name : string.Empty;
        if (shaderName.Contains("Transparent") || mat.IsKeywordEnabled("_ALPHABLEND_ON"))
        {
            mat.renderQueue = 3000; // Transparent
        }
        else if (shaderName.Contains("Cutout") || mat.IsKeywordEnabled("_ALPHATEST_ON"))
        {
            mat.renderQueue = 2450; // AlphaTest
        }
        else
        {
            mat.renderQueue = 2000; // Opaque
        }
    }
}


