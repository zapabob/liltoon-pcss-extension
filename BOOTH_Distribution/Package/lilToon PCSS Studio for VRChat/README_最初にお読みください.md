# lilToon PCSS Studio for VRChat

このたびは lilToon PCSS Studio for VRChat をお迎えくださり、ありがとうございます。

本パッケージは、PC向けVRChatアバターのlilToonマテリアルへ、やわらかい落ち影、接地感、艶と影のまとまりを追加するためのUnity Editor拡張です。PCSS系の見た目を、Unity上でなるべく短い手順で整えられるようにしています。

## まず読むもの

1. `導入手順.md`
   - 導入からPCSS Hub実行までの最短手順です。
2. `BOOTH_利用規約.md`
   - 利用できる範囲、再配布、サポート範囲をまとめています。
3. `THIRD_PARTY_NOTICES.md`
   - VRChat SDK、lilToon、Modular Avatarなど外部依存の扱いです。

## 同梱内容

- `Package/lilToon PCSS Studio for VRChat`
  - Unityへ入れる本体です。
- `Docs`
  - 導入手順、販売ページ用説明、利用規約、第三者表記です。
- `CHANGELOG.md`
  - 更新履歴です。

## 対応環境

- Unity 2022.3.22f1
- VRChat Avatars SDK 3.10.3以降
- lilToon 2.3.2以降
- Modular Avatar 1.17.1以降
- VRC Light Volumes 2.1.3以降は任意

QuestおよびAndroid向けアップロードでは、本パッケージのPCSS表現は対象外です。PCアバター向けの見た目調整としてお使いください。

## 推奨入口

Unityを開いたあと、上部メニューから次を選んでください。

`Tools > lilToon-PCSS-Extension > PCSS Hub (PC)`

アバターを選び、プリセットを選んで、PC向けセットアップを実行します。導入後にNDMF ConsoleやVRChat SDK Builderの警告を確認し、問題がなければアップロードへ進みます。

標準では `AAO Performance Safe (0 Lights)` が有効です。PCSS生成ライトとアバター配下の既存Lightコンポーネントを残さず、マテリアル側の影・艶設定で見た目を整えるため、Avatar OptimizerやVRChat性能ランクの妨げになりにくい構成です。VeryPoorが残る場合は、三角形数やSkinned Mesh Renderer数など、元アバター側の性能項目も確認してください。

肌向けの追加プリセットとして `うるみ艶肌 / Dewy Skin Gloss` と `ほのか上気肌 / Soft Flush Skin` を同梱しています。前者は汗ばみのような細いハイライトを、健全なポートレート向けのしっとりした艶として扱う設定です。後者は顔マテリアル向けの柔らかな頬染め、暖かい影色、控えめな肌艶をまとめた血色感プリセットです。

古い `PCSS v2.2.0` 系unitypackageを導入済みのアバターには、`Tools > lilToon-PCSS-Extension > Repair > Legacy PCSS v2.2.0 Intake` を用意しています。旧PCSSのサンプル数、柔らかさ、半径などを現在のAAO安全なマテリアル設定へ移し、アバター上に残る旧ランタイム部品は取り除きます。

## 追加プリセット: 高揚発色 / Excited Tone

旧PCSS v2.2.0系に含まれていた成人寄りの表現名は、そのまま同梱していません。本パッケージでは、運動後やライブ照明のような温かい血色感を扱う健全な `Excited Tone` / `高揚発色` として実装しています。顔・肌らしいマテリアルには上気した温かい色調、控えめな艶、柔らかいPCSS影をまとめて設定し、衣装や髪には強い発色を乗せないようにしています。

カメラ深度の確認が必要な場合は、Unity上で `Tools > lilToon-PCSS-Extension > Preview > Enable Camera Depth Texture` を使えます。これは旧ZIP内のカメラ深度補助から有用な部分だけを取り出したEditor用プレビュー機能で、アバター本体へRuntimeコンポーネントを残しません。
