import os
import shutil
import sys
import re

def cleanup_old_package_files():
    """
    UnityプロジェクトのAssetsフォルダから、正規表現に一致する古いlilToon PCSS Extensionの
    ディレクトリと関連ファイルをすべて削除します。
    """
    print("🧹 古いパッケージファイルのクリーンアップを開始します...")
    
    project_root = os.getcwd()
    assets_path = os.path.join(project_root, "Assets")
    
    if not os.path.isdir(assets_path):
        print(f"❌ エラー: Assetsディレクトリが見つかりません。Unityプロジェクトのルートで実行してください。")
        print(f"   現在のディレクトリ: {project_root}")
        return False
        
    # 削除対象を特定するための正規表現パターン
    pattern = re.compile(r"^com\.liltoon\.pcss-extension.*")
    
    found_files = False
    
    # Assetsフォルダ内のすべてのアイテムをチェック
    for filename in os.listdir(assets_path):
        if pattern.match(filename):
            path_to_delete = os.path.join(assets_path, filename)
            try:
                if os.path.isdir(path_to_delete):
                    shutil.rmtree(path_to_delete)
                    print(f"🗑️  ディレクトリを削除しました: {path_to_delete}")
                else:
                    os.remove(path_to_delete)
                    print(f"🗑️  ファイルを削除しました: {path_to_delete}")
                found_files = True
                
                # 対応する.metaファイルも削除
                meta_file = f"{path_to_delete}.meta"
                if os.path.exists(meta_file):
                    os.remove(meta_file)
                    print(f"🗑️  メタファイルを削除しました: {meta_file}")

            except OSError as e:
                print(f"❌ エラー: 削除に失敗しました: {path_to_delete}")
                print(f"   詳細: {e}")
                # 1つのファイルの削除に失敗しても処理を続ける
                continue

    if not found_files:
        print("✅ クリーンアップ対象の古いファイルは見つかりませんでした。")
    else:
        print("\n🎉 クリーンアップが完了しました！")

    print("\n🔔 Unity Editorに戻り、アセットの再インポートが完了するのをお待ちください。")
    print("   コンパイルエラーがすべて解消されているはずです。")
    print("   もしエラーが解決しない場合は、VRChat Creator Companion (VCC)で")
    print("   [Manage Project] > [...] > [Reinstall All Packages] をお試しください。")
    
    return True

if __name__ == "__main__":
    if cleanup_old_package_files():
        sys.exit(0)
    else:
        sys.exit(1) 