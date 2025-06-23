import os
import shutil
import sys

def cleanup_old_package_files():
    """
    UnityプロジェクトのAssetsフォルダから、古いlilToon PCSS Extensionの
    ディレクトリと関連ファイルを削除します。
    """
    print("🧹 古いパッケージファイルのクリーンアップを開始します...")
    
    # このスクリプトはプロジェクトルートで実行されることを想定
    project_root = os.getcwd()
    assets_path = os.path.join(project_root, "Assets")
    
    if not os.path.isdir(assets_path):
        print(f"❌ エラー: Assetsディレクトリが見つかりません。Unityプロジェクトのルートで実行してください。")
        print(f"   現在のディレクトリ: {project_root}")
        return False
        
    # 削除対象のディレクトリとメタファイル
    old_package_dir = os.path.join(assets_path, "com.liltoon.pcss-extension")
    old_package_meta = f"{old_package_dir}.meta"
    
    found_files = False
    
    # 古いディレクトリの削除
    if os.path.isdir(old_package_dir):
        try:
            shutil.rmtree(old_package_dir)
            print(f"🗑️  ディレクトリを削除しました: {old_package_dir}")
            found_files = True
        except OSError as e:
            print(f"❌ エラー: ディレクトリの削除に失敗しました: {old_package_dir}")
            print(f"   詳細: {e}")
            return False
    
    # 古いメタファイルの削除
    if os.path.exists(old_package_meta):
        try:
            os.remove(old_package_meta)
            print(f"🗑️  メタファイルを削除しました: {old_package_meta}")
            found_files = True
        except OSError as e:
            print(f"❌ エラー: メタファイルの削除に失敗しました: {old_package_meta}")
            print(f"   詳細: {e}")
            return False

    if not found_files:
        print("✅ クリーンアップ対象の古いファイルは見つかりませんでした。")
    else:
        print("\n🎉 クリーンアップが完了しました！")

    print("\n🔔 Unity Editorに戻り、アセットの再インポートが完了するのをお待ちください。")
    print("   その後、VRChat Creator Companion (VCC) を開き、")
    print("   'lilToon PCSS Extension'がリストにあることを確認し、最新バージョンに更新してください。")
    
    return True

if __name__ == "__main__":
    if cleanup_old_package_files():
        sys.exit(0)
    else:
        sys.exit(1) 