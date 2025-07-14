using UnityEngine;
using UnityEditor;
using VRC.SDKBase.Editor.BuildPipeline; // VRChat SDKのAPIを利用するために忁E��E

// IVRCSDKBuildRequestedCallbackを実裁E��ることで、VRChatのビルド�Eロセスに介�Eできる
public class AutomatedMaterialBackup : IVRCSDKBuildRequestedCallback
{
    private const string MenuRoot = "Tools/lilToon PCSS Extension/";
    private const string UtilitiesMenu = MenuRoot + "Utilities/";
    // こEコールバックは、ビルド�E種類（侁E Avatar, World�E�に関わらず呼び出されめE
    public int callbackOrder => 0; // 褁E��のコールバックがある場合�E実行頁E��、Eは標準的な優先度

    // onBuildRequestedは、VRChat SDKのビルド�Eタンが押された時に呼び出されめE
    public bool OnBuildRequested(VRCSDKRequestedBuildType buildType)
    {
        // アバターのビルド時のみ処琁E��実行すめE
        if (buildType == VRCSDKRequestedBuildType.Avatar)
        {
            // シーンにぁE��アクチE��ブなアバターを取得すめE
            // (より堁E��にするなら、ビルド対象のアバターを正確に特定する方法を検討すめE
            var activeAvatar = VRC.SDKBase.Editor.VRC_SdkBuilder.GetMainAvatar();

            if (activeAvatar != null)
            {
                bool doBackup = EditorUtility.DisplayDialog(
                    "Material Backup",
                    $"'{activeAvatar.name}'のマテリアルをバチE��アチE�Eしますか�E�\n\n" +
                    "アチE�Eロード後にマテリアルが破損した場合に復允E��きます、E,
                    "はぁE��バチE��アチE�Eする",
                    "ぁE��ぁE
                );

                if (doBackup)
                {
                    // 以前作�EしたMaterialBackupクラスのバックアチE�E機�Eを呼び出ぁE
                    MaterialBackup.BackupMaterials(activeAvatar);
                    Debug.Log($"[AutomatedMaterialBackup] Succeeded to backup materials for {activeAvatar.name}.");
                }
            }
        }

        // trueを返すとビルド�Eロセスが続行され、falseを返すとビルドがキャンセルされめE
        return true;
    }
}
