# lilToon PCSS Extension v1.6.0 パッケージング実装ログ

## 日時
2025-07-04

## 概要
- セマンティックバージョニング（MAJOR.MINOR.PATCH）に従い、v1.6.0へMINORバージョンアップ。
- セットアップセンター統合・バッチセットアップ・プリセット拡張・UI統一など新機能を追加。
- Unity公式推奨のzip形式で再パッケージング。

## 主な作業内容
- `package.json`のversionを1.6.0に更新、urlも修正
- `README.md`のバージョン表記・主な変更点を1.6.0に更新
- `CHANGELOG.md`に[1.6.0]セクションを追加
- `com.liltoon.pcss-extension`ディレクトリ全体を`com.liltoon.pcss-extension_v1.6.0.zip`としてパッケージ化
- _docs/に本実装ログを作成

## セマンティックバージョニング規則（Unity公式）
- MAJOR: 破壊的変更
- MINOR: 後方互換な新機能追加
- PATCH: バグ修正のみ
- 参考: [Unity公式ドキュメント](https://docs.unity3d.com/Manual/upm-semver.html)

## パッケージング手順
1. `com.liltoon.pcss-extension`ディレクトリの内容を確認
2. Powershellで以下コマンドを実行：
   ```powershell
   Compress-Archive -Path com.liltoon.pcss-extension -DestinationPath com.liltoon.pcss-extension_v1.6.0.zip -Force
   ```
3. 作成されたzipを配布用パッケージとして利用

## 備考
- Booth/手動導入時は`Packages`配下に展開
- VCC/UPM経由でも導入可能
- すべての新機能・修正はセットアップセンター（PCSSSetupCenterWindow）に集約

---

今後もセマンティックバージョニングを厳守し、運用性・UI統一を維持する方針。 