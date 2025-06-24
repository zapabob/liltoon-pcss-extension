# 2025-06-24 Booth用README実装ログ

## 実装内容

lilToon PCSS Extension v1.5.4のBooth販売用READMEを作成しました。このREADMEは製品の特徴、導入方法、使用方法を明確に説明し、購入者が簡単に製品を利用できるようにすることを目的としています。

## 実装詳細

### 1. 製品概要セクション
- 製品の主な特徴と目的を簡潔に説明
- PCSSテクノロジーによる映画品質の柔らかい影の効果を強調

### 2. 主な機能セクション
- 絵文字を使用して視覚的に分かりやすく機能を列挙
- 映画品質のソフトシャドウ、ワンクリックセットアップ、プリセット、パフォーマンス最適化などの主要機能を強調
- Unity 2022.3 LTSとURP 14.0.10の完全対応を明記

### 3. 詳細機能説明セクション
- 各主要機能について詳細な説明を追加
- 映画品質のソフトシャドウの技術的な背景
- プリセットシステムの4種類の選択肢（Realistic、Anime、Cinematic、Custom）を説明
- パフォーマンス最適化の仕組みとVRChatランクへの対応
- ModularAvatar連携の詳細と利点
- PhysBone対応による動的なライト制御の説明
- Poiyomiシェーダー互換性の説明

### 4. 導入手順セクション
- Booth版（手動インストール）を最初に記載し、購入者が最も使用する可能性の高い方法を優先
- VCC（VRChat Creator Companion）を使用したインストール方法も詳細に説明
- Unity Package Managerを使用したインストール方法も提供

### 5. 使用方法セクション
- Avatar Selectorを使用したアバターへの適用手順を詳細に説明
- ModularAvatarとの連携方法を説明
- パフォーマンス設定の調整方法を説明

### 6. システム要件セクション
- 必要なソフトウェアとそのバージョンを表形式で明確に提示
- Unity、lilToon、VRChat SDK、ModularAvatarの必要バージョンを明記

### 7. よくある質問（FAQ）セクション
- 購入前後によくある質問と回答を追加
- 他のシェーダーとの併用可能性
- VRChatのパフォーマンスへの影響
- Quest対応の有無
- アップデート方法

### 8. サポートとライセンスセクション
- サポート方法（公式サイト、GitHub、Boothメッセージ）を記載
- MITライセンスであることを明記

## 成果物

`BOOTH_README.md`ファイルとして保存され、Booth販売ページにコピー＆ペーストして使用できる形式になっています。

## 今後の展開

- 製品イメージのプレースホルダーを実際のスクリーンショットに置き換える
- ユーザーからのフィードバックに基づいて内容を更新
- 具体的な使用例や事例を追加 

# 2025-06-24 表情が赤くなるライト機能のBaseレイヤー実装ログ

## 概要
表情が赤くなるライト機能をFXレイヤーからBaseレイヤーに変更し、より安定した動作を実現しました。これにより、FXレイヤーの制約を受けずにライトの制御が可能になります。

## 実装内容

### 1. ModularAvatarLightToggleMenuの拡張
ModularAvatarを使用した表情が赤くなるライト機能をBaseレイヤーで実装しました。

```csharp
[MenuItem("lilToon/PCSS Extension/ModularAvatar/表情赤くなるライトメニューを追加 (Base)")]
public static void AddRedFaceToggleMenu()
{
    // アバターのルートを取得
    var selected = Selection.activeGameObject;
    
    // パラメータ設定（Baseレイヤー用）
    param.parameters.Add(new ParameterConfig()
    {
        name = "RedFaceLight",
        defaultValue = 0,
        syncType = ParameterSyncType.Bool,
        saved = true
    });
    
    // メニュー設定
    var control = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionMenu.Control
    {
        name = "表情赤くなる",
        type = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionMenu.ControlType.Toggle,
        parameter = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.Parameter { name = "RedFaceLight" }
    };
    
    // ライトオブジェクト設定
    var toggle = redLightObj.GetComponent<ModularAvatarGameObjectToggle>();
    toggle.parameter = "RedFaceLight";
    toggle.isOn = false;
    toggle.layer = ModularAvatarGameObjectToggle.LayerType.Base; // Baseレイヤーで動作
}
```

### 2. 赤いライト生成機能の実装
頭部に自動的に赤いライトを生成する機能を追加しました。

```csharp
private static GameObject CreateRedLight(Transform root)
{
    // 頭部を探す
    Transform headTransform = FindHeadBone(root);
    
    // 赤いライトを作成
    GameObject redLightObj = new GameObject("RedFaceLight");
    redLightObj.transform.SetParent(headTransform);
    redLightObj.transform.localPosition = new Vector3(0, 0, 0.1f); // 顔の前方に配置
    
    Light redLight = redLightObj.AddComponent<Light>();
    redLight.type = LightType.Point;
    redLight.color = new Color(1f, 0.2f, 0.2f); // 赤色
    redLight.intensity = 0.8f;
    redLight.range = 0.5f;
    redLight.shadows = LightShadows.None; // 影なし（パフォーマンス対策）
    
    return redLightObj;
}
```

### 3. VRChatExpressionMenuCreatorの拡張
VRChat用のアニメーション・エクスプレッションメニューを作成する機能を拡張し、表情が赤くなるライト用の設定を追加しました。

```csharp
public static void CreateRedFaceExpressionMenu(Material targetMaterial)
{
    // 必ずBaseレイヤーを使用
    bool useBaseLayer = true;
    string parameterPrefix = "RedFace";
    
    // アニメーターコントローラー作成
    var animatorController = CreateRedFaceAnimatorController(targetMaterial, useBaseLayer, parameterPrefix);
    
    // アニメーション作成
    CreateRedFaceAnimations(targetMaterial, parameterPrefix);
}
```

### 4. 強度調整機能の実装
ライトの強度を3段階（弱・中・強）で調整できる機能を実装しました。

```csharp
private static void CreateRedFaceIntensityAnimation(string materialPath, string animationName, float intensity)
{
    // ライトの強度アニメーション
    var binding = new EditorCurveBinding
    {
        path = "RedFaceLight",
        type = typeof(Light),
        propertyName = "m_Intensity"
    };
    
    var curve = new AnimationCurve();
    curve.AddKey(0f, intensity);
    
    // ライトの色も調整（強度に応じて赤みを増す）
    var colorBindingG = new EditorCurveBinding
    {
        path = "RedFaceLight",
        type = typeof(Light),
        propertyName = "m_Color.g"
    };
    
    var colorCurveG = new AnimationCurve();
    colorCurveG.AddKey(0f, 0.2f - (intensity * 0.1f)); // 強度に応じて減少
}
```

## 変更点まとめ
1. 表情が赤くなるライト機能をFXレイヤーからBaseレイヤーに移行
2. ModularAvatarとの連携機能を強化
3. 自動的に頭部にライトを生成する機能を追加
4. ライトの強度調整機能を実装（3段階）
5. エディタ拡張メニューの追加

## 今後の展望
- さらに細かいライト調整機能の追加
- 複数のライトを使用した表現の強化
- 他のシェーダー機能との連携強化

以上の変更により、VRChatアバターの表情表現がより豊かになり、かつFXレイヤーの制約を受けずに安定して動作するようになりました。 