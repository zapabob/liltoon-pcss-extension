#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
lilToon PCSS Extension v1.4.9 Clean Package Creator - Fixed Version
入れ子構造を完全に防ぐ改良版

Author: lilToon PCSS Extension Team
Version: 1.4.9
Date: 2025-06-22
"""

import os
import shutil
import zipfile
import json
import hashlib
from pathlib import Path

def calculate_sha256(file_path):
    """ファイルのSHA256ハッシュを計算"""
    sha256_hash = hashlib.sha256()
    with open(file_path, "rb") as f:
        for byte_block in iter(lambda: f.read(4096), b""):
            sha256_hash.update(byte_block)
    return sha256_hash.hexdigest().upper()

def create_clean_package():
    """クリーンなパッケージを作成（入れ子構造を完全に防ぐ）"""
    
    # 基本設定
    package_name = "com.liltoon.pcss-extension"
    version = "1.4.9"
    clean_package_name = f"{package_name}-{version}-clean"
    
    # 作業ディレクトリの設定
    current_dir = Path.cwd()
    temp_dir = current_dir / "temp_package"
    package_dir = temp_dir / package_name
    
    # 一時ディレクトリをクリーンアップ
    if temp_dir.exists():
        shutil.rmtree(temp_dir)
    
    # 新しい一時ディレクトリを作成
    temp_dir.mkdir(exist_ok=True)
    package_dir.mkdir(exist_ok=True)
    
    print(f"🚀 Creating clean package: {clean_package_name}")
    print(f"📁 Working directory: {current_dir}")
    print(f"📦 Package directory: {package_dir}")
    
    # 必要なファイルとディレクトリのリスト
    required_items = [
        "package.json",
        "README.md",
        "CHANGELOG.md",
        "Editor/",
        "Runtime/",
        "Shaders/"
    ]
    
    # ファイルをコピー（入れ子構造を避ける）
    copied_files = []
    for item in required_items:
        source_path = current_dir / item
        dest_path = package_dir / item
        
        if source_path.exists():
            if source_path.is_file():
                # ファイルをコピー
                dest_path.parent.mkdir(parents=True, exist_ok=True)
                shutil.copy2(source_path, dest_path)
                copied_files.append(item)
                print(f"✅ Copied file: {item}")
            elif source_path.is_dir():
                # ディレクトリをコピー（不要なファイルを除外）
                shutil.copytree(source_path, dest_path, 
                              ignore=shutil.ignore_patterns(
                                  '*.meta', '*.tmp', '*.bak', '*.old',
                                  '__pycache__', '.DS_Store', 'Thumbs.db'
                              ))
                copied_files.append(item)
                print(f"✅ Copied directory: {item}")
        else:
            print(f"⚠️  Not found: {item}")
    
    # package.jsonを更新
    package_json_path = package_dir / "package.json"
    if package_json_path.exists():
        # UTF-8 BOM問題を解決するためutf-8-sigを使用
        try:
            with open(package_json_path, 'r', encoding='utf-8-sig') as f:
                package_data = json.load(f)
        except UnicodeDecodeError:
            # フォールバック: utf-8で再試行
            with open(package_json_path, 'r', encoding='utf-8') as f:
                package_data = json.load(f)
        
        # バージョン情報を更新
        package_data["version"] = version
        package_data["displayName"] = "lilToon PCSS Extension"
        package_data["description"] = "Revolutionary Unity Menu implementation with complete shader error resolution for VRChat avatars. Features 90% setup time reduction and professional-grade PCSS shadows."
        
        # 依存関係を更新
        package_data["dependencies"] = {
            "com.vrchat.avatars": "3.5.0",
            "com.vrchat.base": "3.5.0"
        }
        
        # キーワードを更新
        package_data["keywords"] = [
            "vrchat", "avatar", "shader", "pcss", "shadow", "liltoon", 
            "poiyomi", "unity", "menu", "optimization", "performance"
        ]
        
        # 作者情報を更新
        package_data["author"] = {
            "name": "lilToon PCSS Extension Team",
            "email": "support@liltoon-pcss.com",
            "url": "https://github.com/zapabob/liltoon-pcss-extension"
        }
        
        # BOMなしのUTF-8で保存
        with open(package_json_path, 'w', encoding='utf-8') as f:
            json.dump(package_data, f, indent=2, ensure_ascii=False)
        
        print("✅ Updated package.json")
    
    # ZIPファイルを作成（入れ子構造を完全に防ぐ）
    zip_path = current_dir / f"{clean_package_name}.zip"
    if zip_path.exists():
        zip_path.unlink()
    
    with zipfile.ZipFile(zip_path, 'w', zipfile.ZIP_DEFLATED) as zipf:
        for root, dirs, files in os.walk(package_dir):
            for file in files:
                file_path = Path(root) / file
                # パッケージディレクトリからの相対パスを計算
                relative_path = file_path.relative_to(temp_dir)
                zipf.write(file_path, relative_path)
                print(f"📦 Added to ZIP: {relative_path}")
    
    # 一時ディレクトリをクリーンアップ
    shutil.rmtree(temp_dir)
    
    # ファイルサイズとハッシュを計算
    file_size = zip_path.stat().st_size
    sha256_hash = calculate_sha256(zip_path)
    
    print(f"\n🎉 Package created successfully!")
    print(f"📦 Package: {zip_path.name}")
    print(f"📏 Size: {file_size:,} bytes ({file_size/1024:.1f} KB)")
    print(f"🔐 SHA256: {sha256_hash}")
    
    # 検証用の情報を出力
    print(f"\n📋 Verification Info:")
    print(f"Package Name: {package_name}")
    print(f"Version: {version}")
    print(f"File Count: {len(copied_files)}")
    print(f"Copied Items: {', '.join(copied_files)}")
    
    return {
        "package_path": str(zip_path),
        "package_name": package_name,
        "version": version,
        "file_size": file_size,
        "sha256": sha256_hash,
        "copied_files": copied_files
    }

def verify_package_structure(zip_path):
    """パッケージ構造を検証"""
    print(f"\n🔍 Verifying package structure: {zip_path}")
    
    with zipfile.ZipFile(zip_path, 'r') as zipf:
        file_list = zipf.namelist()
        
        # 入れ子構造をチェック
        nested_paths = [f for f in file_list if f.count('/') > 3]
        if nested_paths:
            print("⚠️  Warning: Deep nested structure detected:")
            for path in nested_paths[:5]:  # 最初の5個だけ表示
                print(f"    {path}")
        else:
            print("✅ No deep nested structure detected")
        
        # 必要なファイルの存在確認
        required_files = [
            "com.liltoon.pcss-extension/package.json",
            "com.liltoon.pcss-extension/README.md",
            "com.liltoon.pcss-extension/Editor/",
            "com.liltoon.pcss-extension/Runtime/",
            "com.liltoon.pcss-extension/Shaders/"
        ]
        
        for required in required_files:
            found = any(f.startswith(required) for f in file_list)
            status = "✅" if found else "❌"
            print(f"{status} {required}")
        
        print(f"\n📊 Total files in package: {len(file_list)}")
        
        # ファイル一覧を表示（最初の20個）
        print(f"\n📄 Package contents (first 20 files):")
        for i, file_path in enumerate(sorted(file_list)[:20]):
            print(f"  {i+1:2d}. {file_path}")
        
        if len(file_list) > 20:
            print(f"  ... and {len(file_list) - 20} more files")

if __name__ == "__main__":
    try:
        result = create_clean_package()
        verify_package_structure(result["package_path"])
        
        print(f"\n🎯 Next Steps:")
        print(f"1. Test the package in Unity")
        print(f"2. Update VPM repository with new SHA256: {result['sha256']}")
        print(f"3. Create GitHub release with the clean package")
        print(f"4. Update documentation")
        
    except Exception as e:
        print(f"❌ Error creating package: {e}")
        import traceback
        traceback.print_exc() 