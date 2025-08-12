# LOD (Level of Detail) システム設計書

## 1. 目的

アバターとカメラの距離に応じてPCSSの品質を動的に変更することで、視覚的な品質を維持しつつ、不要な計算負荷を削減し、パフォーマンスを最適化する。

## 2. 対象コンポーネント

`Assets/Runtime/ModularAvatarPCSSController.cs`

## 3. 追加するプロパティ

`ModularAvatarPCSSController.cs` に、以下のpublicなプロパティを追加する。

```csharp
// LOD設定を有効にするか
public bool EnableLOD = true;

// LODレベルを定義する構造体
[System.Serializable]
public struct LODLevel
{
    public float Distance;      // このLODレベルが適用される最大距離
    public PCSSQuality Quality; // 適用するPCSS品質 (PCSSUtilities.PCSSQuality enumを使用)
}

// ユーザーが設定可能なLODレベルの配列
public LODLevel[] lodLevels = new LODLevel[]
{
    new LODLevel { Distance = 10.0f, Quality = PCSSQuality.Ultra },
    new LODLevel { Distance = 20.0f, Quality = PCSSQuality.High },
    new LODLevel { Distance = 40.0f, Quality = PCSSQuality.Medium },
    new LODLevel { Distance = 60.0f, Quality = PCSSQuality.Low },
};
```

## 4. 実装方針

1.  **メインカメラの取得:**
    *   `Start()` メソッドで `Camera.main` をキャッシュする。パフォーマンスのため、`Update()` 内での毎回の取得は避ける。

2.  **距離の計算:**
    *   `Update()` メソッド内で、アバターのトランスフォーム (`transform.position`) とメインカメラのトランスフォーム (`Camera.main.transform.position`) との距離を計算する。

3.  **LODレベルの決定:**
    *   計算した距離に基づき、`lodLevels` 配列を走査して、現在適用すべき `PCSSQuality` を決定する。
    *   `lodLevels` 配列は `Distance` の昇順でソートされていることを前提とする。
    *   どの距離範囲にも一致しない場合（最も遠いLODレベルよりさらに遠い場合）は、PCSSを完全にオフにする（サンプル数を0にするなど）。

4.  **シェーダーへの適用:**
    *   決定した `PCSSQuality` に基づき、`PCSSUtilities.GetQualityParameters()` を使用して、具体的なパラメータ（サンプル数など）を取得する。
    *   取得したパラメータを `MaterialPropertyBlock` を使用して、アバターのマテリアルに適用する。
    *   この処理は、`EnableLOD` が `true` の場合のみ実行する。

## 5. 処理フロー (Updateメソッド内)

```
if (!EnableLOD) return;

// 1. カメラとの距離を計算
float distance = Vector3.Distance(transform.position, mainCamera.transform.position);

// 2. 適用すべき品質レベルを決定
PCSSQuality targetQuality = PCSSQuality.Low; // デフォルトは最低品質
bool qualitySet = false;
foreach (var lod in lodLevels)
{
    if (distance < lod.Distance)
    {
        targetQuality = lod.Quality;
        qualitySet = true;
        break;
    }
}

// 3. 品質が設定されなかった（最も遠い）場合、PCSSをオフにする
if (!qualitySet)
{
    // PCSSをオフにする処理（例: サンプル数を0に設定）
    propertyBlock.SetFloat("_LocalPCSSSamples", 0);
}
else
{
    // 4. 品質パラメータを取得して適用
    Vector3 qualityParams = PCSSUtilities.GetQualityParameters(targetQuality);
    propertyBlock.SetFloat("_LocalPCSSSamples", qualityParams.x);
    // 他の品質関連パラメータも同様に設定...
}

// 5. MaterialPropertyBlockをレンダラーに適用
foreach (var renderer in renderers)
{
    renderer.SetPropertyBlock(propertyBlock);
}
```

## 6. 考慮事項

- `lodLevels` 配列がユーザーによってソートされていない場合も考慮し、`Start()` メソッドで距離順にソートする処理を追加するのが望ましい。
- パフォーマンスへの影響を最小限にするため、`Update()` 内での処理は可能な限り軽量に保つ。
