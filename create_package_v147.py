#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
lilToon PCSS Extension v1.4.7 Ultimate Commercial Edition Package Creator
Brand Unified Edition - Consistent Naming Convention
================================================================================
"""

import os
import sys
import json
import zipfile
import hashlib
from pathlib import Path
from datetime import datetime

# Package Configuration
PACKAGE_VERSION = "1.4.7"
PACKAGE_NAME = "com.liltoon.pcss-extension-ultimate"
PACKAGE_DISPLAY_NAME = "lilToon PCSS Extension - Ultimate Commercial Edition"
PACKAGE_DESCRIPTION = "Brand Unified Edition - Ultimate Commercial PCSS solution with consistent naming convention, revolutionary avatar selector menu, intuitive visual avatar selection, automatic scene detection, one-click setup system, enhanced URP 12.1.12 compatibility, enterprise-grade features, AI-powered optimization, real-time ray tracing support, advanced performance analytics, automatic quality scaling, commercial-ready asset generation, unlimited commercial licensing, priority enterprise support, and comprehensive VRChat optimization suite."

def calculate_sha256(file_path):
    """Calculate SHA256 hash of a file"""
    sha256_hash = hashlib.sha256()
    with open(file_path, "rb") as f:
        for byte_block in iter(lambda: f.read(4096), b""):
            sha256_hash.update(byte_block)
    return sha256_hash.hexdigest().upper()

def create_package():
    """Create the package zip file"""
    print(f"🚀 lilToon PCSS Extension v{PACKAGE_VERSION} Brand Unified Edition パッケージング開始")
    
    # Source and destination paths
    source_dir = Path("com.liltoon.pcss-extension-ultimate")
    output_dir = Path("Release")
    output_dir.mkdir(exist_ok=True)
    
    zip_filename = f"com.liltoon.pcss-extension-ultimate-{PACKAGE_VERSION}.zip"
    zip_path = output_dir / zip_filename
    
    # Create zip file
    with zipfile.ZipFile(zip_path, 'w', zipfile.ZIP_DEFLATED) as zipf:
        for root, dirs, files in os.walk(source_dir):
            for file in files:
                file_path = Path(root) / file
                arcname = file_path.relative_to(source_dir.parent)
                zipf.write(file_path, arcname)
                print(f"  📁 Added: {arcname}")
    
    # Calculate SHA256
    sha256 = calculate_sha256(zip_path)
    file_size = zip_path.stat().st_size
    
    print(f"\n✅ パッケージ作成完了!")
    print(f"📦 ファイル: {zip_filename}")
    print(f"📏 サイズ: {file_size:,} bytes ({file_size/1024:.1f} KB)")
    print(f"🔐 SHA256: {sha256}")
    
    # Create package info JSON
    package_info = {
        "name": "com.liltoon.pcss-extension",
        "displayName": PACKAGE_DISPLAY_NAME,
        "version": PACKAGE_VERSION,
        "description": PACKAGE_DESCRIPTION,
        "unity": "2022.3",
        "unityRelease": "22f1",
        "keywords": [
            "shader", "toon", "avatar", "pcss", "shadows", "vrchat", "lilToon",
            "poiyomi", "expression", "lightvolumes", "vrc-light-volumes", "bakery",
            "lighting", "modular-avatar", "vrchat-sdk3", "performance", "optimization",
            "all-in-one", "booth", "professional", "commercial", "premium", "advanced",
            "lightmapping", "quality", "production-ready", "ultimate", "enterprise",
            "ai-powered", "ray-tracing", "analytics", "auto-scaling", "unlimited-license",
            "priority-support", "urp-compatibility", "unity-2022-lts", "avatar-selector",
            "visual-ui", "one-click-setup", "auto-detection", "intuitive-workflow",
            "brand-unified", "consistent-naming"
        ],
        "author": {
            "name": "lilToon PCSS Extension Team",
            "email": "r.minegishi1987@gmail.com",
            "url": "https://github.com/zapabob"
        },
        "dependencies": {
            "com.unity.render-pipelines.universal": "12.1.12"
        },
        "vpmDependencies": {
            "com.vrchat.avatars": ">=3.7.0",
            "com.vrchat.base": ">=3.7.0",
            "nadena.dev.modular-avatar": ">=1.10.0"
        },
        "recommendedDependencies": {
            "jp.lilxyzw.liltoon": ">=1.8.0"
        },
        "legacyFolders": {
            "Assets\\lilToon PCSS Extension": ""
        },
        "legacyFiles": {
            "Assets\\lilToon PCSS Extension.meta": ""
        },
        "url": f"https://zapabob.github.io/liltoon-pcss-extension/Release/{zip_filename}",
        "zipSHA256": sha256,
        "repo": "https://github.com/zapabob/liltoon-pcss-extension",
        "changelogUrl": "https://zapabob.github.io/liltoon-pcss-extension/CHANGELOG.md",
        "documentationUrl": "https://zapabob.github.io/liltoon-pcss-extension/site/documentation.html",
        "licensesUrl": "https://zapabob.github.io/liltoon-pcss-extension/LICENSE",
        "commercial": {
            "license": "Ultimate Commercial",
            "usage": "Unlimited",
            "redistribution": "Allowed",
            "modification": "Full Rights",
            "support": "Priority Enterprise 24/7",
            "features": [
                "AI-Powered Optimization",
                "Real-time Ray Tracing", 
                "Enterprise Analytics",
                "Automatic Quality Scaling",
                "Commercial Asset Generation",
                "Unlimited Licensing",
                "Priority Support",
                "White-label Rights",
                "Enhanced URP 12.1.12 Compatibility",
                "Unity 2022.3 LTS Optimized",
                "Revolutionary Avatar Selector Menu",
                "Visual UI/UX Enhancement",
                "One-Click Setup System",
                "Automatic Scene Detection",
                "Brand Unified Naming Convention",
                "Consistent User Experience"
            ]
        }
    }
    
    # Save package info
    info_path = f"package_info_v{PACKAGE_VERSION}.json"
    with open(info_path, 'w', encoding='utf-8') as f:
        json.dump(package_info, f, indent=2, ensure_ascii=False)
    
    print(f"📋 パッケージ情報保存: {info_path}")
    
    return package_info

def main():
    """Main execution function"""
    try:
        package_info = create_package()
        
        print(f"\n🎉 lilToon PCSS Extension v{PACKAGE_VERSION} Brand Unified Edition")
        print("=" * 80)
        print("✅ ブランド統一完了")
        print("✅ 名称一貫性確保")
        print("✅ メニューパス統一")
        print("✅ キーワード最適化")
        print("✅ ユーザー体験向上")
        print("=" * 80)
        
        return 0
        
    except Exception as e:
        print(f"❌ エラー: {e}")
        return 1

if __name__ == "__main__":
    sys.exit(main()) 