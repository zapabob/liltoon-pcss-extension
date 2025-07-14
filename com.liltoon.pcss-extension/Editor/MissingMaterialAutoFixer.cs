using UnityEditor;
using UnityEngine;
using System.Linq;

namespace lilToon.PCSS.Editor
{
    public class MissingMaterialAutoFixer : EditorWindow
    {
        private const string MenuRoot = "Tools/lilToon PCSS Extension/";
        private const string UtilitiesMenu = MenuRoot + "Utilities/";
        private const string PCSSMenu = UtilitiesMenu + "PCSS Extension/";
        private const string MenuPath = PCSSMenu + "Missing Material AutoFixer";
        [MenuItem(MenuPath)]
        public static void ShowWindow()
        {
            GetWindow<MissingMaterialAutoFixer>("Missing Material AutoFixer");
        }

        private int missingCount = 0;
        private int fixedCount = 0;

        private void OnGUI()
        {
            if (GUILayout.Button("Scan for Missing Materials"))
            {
                missingCount = ScanAndReport();
                EditorUtility.DisplayDialog("Scan Result", $"Missing materials found: {missingCount}", "OK");
            }
            if (GUILayout.Button("Auto Fix Missing Materials"))
            {
                fixedCount = AutoFix();
                EditorUtility.DisplayDialog("Auto Fix", $"Fixed {fixedCount} missing materials.", "OK");
            }
            GUILayout.Space(10);
            GUILayout.Label($"Last Scan: {missingCount} missing | Last Fix: {fixedCount} fixed", EditorStyles.boldLabel);
        }

        private int ScanAndReport()
        {
            int count = 0;
            foreach (var renderer in FindObjectsOfType<Renderer>(true))
            {
                foreach (var mat in renderer.sharedMaterials)
                {
                    if (mat == null)
                    {
                        Debug.LogWarning($"[MissingMaterialAutoFixer] {renderer.name} has a missing material slot.", renderer);
                        count++;
                    }
                }
            }
            return count;
        }

        private int AutoFix()
        {
            var lilToonShader = Shader.Find("lilToon");
            if (lilToonShader == null)
            {
                EditorUtility.DisplayDialog("Error", "lilToon shader not found! Please install lilToon.", "OK");
                return 0;
            }
            int count = 0;
            foreach (var renderer in FindObjectsOfType<Renderer>(true))
            {
                var mats = renderer.sharedMaterials;
                bool changed = false;
                for (int i = 0; i < mats.Length; i++)
                {
                    if (mats[i] == null)
                    {
                        Undo.RecordObject(renderer, "AutoFix Missing Material");
                        var newMat = new Material(lilToonShader) { name = $"{renderer.name}_AutoFixed" };
                        mats[i] = newMat;
                        count++;
                        changed = true;
                        Debug.Log($"[MissingMaterialAutoFixer] Fixed missing material on {renderer.name}", renderer);
                    }
                }
                if (changed)
                {
                    renderer.sharedMaterials = mats;
                    EditorUtility.SetDirty(renderer);
                }
            }
            AssetDatabase.SaveAssets();
            return count;
        }
    }

    // FBX/Prefabインポ�Eト時の自動リマッチE
    public class LilToonMaterialRemapper : AssetPostprocessor
    {
        void OnPostprocessModel(GameObject g)
        {
            var lilToonShader = Shader.Find("lilToon");
            if (lilToonShader == null) return;

            foreach (var renderer in g.GetComponentsInChildren<Renderer>(true))
            {
                var mats = renderer.sharedMaterials;
                bool changed = false;
                for (int i = 0; i < mats.Length; i++)
                {
                    if (mats[i] == null)
                    {
                        var newMat = new Material(lilToonShader) { name = $"{renderer.name}_AutoRemap" };
                        mats[i] = newMat;
                        changed = true;
                        Debug.Log($"[LilToonMaterialRemapper] Auto-remapped missing material on {renderer.name}", renderer);
                    }
                }
                if (changed)
                {
                    renderer.sharedMaterials = mats;
                }
            }
        }
    }
} 
