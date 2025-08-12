#if UNITY_EDITOR
namespace lilToon.PCSS.Editor
{
    internal static class PCSSConstants
    {
        public const string MenuBase = "Tools/lilToon-PCSS-Extension/";

        public const string PresetRoot = "Assets/PCSS/Presets";
        public const string EmissionMaterialPath = PresetRoot + "/PCSS_Emission.mat";
        public const string EmissionPrefabPath = PresetRoot + "/PCSS_Emission.prefab";
        public const string MeshRendererPresetPath = PresetRoot + "/PCSS_MeshRenderer.preset";

        public const string EmissionObjectName = "PCSS_Emission";

        public const string ParamLightOn = "PCSS_Light_On";
        public const string ParamLightIntensity = "PCSS_Light_Intensity";
        public const string ParamLightColor = "PCSS_Light_Color";
        public const string ParamShadowOn = "PCSS_Shadow_On";
        public const string ParamShadowStrength = "PCSS_Shadow_Strength";
    }
}
#endif


