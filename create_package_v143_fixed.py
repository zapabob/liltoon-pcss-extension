#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
lilToon PCSS Extension v1.4.3 Package Creator (Fixed Dependencies)
依存関係問題を修正したv1.4.3パッケージの作成
"""

import os
import shutil
import zipfile
import json
import hashlib
from datetime import datetime

def create_package_v143_fixed():
    """v1.4.3パッケージ（依存関係修正版）を作成"""
    
    print("🚀 lilToon PCSS Extension v1.4.3 - Dependency Fix Package Creation")
    print("=" * 80)
    
    # ディレクトリ設定
    base_dir = os.path.dirname(os.path.abspath(__file__))
    source_dir = os.path.join(base_dir, "com.liltoon.pcss-extension-ultimate-1.4.3", "com.liltoon.pcss-extension-ultimate")
    output_dir = os.path.join(base_dir, "Release")
    package_name = "com.liltoon.pcss-extension-ultimate-1.4.3-fixed.zip"
    package_path = os.path.join(output_dir, package_name)
    
    # Releaseディレクトリの作成
    os.makedirs(output_dir, exist_ok=True)
    
    # 既存のパッケージを削除
    if os.path.exists(package_path):
        os.remove(package_path)
        print(f"🗑️ 既存パッケージを削除: {package_name}")
    
    # ソースディレクトリの確認
    if not os.path.exists(source_dir):
        print(f"❌ ソースディレクトリが見つかりません: {source_dir}")
        return False
    
    print(f"📁 ソースディレクトリ: {source_dir}")
    print(f"📦 出力パッケージ: {package_path}")
    
    # ZIPファイルの作成
    try:
        with zipfile.ZipFile(package_path, 'w', zipfile.ZIP_DEFLATED) as zipf:
            for root, dirs, files in os.walk(source_dir):
                for file in files:
                    file_path = os.path.join(root, file)
                    arcname = os.path.relpath(file_path, os.path.dirname(source_dir))
                    zipf.write(file_path, arcname)
                    print(f"  ✅ {arcname}")
        
        print(f"📦 パッケージ作成完了: {package_name}")
        
        # ファイルサイズとSHA256の計算
        file_size = os.path.getsize(package_path)
        sha256_hash = hashlib.sha256()
        with open(package_path, 'rb') as f:
            for chunk in iter(lambda: f.read(4096), b""):
                if isinstance(chunk, bytes):
                    sha256_hash.update(chunk)
        
        sha256 = sha256_hash.hexdigest().upper()
        
        print(f"📊 ファイルサイズ: {file_size:,} bytes ({file_size/1024:.1f}KB)")
        print(f"🔒 SHA256: {sha256}")
        
        # パッケージ情報を保存
        package_info = {
            "name": "com.liltoon.pcss-extension",
            "version": "1.4.3",
            "displayName": "lilToon PCSS Extension - Ultimate Commercial Edition",
            "description": "Ultimate Commercial PCSS solution with enhanced URP compatibility and fixed dependencies",
            "unity": "2022.3",
            "unityRelease": "22f1",
            "file_info": {
                "filename": package_name,
                "size_bytes": file_size,
                "size_kb": round(file_size/1024, 1),
                "sha256": sha256,
                "created": datetime.now().isoformat()
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
            "fixes": [
                "lilToon dependency moved to recommendedDependencies",
                "Enhanced VCC compatibility",
                "Improved dependency resolution",
                "Fixed installation issues"
            ]
        }
        
        info_file = os.path.join(output_dir, "package_info_v1.4.3_fixed.json")
        with open(info_file, 'w', encoding='utf-8') as f:
            json.dump(package_info, f, indent=2, ensure_ascii=False)
        
        print(f"📄 パッケージ情報保存: package_info_v1.4.3_fixed.json")
        
        # 互換性レポートの作成
        compatibility_report = {
            "version": "1.4.3-fixed",
            "compatibility_status": "Enhanced",
            "fixes_applied": [
                {
                    "issue": "lilToon dependency resolution failure",
                    "solution": "Moved to recommendedDependencies",
                    "impact": "Prevents VCC installation errors"
                },
                {
                    "issue": "VCC package not found error",
                    "solution": "Updated dependency management strategy",
                    "impact": "Improved installation success rate"
                }
            ],
            "dependencies": {
                "required": {
                    "com.vrchat.avatars": ">=3.7.0",
                    "com.vrchat.base": ">=3.7.0", 
                    "nadena.dev.modular-avatar": ">=1.10.0",
                    "com.unity.render-pipelines.universal": "12.1.12"
                },
                "recommended": {
                    "jp.lilxyzw.liltoon": ">=1.8.0"
                }
            },
            "installation_notes": [
                "lilToon will be suggested during installation but not required",
                "Users can install lilToon manually if needed",
                "All core functionality remains intact",
                "Enhanced VCC compatibility"
            ]
        }
        
        report_file = os.path.join(output_dir, "compatibility_report_v1.4.3_fixed.json")
        with open(report_file, 'w', encoding='utf-8') as f:
            json.dump(compatibility_report, f, indent=2, ensure_ascii=False)
        
        print(f"📋 互換性レポート保存: compatibility_report_v1.4.3_fixed.json")
        
        print("\n🎉 パッケージ作成完了!")
        print("=" * 80)
        print("📦 Package Summary:")
        print(f"   Name: {package_name}")
        print(f"   Size: {file_size/1024:.1f}KB")
        print(f"   SHA256: {sha256}")
        print(f"   Dependencies: Fixed (lilToon as recommended)")
        print("=" * 80)
        
        return True
        
    except Exception as e:
        print(f"❌ パッケージ作成エラー: {str(e)}")
        return False

if __name__ == "__main__":
    success = create_package_v143_fixed()
    if success:
        print("✅ 処理完了 - 依存関係を修正したv1.4.3パッケージが作成されました")
    else:
        print("❌ 処理失敗 - パッケージ作成に失敗しました") 