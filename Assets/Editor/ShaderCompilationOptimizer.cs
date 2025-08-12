using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using lilToon;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// lilToon 2.1.4 シェーダーコンパイル最適化API
    /// 複数アバター同時ビルド時のシェーダーコンパイル統合機能
    /// </summary>
    public class ShaderCompilationOptimizer : EditorWindow
    {
        private bool enableOptimization = true;
        private bool consolidateShaderCompilations = true;
        private bool enablePerPixelCalculation = true;
        private bool enableDirectionAwareLighting = true;
        private int maxConcurrentCompilations = 4;
        private float compilationTimeout = 30.0f;

        private List<Material> materialsToOptimize = new List<Material>();
        private List<Shader> shadersToOptimize = new List<Shader>();

        [MenuItem("Tools/lilToon-PCSS-Extension/Shader Compilation Optimizer")]
        public static void ShowWindow()
        {
            ShaderCompilationOptimizer window = GetWindow<ShaderCompilationOptimizer>("Shader Compilation Optimizer");
            window.minSize = new Vector2(500, 400);
        }

        private void OnEnable()
        {
            // シーン内のlilToon PCSS Extensionマテリアルを自動検出
            ScanForMaterials();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("lilToon 2.1.4 Shader Compilation Optimizer", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // 基本設定
            EditorGUILayout.LabelField("Optimization Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            enableOptimization = EditorGUILayout.Toggle("Enable Optimization", enableOptimization);
            consolidateShaderCompilations = EditorGUILayout.Toggle("Consolidate Shader Compilations", consolidateShaderCompilations);
            enablePerPixelCalculation = EditorGUILayout.Toggle("Enable Per-Pixel Calculation", enablePerPixelCalculation);
            enableDirectionAwareLighting = EditorGUILayout.Toggle("Enable Direction-Aware Lighting", enableDirectionAwareLighting);

            EditorGUILayout.Space(10);

            // 詳細設定
            EditorGUILayout.LabelField("Advanced Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            maxConcurrentCompilations = EditorGUILayout.IntSlider("Max Concurrent Compilations", maxConcurrentCompilations, 1, 8);
            compilationTimeout = EditorGUILayout.Slider("Compilation Timeout (seconds)", compilationTimeout, 10.0f, 60.0f);

            EditorGUILayout.Space(10);

            // マテリアル一覧
            EditorGUILayout.LabelField("Materials to Optimize", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Scan Materials"))
            {
                ScanForMaterials();
            }
            if (GUILayout.Button("Clear List"))
            {
                materialsToOptimize.Clear();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // マテリアルリスト表示
            EditorGUILayout.LabelField($"Found {materialsToOptimize.Count} materials:");
            EditorGUI.indentLevel++;
            foreach (var material in materialsToOptimize)
            {
                if (material != null)
                {
                    EditorGUILayout.ObjectField(material.name, material, typeof(Material), false);
                }
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(10);

            // 最適化実行ボタン
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Optimize Shader Compilations"))
            {
                OptimizeShaderCompilations();
            }
            if (GUILayout.Button("Apply lilToon 2.1.4 Features"))
            {
                ApplyLilToon2Features();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // 情報表示
            EditorGUILayout.HelpBox("lilToon 2.1.4 Features:\n• Consolidated shader compilations for multiple avatars\n• Per-pixel VRC Light Volumes calculation\n• Direction-aware lighting\n• Enhanced rim light controls", MessageType.Info);
        }

        private void ScanForMaterials()
        {
            materialsToOptimize.Clear();
            shadersToOptimize.Clear();

            // シーン内のマテリアルを検索
            var renderers = FindObjectsOfType<Renderer>();
            foreach (var renderer in renderers)
            {
                if (renderer.sharedMaterials != null)
                {
                    foreach (var material in renderer.sharedMaterials)
                    {
                        if (material != null && material.shader != null)
                        {
                            if (material.shader.name.Contains("lilToon") || material.shader.name.Contains("PCSS"))
                            {
                                if (!materialsToOptimize.Contains(material))
                                {
                                    materialsToOptimize.Add(material);
                                }
                                if (!shadersToOptimize.Contains(material.shader))
                                {
                                    shadersToOptimize.Add(material.shader);
                                }
                            }
                        }
                    }
                }
            }

            // プロジェクト内のマテリアルも検索
            var guids = AssetDatabase.FindAssets("t:Material");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var material = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (material != null && material.shader != null)
                {
                    if (material.shader.name.Contains("lilToon") || material.shader.name.Contains("PCSS"))
                    {
                        if (!materialsToOptimize.Contains(material))
                        {
                            materialsToOptimize.Add(material);
                        }
                    }
                }
            }

            Debug.Log($"Found {materialsToOptimize.Count} materials and {shadersToOptimize.Count} shaders to optimize");
        }

        private void OptimizeShaderCompilations()
        {
            if (!enableOptimization)
            {
                Debug.LogWarning("Optimization is disabled");
                return;
            }

            EditorUtility.DisplayProgressBar("Shader Compilation Optimization", "Starting optimization...", 0.0f);

            try
            {
                int processedCount = 0;
                int totalCount = materialsToOptimize.Count;

                foreach (var material in materialsToOptimize)
                {
                    if (material == null) continue;

                    float progress = (float)processedCount / totalCount;
                    EditorUtility.DisplayProgressBar("Shader Compilation Optimization", 
                        $"Processing {material.name}...", progress);

                    // シェーダーコンパイル最適化を適用
                    OptimizeMaterialShader(material);

                    processedCount++;
                }

                // シェーダーコンパイル統合
                if (consolidateShaderCompilations)
                {
                    ConsolidateShaderCompilations();
                }

                EditorUtility.DisplayDialog("Optimization Complete", 
                    $"Successfully optimized {processedCount} materials.", "OK");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Shader compilation optimization failed: {e.Message}");
                EditorUtility.DisplayDialog("Optimization Failed", 
                    $"Error during optimization: {e.Message}", "OK");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private void OptimizeMaterialShader(Material material)
        {
            if (material == null || material.shader == null) return;

            Undo.RecordObject(material, "Optimize Shader Compilation");

            // lilToon 2.1.4の新機能を適用
            if (material.shader.name.Contains("lilToon"))
            {
                // ピクセル単位計算の有効化
                if (enablePerPixelCalculation)
                {
                    material.EnableKeyword("LIL_FEATURE_PERPIXEL_CALCULATION");
                }

                // 方向性を考慮したライティング
                if (enableDirectionAwareLighting)
                {
                    material.EnableKeyword("LIL_FEATURE_DIRECTION_AWARE_LIGHTING");
                }

                // VRC Light Volumes 2.0.0対応
                if (material.HasProperty("_UseVRCLightVolumes"))
                {
                    material.EnableKeyword("VRC_LIGHT_VOLUMES_ENABLED");
                    material.EnableKeyword("VRC_LIGHT_VOLUMES_MOBILE");
                }
            }

            EditorUtility.SetDirty(material);
        }

        private void ConsolidateShaderCompilations()
        {
            Debug.Log("Consolidating shader compilations for multiple avatars...");

            // シェーダーコンパイル統合APIの実装
            // これはlilToon 2.1.4の内部APIを利用する
            try
            {
                // リフレクションを使用してlilToonの内部APIにアクセス
                var lilToonAssembly = System.AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(asm => asm.GetName().Name.Contains("lilToon"));

                if (lilToonAssembly != null)
                {
                    var consolidationType = lilToonAssembly.GetTypes()
                        .FirstOrDefault(t => t.Name.Contains("Consolidation") || t.Name.Contains("Compilation"));

                    if (consolidationType != null)
                    {
                        var consolidateMethod = consolidationType.GetMethod("ConsolidateShaderCompilations", 
                            BindingFlags.Public | BindingFlags.Static);

                        if (consolidateMethod != null)
                        {
                            consolidateMethod.Invoke(null, new object[] { maxConcurrentCompilations, compilationTimeout });
                            Debug.Log("Shader compilation consolidation completed successfully");
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Could not access lilToon consolidation API: {e.Message}");
            }
        }

        private void ApplyLilToon2Features()
        {
            EditorUtility.DisplayProgressBar("Applying lilToon 2.1.4 Features", "Starting feature application...", 0.0f);

            try
            {
                int processedCount = 0;
                int totalCount = materialsToOptimize.Count;

                foreach (var material in materialsToOptimize)
                {
                    if (material == null) continue;

                    float progress = (float)processedCount / totalCount;
                    EditorUtility.DisplayProgressBar("Applying lilToon 2.1.4 Features", 
                        $"Processing {material.name}...", progress);

                    ApplyLilToon2FeaturesToMaterial(material);

                    processedCount++;
                }

                EditorUtility.DisplayDialog("Feature Application Complete", 
                    $"Successfully applied lilToon 2.1.4 features to {processedCount} materials.", "OK");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Feature application failed: {e.Message}");
                EditorUtility.DisplayDialog("Feature Application Failed", 
                    $"Error during feature application: {e.Message}", "OK");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private void ApplyLilToon2FeaturesToMaterial(Material material)
        {
            if (material == null || material.shader == null) return;

            Undo.RecordObject(material, "Apply lilToon 2.1.4 Features");

            // VRC Light Volumes 2.0.0対応
            if (material.HasProperty("_EnvRimBorder"))
            {
                material.SetFloat("_EnvRimBorder", 0.85f);
            }
            if (material.HasProperty("_EnvRimBlur"))
            {
                material.SetFloat("_EnvRimBlur", 0.35f);
            }

            // ピクセル単位計算の有効化
            if (enablePerPixelCalculation)
            {
                material.EnableKeyword("LIL_FEATURE_PERPIXEL_CALCULATION");
            }

            // 方向性を考慮したライティング
            if (enableDirectionAwareLighting)
            {
                material.EnableKeyword("LIL_FEATURE_DIRECTION_AWARE_LIGHTING");
            }

            EditorUtility.SetDirty(material);
        }
    }
} 