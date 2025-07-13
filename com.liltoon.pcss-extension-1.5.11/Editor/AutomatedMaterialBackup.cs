using UnityEngine;
using UnityEditor;
using VRC.SDKBase.Editor.BuildPipeline; // VRChat SDK縺ｮAPI繧貞茜逕ｨ縺吶ｋ縺溘ａ縺ｫ蠢・ｦ・
using VRC.SDK3.Avatars.Components; // 霑ｽ蜉

// IVRCSDKBuildRequestedCallback繧貞ｮ溯｣・☆繧九％縺ｨ縺ｧ縲〃RChat縺ｮ繝薙Ν繝峨・繝ｭ繧ｻ繧ｹ縺ｫ莉句・縺ｧ縺阪ｋ
public class AutomatedMaterialBackup : IVRCSDKBuildRequestedCallback
{
    // 縺薙・繧ｳ繝ｼ繝ｫ繝舌ャ繧ｯ縺ｯ縲√ン繝ｫ繝峨・遞ｮ鬘橸ｼ井ｾ・ Avatar, World・峨↓髢｢繧上ｉ縺壼他縺ｳ蜃ｺ縺輔ｌ繧・
    public int callbackOrder => 0; // 隍・焚縺ｮ繧ｳ繝ｼ繝ｫ繝舌ャ繧ｯ縺後≠繧句ｴ蜷医・螳溯｡碁・ｺ上・縺ｯ讓呎ｺ也噪縺ｪ蜆ｪ蜈亥ｺｦ

    // onBuildRequested縺ｯ縲〃RChat SDK縺ｮ繝薙Ν繝峨・繧ｿ繝ｳ縺梧款縺輔ｌ縺滓凾縺ｫ蜻ｼ縺ｳ蜃ｺ縺輔ｌ繧・
    public bool OnBuildRequested(VRCSDKRequestedBuildType buildType)
    {
        // 繧｢繝舌ち繝ｼ縺ｮ繝薙Ν繝画凾縺ｮ縺ｿ蜃ｦ逅・ｒ螳溯｡後☆繧・
        if (buildType == VRCSDKRequestedBuildType.Avatar)
        {
            // 繧ｷ繝ｼ繝ｳ蜀・・譛蛻昴・繧｢繝舌ち繝ｼ繧貞叙蠕・
            var avatars = GameObject.FindObjectsOfType<VRCAvatarDescriptor>();
            if (avatars.Length > 0)
            {
                var activeAvatar = avatars[0].gameObject;
                bool doBackup = EditorUtility.DisplayDialog(
                    "Material Backup",
                    $"'{activeAvatar.name}'縺ｮ繝槭ユ繝ｪ繧｢繝ｫ繧偵ヰ繝・け繧｢繝・・縺励∪縺吶°・歃n\n" +
                    "繧｢繝・・繝ｭ繝ｼ繝牙ｾ後↓繝槭ユ繝ｪ繧｢繝ｫ縺檎ｴ謳阪＠縺溷ｴ蜷医↓蠕ｩ蜈・〒縺阪∪縺吶・,
                    "縺ｯ縺・√ヰ繝・け繧｢繝・・縺吶ｋ",
                    "縺・＞縺・
                );

                if (doBackup)
                {
                    // 莉･蜑堺ｽ懈・縺励◆MaterialBackup繧ｯ繝ｩ繧ｹ縺ｮ繝舌ャ繧ｯ繧｢繝・・讖溯・繧貞他縺ｳ蜃ｺ縺・
                    MaterialBackup.BackupMaterials(activeAvatar);
                    Debug.Log($"[AutomatedMaterialBackup] Succeeded to backup materials for {activeAvatar.name}.");
                }
            }
        }

        // true繧定ｿ斐☆縺ｨ繝薙Ν繝峨・繝ｭ繧ｻ繧ｹ縺檎ｶ夊｡後＆繧後’alse繧定ｿ斐☆縺ｨ繝薙Ν繝峨′繧ｭ繝｣繝ｳ繧ｻ繝ｫ縺輔ｌ繧・
        return true;
    }
}
