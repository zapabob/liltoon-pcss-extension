using UnityEditor;
using UnityEngine;
using System.Linq;

public class ShaderAutoConverter : AssetPostprocessor
{
    // モデルインポート後にマテリアルのシェーダーを自動変換
    void OnPostprocessModel(GameObject root)
    {
        // インポートされたモデルからすべてのRendererコンポーネントを取得
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.sharedMaterials)
            {
                if (material == null)
                {
                    Debug.LogWarning($"[ShaderAutoConverter] Material is null on renderer: {renderer.name}. Skipping shader conversion.");
                    continue;
                }
                
                // 現在のシェーダー名を取得
                string currentShaderName = material.shader.name;
                
                // 変換対象のシェーダーリスト
                string[] targetShaderNames = new string[]
                {
                    "Standard",
                    "Universal Render Pipeline/Lit",
                    // 他に自動変換したいシェーダー名があればここに追加
                };
                
                // lilToonまたはlilToon PCSS Extensionシェーダーを検索
                Shader lilToonShader = Shader.Find("lilToon");
                Shader lilToonPCSSShader = Shader.Find("lilToon/lilToon_PCSS"); // PCSS Extensionのシェーダー名に合わせて調整
                Shader targetNewShader = null;
                
                // 優先順位: PCSS Extension -> lilToon
                if (lilToonPCSSShader != null)
                {
                    targetNewShader = lilToonPCSSShader;
                }
                else if (lilToonShader != null)
                {
                    targetNewShader = lilToonShader;
                }
                
                // 変換対象のシェーダーであり、かつ新しいシェーダーが見つかった場合
                if (targetShaderNames.Contains(currentShaderName) && targetNewShader != null)
                {
                    // ユーザーに確認ダイアログを表示
                    if (EditorUtility.DisplayDialog(
                        "Shader Auto Converter",
                        $"モデル '{root.name}' のマテリアル '{material.name}' が '{currentShaderName}' シェーダーを使用しています。\n" +
                        $"これを '{targetNewShader.name}' シェーダーに自動変換しますか？",
                        "はい",
                        "いいえ"))
                    {
                        // シェーダーを変換
                        material.shader = targetNewShader;
                        Debug.Log($"[ShaderAutoConverter] Converted material '{material.name}' shader from '{currentShaderName}' to '{targetNewShader.name}'.");
                        
                        // 必要に応じて、基本的なプロパティをコピー（Main Color, Main Textureなど）
                        // ここでは基本的なプロパティのみをコピーします
                        // より詳細なプロパティのマッピングが必要な場合は、ここに追加ロジックを記述してください
                        if (material.HasProperty("_Color") && material.HasProperty("_MainTex"))
                        {
                            Color color = material.color;
                            Texture mainTex = material.mainTexture;
                            material.SetColor("_Color", color);
                            material.SetTexture("_MainTex", mainTex);
                        }
                    }
                    else
                    {
                        Debug.Log($"[ShaderAutoConverter] User declined shader conversion for material '{material.name}'.");
                    }
                }
                else if (targetShaderNames.Contains(currentShaderName) && targetNewShader == null)
                {
                    Debug.LogWarning($"[ShaderAutoConverter] Could not find 'lilToon' or 'lilToon/lilToon_PCSS' shader in the project. Material '{material.name}' shader '{currentShaderName}' was not converted.");
                }
            }
        }
    }
}
