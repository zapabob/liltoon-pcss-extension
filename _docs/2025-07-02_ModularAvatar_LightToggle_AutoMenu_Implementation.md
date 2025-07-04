# 2025-07-02 ModularAvatar Lightトグル自動生成メニュー 実装ログ

## 概要
ModularAvatarのObject Toggle機能を活用し、Unityエディタ上でアバター直下にLightトグル用のGameObjectを自動生成し、配下の全LightコンポーネントをObject Toggleに一括登録するEditor拡張（`ModularAvatarLightToggleMenu.cs`）を実装。

---

## 実装経緯・要件
- VRChatアバターでModularAvatarのObject Toggleを使い、複数のLightをまとめてON/OFFしたい要望があった。
- 手動でObject Toggleを設定するのは手間がかかるため、Unityメニューから自動生成できるツールが求められた。
- 公式推奨のObject Toggle方式に準拠し、アバター直下にLightToggleオブジェクトを作成し、配下のLightを全て登録する仕様とした。

---

## 実装内容
- Unityメニュー「Tools > ModularAvatar > Add Light Toggle」を追加。
- 選択中のアバター直下にLightToggle用のGameObjectを自動生成。
- アバター配下の全Lightコンポーネントを探索し、Object Toggleに一括登録。
- ModularAvatarObjectToggleコンポーネントを自動で付与。
- 既存のLightToggleオブジェクトがあれば重複生成しない。
- Undo対応、エラーハンドリング、ユーザーフィードバックも考慮。

---

## 使い方
1. UnityエディタでアバターのルートGameObjectを選択。
2. メニュー「Tools > ModularAvatar > Add Light Toggle」を実行。
3. アバター直下にLightToggleオブジェクトが生成され、配下の全LightがObject Toggleに登録される。
4. VRChat内でModularAvatarのメニューからLightのON/OFFトグルが可能。

---

## 注意点・ベストプラクティス
- 既存のLightToggleオブジェクトがある場合は重複生成しない。
- Object Toggleの仕様上、トグル対象はGameObject単位（Lightコンポーネント単体ではない）であることに注意。
- ModularAvatarのバージョンや仕様変更により動作が変わる場合があるため、公式ドキュメントも参照推奨。
- Undo機能やエラーハンドリングも実装済み。

---

## 参考
- [ModularAvatar公式ドキュメント](https://modular-avatar.nadena.dev/docs/)
- [Unity Logger/ILogHandler 実装例](https://docs.unity3d.com/ScriptReference/Logger.html)

---

## 実装ファイル
- `Assets/Editor/ModularAvatarLightToggleMenu.cs`

---

## 実装日
2025-07-02

---

## 実装担当
AIアシスタント（ユーザー要望に基づく自動生成） 