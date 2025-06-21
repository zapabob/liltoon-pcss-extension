#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
lilToon PCSS Extension v1.4.2 Ultimate Commercial Edition パッケージング
"""

import os
import zipfile
import hashlib
import json
from datetime import datetime

def calculate_sha256(file_path):
    """ファイルのSHA256ハッシュを計算"""
    sha256_hash = hashlib.sha256()
    with open(file_path, 'rb') as f:
        for byte_block in iter(lambda: f.read(4096), b''):
            sha256_hash.update(byte_block)
    return sha256_hash.hexdigest().upper()

def main():
    print('🚀 lilToon PCSS Extension v1.4.2 Ultimate Commercial Edition パッケージング開始')
    
    # パッケージ情報
    version = '1.4.2'
    package_name = f'com.liltoon.pcss-extension-ultimate-{version}.zip'
    source_dir = 'com.liltoon.pcss-extension-ultimate'
    
    if not os.path.exists(source_dir):
        print(f'❌ エラー: {source_dir} ディレクトリが見つかりません')
        return
    
    # パッケージ作成
    print(f'📦 パッケージ作成中: {package_name}')
    
    with zipfile.ZipFile(package_name, 'w', zipfile.ZIP_DEFLATED) as zipf:
        file_count = 0
        for root, dirs, files in os.walk(source_dir):
            for file in files:
                file_path = os.path.join(root, file)
                arcname = os.path.relpath(file_path, '.')
                zipf.write(file_path, arcname)
                file_count += 1
                if file_count % 10 == 0:
                    print(f'  📁 {file_count}ファイル処理済み...')
    
    # ファイル情報取得
    file_size = os.path.getsize(package_name)
    sha256 = calculate_sha256(package_name)
    
    print(f'')
    print(f'✅ パッケージング完了！')
    print(f'📦 ファイル名: {package_name}')
    print(f'📏 ファイルサイズ: {file_size:,} bytes ({file_size/1024/1024:.2f} MB)')
    print(f'🔐 SHA256: {sha256}')
    print(f'📄 ファイル数: {file_count}')
    
    # package.json更新
    package_json_path = os.path.join(source_dir, 'package.json')
    if os.path.exists(package_json_path):
        try:
            with open(package_json_path, 'r', encoding='utf-8') as f:
                package_data = json.load(f)
            
            package_data['version'] = version
            
            with open(package_json_path, 'w', encoding='utf-8') as f:
                json.dump(package_data, f, indent=2, ensure_ascii=False)
            
            print(f'📝 package.json更新完了: v{version}')
        except Exception as e:
            print(f'⚠️ package.json更新エラー: {e}')
    
    # VPMリポジトリ情報生成
    vpm_info = {
        "name": "com.liltoon.pcss-extension",
        "version": version,
        "url": f"https://zapabob.github.io/liltoon-pcss-extension/Release/{package_name}",
        "zipSHA256": sha256,
        "size": file_size,
        "releaseDate": datetime.now().isoformat()
    }
    
    with open(f'vpm_info_{version}.json', 'w', encoding='utf-8') as f:
        json.dump(vpm_info, f, indent=2, ensure_ascii=False)
    
    print(f'📊 VPMリポジトリ情報生成: vpm_info_{version}.json')
    
    print(f'')
    print(f'🎉 lilToon PCSS Extension v{version} Ultimate Commercial Edition')
    print(f'   パッケージング完了！本格リリース準備完了！')
    print(f'')
    print(f'🚀 次のステップ:')
    print(f'   1. Release/{package_name} をGitHub Pagesにアップロード')
    print(f'   2. docs/index.json のSHA256ハッシュを更新')
    print(f'   3. VCCでテストインストール実行')
    print(f'   4. BOOTH商品ページ更新')

if __name__ == '__main__':
    main() 