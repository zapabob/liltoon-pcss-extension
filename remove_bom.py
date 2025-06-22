#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
UTF-8 BOM除去ユーティリティ
"""

import os
import sys
import codecs

def remove_bom_inplace(path):
    """BOMマークを除去してファイルをin-placeで書き換え"""
    buffer_size = 4096
    bom_length = len(codecs.BOM_UTF8)
    
    with open(path, "r+b") as fp:
        chunk = fp.read(buffer_size)
        if chunk.startswith(codecs.BOM_UTF8):
            print(f"🔧 BOM除去中: {path}")
            i = 0
            chunk = chunk[bom_length:]
            while chunk:
                fp.seek(i)
                fp.write(chunk)
                i += len(chunk)
                fp.seek(bom_length, os.SEEK_CUR)
                chunk = fp.read(buffer_size)
            fp.seek(-bom_length, os.SEEK_CUR)
            fp.truncate()
            return True
        else:
            print(f"✅ BOM不要: {path}")
            return False

def remove_bom_from_directory(directory):
    """ディレクトリ内のすべてのJSONファイルからBOMを除去"""
    json_files = []
    
    for root, dirs, files in os.walk(directory):
        for file in files:
            if file.endswith('.json'):
                json_files.append(os.path.join(root, file))
    
    print(f"📁 対象ディレクトリ: {directory}")
    print(f"📄 JSONファイル数: {len(json_files)}")
    print("=" * 50)
    
    removed_count = 0
    for json_file in json_files:
        if remove_bom_inplace(json_file):
            removed_count += 1
    
    print("=" * 50)
    print(f"🎉 BOM除去完了: {removed_count}/{len(json_files)} ファイル")
    return removed_count

def main():
    """メイン処理"""
    if len(sys.argv) > 1:
        target = sys.argv[1]
        if os.path.isfile(target):
            remove_bom_inplace(target)
        elif os.path.isdir(target):
            remove_bom_from_directory(target)
        else:
            print(f"❌ エラー: {target} が見つかりません")
    else:
        # デフォルトでv1.4.9ディレクトリを処理
        target_dir = "com.liltoon.pcss-extension-1.4.9"
        if os.path.exists(target_dir):
            remove_bom_from_directory(target_dir)
        else:
            print(f"❌ エラー: {target_dir} が見つかりません")

if __name__ == "__main__":
    main() 