# lilToon PCSS Extension v1.5.4 リリースログ

## 概要
lilToon PCSS Extension v1.5.4をリリースしました。このバージョンでは、表情が赤くなるライト機能をFXレイヤーからBaseレイヤーに移行し、より安定した動作を実現しました。また、GitHub Pagesの整備や新しいサンプルの追加も行いました。

## 主な変更点

### 🔴 表情が赤くなるライト機能のBaseレイヤー実装
- **FXレイヤーからBaseレイヤーへの移行**
  - より安定した動作を実現
  - FXレイヤーの制約を受けずにライトの制御が可能に
  - アバターのエクスプレッションメニューとの連携強化

- **ModularAvatarとの連携機能強化**
  - `lilToon/PCSS Extension/ModularAvatar/表情赤くなるライトメニューを追加 (Base)` メニューの追加
  - アバターのルートを選択するだけで自動セットアップ
  - 既存のModularAvatarメニューとの統合

- **自動頭部ライト生成機能**
  - アバターの頭部を自動検出
  - 適切な位置に赤いライトを生成
  - Humanoidアバターの自動認識対応

- **ライト強度調整機能の実装**
  - 3段階の強度調整（弱・中・強）
  - 強度に応じた色調整（赤みの増減）
  - アニメーションによる滑らかな遷移

- **VRChat Expression Menu対応**
  - `lilToon/PCSS Extension/Create RedFace Light Menu (Base)` メニューの追加
  - アニメーターコントローラーの自動生成
  - エクスプレッションパラメーターの自動設定

### 🛠️ GitHub Pagesの整備
- **最新バージョン情報の更新**
  - バージョン表示を1.5.4に更新
  - 最新の変更点を反映
  - パッケージ情報の更新

- **表情が赤くなるライト機能のドキュメント追加**
  - 機能の詳細説明
  - セットアップガイド
  - 使用例とベストプラクティス

- **VPMリポジトリの更新**
  - 最新パッケージ情報の反映
  - インストール手順の更新
  - 依存関係の整理

### 📦 新しいサンプルの追加
- **「🔴 Red Face Light」サンプル**
  - 表情が赤くなるライト機能のBaseレイヤー実装例
  - サンプルシーンとプリセット
  - 詳細な解説とチュートリアル

## 技術的な改善点
- ModularAvatarLightToggleMenuクラスの拡張
- VRChatExpressionMenuCreatorクラスへの機能追加
- ライトの強度調整用アニメーション生成機能
- 頭部自動検出アルゴリズムの実装
- Baseレイヤーでのライト制御の最適化

## インストール方法
1. **VCC (VRChat Creator Companion) を使用する場合:**
   - VCCで「Add Repository」を選択
   - `https://zapabob.github.io/liltoon-pcss-extension/index.json` を追加
   - パッケージリストから「lilToon PCSS Extension」をインストール

2. **Unity Package Manager を使用する場合:**
   - Window > Package Manager を開く
   - 「+」ボタン > Add package from git URL を選択
   - `https://github.com/zapabob/liltoon-pcss-extension.git?path=com.liltoon.pcss-extension#v1.5.4` を入力

## 使用方法
1. アバターのルートオブジェクトを選択
2. メニューから `lilToon/PCSS Extension/ModularAvatar/表情赤くなるライトメニューを追加 (Base)` を選択
3. 自動的にライトが生成され、エクスプレッションメニューに追加されます
4. VRChatでアバターをアップロードし、エクスプレッションメニューから「表情赤くなる」を選択

## 今後の予定
- さらに細かいライト調整機能の追加
- 複数のライトを使用した表現の強化
- 他のシェーダー機能との連携強化
- モバイル向けの最適化

## 謝辞
本機能の開発にあたり、多くのフィードバックとサポートをいただいたコミュニティの皆様に感謝いたします。特に、FXレイヤーでの問題点を指摘し、Baseレイヤーへの移行を提案してくださったユーザーの皆様に深く感謝申し上げます。 