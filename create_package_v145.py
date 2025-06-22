#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
lilToon PCSS Extension v1.4.5 Ultimate Commercial Edition Package Creator
Unity 2022.3 LTS URP Compatibility Fix
"""

import os
import zipfile
import hashlib
import json
import shutil
from pathlib import Path

def calculate_sha256(file_path):
    """ファイルのSHA256ハッシュを計算"""
    sha256_hash = hashlib.sha256()
    with open(file_path, "rb") as f:
        for byte_block in iter(lambda: f.read(4096), b""):
            sha256_hash.update(byte_block)
    return sha256_hash.hexdigest().upper()

def create_package():
    """v1.4.5パッケージを作成"""
    print("🚀 lilToon PCSS Extension v1.4.5 Ultimate Commercial Edition Package Creator")
    print("🔧 Unity 2022.3 LTS URP Compatibility Fix")
    print("=" * 80)
    
    # パッケージディレクトリ
    package_dir = "com.liltoon.pcss-extension-ultimate"
    package_name = "com.liltoon.pcss-extension-ultimate-1.4.5.zip"
    release_dir = "Release"
    
    # Releaseディレクトリ作成
    os.makedirs(release_dir, exist_ok=True)
    
    # パッケージ作成
    print(f"📦 パッケージ作成中: {package_name}")
    
    with zipfile.ZipFile(os.path.join(release_dir, package_name), 'w', zipfile.ZIP_DEFLATED) as zipf:
        # パッケージディレクトリ内のすべてのファイルを追加
        for root, dirs, files in os.walk(package_dir):
            for file in files:
                file_path = os.path.join(root, file)
                arcname = os.path.relpath(file_path, ".")
                zipf.write(file_path, arcname)
                print(f"  ✅ {arcname}")
    
    # パッケージサイズ取得
    package_path = os.path.join(release_dir, package_name)
    package_size = os.path.getsize(package_path)
    
    # SHA256ハッシュ計算
    print(f"🔐 SHA256ハッシュ計算中...")
    sha256_hash = calculate_sha256(package_path)
    
    # パッケージ情報作成
    package_info = {
        "name": "com.liltoon.pcss-extension",
        "version": "1.4.5",
        "displayName": "lilToon PCSS Extension - Ultimate Commercial Edition",
        "description": "Unity 2022.3 LTS URP Compatibility Fix - Ultimate Commercial PCSS solution",
        "unity": "2022.3",
        "unityRelease": "22f1",
        "dependencies": {
            "com.unity.render-pipelines.universal": "12.1.12"
        },
        "package": {
            "filename": package_name,
            "size": package_size,
            "sha256": sha256_hash,
            "url": f"https://zapabob.github.io/liltoon-pcss-extension/Release/{package_name}"
        },
        "compatibility": {
            "unity_version": "2022.3.22f1+",
            "urp_version": "12.1.12",
            "platform": ["PC", "Quest"],
            "vrchat_sdk": "3.7.0+"
        },
        "creation_date": "2025-06-22",
        "creation_purpose": "Unity 2022.3 LTS URP Compatibility Fix"
    }
    
    # パッケージ情報をJSONファイルに保存
    info_filename = f"package_info_v1.4.5.json"
    with open(info_filename, 'w', encoding='utf-8') as f:
        json.dump(package_info, f, ensure_ascii=False, indent=2)
    
    # Releaseディレクトリにもコピー
    shutil.copy2(info_filename, os.path.join(release_dir, info_filename))
    
    print("=" * 80)
    print("✅ パッケージ作成完了!")
    print(f"📦 パッケージ名: {package_name}")
    print(f"📏 パッケージサイズ: {package_size:,} bytes ({package_size/1024:.1f} KB)")
    print(f"🔐 SHA256: {sha256_hash}")
    print(f"📍 保存場所: {os.path.join(release_dir, package_name)}")
    print(f"📄 情報ファイル: {info_filename}")
    print("=" * 80)
    
    return sha256_hash, package_size

if __name__ == "__main__":
    create_package() 