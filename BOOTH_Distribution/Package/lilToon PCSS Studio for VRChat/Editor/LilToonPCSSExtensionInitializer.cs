using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// Initializes editor support for the lilToon PCSS extension.
    /// </summary>
    [InitializeOnLoad]
    public class LilToonPCSSExtensionInitializer
    {
        private const string PCSS_EXTENSION_DEFINE = "LILTOON_PCSS_EXTENSION";
        private const string LILTOON_DEFINE = "LILTOON";
        private const string PCSS_EXTENSION_SHADER_NAME = "lilToon/PCSS Extension";
        
        static LilToonPCSSExtensionInitializer()
        {
            try { RepairLilToonEmptySettingsJson(); } catch { /* best-effort */ }
            EditorApplication.delayCall += Initialize;
        }
        
        private static void Initialize()
        {
            CheckAndSetupExtension();
            RegisterShaderVariants();
            SetupMenuIntegration();
            SetupMaterialChangeCallback();
#if PCSS_DEV
            System.Type _ = typeof(lilToon.PCSS.VRCLightVolumesIntegration);
#endif
        }
        
        private static void CheckAndSetupExtension()
        {
            bool hasLilToon = IsLilToonInstalled();
            
            if (hasLilToon)
            {
                AddDefineSymbol(PCSS_EXTENSION_DEFINE);
                Debug.Log("[lilToon PCSS Extension] lilToon detected. PCSS extension support is enabled.");
            }
            else
            {
                RemoveDefineSymbol(PCSS_EXTENSION_DEFINE);
                Debug.LogWarning("[lilToon PCSS Extension] lilToon was not found. Install lilToon before using this extension.");
            }
        }
        
        private static bool IsLilToonInstalled()
        {
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
            
            var packagePaths = new[] { "Packages/jp.lilxyzw.liltoon", "Packages/com.lilxyzw.liltoon" };
            foreach (var p in packagePaths)
            {
                if (Directory.Exists(p)) return true;
            }
            
            return false;
        }

        private static void RepairLilToonEmptySettingsJson()
        {
            var candidateRoots = new List<string> { "Packages/jp.lilxyzw.liltoon", "Packages/com.lilxyzw.liltoon" };
            candidateRoots.Add("ProjectSettings");
            candidateRoots.Add("Assets");
            foreach (var root in candidateRoots)
            {
                if (!Directory.Exists(root)) continue;
                string[] jsons;
                try { jsons = Directory.GetFiles(root, "*.json", SearchOption.AllDirectories); }
                catch { continue; }
                foreach (var path in jsons)
                {
                    var file = Path.GetFileName(path).ToLowerInvariant();
                    if (!(file.Contains("lil") && (file.Contains("setting") || file.Contains("config")))) continue;
                    try
                    {
                        var text = File.ReadAllText(path);
                        if (string.IsNullOrWhiteSpace(text))
                        {
                            File.WriteAllText(path, "{}");
                            Debug.Log($"[lilToon PCSS] Repaired empty lilToon settings JSON: {path}");
                        }
                        else
                        {
                            var trimmed = text.Trim();
                            if (trimmed == "[]")
                            {
                                File.WriteAllText(path, "{}");
                                Debug.Log($"[lilToon PCSS] Normalized [] to {{}} for: {path}");
                            }
                        }
                    }
                    catch { /* Ignore read-only or locked files. */ }
                }
            }
        }
        
        private static void RegisterShaderVariants()
        {
            var shaderVariantCollection = Resources.Load<ShaderVariantCollection>("PCSSShaderVariants");
            if (shaderVariantCollection == null)
            {
                CreateShaderVariantCollection();
            }
        }
        
        private static void CreateShaderVariantCollection()
        {
            var collection = new ShaderVariantCollection();
            
            var pcssShader = Shader.Find(PCSS_EXTENSION_SHADER_NAME);
            if (pcssShader != null)
            {
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
            
            string resourcesPath = "Assets/Resources";
            if (!Directory.Exists(resourcesPath))
            {
                Directory.CreateDirectory(resourcesPath);
            }
            
            AssetDatabase.CreateAsset(collection, "Assets/Resources/PCSSShaderVariants.shadervariants");
            AssetDatabase.SaveAssets();
        }
        
        /// <summary>
        /// Sets up editor menu integration.
        /// </summary>
        private static void SetupMenuIntegration()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }
        
        private static void SetupMaterialChangeCallback()
        {
            Undo.postprocessModifications += OnPostprocessModifications;
        }
        
        private static UndoPropertyModification[] OnPostprocessModifications(UndoPropertyModification[] modifications)
        {
            foreach (var mod in modifications)
            {
                if (mod.currentValue != null && mod.currentValue.target is Material material)
                {
                    if (material.shader != null && material.shader.name == PCSS_EXTENSION_SHADER_NAME)
                    {
                        EnsureRequiredProperties(material);
                        SetupShaderKeywords(material);
                    }
                }
            }
            return modifications;
        }
        
        public static void EnsureRequiredProperties(Material material)
        {
            if (material.HasProperty("_MainTex") && material.GetTexture("_MainTex") == null)
                material.SetTexture("_MainTex", Texture2D.whiteTexture);
            
            if (material.HasProperty("_Color") && material.GetColor("_Color") == default)
                material.SetColor("_Color", Color.white);
            
            if (material.HasProperty("_ShadowColorTex") && material.GetTexture("_ShadowColorTex") == null)
                material.SetTexture("_ShadowColorTex", Texture2D.blackTexture);
            
            // Set default PCSS values when the material supports PCSS.
            if (material.HasProperty("_UsePCSS") && !material.HasProperty("_PCSSPresetMode"))
            {
                material.SetFloat("_PCSSPresetMode", 1.0f); // Anime preset
                material.SetFloat("_LocalPCSSFilterRadius", 0.01f);
                material.SetFloat("_LocalPCSSLightSize", 0.1f);
                material.SetFloat("_PCSSBias", 0.001f);
                material.SetFloat("_PCSSIntensity", 1.0f);
                material.SetFloat("_PCSSQuality", 1.0f); // Medium
                material.SetFloat("_LocalPCSSSamples", 16.0f);
            }
        }
        
        public static void SetupShaderKeywords(Material material)
        {
            SetKeyword(material, "_USEPCSS_ON", material.HasProperty("_UsePCSS") && material.GetFloat("_UsePCSS") > 0.5f);
            
            SetKeyword(material, "_USESHADOW_ON", material.HasProperty("_UseShadow") && material.GetFloat("_UseShadow") > 0.5f);
            
            SetKeyword(material, "_USESHADOWCLAMP_ON", material.HasProperty("_UseShadowClamp") && material.GetFloat("_UseShadowClamp") > 0.5f);
            
            SetKeyword(material, "_USEVRCLIGHT_VOLUMES_ON", material.HasProperty("_UseVRCLightVolumes") && material.GetFloat("_UseVRCLightVolumes") > 0.5f);
        }
        
        private static void SetKeyword(Material material, string keyword, bool state)
        {
            if (state)
                material.EnableKeyword(keyword);
            else
                material.DisableKeyword(keyword);
        }
        
        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (obj == null) return;
            
            if (HasPCSSMaterials(obj))
            {
                Rect iconRect = new Rect(selectionRect.xMax - 16, selectionRect.y, 16, 16);
                GUI.Label(iconRect, "笞｡", new GUIStyle { fontSize = 12, normal = { textColor = Color.yellow } });
            }
        }
        
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
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Utilities/PCSS Extension/About", false, 2000)]
        public static void ShowAbout()
        {
            EditorUtility.DisplayDialog(
                "lilToon PCSS Extension",
                "lilToon PCSS Extension v1.5.4\n\n" +
                "Adds PCSS (Percentage-Closer Soft Shadows) support to lilToon materials.\n" +
                "See the README for setup details.",
                "OK"
            );
        }
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Utilities/PCSS Extension/Open Documentation", false, 2001)]
        public static void OpenDocumentation()
        {
            string readmePath = AssetDatabase.FindAssets("liltoon_pcss_readme t:TextAsset")[0];
            if (!string.IsNullOrEmpty(readmePath))
            {
                string path = AssetDatabase.GUIDToAssetPath(readmePath);
                Application.OpenURL("file://" + Path.GetFullPath(path));
            }
        }
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Utilities/PCSS Extension/Check Installation", false, 2002)]
        public static void CheckInstallation()
        {
            bool hasLilToon = IsLilToonInstalled();
            bool hasPCSSShader = Shader.Find(PCSS_EXTENSION_SHADER_NAME) != null;
            
            string message = "Installation Status\n\n";
            message += $"lilToon: {(hasLilToon ? "Installed" : "Missing")}\n";
            message += $"PCSS Extension: {(hasPCSSShader ? "Installed" : "Missing")}\n";
            
            if (hasLilToon && hasPCSSShader)
            {
                message += "\nAll required components are installed.";
            }
            else
            {
                message += "\nSome required components are missing.";
                if (!hasLilToon) message += "\n- Install lilToon first.";
                if (!hasPCSSShader) message += "\n- PCSS Extension shader was not found.";
            }
            
            EditorUtility.DisplayDialog("PCSS Extension Installation Status", message, "OK");
        }
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Utilities/PCSS Extension/Fix Materials", false, 2003)]
        public static void FixMaterials()
        {
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
                        var go = renderer.gameObject;
                        if (PrefabUtility.IsPartOfPrefabInstance(go))
                        {
                            PrefabUtility.RecordPrefabInstancePropertyModifications(renderer);
                        }
                    }
                }
            }

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
            EditorUtility.DisplayDialog("PCSS Extension Material Repair", $"Automatically repaired {fixedMaterials.Count} material(s).\nMissing shaders and required properties were restored where possible.", "OK");
        }
    }
}
#endif 
