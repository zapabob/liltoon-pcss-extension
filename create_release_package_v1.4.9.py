#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
lilToon PCSS Extension v1.4.9 Release Package Creator
Unity Menu Avatar Selector & Complete Shader Fix版のリリースパッケージを作成します。
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
    """v1.4.9のリリースパッケージを作成"""
    
    print("🚀 lilToon PCSS Extension v1.4.9 Release Package Creator")
    print("🎯 Unity Menu Avatar Selector & Complete Shader Fix Edition")
    print("=" * 60)
    
    # パッケージディレクトリ
    source_dir = "com.liltoon.pcss-extension-1.4.9"
    if not os.path.exists(source_dir):
        print(f"❌ エラー: {source_dir} が見つかりません")
        return False, None
    
    # 出力設定
    output_name = "com.liltoon.pcss-extension-1.4.9"
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
        
        # package.jsonの内容を確認・修正
        package_json_path = os.path.join(package_dest, "package.json")
        if os.path.exists(package_json_path):
            with open(package_json_path, 'r', encoding='utf-8') as f:
                package_data = json.load(f)
            
            # v1.4.9の設定を確認
            if package_data.get("version") != "1.4.9":
                package_data["version"] = "1.4.9"
                package_data["name"] = "com.liltoon.pcss-extension"
                package_data["displayName"] = "lilToon PCSS Extension"
                package_data["description"] = "Revolutionary Unity Menu implementation with complete shader error resolution. Features Unity Menu Avatar Selector, 4 preset systems, automatic avatar detection, one-click setup (90% time reduction), full Built-in RP & URP compatibility, Unity 2022.3 LTS support, and comprehensive VRChat optimization."
                
                with open(package_json_path, 'w', encoding='utf-8') as f:
                    json.dump(package_data, f, indent=2, ensure_ascii=False)
                
                print("✅ package.json をv1.4.9用に更新しました")
        
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
        
        print("\n🎉 v1.4.9 リリースパッケージ作成完了！")
        print("=" * 60)
        print(f"📁 ファイル名: {output_zip}")
        print(f"📊 ファイルサイズ: {file_size_mb:.2f} MB ({file_size:,} bytes)")
        print(f"🔒 SHA256: {sha256_hash}")
        print(f"📅 作成日時: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
        
        # VPMファイル更新用の情報を出力
        print("\n📝 VPMファイル更新情報:")
        print(f'  "zipSHA256": "{sha256_hash}",')
        print(f'  "url": "https://github.com/zapabob/liltoon-pcss-extension/releases/download/v1.4.9/{output_zip}"')
        
        # 一時ディレクトリ削除
        shutil.rmtree(temp_dir)
        
        return True, sha256_hash
        
    except Exception as e:
        print(f"❌ エラーが発生しました: {str(e)}")
        if os.path.exists(temp_dir):
            shutil.rmtree(temp_dir)
        return False, None

def main():
    """メイン処理"""
    success, sha256_hash = create_release_package()
    
    if success:
        print("\n🚀 次のステップ:")
        print("1. GitHubでv1.4.9のリリースを作成")
        print("2. 作成したZIPファイルをリリースにアップロード")
        print("3. VPMファイルのSHA256ハッシュを更新")
        print("4. GitHub Pagesを更新")
        
        print("\n🎯 v1.4.9の主要機能:")
        print("- Unity Menu Avatar Selector")
        print("- Complete Shader Error Resolution")
        print("- 4 Preset Systems")
        print("- Automatic Avatar Detection")
        print("- One-Click Setup (90% Time Reduction)")
        print("- Built-in RP & URP Full Compatibility")
        print("- Unity 2022.3 LTS Support")
        
        sys.exit(0)
    else:
        print("\n❌ v1.4.9パッケージ作成に失敗しました")
        sys.exit(1)

if __name__ == "__main__":
    main() 