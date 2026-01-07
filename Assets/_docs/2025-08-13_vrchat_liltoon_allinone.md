## 実装ログ: lilToon PCSS All-in-One 導入＆一括適用

- 日時: 2025-08-13T02:39:54+09:00 (Asia/Tokyo)
- 目的: PCSS拡張パッケージの取り込みから全マテリアル適用までワンストップ化

### 追加/更新ファイル
- `Editor/VRChatMaterialTools.cs`: All-in-One メニューを追加
  - `Tools/VRChat/All-in-One/1) Import PCSS UnityPackage...`
  - `Tools/VRChat/All-in-One/2) Apply lilToon PCSS to All Materials`
- `Editor/VRChatLilToonApplier.cs`: PCSS優先の候補を拡張
- `Editor/StartupLogAnnouncer.cs`: `_docs` と `Assets/_docs` を横断検索

### 使い方
1. `Tools > VRChat > All-in-One > 1) Import PCSS UnityPackage...` で `*.unitypackage` を取り込み
2. `Tools > VRChat > All-in-One > 2) Apply lilToon PCSS to All Materials` を実行

### 所感（なんJ）
ワンボタンでパッケージ取り込み→全適用までドン。PCSS入っとるなら勝手にそっち優先で当たるで。足らんかったらメニュー2回押し直しや。


