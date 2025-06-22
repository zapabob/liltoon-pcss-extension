#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
lilToon PCSS Extension v1.4.7 Release Package Creator
リポジトリ整理完了版のリリースパッケージを作成します。
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
    """v1.4.7のリリースパッケージを作成"""
    
    print("🚀 lilToon PCSS Extension v1.4.7 Release Package Creator")
    print("=" * 60)
    
    # パッケージディレクトリ
    source_dir = "com.liltoon.pcss-extension-ultimate"
    if not os.path.exists(source_dir):
        print(f"❌ エラー: {source_dir} が見つかりません")
        return False
    
    # 出力設定
    output_name = "com.liltoon.pcss-extension-1.4.7"
    output_zip = f"{output_name}.zip"
    temp_dir = f"temp_{output_name}"
    
    try:
        # 一時ディレクトリ作成
        if os.path.exists(temp_dir):
            shutil.rmtree(temp_dir)
        os.makedirs(temp_dir)
        
        # パッケージディレクトリをコピー
        package_dest = os.path.join(temp_dir, "com.liltoon.pcss-extension")
        shutil.copytree(source_dir, package_dest)
        
        print(f"✅ パッケージファイルをコピー: {source_dir} → {package_dest}")
        
        # package.jsonの名称を確認・修正
        package_json_path = os.path.join(package_dest, "package.json")
        if os.path.exists(package_json_path):
            with open(package_json_path, 'r', encoding='utf-8') as f:
                package_data = json.load(f)
            
            # 名称統一の確認
            if package_data.get("name") != "com.liltoon.pcss-extension":
                package_data["name"] = "com.liltoon.pcss-extension"
                package_data["displayName"] = "lilToon PCSS Extension"
                
                with open(package_json_path, 'w', encoding='utf-8') as f:
                    json.dump(package_data, f, indent=2, ensure_ascii=False)
                
                print("✅ package.json の名称を統一しました")
        
        # ZIPファイル作成
        print(f"📦 ZIPファイル作成中: {output_zip}")
        
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
        
        print("\n🎉 リリースパッケージ作成完了！")
        print("=" * 60)
        print(f"📁 ファイル名: {output_zip}")
        print(f"📊 ファイルサイズ: {file_size_mb:.2f} MB ({file_size:,} bytes)")
        print(f"🔒 SHA256: {sha256_hash}")
        print(f"📅 作成日時: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
        
        # VPMファイル更新用の情報を出力
        print("\n📝 VPMファイル更新情報:")
        print(f'  "zipSHA256": "{sha256_hash}",')
        print(f'  "url": "https://github.com/zapabob/liltoon-pcss-extension/releases/download/v1.4.7/{output_zip}"')
        
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
        print("1. GitHubでv1.4.7のリリースを作成")
        print("2. 作成したZIPファイルをリリースにアップロード")
        print("3. VPMファイルのSHA256ハッシュを更新")
        print("4. GitHub Pagesを更新")
        sys.exit(0)
    else:
        print("\n❌ パッケージ作成に失敗しました")
        sys.exit(1)

if __name__ == "__main__":
    main() 