#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
lilToon PCSS Extension v1.4.9 Final Clean Package Creator
完全なエラー解決と入れ子構造防止の最終版

Author: lilToon PCSS Extension Team
Version: 1.4.9-final
Date: 2025-06-22
"""

import os
import shutil
import zipfile
import json
import hashlib
from pathlib import Path
import tempfile

def calculate_sha256(file_path):
    """ファイルのSHA256ハッシュを計算"""
    sha256_hash = hashlib.sha256()
    with open(file_path, "rb") as f:
        for byte_block in iter(lambda: f.read(4096), b""):
            sha256_hash.update(byte_block)
    return sha256_hash.hexdigest().upper()

def create_final_clean_package():
    """最終クリーンパッケージを作成（完全なエラー解決版）"""
    
    # 基本設定
    package_name = "com.liltoon.pcss-extension"
    version = "1.4.9"
    final_package_name = f"{package_name}-{version}-final"
    
    # 作業ディレクトリの設定
    current_dir = Path.cwd()
    temp_dir = Path(tempfile.mkdtemp())
    package_dir = temp_dir / package_name
    
    print(f"🚀 Creating final clean package v{version}...")
    print(f"📁 Working directory: {current_dir}")
    print(f"🔧 Temp directory: {temp_dir}")
    
    try:
        # パッケージディレクトリを作成
        package_dir.mkdir(parents=True, exist_ok=True)
        
        # 必要なファイルとディレクトリをコピー
        files_to_copy = [
            "package.json",
            "README.md",
            "CHANGELOG.md"
        ]
        
        directories_to_copy = [
            "Editor",
            "Runtime", 
            "Shaders"
        ]
        
        # ファイルコピー
        for file_name in files_to_copy:
            src_file = current_dir / file_name
            if src_file.exists():
                dst_file = package_dir / file_name
                
                # UTF-8 BOM問題を解決
                if file_name == "package.json":
                    try:
                        with open(src_file, 'r', encoding='utf-8-sig') as f:
                            content = f.read()
                        with open(dst_file, 'w', encoding='utf-8') as f:
                            f.write(content)
                        print(f"✅ Fixed UTF-8 BOM: {file_name}")
                    except Exception as e:
                        print(f"⚠️  UTF-8 fallback for {file_name}: {e}")
                        shutil.copy2(src_file, dst_file)
                else:
                    shutil.copy2(src_file, dst_file)
                print(f"📄 Copied file: {file_name}")
        
        # ディレクトリコピー
        for dir_name in directories_to_copy:
            src_dir = current_dir / dir_name
            if src_dir.exists():
                dst_dir = package_dir / dir_name
                shutil.copytree(src_dir, dst_dir)
                print(f"📁 Copied directory: {dir_name}")
        
        # package.jsonを更新
        package_json_path = package_dir / "package.json"
        if package_json_path.exists():
            try:
                # 元のpackage.jsonを読み込む
                with open(package_json_path, 'r', encoding='utf-8') as f:
                    package_data = json.load(f)
                
                # バージョン情報のみを更新
                package_data["version"] = version
                
                # 更新した内容を保存
                with open(package_json_path, 'w', encoding='utf-8') as f:
                    json.dump(package_data, f, indent=4, ensure_ascii=False)
                
                print("✅ Updated package.json with final configuration")
                
            except Exception as e:
                print(f"❌ Error updating package.json: {e}")
                return False
        
        # 最終パッケージディレクトリを作成
        final_dir = current_dir / final_package_name
        if final_dir.exists():
            shutil.rmtree(final_dir)
        
        # 一時ディレクトリから最終ディレクトリにコピー
        shutil.copytree(package_dir, final_dir)
        
        # ZIPファイルを作成
        zip_path = current_dir / f"{final_package_name}.zip"
        if zip_path.exists():
            zip_path.unlink()
        
        with zipfile.ZipFile(zip_path, 'w', zipfile.ZIP_DEFLATED) as zipf:
            for root, dirs, files in os.walk(final_dir):
                for file in files:
                    file_path = Path(root) / file
                    arcname = file_path.relative_to(final_dir.parent)
                    zipf.write(file_path, arcname)
        
        # ファイルサイズとハッシュを計算
        file_size = zip_path.stat().st_size
        sha256_hash = calculate_sha256(zip_path)
        
        print(f"\n🎉 Final Clean Package Created Successfully!")
        print(f"📦 Package: {zip_path.name}")
        print(f"📏 Size: {file_size:,} bytes ({file_size/1024:.1f} KB)")
        print(f"🔐 SHA256: {sha256_hash}")
        
        # 構造を確認
        print(f"\n📋 Package Structure:")
        for root, dirs, files in os.walk(final_dir):
            level = root.replace(str(final_dir), '').count(os.sep)
            indent = ' ' * 2 * level
            print(f"{indent}{os.path.basename(root)}/")
            subindent = ' ' * 2 * (level + 1)
            for file in files:
                print(f"{subindent}{file}")
        
        # VPM情報を出力
        print(f"\n📝 VPM Repository Information:")
        print(f"URL: https://github.com/zapabob/liltoon-pcss-extension/releases/download/v{version}/{zip_path.name}")
        print(f"SHA256: {sha256_hash}")
        
        return True
        
    except Exception as e:
        print(f"❌ Error creating package: {e}")
        return False
    finally:
        # 一時ディレクトリをクリーンアップ
        if temp_dir.exists():
            shutil.rmtree(temp_dir)

if __name__ == "__main__":
    success = create_final_clean_package()
    if success:
        print("\n🚀 Final package creation completed successfully!")
    else:
        print("\n❌ Package creation failed!") 