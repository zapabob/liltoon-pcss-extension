# 2025-06-23_Shader_Compilation_Fix_and_Material_Compatibility.md

## 1. はじめに

本ログは、`lilToon PCSS Extension` で発生していたコンパイルエラーの解消、およびliltoon標準シェーダーとのマテリアル互換性を確保するために実施した一連の修正作業を記録するものである。

## 2. 問題の特定

ユーザーより、以下の2つの主要な問題が報告された。

1.  **コンパイルエラー:** Unityプロジェクトで拡張シェーダーに関連するコンパイルエラーが発生し、ビルドが正常に完了しない。
2.  **マテリアルの非互換性:** 拡張シェーダーがliltoonの標準シェーダーとマテリアルを共有できず、シェーダーを切り替えるたびに設定がリセットされてしまう。

## 3. 原因分析

調査の結果、エラーの根本原因は以下の2点にあると特定した。

1.  **独立したシェーダーGUI:** `Editor/LilToonPCSSShaderGUI.cs` が、liltoon本体の `liltoon.lilToonShaderGUI` を継承せず、`ShaderGUI` を直接継承して独立したGUIとして実装されていた。これにより、liltoon本体のアップデート（APIの変更など）に追従できず、コンパイルエラーが発生していた。また、マテリアルの互換性も失われていた。
2.  **シェーダーとGUIの不整合:** GUIスクリプトとシェーダーファイル (`.shader`) の間で、シェーダーキーワードやプロパティの定義に食い違いが発生していた。特に、PCSS機能を有効化するキーワードが `_PCSS_ON` (GUI側) と `_USE_PCSS_ON` (シェーダー側) で異なっており、機能が正しく動作しない状態だった。

## 4. 解決策と実装内容

上記の問題を解決するため、以下の修正を実施した。

### 4.1. `Editor/LilToonPCSSShaderGUI.cs` の修正

-   クラスの継承元を `ShaderGUI` から `liltoon.lilToonShaderGUI` に変更。
-   `OnGUI` メソッドをオーバーライドし、最初に `base.OnGUI()` を呼び出すことでliltoon標準のGUIを描画。その後、PCSS拡張機能に固有のUIのみを追加で描画するようにした。
-   liltoon本体と重複するプロパティ定義（色、メインテクスチャ、レンダリングモードなど）を削除し、コードを簡潔化した。
-   PCSS機能のキーワードを `_PCSS_ON` に統一した。

### 4.2. `Shaders/lilToon_PCSS_Extension.shader` の修正

-   `#pragma shader_feature_local` で定義するキーワードを、GUIの変更に合わせて `_USE_PCSS_ON` から `_PCSS_ON` に変更。
-   シェーダー内の参照箇所 (`#if defined(...)`) も同様に `_PCSS_ON` に統一。
-   GUIから制御できなくなった古い機能 (`Shadow Clamp`) に関連するプロパティとロジックをシェーダーから完全に削除。
-   GUIに追加されたVRChat Light Volumes機能に対応するため、プロパティとシェーダーキーワード (`_VRC_LIGHT_VOLUMES_ON`) を追加。

## 5. 結論

本修正により、`lilToon PCSS Extension` のコンパイルエラーは解消され、liltoon標準シェーダーとの完全なマテリアル互換性が確保された。これにより、ユーザーは安定した環境で、かつ利便性を損なうことなく、本拡張機能を使用できるようになった。 