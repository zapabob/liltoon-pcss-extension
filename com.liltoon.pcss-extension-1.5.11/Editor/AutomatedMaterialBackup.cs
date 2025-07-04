using UnityEngine;
using UnityEditor;
using VRC.SDKBase.Editor.BuildPipeline; // VRChat SDKのAPIを利用するために必要
using VRC.SDK3.Avatars.Components; // 追加

// IVRCSDKBuildRequestedCallbackを実装することで、VRChatのビルドプロセスに介入できる
public class AutomatedMaterialBackup : IVRCSDKBuildRequestedCallback
{
    // このコールバックは、ビルドの種類（例: Avatar, World）に関わらず呼び出される
    public int callbackOrder => 0; // 複数のコールバックがある場合の実行順序。0は標準的な優先度

    // onBuildRequestedは、VRChat SDKのビルドボタンが押された時に呼び出される
    public bool OnBuildRequested(VRCSDKRequestedBuildType buildType)
    {
        // アバターのビルド時のみ処理を実行する
        if (buildType == VRCSDKRequestedBuildType.Avatar)
        {
            // シーン内の最初のアバターを取得
            var avatars = GameObject.FindObjectsOfType<VRCAvatarDescriptor>();
            if (avatars.Length > 0)
            {
                var activeAvatar = avatars[0].gameObject;
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
                    MaterialBackup.BackupMaterials(activeAvatar);
                    Debug.Log($"[AutomatedMaterialBackup] Succeeded to backup materials for {activeAvatar.name}.");
                }
            }
        }

        // trueを返すとビルドプロセスが続行され、falseを返すとビルドがキャンセルされる
        return true;
    }
}
