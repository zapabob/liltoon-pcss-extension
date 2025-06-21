using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
namespace lilToon.PCSS
{
    /// <summary>
    /// lilToon PCSS Extension初期化クラス
    /// lilToonの読み込み後に自動的にPCSSエクステンションを統合します
    /// </summary>
    [InitializeOnLoad]
    public class LilToonPCSSExtensionInitializer
    {
        private const string PCSS_EXTENSION_DEFINE = "LILTOON_PCSS_EXTENSION";
        private const string LILTOON_DEFINE = "LILTOON";
        
        static LilToonPCSSExtensionInitializer()
        {
            EditorApplication.delayCall += Initialize;
        }
        
        private static void Initialize()
        {
            CheckAndSetupExtension();
            RegisterShaderVariants();
            SetupMenuIntegration();
        }
        
        /// <summary>
        /// PCSS拡張の設定とシンボル定義を行います
        /// </summary>
        private static void CheckAndSetupExtension()
        {
            // lilToonが存在するかチェック
            bool hasLilToon = IsLilToonInstalled();
            
            if (hasLilToon)
            {
                AddDefineSymbol(PCSS_EXTENSION_DEFINE);
                Debug.Log("[lilToon PCSS Extension] lilToonが検出されました。PCSS拡張が有効化されました。");
            }
            else
            {
                RemoveDefineSymbol(PCSS_EXTENSION_DEFINE);
                Debug.LogWarning("[lilToon PCSS Extension] lilToonが見つかりません。先にlilToonをインストールしてください。");
            }
        }
        
        /// <summary>
        /// lilToonがインストールされているかチェック
        /// </summary>
        private static bool IsLilToonInstalled()
        {
            // lilToonのシェーダーファイルを検索
            string[] shaderGuids = AssetDatabase.FindAssets("t:Shader", new[] { "Assets" });
            
            foreach (string guid in shaderGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("lilToon") && path.Contains(".shader"))
                {
                    Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
                    if (shader != null && shader.name.StartsWith("lilToon/"))
                    {
                        return true;
                    }
                }
            }
            
            // UPMパッケージとしてのlilToonもチェック
            var packagePath = "Packages/com.lilxyzw.liltoon";
            if (Directory.Exists(packagePath))
            {
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// シェーダーバリアントを登録
        /// </summary>
        private static void RegisterShaderVariants()
        {
            // PCSSシェーダーバリアントを事前コンパイルリストに追加
            var shaderVariantCollection = Resources.Load<ShaderVariantCollection>("PCSSShaderVariants");
            if (shaderVariantCollection == null)
            {
                CreateShaderVariantCollection();
            }
        }
        
        /// <summary>
        /// シェーダーバリアントコレクションを作成
        /// </summary>
        private static void CreateShaderVariantCollection()
        {
            var collection = new ShaderVariantCollection();
            
            // PCSSシェーダーのバリアントを追加
            var pcssShader = Shader.Find("lilToon/PCSS Extension");
            if (pcssShader != null)
            {
                // 主要なキーワード組み合わせを追加
                string[] keywords = {
                    "_USEPCSS_ON",
                    "_USESHADOW_ON",
                    "_USEPCSS_ON _USESHADOW_ON"
                };
                
                foreach (string keyword in keywords)
                {
                    var variant = new ShaderVariantCollection.ShaderVariant(
                        pcssShader, 
                        UnityEngine.Rendering.PassType.ForwardBase, 
                        keyword.Split(' ')
                    );
                    collection.Add(variant);
                }
            }
            
            // Resourcesフォルダに保存
            string resourcesPath = "Assets/Resources";
            if (!Directory.Exists(resourcesPath))
            {
                Directory.CreateDirectory(resourcesPath);
            }
            
            AssetDatabase.CreateAsset(collection, "Assets/Resources/PCSSShaderVariants.shadervariants");
            AssetDatabase.SaveAssets();
        }
        
        /// <summary>
        /// メニュー統合の設定
        /// </summary>
        private static void SetupMenuIntegration()
        {
            // lilToonのメニューシステムと統合
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }
        
        /// <summary>
        /// ヒエラルキーウィンドウでのGUI表示
        /// </summary>
        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (obj == null) return;
            
            // アバターオブジェクトにPCSSアイコンを表示
            if (HasPCSSMaterials(obj))
            {
                Rect iconRect = new Rect(selectionRect.xMax - 16, selectionRect.y, 16, 16);
                GUI.Label(iconRect, "⚡", new GUIStyle { fontSize = 12, normal = { textColor = Color.yellow } });
            }
        }
        
        /// <summary>
        /// オブジェクトがPCSSマテリアルを持っているかチェック
        /// </summary>
        private static bool HasPCSSMaterials(GameObject obj)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material != null && material.shader != null && 
                        material.shader.name.Contains("lilToon/PCSS"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        /// <summary>
        /// プリプロセッサシンボルを追加
        /// </summary>
        private static void AddDefineSymbol(string symbol)
        {
            var target = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
            
            if (!defines.Contains(symbol))
            {
                defines += ";" + symbol;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(target, defines);
            }
        }
        
        /// <summary>
        /// プリプロセッサシンボルを削除
        /// </summary>
        private static void RemoveDefineSymbol(string symbol)
        {
            var target = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
            
            if (defines.Contains(symbol))
            {
                defines = defines.Replace(";" + symbol, "").Replace(symbol, "");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(target, defines);
            }
        }
        
        /// <summary>
        /// PCSS拡張のメニューアイテム
        /// </summary>
        [MenuItem("lilToon/PCSS Extension/About", false, 2000)]
        public static void ShowAbout()
        {
            EditorUtility.DisplayDialog(
                "lilToon PCSS Extension",
                "lilToon PCSS Extension v1.0.0\n\n" +
                "このプラグインはlilToonにPCSS（Percentage-Closer Soft Shadows）機能を追加します。\n\n" +
                "詳細な使用方法については、READMEファイルをご確認ください。",
                "OK"
            );
        }
        
        [MenuItem("lilToon/PCSS Extension/Open Documentation", false, 2001)]
        public static void OpenDocumentation()
        {
            string readmePath = AssetDatabase.FindAssets("liltoon_pcss_readme t:TextAsset")[0];
            if (!string.IsNullOrEmpty(readmePath))
            {
                string path = AssetDatabase.GUIDToAssetPath(readmePath);
                Application.OpenURL("file://" + Path.GetFullPath(path));
            }
        }
        
        [MenuItem("lilToon/PCSS Extension/Check Installation", false, 2002)]
        public static void CheckInstallation()
        {
            bool hasLilToon = IsLilToonInstalled();
            bool hasPCSSShader = Shader.Find("lilToon/PCSS Extension") != null;
            
            string message = "インストール状況:\n\n";
            message += $"lilToon: {(hasLilToon ? "✓ インストール済み" : "✗ 未インストール")}\n";
            message += $"PCSS Extension: {(hasPCSSShader ? "✓ インストール済み" : "✗ 未インストール")}\n";
            
            if (hasLilToon && hasPCSSShader)
            {
                message += "\n✓ すべて正常にインストールされています！";
            }
            else
            {
                message += "\n⚠ インストールに問題があります。";
            }
            
            EditorUtility.DisplayDialog("インストール確認", message, "OK");
        }
    }
}
#endif 