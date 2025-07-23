using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// lilToon PCSS Extension初期化クラス
    /// lilToonの読み込み後に自動的にPCSSエクスチE��ションを統合しまぁE
    /// </summary>
    [InitializeOnLoad]
    public class LilToonPCSSExtensionInitializer
    {
        private const string PCSS_EXTENSION_DEFINE = "LILTOON_PCSS_EXTENSION";
        private const string LILTOON_DEFINE = "LILTOON";
        private const string PCSS_EXTENSION_SHADER_NAME = "lilToon/PCSS Extension";
        
        static LilToonPCSSExtensionInitializer()
        {
            EditorApplication.delayCall += Initialize;
        }
        
        private static void Initialize()
        {
            CheckAndSetupExtension();
            RegisterShaderVariants();
            SetupMenuIntegration();
            SetupMaterialChangeCallback();
            // --- ダミ�E参�E: VRCLightVolumesIntegrationの自動削除防止 ---
            System.Type _ = typeof(lilToon.PCSS.VRCLightVolumesIntegration); // AutoFIX/最適化による削除防止
        }
        
        /// <summary>
        /// PCSS拡張の設定とシンボル定義を行いまぁE
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
            var pcssShader = Shader.Find(PCSS_EXTENSION_SHADER_NAME);
            if (pcssShader != null)
            {
                // 主要なキーワード組み合わせを追加
                string[] keywords = {
                    "_USEPCSS_ON",
                    "_USESHADOW_ON",
                    "_USEPCSS_ON _USESHADOW_ON",
                    "_USESHADOWCLAMP_ON",
                    "_USEVRCLIGHT_VOLUMES_ON",
                    "_USEPCSS_ON _USESHADOW_ON _USESHADOWCLAMP_ON",
                    "_USEPCSS_ON _USESHADOW_ON _USEVRCLIGHT_VOLUMES_ON"
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
        /// メニュー統合�E設宁E
        /// </summary>
        private static void SetupMenuIntegration()
        {
            // lilToonのメニューシスチE��と統吁E
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }
        
        /// <summary>
        /// マテリアル変更コールバックの設宁E
        /// </summary>
        private static void SetupMaterialChangeCallback()
        {
            // マテリアルのシェーダー変更時�Eコールバックを登録
            // Unity 2019以降ではMaterial.onBeforePropertyChangeが存在しなぁE��め、E
            // Editorのコールバックを使用
            Undo.postprocessModifications += OnPostprocessModifications;
        }
        
        /// <summary>
        /// マテリアルプロパティ変更時�Eコールバック
        /// </summary>
        private static UndoPropertyModification[] OnPostprocessModifications(UndoPropertyModification[] modifications)
        {
            foreach (var mod in modifications)
            {
                if (mod.currentValue != null && mod.currentValue.target is Material material)
                {
                    // シェーダープロパティの変更を検�E
                    if (material.shader != null && material.shader.name == PCSS_EXTENSION_SHADER_NAME)
                    {
                        // シェーダーが変更された場合、忁E��なプロパティを設宁E
                        EnsureRequiredProperties(material);
                        SetupShaderKeywords(material);
                    }
                }
            }
            return modifications;
        }
        
        /// <summary>
        /// マテリアルに忁E��なプロパティが存在することを確俁E
        /// </summary>
        public static void EnsureRequiredProperties(Material material)
        {
            // 基本プロパティの確俁E
            if (material.HasProperty("_MainTex") && material.GetTexture("_MainTex") == null)
                material.SetTexture("_MainTex", Texture2D.whiteTexture);
            
            if (material.HasProperty("_Color") && material.GetColor("_Color") == default)
                material.SetColor("_Color", Color.white);
            
            // シャドウ関連プロパティの確俁E
            if (material.HasProperty("_ShadowColorTex") && material.GetTexture("_ShadowColorTex") == null)
                material.SetTexture("_ShadowColorTex", Texture2D.blackTexture);
            
            // PCSS関連のチE��ォルト値設宁E
            if (material.HasProperty("_UsePCSS") && !material.HasProperty("_PCSSPresetMode"))
            {
                material.SetFloat("_PCSSPresetMode", 1.0f); // AnimeプリセチE��
                material.SetFloat("_LocalPCSSFilterRadius", 0.01f);
                material.SetFloat("_LocalPCSSLightSize", 0.1f);
                material.SetFloat("_PCSSBias", 0.001f);
                material.SetFloat("_PCSSIntensity", 1.0f);
                material.SetFloat("_PCSSQuality", 1.0f); // Medium
                material.SetFloat("_LocalPCSSSamples", 16.0f);
            }
        }
        
        /// <summary>
        /// シェーダーキーワードを設宁E
        /// </summary>
        public static void SetupShaderKeywords(Material material)
        {
            // PCSSキーワーチE
            SetKeyword(material, "_USEPCSS_ON", material.HasProperty("_UsePCSS") && material.GetFloat("_UsePCSS") > 0.5f);
            
            // シャドウキーワーチE
            SetKeyword(material, "_USESHADOW_ON", material.HasProperty("_UseShadow") && material.GetFloat("_UseShadow") > 0.5f);
            
            // シャドウクランプキーワーチE
            SetKeyword(material, "_USESHADOWCLAMP_ON", material.HasProperty("_UseShadowClamp") && material.GetFloat("_UseShadowClamp") > 0.5f);
            
            // VRCライト�EリュームキーワーチE
            SetKeyword(material, "_USEVRCLIGHT_VOLUMES_ON", material.HasProperty("_UseVRCLightVolumes") && material.GetFloat("_UseVRCLightVolumes") > 0.5f);
        }
        
        /// <summary>
        /// シェーダーキーワードを設定する�Eルパ�EメソチE��
        /// </summary>
        private static void SetKeyword(Material material, string keyword, bool state)
        {
            if (state)
                material.EnableKeyword(keyword);
            else
                material.DisableKeyword(keyword);
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
        /// オブジェクトがPCSSマテリアルを持ってぁE��かチェチE��
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
        /// プリプロセチE��シンボルを追加
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
        /// プリプロセチE��シンボルを削除
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
        /// PCSS拡張のメニューアイチE��
        /// </summary>
        [MenuItem("Tools/lilToon PCSS Extension/Utilities/PCSS Extension/About", false, 2000)]
        public static void ShowAbout()
        {
            EditorUtility.DisplayDialog(
                "lilToon PCSS Extension",
                "lilToon PCSS Extension v1.5.4\n\n" +
                "このプラグインはlilToonにPCSS（Percentage-Closer Soft Shadows）機能を追加します。\n" +
                "詳細な使用方法については、READMEファイルをご確認ください。",
                "OK"
            );
        }
        
        [MenuItem("Tools/lilToon PCSS Extension/Utilities/PCSS Extension/Open Documentation", false, 2001)]
        public static void OpenDocumentation()
        {
            string readmePath = AssetDatabase.FindAssets("liltoon_pcss_readme t:TextAsset")[0];
            if (!string.IsNullOrEmpty(readmePath))
            {
                string path = AssetDatabase.GUIDToAssetPath(readmePath);
                Application.OpenURL("file://" + Path.GetFullPath(path));
            }
        }
        
        [MenuItem("Tools/lilToon PCSS Extension/Utilities/PCSS Extension/Check Installation", false, 2002)]
        public static void CheckInstallation()
        {
            bool hasLilToon = IsLilToonInstalled();
            bool hasPCSSShader = Shader.Find(PCSS_EXTENSION_SHADER_NAME) != null;
            
            string message = "インストール状況:\n\n";
            message += $"lilToon: {(hasLilToon ? "✅ インストール済み" : "❌ 未インストール")}\n";
            message += $"PCSS Extension: {(hasPCSSShader ? "✅ インストール済み" : "❌ 未インストール")}\n";
            
            if (hasLilToon && hasPCSSShader)
            {
                message += "\n✅ すべて正常にインストールされています！";
            }
            else
            {
                message += "\n❌ インストールに問題があります。";
                if (!hasLilToon) message += "\n- lilToonをインストールしてください。";
                if (!hasPCSSShader) message += "\n- PCSS Extensionシェーダーが見つかりません。";
            }
            
            EditorUtility.DisplayDialog("PCSS Extension インストール状況", message, "OK");
        }
        
        [MenuItem("Tools/lilToon PCSS Extension/Utilities/PCSS Extension/Fix Materials", false, 2003)]
        public static void FixMaterials()
        {
            // シーン内のすべてのマテリアルを修復
            var renderers = GameObject.FindObjectsOfType<Renderer>();
            List<Material> fixedMaterials = new List<Material>();
            Shader pcssShader = Shader.Find(PCSS_EXTENSION_SHADER_NAME);

            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material == null) continue;
                    bool needsFix = false;
                    if (material.shader == null)
                    {
                        needsFix = true;
                    }
                    else if (material.shader.name != PCSS_EXTENSION_SHADER_NAME)
                    {
                        if (material.shader.name.Contains("lilToon"))
                            needsFix = true;
                    }
                    else if (material.shader.name == PCSS_EXTENSION_SHADER_NAME)
                    {
                        needsFix = true;
                    }
                    if (needsFix && pcssShader != null)
                    {
                        material.shader = pcssShader;
                        EnsureRequiredProperties(material);
                        SetupShaderKeywords(material);
                        if (!fixedMaterials.Contains(material))
                            fixedMaterials.Add(material);
                        EditorUtility.SetDirty(material);
                        // --- Prefabインスタンス対応---
                        var go = renderer.gameObject;
                        if (PrefabUtility.IsPartOfPrefabInstance(go))
                        {
                            PrefabUtility.RecordPrefabInstancePropertyModifications(renderer);
                        }
                    }
                }
            }

            // プロジェクト内のマテリアルも修復
            string[] materialGuids = AssetDatabase.FindAssets("t:Material");
            foreach (string guid in materialGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (material == null) continue;
                bool needsFix = false;
                if (material.shader == null)
                {
                    needsFix = true;
                }
                else if (material.shader.name != PCSS_EXTENSION_SHADER_NAME)
                {
                    if (material.shader.name.Contains("lilToon"))
                        needsFix = true;
                }
                else if (material.shader.name == PCSS_EXTENSION_SHADER_NAME)
                {
                    needsFix = true;
                }
                if (needsFix && pcssShader != null)
                {
                    material.shader = pcssShader;
                    EnsureRequiredProperties(material);
                    SetupShaderKeywords(material);
                    if (!fixedMaterials.Contains(material))
                        fixedMaterials.Add(material);
                    EditorUtility.SetDirty(material);
                }
            }

            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("PCSS Extension マテリアル修復", $"{fixedMaterials.Count}個�Eマテリアルを�E動修復しました、En(シェーダーmissingめE�Eロパティ欠損も自動復旧)", "OK");
        }
    }
}
#endif 
