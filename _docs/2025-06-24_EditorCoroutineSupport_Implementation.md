# 2025-06-24 EditorCoroutine（com.unity.editor-coroutines）対応実装ログ

## 概要
Unity公式のEditorコルーチン（com.unity.editor-coroutines）パッケージへの正式対応を明記し、VPMリポジトリ（index.json）およびユーザー向けREADMEに反映した。

## 実装経緯
- Unity 2022.3 LTS以降、Editor拡張の非同期処理には公式Editorコルーチンパッケージ（com.unity.editor-coroutines）が推奨されている。
- ユーザーに依存パッケージの明示と、安定したエディタ拡張体験を提供するため、公式対応を明記。

## 編集内容
- `docs/index.json`：
  - descriptionに「Unity公式Editorコルーチン対応」を追記
  - editorCoroutineSupportフィールドを新設し、パッケージ名・バージョン・説明を記載
- `docs/README.md`：
  - 機能一覧に「Unity公式Editorコルーチン対応」を追記
  - 互換性リストに「Editor Coroutines: com.unity.editor-coroutines 1.0.0（Unity公式Editorコルーチン対応）」を追加

## 反映ファイル
- docs/index.json
- docs/README.md

## 参考
- [Unity公式 Editor Coroutines パッケージ解説](https://docs.unity3d.com/6000.1/Documentation/Manual/com.unity.editorcoroutines.html)

---
今後もUnity公式パッケージの動向に合わせて、依存関係・機能説明の明示を徹底する。 