# SOP: BOOTH配布パッケージ作成手順

## 目的

lilToon PCSS Studio for VRChat をBOOTHで販売・配布するときに、購入者が迷わず導入できるフォルダー構成、説明文、規約、更新履歴、FAQをそろえる。

## 標準フォルダー構成

```text
lilToon PCSS Studio for VRChat/
├─ 00_README_最初にお読みください.md / .txt / .html
├─ 01_INSTALL_導入手順.md / .txt / .html
├─ 02_BOOTH商品ページテンプレート.md / .txt / .html
├─ 03_TERMS_利用規約.md / .txt / .html
├─ 04_CHANGELOG_更新履歴.md / .txt / .html
├─ 05_FAQ_トラブルシュート.md / .txt / .html
├─ 06_SUPPORT_問い合わせテンプレート.md / .txt / .html
├─ Package/
│  └─ lilToon PCSS Studio for VRChat/
├─ Docs/
└─ ThirdParty/
```

## 作成基準

- BOOTHのダウンロード商品はZIPで配布し、購入者が解凍して使う前提にする。
- スマートフォンではZIPやUnityファイルを扱いにくい場合があるため、説明文はBOOTH商品ページと同梱ドキュメントの両方に置く。
- Unityパッケージの導入前提、対応バージョン、PC専用であること、Quest/Android非対応範囲を明記する。
- VRChat、lilToon、Modular Avatar、VRC Light Volumesなど外部依存は同梱しないものとして案内する。
- 成人寄り・誤解を招く表現は避け、`高揚発色`、`温かい血色感`、`撮影向け`など健全な言葉に置き換える。
- 競合製品名を攻撃的に使わず、自製品の機能・導入しやすさ・AAO併用方針で説明する。

## 出荷前チェック

- [ ] ZIPを解凍して、`Package/lilToon PCSS Studio for VRChat` が存在する。
- [ ] `00_README` と `01_INSTALL` がmd/txt/htmlの3形式で入っている。
- [ ] BOOTH商品ページへ貼れる商品説明テンプレートが入っている。
- [ ] 利用規約、更新履歴、FAQ、問い合わせテンプレートが入っている。
- [ ] `Editor/PCSSCameraDepthPreview.cs` と `Excited Tone` 関連の実装がPackage内に入っている。
- [ ] UnityでC#コンパイルエラーが出ていない。
- [ ] ZIP名、バージョン、更新日が販売ページと一致している。

## 参考

- BOOTHヘルプセンター: ダウンロード商品は購入履歴やライブラリから取得でき、ZIPは解凍ソフトが必要な場合がある。
- BOOTHヘルプセンター: 閲覧環境が限られるデータは商品紹介に明記する。

