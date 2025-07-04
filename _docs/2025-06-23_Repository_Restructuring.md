# 実装ログ: リポジトリ構造の完全な再構築

## 概要

一連の解決困難なUnityコンパイルエラーの根本原因が、リポジトリの不適切なファイル構造にあると断定し、本日、リポジトリ全体の構造を抜本的に再構築する整理作業を実施しました。

## 背景

これまで、`VRCPlayerApi`の未定義エラーや`Mono.Cecil.AssemblyResolutionException`など、様々なコンパイルエラーに悩まされてきました。コードレベルでの修正を繰り返しても問題が再発することから、原因はより深い階層、すなわちプロジェクトのファイル構造そのものにあると結論付けました。

特に、以下の問題が確認されていました。

-   **ソースコードの散在:** VPMパッケージの本体であるべき `Editor`, `Runtime`, `Shaders` などのディレクトリが、リポジトリのルートに直接配置されていました。
-   **過去の成果物の残存:** `Release` ディレクトリや大量の `.zip` ファイル、`Assets` フォルダなど、過去のビルド成果物やUnityプロジェクトの一時ファイルがリポジトリ内に混在し、名前空間やアセンブリ定義の競合を引き起こしていました。
-   **非標準な構造:** 全体として、VRChat Creator Companion (VCC) が正しくパッケージとして認識できる標準的な構造から逸脱していました。

## 実施した内容

上記の問題を解決し、クリーンでメンテナンス性の高いリポジトリを実現するため、以下の手順で整理・再構築を行いました。

1.  **パッケージコンテナの作成:**
    -   VPMパッケージの公式なルートとして `com.liltoon.pcss-extension` ディレクトリを最上位に作成しました。

2.  **ソースコードの集約:**
    -   ルートに散らばっていた `Editor`, `Runtime`, `Shaders` の各ディレクトリを、すべて `com.liltoon.pcss-extension` 内に移動しました。
    -   `package.json`, `CHANGELOG.md`, `README.md` といったパッケージのメタデータファイルも同様に集約しました。

3.  **徹底的なクリーンアップ:**
    -   コンフリクトの元凶となっていた `Assets` ディレクトリを完全に削除しました。
    -   過去の全バージョンのビルド成果物が含まれていた `Release` ディレクトリを削除しました。
    -   ルートディレクトリにあったすべての `.zip` ファイルを削除しました。
    -   `docs/Release` ディレクトリなど、GitHub Pages内に残っていた古いリリース関連ファイルも一掃しました。
    -   その他、ルートにあった古いリリースノート等の不要なファイルを削除しました。

4.  **スクリプトの整理:**
    -   各種のビルド用・補助用スクリプト（`.py`, `.ps1`）を、すべて `scripts` ディレクトリにまとめて整理しました。

## 期待される効果

この再構築により、以下の効果が期待されます。

-   **コンパイルエラーの完全解決:** ファイルの競合がなくなり、UnityおよびVCCがパッケージを正しく認識・コンパイルできるようになります。
-   **メンテナンス性の向上:** パッケージのソースコードが一箇所にまとまったことで、将来の機能追加や修正が容易になります。
-   **VCCとの互換性確保:** 標準的なVPMパッケージ構造に準拠したことで、VCCでのインストール・アップデートが安定して行えるようになります。

今後は、この新しいクリーンな構造を維持し、開発を進めていきます。 