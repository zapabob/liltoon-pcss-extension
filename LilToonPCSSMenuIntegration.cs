using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
namespace lilToon.PCSS
{
    public class LilToonPCSSMenuIntegration : EditorWindow
    {
        private GameObject selectedAvatar;
        private Vector2 scrollPosition;
        private bool showAdvancedSettings = false;
        private bool autoDetectMaterials = true;
        private bool preserveOriginalMaterials = true;
        private bool createBackup = true;
        
        // PCSS設定
        private float blockerSearchRadius = 0.01f;
        private float filterRadius = 0.02f;
        private int sampleCount = 16;
        private float lightSize = 1.5f;
        
        // 品質プリセット
        private enum QualityPreset
        {
            Light = 0,
            Balanced = 1,
            High = 2,
            Custom = 3
        }
        private QualityPreset selectedPreset = QualityPreset.Balanced;
        
        [MenuItem("lilToon/PCSS Avatar Setup", false, 1000)]
        public static void ShowWindow()
        {
            var window = GetWindow<LilToonPCSSMenuIntegration>("lilToon PCSS Setup");
            window.minSize = new Vector2(400, 600);
            window.Show();
        }
        
        [MenuItem("GameObject/lilToon PCSS/Apply to Selected Avatar", false, 0)]
        public static void ApplyToSelectedAvatar()
        {
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject != null)
            {
                ApplyPCSSToAvatar(selectedObject);
            }
            else
            {
                EditorUtility.DisplayDialog("エラー", "アバターを選択してください。", "OK");
            }
        }
        
        [MenuItem("GameObject/lilToon PCSS/Apply to Selected Avatar", true)]
        public static bool ValidateApplyToSelectedAvatar()
        {
            return Selection.activeGameObject != null;
        }
        
        void OnGUI()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("lilToon PCSS Avatar Setup", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // アバター選択
            EditorGUILayout.LabelField("アバター選択", EditorStyles.boldLabel);
            selectedAvatar = (GameObject)EditorGUILayout.ObjectField("Target Avatar", selectedAvatar, typeof(GameObject), true);
            
            if (selectedAvatar == null)
            {
                EditorGUILayout.HelpBox("アバターのGameObjectを選択してください。", MessageType.Info);
                EditorGUILayout.EndVertical();
                return;
            }
            
            EditorGUILayout.Space();
            
            // ModularAvatar検出
            bool hasModularAvatar = HasModularAvatarComponents(selectedAvatar);
            if (hasModularAvatar)
            {
                EditorGUILayout.HelpBox("✓ ModularAvatarコンポーネントが検出されました。", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("ModularAvatarコンポーネントが見つかりません。通常のアバターとして処理します。", MessageType.Warning);
            }
            
            EditorGUILayout.Space();
            
            // 品質プリセット選択
            EditorGUILayout.LabelField("品質プリセット", EditorStyles.boldLabel);
            selectedPreset = (QualityPreset)EditorGUILayout.EnumPopup("Quality Preset", selectedPreset);
            
            ApplyQualityPreset();
            
            EditorGUILayout.Space();
            
            // 基本設定
            EditorGUILayout.LabelField("基本設定", EditorStyles.boldLabel);
            autoDetectMaterials = EditorGUILayout.Toggle("自動マテリアル検出", autoDetectMaterials);
            preserveOriginalMaterials = EditorGUILayout.Toggle("元のマテリアルを保持", preserveOriginalMaterials);
            createBackup = EditorGUILayout.Toggle("バックアップを作成", createBackup);
            
            EditorGUILayout.Space();
            
            // 詳細設定
            showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "詳細設定");
            if (showAdvancedSettings)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.LabelField("PCSS パラメータ", EditorStyles.boldLabel);
                blockerSearchRadius = EditorGUILayout.Slider("Blocker Search Radius", blockerSearchRadius, 0.001f, 0.1f);
                filterRadius = EditorGUILayout.Slider("Filter Radius", filterRadius, 0.001f, 0.1f);
                sampleCount = EditorGUILayout.IntSlider("Sample Count", sampleCount, 4, 64);
                lightSize = EditorGUILayout.Slider("Light Size", lightSize, 0.1f, 10.0f);
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();
            
            // マテリアル情報表示
            ShowMaterialInfo();
            
            EditorGUILayout.Space();
            
            // 適用ボタン
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("PCSS効果を適用", GUILayout.Height(40)))
            {
                ApplyPCSSToAvatar(selectedAvatar);
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.Space();
            
            // リセットボタン
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("元に戻す", GUILayout.Height(30)))
            {
                RevertPCSSFromAvatar(selectedAvatar);
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.EndVertical();
        }
        
        private void ApplyQualityPreset()
        {
            if (selectedPreset == QualityPreset.Custom) return;
            
            switch (selectedPreset)
            {
                case QualityPreset.Light:
                    blockerSearchRadius = 0.005f;
                    filterRadius = 0.01f;
                    sampleCount = 8;
                    lightSize = 1.0f;
                    break;
                case QualityPreset.Balanced:
                    blockerSearchRadius = 0.01f;
                    filterRadius = 0.02f;
                    sampleCount = 16;
                    lightSize = 1.5f;
                    break;
                case QualityPreset.High:
                    blockerSearchRadius = 0.02f;
                    filterRadius = 0.03f;
                    sampleCount = 32;
                    lightSize = 2.0f;
                    break;
            }
        }
        
        private void ShowMaterialInfo()
        {
            if (selectedAvatar == null) return;
            
            var renderers = selectedAvatar.GetComponentsInChildren<Renderer>();
            var lilToonMaterials = new List<Material>();
            
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material != null && IsLilToonMaterial(material))
                    {
                        lilToonMaterials.Add(material);
                    }
                }
            }
            
            EditorGUILayout.LabelField("マテリアル情報", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"lilToonマテリアル数: {lilToonMaterials.Count}");
            
            if (lilToonMaterials.Count > 0)
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(100));
                foreach (var material in lilToonMaterials)
                {
                    EditorGUILayout.LabelField($"• {material.name}");
                }
                EditorGUILayout.EndScrollView();
            }
        }
        
        private static bool HasModularAvatarComponents(GameObject avatar)
        {
            // ModularAvatarのコンポーネントを検出
            var components = avatar.GetComponentsInChildren<Component>();
            return components.Any(c => c != null && c.GetType().Namespace != null && 
                                 c.GetType().Namespace.Contains("nadena.dev.modular_avatar"));
        }
        
        private static bool IsLilToonMaterial(Material material)
        {
            if (material == null || material.shader == null) return false;
            return material.shader.name.Contains("lilToon") || material.shader.name.Contains("lil_");
        }
        
        public static void ApplyPCSSToAvatar(GameObject avatar)
        {
            if (avatar == null)
            {
                EditorUtility.DisplayDialog("エラー", "アバターが選択されていません。", "OK");
                return;
            }
            
            var window = GetWindow<LilToonPCSSMenuIntegration>();
            
            try
            {
                EditorUtility.DisplayProgressBar("PCSS適用中", "マテリアルを処理中...", 0.0f);
                
                var renderers = avatar.GetComponentsInChildren<Renderer>();
                var processedMaterials = new List<Material>();
                int totalMaterials = 0;
                int processedCount = 0;
                
                // 総数をカウント
                foreach (var renderer in renderers)
                {
                    totalMaterials += renderer.sharedMaterials.Length;
                }
                
                foreach (var renderer in renderers)
                {
                    var materials = renderer.sharedMaterials;
                    var newMaterials = new Material[materials.Length];
                    
                    for (int i = 0; i < materials.Length; i++)
                    {
                        var material = materials[i];
                        processedCount++;
                        
                        EditorUtility.DisplayProgressBar("PCSS適用中", 
                            $"マテリアル処理中: {material?.name ?? "null"} ({processedCount}/{totalMaterials})", 
                            (float)processedCount / totalMaterials);
                        
                        if (material != null && IsLilToonMaterial(material))
                        {
                            var newMaterial = ConvertToPCSSMaterial(material, window);
                            newMaterials[i] = newMaterial;
                            processedMaterials.Add(newMaterial);
                        }
                        else
                        {
                            newMaterials[i] = material;
                        }
                    }
                    
                    renderer.sharedMaterials = newMaterials;
                }
                
                // ModularAvatarの設定を更新
                UpdateModularAvatarSettings(avatar);
                
                // ModularAvatar統合メニューを追加
                if (HasModularAvatarComponents(avatar))
                {
                    ModularAvatarPCSSIntegration.AddPCSSControlMenu(avatar);
                }
                
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("完了", 
                    $"PCSS効果の適用が完了しました。\n処理されたマテリアル数: {processedMaterials.Count}\n" +
                    (HasModularAvatarComponents(avatar) ? "ModularAvatar制御メニューも追加されました。" : ""), "OK");
                
                // シーンを保存するか確認
                if (EditorUtility.DisplayDialog("保存確認", "シーンを保存しますか？", "保存", "後で"))
                {
                    UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
                }
            }
            catch (System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("エラー", $"PCSS適用中にエラーが発生しました:\n{e.Message}", "OK");
                Debug.LogError($"PCSS適用エラー: {e}");
            }
        }
        
        private static Material ConvertToPCSSMaterial(Material originalMaterial, LilToonPCSSMenuIntegration window)
        {
            // PCSS対応シェーダーを取得
            Shader pcssShader = Shader.Find("lilToon/PCSS Extension");
            if (pcssShader == null)
            {
                pcssShader = Shader.Find("lilToon/PCSS Extension URP");
            }
            
            if (pcssShader == null)
            {
                Debug.LogError("PCSS拡張シェーダーが見つかりません。");
                return originalMaterial;
            }
            
            // 新しいマテリアルを作成
            var newMaterial = new Material(pcssShader);
            newMaterial.name = originalMaterial.name + "_PCSS";
            
            // 基本プロパティをコピー
            CopyMaterialProperties(originalMaterial, newMaterial);
            
            // PCSS設定を適用
            newMaterial.SetFloat("_UsePCSS", 1.0f);
            newMaterial.SetFloat("_PCSSBlockerSearchRadius", window.blockerSearchRadius);
            newMaterial.SetFloat("_PCSSFilterRadius", window.filterRadius);
            newMaterial.SetFloat("_PCSSSampleCount", window.sampleCount);
            newMaterial.SetFloat("_PCSSLightSize", window.lightSize);
            newMaterial.SetFloat("_UseShadow", 1.0f);
            
            // キーワードを設定
            newMaterial.EnableKeyword("_USEPCSS_ON");
            newMaterial.EnableKeyword("_USESHADOW_ON");
            
            // マテリアルを保存
            string path = $"Assets/Materials/PCSS/{newMaterial.name}.mat";
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            AssetDatabase.CreateAsset(newMaterial, path);
            
            return newMaterial;
        }
        
        private static void CopyMaterialProperties(Material source, Material destination)
        {
            // 共通プロパティをコピー
            var commonProperties = new string[]
            {
                "_Color", "_MainTex", "_MainTex_ST",
                "_ShadowBorder", "_ShadowBlur", "_ShadowColorTex",
                "_Cutoff", "_Cull", "_SrcBlend", "_DstBlend", "_ZTest", "_ZWrite"
            };
            
            foreach (var property in commonProperties)
            {
                if (source.HasProperty(property) && destination.HasProperty(property))
                {
                    try
                    {
                        if (source.HasProperty(property))
                        {
                            var sourceShader = source.shader;
                            int propertyIndex = sourceShader.FindPropertyIndex(property);
                            if (propertyIndex >= 0)
                            {
                                var type = sourceShader.GetPropertyType(propertyIndex);
                                switch (type)
                                {
                                    case UnityEngine.Rendering.ShaderPropertyType.Color:
                                        destination.SetColor(property, source.GetColor(property));
                                        break;
                                    case UnityEngine.Rendering.ShaderPropertyType.Vector:
                                        destination.SetVector(property, source.GetVector(property));
                                        break;
                                    case UnityEngine.Rendering.ShaderPropertyType.Float:
                                    case UnityEngine.Rendering.ShaderPropertyType.Range:
                                        destination.SetFloat(property, source.GetFloat(property));
                                        break;
                                    case UnityEngine.Rendering.ShaderPropertyType.Texture:
                                        destination.SetTexture(property, source.GetTexture(property));
                                        break;
                                }
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning($"プロパティ {property} のコピーに失敗: {e.Message}");
                    }
                }
            }
        }
        
        private static void UpdateModularAvatarSettings(GameObject avatar)
        {
            // ModularAvatarの設定を更新（必要に応じて）
            var modularAvatarComponents = avatar.GetComponentsInChildren<Component>()
                .Where(c => c != null && c.GetType().Namespace != null && 
                           c.GetType().Namespace.Contains("nadena.dev.modular_avatar"));
            
            if (modularAvatarComponents.Any())
            {
                Debug.Log("ModularAvatarコンポーネントが検出されました。設定を更新中...");
                // 必要に応じてModularAvatarの設定を更新
            }
        }
        
        public static void RevertPCSSFromAvatar(GameObject avatar)
        {
            if (avatar == null) return;
            
            if (!EditorUtility.DisplayDialog("確認", "PCSS効果を元に戻しますか？\n（バックアップがある場合のみ復元可能）", "実行", "キャンセル"))
            {
                return;
            }
            
            try
            {
                var renderers = avatar.GetComponentsInChildren<Renderer>();
                int revertedCount = 0;
                
                foreach (var renderer in renderers)
                {
                    var materials = renderer.sharedMaterials;
                    var newMaterials = new Material[materials.Length];
                    
                    for (int i = 0; i < materials.Length; i++)
                    {
                        var material = materials[i];
                        if (material != null && material.name.Contains("_PCSS"))
                        {
                            // 元のマテリアルを探す
                            string originalName = material.name.Replace("_PCSS", "");
                            var originalMaterial = AssetDatabase.LoadAssetAtPath<Material>($"Assets/Materials/{originalName}.mat");
                            
                            if (originalMaterial != null)
                            {
                                newMaterials[i] = originalMaterial;
                                revertedCount++;
                            }
                            else
                            {
                                newMaterials[i] = material;
                            }
                        }
                        else
                        {
                            newMaterials[i] = material;
                        }
                    }
                    
                    renderer.sharedMaterials = newMaterials;
                }
                
                EditorUtility.DisplayDialog("完了", $"PCSS効果の復元が完了しました。\n復元されたマテリアル数: {revertedCount}", "OK");
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("エラー", $"復元中にエラーが発生しました:\n{e.Message}", "OK");
                Debug.LogError($"PCSS復元エラー: {e}");
            }
        }
    }
}
#endif 