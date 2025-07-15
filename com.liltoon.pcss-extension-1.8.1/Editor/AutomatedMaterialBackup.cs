using UnityEngine;
using UnityEditor;
using VRC.SDKBase.Editor.BuildPipeline; // VRChat SDKのAPIを利用するために必要
#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.Components;
using System.Linq;
#endif

// IVRCSDKBuildRequestedCallbackを実装することで、VRChatのビルドプロセスに介入できる
public class AutomatedMaterialBackup : IVRCSDKBuildRequestedCallback
{
    private const string MenuRoot = "Tools/lilToon PCSS Extension/";
    private const string UtilitiesMenu = MenuRoot + "Utilities/";
    // このコールバックは、ビルドの種類（例: Avatar, World）に関わらず呼び出されます
    public int callbackOrder => 0; // 複数のコールバックがある場合の実行順。0は標準的な優先度

    // onBuildRequestedは、VRChat SDKのビルドボタンが押された時に呼び出されます
    public bool OnBuildRequested(VRCSDKRequestedBuildType buildType)
    {
        // アバターのビルド時のみ処理を実行する
        if (buildType == VRCSDKRequestedBuildType.Avatar)
        {
            // シーンにあるアクティブなアバターを取得する
#if VRC_SDK_VRCSDK3
            var activeAvatar = GameObject.FindObjectsOfType<VRCAvatarDescriptor>().FirstOrDefault();
#else
            var activeAvatar = null;
#endif
            if (activeAvatar != null)
            {
                bool doBackup = EditorUtility.DisplayDialog(
                    "Material Backup",
                    $"'{activeAvatar.name}'のマテリアルをバックアップしますか？\n\n" +
                    "アップロード後にマテリアルが破損した場合に復元できます。",
                    "はい、バックアップする",
                    "いいえ"
                );

                if (doBackup)
                {
                    // 以前作成したMaterialBackupクラスのバックアップ機能を呼び出す
                    MaterialBackup.BackupMaterials(activeAvatar.gameObject);
                    Debug.Log($"[AutomatedMaterialBackup] Succeeded to backup materials for {activeAvatar.name}.");
                }
            }
        }

        // trueを返すとビルドプロセスが続行され、falseを返すとビルドがキャンセルされます
        return true;
    }
}
