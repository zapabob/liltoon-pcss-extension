#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
lilToon PCSS Extension v1.4.3 URP Compatibility Enhanced Edition パッケージング
URP 12.1.12 互換性強化版
"""

import os
import zipfile
import hashlib
import json
from datetime import datetime
import shutil

def calculate_sha256(file_path):
    """ファイルのSHA256ハッシュを計算"""
    sha256_hash = hashlib.sha256()
    with open(file_path, 'rb') as f:
        for byte_block in iter(lambda: f.read(4096), b''):
            sha256_hash.update(byte_block)
    return sha256_hash.hexdigest().upper()

def create_compatibility_report():
    """互換性レポートを作成"""
    report = {
        "version": "1.4.3",
        "release_date": "2025-06-22",
        "urp_version": "12.1.12",
        "unity_compatibility": {
            "minimum": "2022.3.22f1",
            "recommended": "2022.3.22f1",
            "tested": ["2022.3.22f1", "2023.2.20f1", "2023.3.0f1"]
        },
        "features": {
            "ai_optimization": True,
            "ray_tracing": True,
            "enterprise_analytics": True,
            "commercial_licensing": True,
            "urp_compatibility": "Enhanced",
            "quest_optimization": True,
            "pc_optimization": True
        },
        "compatibility_improvements": [
            "URP dependency reduced to 12.1.12 for broader compatibility",
            "Enhanced Unity 2022.3 LTS support",
            "Improved VCC package manager integration",
            "Reduced dependency conflicts",
            "Better cross-version stability"
        ]
    }
    
    with open('compatibility_report_v1.4.3.json', 'w', encoding='utf-8') as f:
        json.dump(report, f, indent=2, ensure_ascii=False)
    
    return report

def main():
    print('🚀 lilToon PCSS Extension v1.4.3 URP Compatibility Enhanced Edition パッケージング開始')
    print('📦 URP 12.1.12 互換性強化版の作成')
    
    # パッケージ情報
    version = '1.4.3'
    package_name = f'com.liltoon.pcss-extension-ultimate-{version}.zip'
    source_dir = 'com.liltoon.pcss-extension-ultimate'
    
    if not os.path.exists(source_dir):
        print(f'❌ エラー: {source_dir} ディレクトリが見つかりません')
        return False
    
    # 互換性レポート作成
    print('📊 互換性レポートを作成中...')
    compatibility_report = create_compatibility_report()
    
    # パッケージ作成
    print('📦 パッケージファイルを作成中...')
    file_count = 0
    
    with zipfile.ZipFile(package_name, 'w', zipfile.ZIP_DEFLATED, compresslevel=9) as zipf:
        for root, dirs, files in os.walk(source_dir):
            for file in files:
                file_path = os.path.join(root, file)
                arcname = os.path.relpath(file_path, '.')
                zipf.write(file_path, arcname)
                file_count += 1
                if file_count % 10 == 0:
                    print(f'  📁 処理済み: {file_count} ファイル')
        
        # 互換性レポートをパッケージに追加
        zipf.write('compatibility_report_v1.4.3.json', 'compatibility_report_v1.4.3.json')
    
    # ファイル情報計算
    file_size = os.path.getsize(package_name)
    file_size_mb = file_size / (1024 * 1024)
    sha256_hash = calculate_sha256(package_name)
    
    # パッケージ情報作成
    package_info = {
        "name": "com.liltoon.pcss-extension-ultimate",
        "version": version,
        "displayName": f"lilToon PCSS Extension - Ultimate Commercial Edition v{version}",
        "description": "URP 12.1.12 Compatibility Enhanced Edition - Enterprise-grade PCSS solution with enhanced Unity compatibility",
        "filename": package_name,
        "size": file_size,
        "size_mb": round(file_size_mb, 2),
        "sha256": sha256_hash,
        "file_count": file_count,
        "created": datetime.now().isoformat(),
        "unity_compatibility": compatibility_report["unity_compatibility"],
        "urp_version": "12.1.12",
        "features": compatibility_report["features"],
        "improvements": compatibility_report["compatibility_improvements"],
        "commercial": {
            "license": "Ultimate Commercial",
            "usage": "Unlimited",
            "redistribution": "Allowed",
            "support": "Priority Enterprise"
        }
    }
    
    # パッケージ情報をJSONファイルに保存
    info_filename = f'package_info_v{version}.json'
    with open(info_filename, 'w', encoding='utf-8') as f:
        json.dump(package_info, f, indent=2, ensure_ascii=False)
    
    # 結果表示
    print('\n' + '='*80)
    print('🎉 lilToon PCSS Extension v1.4.3 URP Compatibility Enhanced Edition パッケージング完了!')
    print('='*80)
    print(f'📦 パッケージ名: {package_name}')
    print(f'📊 ファイルサイズ: {file_size_mb:.2f} MB ({file_size:,} bytes)')
    print(f'📁 ファイル数: {file_count}')
    print(f'🔐 SHA256: {sha256_hash}')
    print(f'⚡ URP バージョン: 12.1.12 (互換性強化)')
    print(f'🎯 Unity 互換性: 2022.3 LTS+')
    print(f'📄 パッケージ情報: {info_filename}')
    print(f'📊 互換性レポート: compatibility_report_v1.4.3.json')
    
    print('\n🌟 主な改善点:')
    for improvement in compatibility_report["compatibility_improvements"]:
        print(f'  ✅ {improvement}')
    
    print('\n💼 商用ライセンス:')
    print('  🏢 Ultimate Commercial Edition')
    print('  ♾️  無制限商用利用')
    print('  🔄 再配布権限あり')
    print('  🛠️ 完全修正権限')
    print('  📞 優先エンタープライズサポート')
    
    print('\n🚀 次のステップ:')
    print('  1. GitHubリポジトリにv1.4.3タグを作成')
    print('  2. リリースページにパッケージをアップロード')
    print('  3. VPMリポジトリのvpm.jsonを更新')
    print('  4. BOOTHページの商品説明を更新')
    print('  5. ドキュメントの互換性情報を更新')
    
    return True

if __name__ == "__main__":
    try:
        success = main()
        if success:
            print('\n✅ パッケージング処理が正常に完了しました')
        else:
            print('\n❌ パッケージング処理中にエラーが発生しました')
    except Exception as e:
        print(f'\n💥 予期しないエラーが発生しました: {str(e)}')
        import traceback
        traceback.print_exc() 