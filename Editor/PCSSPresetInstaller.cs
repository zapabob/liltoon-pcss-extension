#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace lilToon.PCSS.Editor
{
    public static class PCSSPresetInstaller
    {
        private const string PresetRoot = PCSSConstants.PresetRoot;
        private const string EmissionMatPath = PCSSConstants.EmissionMaterialPath;
        private const string EmissionPrefabPath = PCSSConstants.EmissionPrefabPath;
        private const string MeshRendererPresetPath = PCSSConstants.MeshRendererPresetPath;

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Install Presets", priority = 5)]
        public static void Install()
        {
            EnsureFolder(PresetRoot);

            var emissionMat = CreateOrUpdateEmissionMaterial(EmissionMatPath);
            var prefab = CreateOrUpdateEmissionPrefab(EmissionPrefabPath, emissionMat);
            CreateOrUpdateMeshRendererPreset(MeshRendererPresetPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = prefab ? prefab : (Object)emissionMat;
            Debug.Log("[PCSS Presets] Installed presets to " + PresetRoot);
        }

        private static void EnsureFolder(string path)
        {
            var split = path.Split('/');
            var current = split[0];
            if (!AssetDatabase.IsValidFolder(current)) AssetDatabase.CreateFolder("Assets", current.Substring("Assets/".Length));
            for (int i = 1; i < split.Length; i++)
            {
                var next = current + "/" + split[i];
                if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(current, split[i]);
                current = next;
            }
        }

        private static Material CreateOrUpdateEmissionMaterial(string path)
        {
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                mat = new Material(Shader.Find("Standard"));
                AssetDatabase.CreateAsset(mat, path);
            }
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", Color.white * 1.0f);
            mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            EditorUtility.SetDirty(mat);
            return mat;
        }

        private static GameObject CreateOrUpdateEmissionPrefab(string path, Material emissionMat)
        {
            GameObject root;
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing == null)
            {
                root = GameObject.CreatePrimitive(PrimitiveType.Quad);
                root.name = PCSSConstants.EmissionObjectName;
                var col = root.GetComponent<Collider>();
                if (col) Object.DestroyImmediate(col);
                var mr = root.GetComponent<MeshRenderer>();
                mr.sharedMaterial = emissionMat;
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.receiveShadows = false;
                var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
                Object.DestroyImmediate(root);
                return prefab;
            }
            else
            {
                root = existing;
                var mr = root.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    mr.sharedMaterial = emissionMat;
                    mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    mr.receiveShadows = false;
                    EditorUtility.SetDirty(mr);
                }
                return existing;
            }
        }

        private static void CreateOrUpdateMeshRendererPreset(string path)
        {
            var temp = new GameObject("_pcss_temp");
            try
            {
                var mr = temp.AddComponent<MeshRenderer>();
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.receiveShadows = false;
                mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                var preset = new Preset(mr);

                var existing = AssetDatabase.LoadAssetAtPath<Preset>(path);
                if (existing == null)
                {
                    AssetDatabase.CreateAsset(preset, path);
                }
                else
                {
                    EditorUtility.CopySerialized(preset, existing);
                    EditorUtility.SetDirty(existing);
                }
            }
            finally
            {
                Object.DestroyImmediate(temp);
            }
        }
    }
}
#endif


