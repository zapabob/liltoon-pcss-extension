#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
lilToon PCSS Extension v1.5.0 Release Package Creator
汎用的なリリースパッケージを作成します。
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

def create_release_package():
    """v1.5.0のリリースパッケージを作成"""
    
    # package.jsonからバージョン情報を取得
    try:
        with open('package.json', 'r', encoding='utf-8-sig') as f:
            package_data = json.load(f)
        version = package_data.get("version", "1.5.0")
        package_name = package_data.get("name", "com.liltoon.pcss-extension")
        print(f"📦 Version {version} のパッケージを作成します。")
    except FileNotFoundError:
        print("❌ エラー: package.json が見つかりません。")
        return False
        
    print("=" * 60)
    
    # パッケージに含めるディレクトリとファイル
    source_dirs = ["Editor", "Runtime", "Shaders"]
    source_files = ["package.json", "CHANGELOG.md", "README.md", "LICENSE"]

    # 出力設定
    output_name = f"{package_name}-{version}"
    output_zip = f"{output_name}.zip"
    temp_dir = f"temp_{output_name}"
    
    try:
        # 一時ディレクトリ作成
        if os.path.exists(temp_dir):
            shutil.rmtree(temp_dir)
        
        package_dest_root = os.path.join(temp_dir, package_name)
        os.makedirs(package_dest_root)

        # ディレクトリをコピー
        for dir_name in source_dirs:
            if os.path.exists(dir_name):
                shutil.copytree(dir_name, os.path.join(package_dest_root, dir_name))
                print(f"✅ ディレクトリをコピー: {dir_name}")

        # ファイルをコピー
        for file_name in source_files:
             if os.path.exists(file_name):
                shutil.copy(file_name, package_dest_root)
                print(f"✅ ファイルをコピー: {file_name}")

        print(f"📦 ZIPファイル作成中: {output_zip}")
        
        # ZIPファイル作成 (ルートにディレクトリが含まれるように)
        shutil.make_archive(output_name, 'zip', temp_dir)

        # SHA256ハッシュ計算
        sha256_hash = calculate_sha256(output_zip)
        
        # 結果表示
        file_size = os.path.getsize(output_zip)
        file_size_mb = file_size / (1024 * 1024)
        
        print("\n🎉 リリースパッケージ作成完了！")
        print("=" * 60)
        print(f"📁 ファイル名: {output_zip}")
        print(f"📊 ファイルサイズ: {file_size_mb:.2f} MB ({file_size:,} bytes)")
        print(f"🔒 SHA256: {sha256_hash}")
        print(f"📅 作成日時: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
        
        # VPMファイル更新用の情報を出力
        print("\n📝 VPMファイル更新情報:")
        print(f'  "zipSHA256": "{sha256_hash}",')
        print(f'  "url": "https://github.com/zapabob/liltoon-pcss-extension/releases/download/v{version}/{output_zip}"')
        
        # 一時ディレクトリ削除
        shutil.rmtree(temp_dir)
        
        return True
        
    except Exception as e:
        print(f"❌ エラーが発生しました: {str(e)}")
        if os.path.exists(temp_dir):
            shutil.rmtree(temp_dir)
        return False

def main():
    """メイン処理"""
    success = create_release_package()
    
    if success:
        print("\n🚀 次のステップ:")
        print("1. GitHubで新しいリリースを作成")
        print("2. 作成したZIPファイルをリリースにアップロード")
        print("3. VPMリポジトリのjsonファイルを更新")
        sys.exit(0)
    else:
        print("\n❌ パッケージ作成に失敗しました")
        sys.exit(1)

if __name__ == "__main__":
    main() 