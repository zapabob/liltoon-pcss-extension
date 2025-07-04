# v1.5.11 実装ログ（2025-07-03）

## 概要
- lilToon PCSS Extension v1.5.11へアップグレード
- GUIDベースのマテリアルバックアップ・リストア機能を強化
- パス変更・リネーム後も正確に復元可能
- バージョンアップ・安定性向上・ドキュメント整理
- パッケージ化・配布用zip作成

## 主要変更点
- MaterialBackup.csをGUIDベース管理に全面改修
- バックアップはJSON形式で保存、復元時はGUID一致でマテリアルを特定
- テクスチャもGUIDで管理し、参照切れを大幅に防止
- README.md/CHANGELOG.md/package.jsonを最新版に更新
- v1.5.11としてパッケージzip（com.liltoon.pcss-extension-1.5.11.zip）を作成

## パッケージ化手順
1. すべての変更をgit commit
2. git archiveでzip化
3. Unityプロジェクトにインポートして動作確認

## 今後の展望
- バックアップ世代管理・差分比較UIの追加検討
- 他パッケージとの互換性強化

--- 