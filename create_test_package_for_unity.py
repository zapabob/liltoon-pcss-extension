#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
lilToon PCSS Extension v1.4.9 Unity Test Package Creator
Unityプロジェクトで直接テストできるクリーンなパッケージを作成
"""

import os
import sys
import shutil
import json
import zipfile
import hashlib
from pathlib import Path
import tempfile

def remove_bom(file_path):
    """UTF-8 BOMを削除"""
    try:
        with open(file_path, 'rb') as f:
            content = f.read()
        
        if content.startswith(b'\xef\xbb\xbf'):
            with open(file_path, 'wb') as f:
                f.write(content[3:])
            print(f"✅ Fixed UTF-8 BOM: {os.path.basename(file_path)}")
    except Exception as e:
        print(f"❌ BOM removal failed for {file_path}: {e}")

def should_exclude_file(file_path):
    """除外すべきファイルかどうかを判定"""
    file_name = file_path.name.lower()
    
    # 具体的なファイル名で除外（誤除外を防ぐ）
    excluded_files = [
        'boothpackageexporter.cs',
        'vccsetupwizard.cs'
    ]
    
    # 除外ファイルをチェック
    return file_name in excluded_files

def create_unity_test_package():
    """Unity直接テスト用パッケージを作成"""
    print("🚀 Creating Unity Test Package v1.4.9 (Clean Edition)...")
    
    current_dir = Path.cwd()
    print(f"📁 Working directory: {current_dir}")
    
    # 必要なファイル/フォルダのリスト
    required_items = [
        "package.json",
        "README.md", 
        "CHANGELOG.md",
        "Editor",
        "Runtime", 
        "Shaders"
    ]
    
    # 存在確認
    missing_items = []
    for item in required_items:
        if not Path(item).exists():
            missing_items.append(item)
    
    if missing_items:
        print(f"❌ Missing required items: {missing_items}")
        return False
    
    # テンポラリディレクトリで作業
    with tempfile.TemporaryDirectory() as temp_dir:
        print(f"🔧 Temp directory: {temp_dir}")
        
        # パッケージディレクトリ作成
        package_name = "com.liltoon.pcss-extension-1.4.9-unity-test-clean"
        package_dir = Path(temp_dir) / package_name
        package_dir.mkdir()
        
        # ファイルコピー
        files_to_copy = [
            "package.json",
            "README.md",
            "CHANGELOG.md"
        ]
        
        for file_name in files_to_copy:
            src = Path(file_name)
            dst = package_dir / file_name
            if src.exists():
                shutil.copy2(src, dst)
                remove_bom(dst)
                print(f"📄 Copied file: {file_name}")
        
        # ディレクトリコピー（除外ファイルをスキップ）
        dirs_to_copy = ["Editor", "Runtime", "Shaders"]
        excluded_files = []
        
        for dir_name in dirs_to_copy:
            src_dir = Path(dir_name)
            dst_dir = package_dir / dir_name
            if src_dir.exists():
                dst_dir.mkdir()
                
                # ファイルを個別にコピー（除外チェック付き）
                for src_file in src_dir.rglob('*'):
                    if src_file.is_file():
                        if should_exclude_file(src_file):
                            excluded_files.append(str(src_file))
                            print(f"⚠️ Excluded: {src_file.name}")
                            continue
                        
                        # 相対パスを維持してコピー
                        rel_path = src_file.relative_to(src_dir)
                        dst_file = dst_dir / rel_path
                        dst_file.parent.mkdir(parents=True, exist_ok=True)
                        shutil.copy2(src_file, dst_file)
                        
                        # .csファイルのBOM削除
                        if src_file.suffix == '.cs':
                            remove_bom(dst_file)
                
                print(f"📁 Copied directory: {dir_name} (with exclusions)")
        
        # 除外ファイル一覧表示
        if excluded_files:
            print(f"\n🚫 Excluded Files:")
            for excluded in excluded_files:
                print(f"   - {excluded}")
        
        # package.jsonを更新
        package_json_path = package_dir / "package.json"
        try:
            with open(package_json_path, 'r', encoding='utf-8') as f:
                package_data = json.load(f)
            
            # Unity Test用設定
            package_data.update({
                "name": "com.liltoon.pcss-extension",
                "displayName": "lilToon PCSS Extension - Unity Test Clean",
                "version": "1.4.9",
                "description": "Unity Test Clean Version - Core PCSS Features Only (No BOOTH/VCC dependencies)",
                "unity": "2022.3",
                "unityRelease": "22f1"
            })
            
            with open(package_json_path, 'w', encoding='utf-8') as f:
                json.dump(package_data, f, indent=2, ensure_ascii=False)
            
            print("✅ Updated package.json for Unity Test Clean")
            
        except Exception as e:
            print(f"❌ Failed to update package.json: {e}")
            return False
        
        # ZIPファイル作成
        zip_name = f"{package_name}.zip"
        zip_path = current_dir / zip_name
        
        with zipfile.ZipFile(zip_path, 'w', zipfile.ZIP_DEFLATED) as zipf:
            for file_path in package_dir.rglob('*'):
                if file_path.is_file():
                    arc_name = file_path.relative_to(package_dir.parent)
                    zipf.write(file_path, arc_name)
        
        # ファイルサイズとハッシュ計算
        file_size = zip_path.stat().st_size
        
        with open(zip_path, 'rb') as f:
            sha256_hash = hashlib.sha256(f.read()).hexdigest().upper()
        
        print(f"\n🎉 Unity Test Clean Package Created Successfully!")
        print(f"📦 Package: {zip_name}")
        print(f"📏 Size: {file_size:,} bytes ({file_size/1024:.1f} KB)")
        print(f"🔐 SHA256: {sha256_hash}")
        
        # パッケージ構造表示
        print(f"\n📋 Package Structure:")
        print(f"{package_name}/")
        for item in sorted(package_dir.rglob('*')):
            if item.is_file():
                rel_path = item.relative_to(package_dir)
                indent = "  " * (len(rel_path.parts) - 1)
                print(f"{indent}{rel_path.name}")
            elif item != package_dir:
                rel_path = item.relative_to(package_dir)
                indent = "  " * (len(rel_path.parts) - 1)
                print(f"{indent}{rel_path.name}/")
        
        print(f"\n📝 Unity インストール手順:")
        print(f"1. Unity Editorを閉じる")
        print(f"2. {zip_name} を展開")
        print(f"3. {package_name} フォルダを Packages/ にコピー")
        print(f"4. Unity Editorを開く")
        print(f"5. Package Managerで確認")
        print(f"6. 'Window/lilToon PCSS Extension/Avatar Selector' でテスト")
        
        print(f"\n✨ Features Included:")
        print(f"   - Core PCSS Shader Extensions")
        print(f"   - Avatar Selector Menu")
        print(f"   - VRChat Optimization Settings")
        print(f"   - Performance Optimizer")
        print(f"   - Light Volumes Integration")
        
        print(f"\n🚫 Features Excluded:")
        print(f"   - BOOTH Package Exporter")
        print(f"   - VCC Setup Wizard")
        print(f"   - Commercial Distribution Tools")
        
        print(f"\n🚀 Unity Test Clean Package creation completed successfully!")
        return True

if __name__ == "__main__":
    try:
        success = create_unity_test_package()
        if success:
            print("\n✅ All operations completed successfully!")
        else:
            print("\n❌ Some operations failed!")
            sys.exit(1)
    except Exception as e:
        print(f"\n💥 Unexpected error: {e}")
        sys.exit(1) 