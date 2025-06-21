# lilToon PCSS Extension - 使用ガイド

## 概要
このシェーダーは、lilToonにPCSS（Percentage-Closer Soft Shadows）機能を追加した拡張版です。PCSSにより、光源からの距離に応じて自然にぼけるソフトシャドウを実現できます。

## 特徴
- **リアルなソフトシャドウ**: 光源に近いオブジェクトはシャープな影、遠いオブジェクトはぼけた影を生成
- **lilToon互換**: 既存のlilToonの機能と併用可能
- **高品質なサンプリング**: Poisson Diskサンプリングによる高品質なシャドウ
- **パフォーマンス調整可能**: サンプル数やフィルター半径を調整してパフォーマンスと品質のバランスを取れる

## セットアップ手順

### 1. Unity設定
1. **Quality Settings**を開く（Edit > Project Settings > Quality）
2. **Shadows**セクションで以下を設定：
   - Shadow Resolution: High (2048) 以上推奨
   - Shadow Projection: Close Fit
   - Shadow Distance: 適切な値に設定（50-150推奨）
   - Shadow Cascades: 4 Cascades推奨

### 2. Light設定
1. Directional Lightを選択
2. **Shadows**を**Soft Shadows**に設定
3. **Shadow Bias**を0.05-0.1に設定
4. **Shadow Normal Bias**を0.4-1.0に設定

### 3. マテリアル設定
1. 新しいマテリアルを作成
2. シェーダーを**lilToon/PCSS Extension**に変更
3. 以下のパラメータを調整：

## パラメータ説明

### PCSS設定
- **Use PCSS**: PCSSを有効/無効にする
- **PCSS Blocker Search Radius** (0.001-0.1): ブロッカー検索の範囲（小さいほどシャープ）
- **PCSS Filter Radius** (0.001-0.1): フィルタリング範囲（大きいほどぼける）
- **PCSS Sample Count** (4-64): サンプリング数（多いほど高品質だが重い）
- **PCSS Light Size** (0.1-10.0): 仮想的な光源サイズ（大きいほどソフト）

### 推奨設定値

#### 高品質設定（重い）
- Blocker Search Radius: 0.02
- Filter Radius: 0.03
- Sample Count: 32
- Light Size: 2.0

#### バランス設定（推奨）
- Blocker Search Radius: 0.01
- Filter Radius: 0.02
- Sample Count: 16
- Light Size: 1.5

#### 軽量設定（軽い）
- Blocker Search Radius: 0.005
- Filter Radius: 0.01
- Sample Count: 8
- Light Size: 1.0

## パフォーマンス最適化

### GPU最適化
1. **Sample Count**を下げる（8-16推奨）
2. **Filter Radius**を小さくする
3. 不要なオブジェクトの**Cast Shadows**をオフにする

### シャドウ最適化
1. **Shadow Distance**を適切に設定
2. **Shadow Cascade**を調整
3. 小さなオブジェクトの**Receive Shadows**をオフにする

## トラブルシューティング

### 影が表示されない場合
1. Lightの**Shadows**が**Soft Shadows**になっているか確認
2. オブジェクトの**Cast Shadows**と**Receive Shadows**が有効か確認
3. **Use Shadow**が有効になっているか確認

### パフォーマンスが悪い場合
1. **Sample Count**を下げる
2. **Shadow Resolution**を下げる
3. **Shadow Distance**を短くする

### 影が粗い場合
1. **Shadow Resolution**を上げる
2. **Sample Count**を増やす
3. **Shadow Bias**を調整する

## 注意事項
- PCSSは計算負荷が高いため、モバイル端末では使用を控えることを推奨
- VRChatなどでの使用時は、パフォーマンスランクに注意
- 大量のオブジェクトに適用する場合は、LODシステムの使用を検討

## 技術仕様
- **対応Unity版本**: 2019.4 LTS以降
- **対応レンダーパイプライン**: Built-in Render Pipeline
- **シェーダーモデル**: 3.0以降
- **対応プラットフォーム**: PC, Console（モバイルは非推奨） 