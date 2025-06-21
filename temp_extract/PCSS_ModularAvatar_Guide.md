# lilToon PCSS × ModularAvatar 統合ガイド

## 概要
このシステムにより、ModularAvatarを使用してlilToon+PCSS効果をワンクリックでアバターに適用し、VRChatのアバターメニューから制御できるようになります。

## 特徴
- **ワンクリック適用**: Unityメニューから簡単にPCSS効果を適用
- **ModularAvatar完全対応**: 自動的にメニューとアニメーターを統合
- **VRChatメニュー制御**: ゲーム内でPCSS効果のON/OFF切り替え
- **品質調整**: リアルタイムでPCSS品質を調整可能
- **非破壊的**: 元のマテリアルを保持して安全に適用

## 必要な環境
- Unity 2019.4 LTS以降
- VRChat SDK3 (Avatars)
- ModularAvatar v1.4.0以降
- lilToon v1.10.3以降

## セットアップ手順

### 1. 事前準備
1. **VRChat SDK3**をインポート
2. **ModularAvatar**をインポート
3. **lilToon**をインポート
4. 本PCSS拡張シェーダーファイルをプロジェクトに追加

### 2. アバター準備
1. アバターをシーンに配置
2. アバターのルートに**VRC Avatar Descriptor**コンポーネントを追加
3. 必要に応じて**ModularAvatar**コンポーネントを設定

### 3. PCSS適用

#### 方法1: Unityメニューから
1. **lilToon > PCSS Avatar Setup**を選択
2. ウィンドウが開いたら対象アバターを選択
3. 品質プリセットを選択（推奨: Balanced）
4. **「PCSS効果を適用」**ボタンをクリック

#### 方法2: 右クリックメニューから
1. Hierarchyでアバターを選択
2. 右クリックして**「lilToon PCSS > Apply to Selected Avatar」**を選択

### 4. 自動生成される要素

適用後、以下が自動生成されます：

#### アセット
- `Assets/PCSS/` フォルダ
  - PCSS対応マテリアル（元の名前_PCSS）
  - `PCSS_Control_Menu.asset` - VRChatメニュー
  - `PCSS_Controller.controller` - アニメーターコントローラー
  - `PCSS_On.anim` / `PCSS_Off.anim` - アニメーションクリップ

#### GameObject構造
```
Avatar Root
├── [既存のアバター構造]
└── PCSS_Control
    ├── ModularAvatarMenuInstaller
    ├── ModularAvatarParameters
    └── ModularAvatarMergeAnimator
```

### 5. VRChatでの使用方法

アバターをアップロード後、VRChatのアバターメニューに以下が追加されます：

#### メニュー項目
- **PCSS効果** (Toggle): PCSS効果のON/OFF切り替え
- **PCSS品質** (Four Axis Puppet): 品質レベルの調整

#### パラメータ
- `PCSS_Enable` (Bool): PCSS有効/無効
- `PCSS_Quality` (Float): 品質レベル (0.0-1.0)

## 高度な設定

### カスタム品質プリセット
ウィンドウの詳細設定で以下を調整可能：
- **Blocker Search Radius**: ブロッカー検索範囲
- **Filter Radius**: フィルタリング範囲
- **Sample Count**: サンプリング数
- **Light Size**: 仮想光源サイズ

### マニュアル統合
ModularAvatarを使わない場合：
```csharp
// スクリプトから直接呼び出し
LilToonPCSSMenuIntegration.ApplyPCSSToAvatar(avatarGameObject);
```

## トラブルシューティング

### よくある問題

#### 1. ModularAvatarが検出されない
**症状**: "ModularAvatarコンポーネントが見つかりません"
**解決策**: 
- ModularAvatarが正しくインストールされているか確認
- アバターにModularAvatarコンポーネントを追加

#### 2. メニューが表示されない
**症状**: VRChatでPCSSメニューが表示されない
**解決策**:
- VRChat SDK3が正しくインストールされているか確認
- アバターのExpression Menuが設定されているか確認
- ModularAvatarのMenu Installerが正しく動作しているか確認

#### 3. アニメーションが動作しない
**症状**: PCSS効果のON/OFF切り替えが動作しない
**解決策**:
- アニメーターコントローラーが正しく生成されているか確認
- FXレイヤーにアニメーターが正しくマージされているか確認
- パラメータ名が正しく設定されているか確認

#### 4. パフォーマンスが悪い
**症状**: FPSが低下する
**解決策**:
- 品質プリセットを「Light」に変更
- Sample Countを下げる（8-16推奨）
- 不要なオブジェクトでPCSSを無効化

### デバッグ方法

#### コンソールログの確認
```
"PCSS制御メニューがModularAvatarに追加されました。" // 正常
"ModularAvatarが見つかりません。" // ModularAvatar未インストール
"PCSS拡張シェーダーが見つかりません。" // シェーダー不足
```

#### 手動確認
1. `Assets/PCSS/`フォルダの存在確認
2. アバター内の`PCSS_Control`オブジェクトの確認
3. ModularAvatarコンポーネントの設定確認

## パフォーマンス最適化

### VRChat用最適化設定
- **PC用**: Balanced品質プリセット
- **Quest用**: Light品質プリセット + Sample Count 8

### バッチ処理
複数アバターに一括適用する場合：
```csharp
foreach(var avatar in selectedAvatars)
{
    LilToonPCSSMenuIntegration.ApplyPCSSToAvatar(avatar);
}
```

## 更新・削除

### PCSS効果の削除
1. PCSS Setupウィンドウで**「元に戻す」**ボタンをクリック
2. または手動で`PCSS_Control`オブジェクトを削除

### アップデート時の注意
- 新しいバージョンの適用前に既存のPCSS設定をバックアップ
- マテリアル設定は手動で再適用が必要な場合があります

## 技術仕様

### 対応シェーダー
- `lilToon/PCSS Extension` (Built-in RP)
- `lilToon/PCSS Extension URP` (Universal RP)

### 生成ファイル形式
- `.mat` - マテリアルファイル
- `.asset` - VRCメニューファイル
- `.controller` - アニメーターコントローラー
- `.anim` - アニメーションクリップ

### ModularAvatar互換性
- ModularAvatarMenuInstaller
- ModularAvatarParameters
- ModularAvatarMergeAnimator

これで、ModularAvatarを使用したlilToon+PCSS効果の完全な統合が可能になります！ 