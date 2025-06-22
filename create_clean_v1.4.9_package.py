#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
lilToon PCSS Extension v1.4.9 Clean Release Package Creator
現行最新版のみを含むクリーンなリリースパッケージを作成します。
過去バージョンディレクトリは除外されます。
"""

import os
import sys
import json
import shutil
import zipfile
import hashlib
from datetime import datetime
from pathlib import Path

def calculate_sha256(file_path):
    """ファイルのSHA256ハッシュを計算"""
    sha256_hash = hashlib.sha256()
    with open(file_path, "rb") as f:
        for byte_block in iter(lambda: f.read(4096), b""):
            sha256_hash.update(byte_block)
    return sha256_hash.hexdigest().upper()

def should_exclude_path(path):
    """除外すべきパスかどうかを判定"""
    exclude_patterns = [
        # 過去バージョンディレクトリ
        "com.liltoon.pcss-extension-1.4.7",
        "com.liltoon.pcss-extension-1.4.8", 
        "com.liltoon.pcss-extension-ultimate",
        # 開発用ファイル
        ".git",
        ".github",
        ".specstory",
        "Release",
        "scripts",
        "docs",
        # 過去のZIPファイル
        "com.liltoon.pcss-extension-1.4.7.zip",
        "com.liltoon.pcss-extension-1.4.8.zip",
        # 開発用スクリプト
        "create_release_package.py",
        "create_release_package_v1.4.9.py",
        "remove_bom.py",
        # その他の開発ファイル
        ".cursorindexingignore",
        ".gitignore",
        "UNITY_INSTALLATION_GUIDE.md"
    ]
    
    path_str = str(path)
    for pattern in exclude_patterns:
        if pattern in path_str:
            return True
    return False

def copy_current_version_files(temp_dir):
    """現行バージョンのファイルのみをコピー"""
    package_dest = os.path.join(temp_dir, "com.liltoon.pcss-extension")
    os.makedirs(package_dest)
    
    # コピーするファイル/ディレクトリのリスト
    include_items = [
        "package.json",
        "README.md", 
        "CHANGELOG.md",
        "Editor",
        "Runtime",
        "Shaders"
    ]
    
    copied_count = 0
    
    for item in include_items:
        source_path = item
        if os.path.exists(source_path):
            dest_path = os.path.join(package_dest, item)
            
            if os.path.isfile(source_path):
                shutil.copy2(source_path, dest_path)
                print(f"✅ ファイルコピー: {item}")
                copied_count += 1
            elif os.path.isdir(source_path):
                shutil.copytree(source_path, dest_path)
                print(f"✅ ディレクトリコピー: {item}")
                copied_count += 1
        else:
            print(f"⚠️  見つかりません: {item}")
    
    print(f"📦 コピー完了: {copied_count} 項目")
    return package_dest

def create_clean_package():
    """v1.4.9のクリーンなリリースパッケージを作成"""
    
    print("🚀 lilToon PCSS Extension v1.4.9 Clean Release Package Creator")
    print("🧹 現行最新版のみを含むクリーンパッケージ作成")
    print("=" * 60)
    
    # 出力設定
    output_name = "com.liltoon.pcss-extension-1.4.9-clean"
    output_zip = f"{output_name}.zip"
    temp_dir = f"temp_{output_name}"
    
    try:
        # 一時ディレクトリ作成
        if os.path.exists(temp_dir):
            shutil.rmtree(temp_dir)
        os.makedirs(temp_dir)
        
        # 現行バージョンのファイルのみをコピー
        package_dest = copy_current_version_files(temp_dir)
        
        # package.jsonの内容を確認・最適化
        package_json_path = os.path.join(package_dest, "package.json")
        if os.path.exists(package_json_path):
            # BOM除去処理
            import codecs
            with open(package_json_path, "r+b") as fp:
                chunk = fp.read(4096)
                if chunk.startswith(codecs.BOM_UTF8):
                    print("🔧 package.json BOM除去中...")
                    bom_length = len(codecs.BOM_UTF8)
                    i = 0
                    chunk = chunk[bom_length:]
                    while chunk:
                        fp.seek(i)
                        fp.write(chunk)
                        i += len(chunk)
                        fp.seek(bom_length, os.SEEK_CUR)
                        chunk = fp.read(4096)
                    fp.seek(-bom_length, os.SEEK_CUR)
                    fp.truncate()
            
            with open(package_json_path, 'r', encoding='utf-8') as f:
                package_data = json.load(f)
            
            # v1.4.9の設定を確認・最適化
            package_data["version"] = "1.4.9"
            package_data["name"] = "com.liltoon.pcss-extension"
            package_data["displayName"] = "lilToon PCSS Extension"
            package_data["description"] = "Revolutionary Unity Menu implementation with complete shader error resolution. Features Unity Menu Avatar Selector, 4 preset systems, automatic avatar detection, one-click setup (90% time reduction), full Built-in RP & URP compatibility, Unity 2022.3 LTS support, and comprehensive VRChat optimization."
            
            # クリーンな依存関係設定
            package_data["dependencies"] = {
                "com.unity.render-pipelines.universal": "12.1.12"
            }
            
            package_data["vpmDependencies"] = {
                "com.vrchat.avatars": ">=3.7.0",
                "com.vrchat.base": ">=3.7.0"
            }
            
            package_data["recommendedDependencies"] = {
                "jp.lilxyzw.liltoon": ">=1.8.0",
                "nadena.dev.modular-avatar": ">=1.10.0"
            }
            
            with open(package_json_path, 'w', encoding='utf-8') as f:
                json.dump(package_data, f, indent=2, ensure_ascii=False)
            
            print("✅ package.json を最適化しました")
        
        # ZIPファイル作成
        print(f"📦 クリーンZIPファイル作成中: {output_zip}")
        
        with zipfile.ZipFile(output_zip, 'w', zipfile.ZIP_DEFLATED) as zipf:
            for root, dirs, files in os.walk(temp_dir):
                for file in files:
                    file_path = os.path.join(root, file)
                    arcname = os.path.relpath(file_path, temp_dir)
                    zipf.write(file_path, arcname)
        
        # SHA256ハッシュ計算
        sha256_hash = calculate_sha256(output_zip)
        
        # 結果表示
        file_size = os.path.getsize(output_zip)
        file_size_mb = file_size / (1024 * 1024)
        
        print("\n🎉 v1.4.9 クリーンリリースパッケージ作成完了！")
        print("=" * 60)
        print(f"📁 ファイル名: {output_zip}")
        print(f"📊 ファイルサイズ: {file_size_mb:.2f} MB ({file_size:,} bytes)")
        print(f"🔒 SHA256: {sha256_hash}")
        print(f"📅 作成日時: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
        
        # 従来版との比較
        old_zip = "com.liltoon.pcss-extension-1.4.9.zip"
        if os.path.exists(old_zip):
            old_size = os.path.getsize(old_zip)
            old_size_mb = old_size / (1024 * 1024)
            size_reduction = ((old_size - file_size) / old_size) * 100
            print(f"\n📉 サイズ最適化:")
            print(f"   従来版: {old_size_mb:.2f} MB")
            print(f"   クリーン版: {file_size_mb:.2f} MB")
            print(f"   削減率: {size_reduction:.1f}%")
        
        # VPMファイル更新用の情報を出力
        print("\n📝 VPMファイル更新情報:")
        print(f'  "zipSHA256": "{sha256_hash}",')
        print(f'  "url": "https://github.com/zapabob/liltoon-pcss-extension/releases/download/v1.4.9/{output_zip}"')
        
        # 一時ディレクトリ削除
        shutil.rmtree(temp_dir)
        
        return True, sha256_hash, output_zip
        
    except Exception as e:
        import traceback
        print(f"❌ エラーが発生しました: {str(e)}")
        print(f"📋 詳細エラー情報:")
        traceback.print_exc()
        if os.path.exists(temp_dir):
            shutil.rmtree(temp_dir)
        return False, None, None

def main():
    """メイン処理"""
    success, sha256_hash, filename = create_clean_package()
    
    if success:
        print("\n🚀 クリーンパッケージの特徴:")
        print("- 過去バージョンディレクトリ除外")
        print("- 開発用ファイル除外") 
        print("- 最適化されたpackage.json")
        print("- 最小限のファイルサイズ")
        print("- Unity Menu Avatar Selector")
        print("- Complete Shader Error Resolution")
        print("- Built-in RP & URP Full Compatibility")
        print("- Unity 2022.3 LTS Support")
        
        print("\n🎯 次のステップ:")
        print("1. GitHubでv1.4.9のリリースを更新")
        print(f"2. 作成した{filename}をリリースにアップロード")
        print("3. VPMファイルのSHA256ハッシュを更新")
        print("4. 古いZIPファイルと置き換え")
        
        sys.exit(0)
    else:
        print("\n❌ クリーンパッケージ作成に失敗しました")
        sys.exit(1)

if __name__ == "__main__":
    main() 