#if UNITY_EDITOR
namespace lilToon.PCSS.Editor
{
    internal static class PCSSConstants
    {
        public const string MenuRoot = "Tools/lilToon-PCSS-Extension/";
        public const string MenuBase = MenuRoot + "Legacy/";

        public const string PresetRoot = "Assets/PCSS/Presets";
        public const string EmissionMaterialPath = PresetRoot + "/PCSS_Emission.mat";
        public const string EmissionPrefabPath = PresetRoot + "/PCSS_Emission.prefab";
        public const string MeshRendererPresetPath = PresetRoot + "/PCSS_MeshRenderer.preset";

        public const string EmissionObjectName = "PCSS_Emission";

        public const string ParamLightOn = "PCSS_Light_On";
        public const string ParamLightIntensity = "PCSS_Light_Intensity";
        public const string ParamLightKeyIntensity = "PCSS_Key_Intensity";
        public const string ParamLightFillIntensity = "PCSS_Fill_Intensity";
        public const string ParamLightRimIntensity = "PCSS_Rim_Intensity";
        public const string ParamLightColor = "PCSS_Light_Color";
        public const string ParamLightSway = "PCSS_Light_Sway";
        public const string ParamShadowOn = "PCSS_Shadow_On";
        public const string ParamShadowStrength = "PCSS_Shadow_Strength";
    }
}
#endif


