# lilToon PCSS Extension v1.5.4

## 製品概要

lilToon PCSS Extension（リルトゥーン PCSS エクステンション）は、VRChatアバターに映画品質の柔らかい影を追加するシェーダー拡張パッケージです。PCSSテクノロジーを使用して、距離に応じて自然に柔らかくなるリアルな影を実現し、アバターの見た目を大幅に向上させます。

![製品イメージ]

## ✨ 主な機能

- 🎬 **映画品質のソフトシャドウ**: 距離に応じて自然に柔らかくなる影を実現
- 🎯 **ワンクリック・セットアップ**: Avatar Selector Menuで簡単設定
- 🎨 **4つのプリセット**: Realistic、Anime、Cinematic、Customから選択可能
- 🚀 **VRChat/Quest自動パフォーマンス最適化**: 自動的にVRChatのパフォーマンスランクに合わせて最適化
- 🛡️ **ModularAvatar連携**: アバターメニューへの簡単な統合とエラー耐性強化
- ⚡ **URP 14.0.10/Unity 2022.3 LTS完全対応**: 最新環境でも安心
- 📝 **サンプル・日本語UI・ドキュメント**: 充実したサポート資料

## 💎 詳細機能説明

### 映画品質のソフトシャドウ
PCSSテクノロジーは、従来のハードシャドウとは異なり、光源からの距離に応じて自然に柔らかくなる影を生成します。これにより、映画のような高品質な見た目を実現し、アバターの立体感と存在感を大幅に向上させます。

### プリセットシステム
初心者から上級者まで、誰でも簡単に理想の影を設定できるよう4つのプリセットを用意しています：
- **Realistic**: 現実世界に近い自然な影の表現
- **Anime**: アニメ調の鮮やかで明確な影
- **Cinematic**: 映画のような劇的で印象的な影
- **Custom**: 細かいパラメータを自由に調整可能

### パフォーマンス最適化
VRChatの各パフォーマンスランク（Excellent、Good、Medium、Poor）に合わせて自動的に最適化を行い、FPSへの影響を最小限に抑えます。Questユーザーにも配慮した軽量化設定も含まれています。

### ModularAvatar連携
ModularAvatarと連携して、VRChat内でアバターメニューからPCSS効果をオン/オフできるようにします。複数の照明設定を切り替えることも可能で、様々なシチュエーションに対応できます。

### PhysBone対応
PhysBoneと連動して動的なライト制御を行うことができ、アバターの動きに合わせて自然に影が変化します。これにより、より生き生きとしたアバター表現が可能になります。

### Poiyomiシェーダー互換
lilToonだけでなく、Poiyomiシェーダーにも対応しており、幅広いアバターで利用可能です。既存のマテリアル設定を維持したまま、PCSS効果を追加することができます。

## 🚀 導入手順

### 1. Booth版（手動インストール）

1. ダウンロードしたZIPファイルを解凍します
2. 解凍した「com.liltoon.pcss-extension」フォルダをUnityプロジェクトの「Packages」フォルダにコピーします
3. Unityを再起動します
4. `Window > lilToon PCSS Extension > Avatar Selector`が表示されていれば導入完了です

### 2. VCC (VRChat Creator Companion) を使用したインストール

1. VRChat Creator Companion (VCC) を開きます
2. 「設定」タブを開き、「リポジトリ」セクションに移動します
3. 「リポジトリを追加」をクリックし、以下のURLを入力します：
   ```
   https://zapabob.github.io/liltoon-pcss-extension/index.json
   ```
4. プロジェクトに移動し、「パッケージを管理」を選択します
5. 「lilToon PCSS Extension」を探し、「追加」をクリックします

### 3. Unity Package Managerを使用したインストール

1. Unityプロジェクトを開きます
2. メニューから「Window」→「Package Manager」を選択します
3. 「+」ボタンをクリックし、「Add package from git URL...」を選択します
4. 以下のURLを入力します：
   ```
   https://github.com/zapabob/liltoon-pcss-extension.git?path=com.liltoon.pcss-extension
   ```
5. 「追加」をクリックします

## 📖 使用方法

### アバターへの適用

1. Unityメニューから「Window」→「lilToon PCSS Extension」→「Avatar Selector」を選択します
2. シーン内のアバターが自動的にリストアップされます
3. 適用したいアバターを選択します
4. プリセット（Realistic、Anime、Cinematic、Custom）を選択します
5. 「適用 (Apply)」をクリックします
6. 必要に応じて詳細設定を調整します

### ModularAvatarとの連携

1. Unityメニューから「Window」→「lilToon PCSS Extension」→「ModularAvatar Menu Creator」を選択します
2. アバターのルートオブジェクトを選択します
3. 「Create Menu」をクリックします
4. VRChat内でアバターメニューからPCSS効果をオン/オフできるようになります

### パフォーマンス設定

1. Unityメニューから「Window」→「lilToon PCSS Extension」→「Performance Optimizer」を選択します
2. 目的のパフォーマンスランク（Excellent、Good、Medium、Poor）を選択します
3. 「Apply Settings」をクリックします
4. Quest対応の最適化も自動的に適用されます

## 🛠️ システム要件

| 項目 | 必要バージョン |
| --- | --- |
| **Unity** | 2022.3.6f1以降 |
| **lilToon** | 1.7.0以降 |
| **VRChat Avatars SDK** | 3.5.0以降 |
| **Modular Avatar** | 1.10.0以降 |

## ❓ よくある質問

**Q: 他のシェーダーと併用できますか？**  
A: はい、lilToonとPoiyomiシェーダーに対応しています。他のシェーダーとも基本的に競合しません。

**Q: VRChatのパフォーマンスに影響しますか？**  
A: パフォーマンス最適化機能により、各ランク（Excellent、Good、Medium、Poor）に合わせて自動調整されるため、影響を最小限に抑えられます。

**Q: Questでも使用できますか？**  
A: はい、Quest対応の最適化設定が含まれています。ただし、PCと比べると機能が制限される場合があります。

**Q: アップデートはどのように行いますか？**  
A: VCCを使用している場合は、VCC内で自動的に更新通知が表示されます。手動インストールの場合は、新しいバージョンをダウンロードして上書きしてください。

## 📝 サポート

不具合や質問がある場合は、以下の方法でサポートを受けることができます：

- 公式サイト: [https://zapabob.github.io/liltoon-pcss-extension/](https://zapabob.github.io/liltoon-pcss-extension/)
- GitHub: [Issues](https://github.com/zapabob/liltoon-pcss-extension/issues)
- Booth: メッセージ機能からお問い合わせください

## ライセンス

このパッケージはMITライセンスで公開されています。個人利用および商用利用が可能です。詳細はライセンス条項をご確認ください。

---

© 2025 lilToon PCSS Extension Team. All Rights Reserved. 