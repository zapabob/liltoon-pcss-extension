# lilToon PCSS Extension

---

## 概要
Unity/VRChat向け高品質影表現拡張「lilToon PCSS Extension」は、lilToonシェーダーにPCSS（ソフトシャドウ）や多彩な影制御機能を追加するパッケージです。ModularAvatar連携や自動マテリアルバックアップ、Booth/GitHub/VCC/VPM配布に対応し、アバター・ワールド制作のワークフローを強力にサポートします。

---

## パッケージ内容
- シェーダー拡張: `Shaders/lilToon_PCSS_Extension.shader` 他
- エディタ拡張: `Editor/AutomatedMaterialBackup.cs` `Editor/ModularAvatarLightToggleMenu.cs` など
- ランタイム: `Runtime/LilToonCompatibilityManager.cs` 他
- サンプル・実装ログ: `_docs/` ディレクトリ
- README, CHANGELOG, package.json

---

## インストール手順
### 1. VCC/VPM経由（推奨）
- VCC（VRChat Creator Companion）で「lilToon PCSS Extension」を検索・追加
- VPM（VRChat Package Manager）対応

### 2. Booth/GitHub配布
- BoothまたはGitHubからzipをダウンロードし、`com.liltoon.pcss-extension`フォルダごと`Assets`または`Packages`に配置
- Unity再起動で自動認識

### 3. .unitypackage形式
- Unityメニュー「Assets > Import Package > Custom Package」からインポート

---

## 要件
- Unity 2022.3 LTS 以降（推奨）
- lilToon v1.4.0 以降
- VRChat SDK3（アバター/ワールド用途）
- ModularAvatar（連携機能利用時）
- Windows11/RTX3080（推奨環境）

---

## 使い方
### 1. シェーダー適用
- マテリアルのShader欄で「lilToon PCSS Extension」を選択
- InspectorでPCSSや影制御パラメータを調整

### 2. マテリアル自動バックアップ/リストア
- マテリアル編集時に自動でGUIDベースのバックアップを作成
- パス変更やリネーム後も正確に復元可能
- 復元は「Tools > lilToon > Restore Material」から

### 3. ModularAvatar連携（ライトトグル自動化）
- アバターのルートを選択し「Tools > ModularAvatar > Add Light Toggle」実行
- LightToggleオブジェクトが自動生成され、全LightがObject Toggleに登録
- VRChat内でON/OFFトグル可能

---

## よくある質問（FAQ）
- Q: lilToon本体が必要ですか？
  - A: 必須です。最新版推奨。
- Q: 旧バージョンからのアップグレード方法は？
  - A: 既存パッケージを削除し最新版を導入してください。マテリアルは自動バックアップされます。
- Q: ModularAvatarが無いと使えませんか？
  - A: シェーダー機能のみなら不要。トグル自動化等はModularAvatar必須です。

---

## ライセンス
- 本パッケージはMITライセンスで配布されています。
- 詳細は`LICENSE.md`を参照してください。

---

## サポート・リンク
- [Booth配布ページ](https://booth.pm/ja/items/xxxxxx)
- [GitHubリポジトリ](https://github.com/xxxxxx/lilToon-PCSS-Extension)
- [ModularAvatar公式ドキュメント](https://modular-avatar.nadena.dev/docs/)
- [実装ログ・技術詳細（_docs/）](./_docs/)

---

## 更新履歴・実装ログ
- 詳細な更新履歴・実装経緯は`_docs/`ディレクトリおよび`CHANGELOG.md`を参照してください。

---

## 免責事項
- 本パッケージの利用によるいかなる損害も作者は責任を負いません。
- Unity/VRChat/lilToon/ModularAvatar等の仕様変更により動作が変わる場合があります。

---

## お問い合わせ
- Booth/GitHubのメッセージ、または`_docs/`内の連絡先をご利用ください。
