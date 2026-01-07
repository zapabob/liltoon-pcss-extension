#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace lilToon.PCSS.Editor
{
    public sealed class PCSSHubWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private GameObject avatarRoot;
        private bool applyPcssToLilToon = true;
        private bool enableVrcLightVolumes = true;
        private bool createHipLightRig = true;
        private bool createModularAvatarToggle = true;
        private bool createIntensitySlider = true;
        private bool includeNonStandardLilToon = false;
        private PCSSHubSetup.LightPreset lightPreset = PCSSHubSetup.LightPreset.Realistic;
        private bool applyShadowPreset = true;
        private bool enablePhysBoneSway = false;
        private bool addHandColliders = false;
        private bool createSwaySlider = true;

        [MenuItem("Tools/lilToon-PCSS-Extension/PCSS Hub (PC)", false, 0)]
        public static void ShowWindow()
        {
            PCSSHubWindow window = GetWindow<PCSSHubWindow>("PCSS Hub (PC)");
            window.minSize = new Vector2(520f, 620f);
            window.Show();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("PCSS Hub (PC)", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("PC向け：lilToon標準シェーダー + PCSS + VRChat Light Volumes + Modular Avatarトグルを一括セットアップします。", MessageType.Info);

            DrawAvatarSection();
            DrawMaterialSection();
            DrawLightRigSection();
            DrawModularAvatarSection();

            EditorGUILayout.Space(16);
            if (GUILayout.Button("PC向けセットアップ実行", GUILayout.Height(36)))
            {
                RunSetup();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawAvatarSection()
        {
            EditorGUILayout.Space(12);
            EditorGUILayout.LabelField("1. Avatar", EditorStyles.boldLabel);
            avatarRoot = (GameObject)EditorGUILayout.ObjectField("Target Avatar", avatarRoot, typeof(GameObject), true);
            if (avatarRoot == null)
            {
                avatarRoot = Selection.activeGameObject;
            }

            Animator animator = avatarRoot != null ? avatarRoot.GetComponent<Animator>() : null;
            if (avatarRoot != null && animator != null && animator.isHuman)
            {
                float height = PCSSHubSetup.EstimateAvatarHeight(animator);
                EditorGUILayout.LabelField("Estimated Height", height > 0f ? $"{height:F2} m" : "計算不可");
            }
            else if (avatarRoot != null)
            {
                EditorGUILayout.HelpBox("Humanoid アバターを選択してください。", MessageType.Warning);
            }
        }

        private void DrawMaterialSection()
        {
            EditorGUILayout.Space(12);
            EditorGUILayout.LabelField("2. Materials", EditorStyles.boldLabel);
            applyPcssToLilToon = EditorGUILayout.Toggle("lilToonをPCSS化", applyPcssToLilToon);
            enableVrcLightVolumes = EditorGUILayout.Toggle("VRC Light Volumesを有効化", enableVrcLightVolumes);
            includeNonStandardLilToon = EditorGUILayout.Toggle("特殊lilToonも含める", includeNonStandardLilToon);
            applyShadowPreset = EditorGUILayout.Toggle("影プリセットも適用", applyShadowPreset);
        }

        private void DrawLightRigSection()
        {
            EditorGUILayout.Space(12);
            EditorGUILayout.LabelField("3. Hip Light Rig", EditorStyles.boldLabel);
            createHipLightRig = EditorGUILayout.Toggle("Hip基準ライトリグを作成", createHipLightRig);
            using (new EditorGUI.DisabledScope(!createHipLightRig))
            {
                lightPreset = (PCSSHubSetup.LightPreset)EditorGUILayout.EnumPopup("ライトプリセット", lightPreset);
                enablePhysBoneSway = EditorGUILayout.Toggle("PhysBoneでライト方向を揺らす", enablePhysBoneSway);
                using (new EditorGUI.DisabledScope(!enablePhysBoneSway))
                {
                    addHandColliders = EditorGUILayout.Toggle("手コライダーを追加", addHandColliders);
                }
            }
        }

        private void DrawModularAvatarSection()
        {
            EditorGUILayout.Space(12);
            EditorGUILayout.LabelField("4. Modular Avatar", EditorStyles.boldLabel);
            createModularAvatarToggle = EditorGUILayout.Toggle("ライトON/OFFトグルを追加", createModularAvatarToggle);
            using (new EditorGUI.DisabledScope(!createModularAvatarToggle))
            {
                createIntensitySlider = EditorGUILayout.Toggle("強度スライダーも追加", createIntensitySlider);
                using (new EditorGUI.DisabledScope(!enablePhysBoneSway))
                {
                    createSwaySlider = EditorGUILayout.Toggle("揺れ強さスライダーも追加", createSwaySlider);
                }
            }
        }

        private void RunSetup()
        {
            if (!PCSSHubSetup.TryGetHumanoidAnimator(avatarRoot, out Animator animator, out string error))
            {
                EditorUtility.DisplayDialog("PCSS Hub", error, "OK");
                return;
            }

            float height = PCSSHubSetup.EstimateAvatarHeight(animator);
            if (height <= 0f)
            {
                EditorUtility.DisplayDialog("PCSS Hub", "身長の推定に失敗しました。", "OK");
                return;
            }

            Undo.RegisterFullObjectHierarchyUndo(avatarRoot, "PCSS Hub Setup");

            int updated = 0;
            int alreadyPcss = 0;
            int total = 0;
            if (applyPcssToLilToon)
            {
                if (!PCSSHubSetup.TryApplyPcssMaterials(avatarRoot, enableVrcLightVolumes, includeNonStandardLilToon, lightPreset, applyShadowPreset, out updated, out alreadyPcss, out total, out error))
                {
                    EditorUtility.DisplayDialog("PCSS Hub", error, "OK");
                    return;
                }
            }

            List<Light> createdLights = null;
            bool usedBoneProxy = false;
            bool usedPhysBone = false;
            if (createHipLightRig)
            {
                if (!PCSSHubSetup.TryCreateHipLightRig(avatarRoot, animator, height, lightPreset, enablePhysBoneSway, addHandColliders, out createdLights, out usedBoneProxy, out usedPhysBone, out error))
                {
                    EditorUtility.DisplayDialog("PCSS Hub", error, "OK");
                    return;
                }
            }

            if (createModularAvatarToggle)
            {
                bool includeSway = enablePhysBoneSway && createSwaySlider;
                if (!ModularAvatarLightToggleBuilder.BuildForAvatar(avatarRoot, createdLights, createIntensitySlider, includeSway, out error))
                {
                    EditorUtility.DisplayDialog("PCSS Hub", error, "OK");
                    return;
                }
            }

            EditorUtility.SetDirty(avatarRoot);
            AssetDatabase.SaveAssets();

            string summary = "完了しました。";
            if (applyPcssToLilToon)
            {
                summary += $"\nPCSS材質: {updated} 更新 / {alreadyPcss} 済み (対象 {total})";
                summary += applyShadowPreset ? $"\n影プリセット: 適用 ({lightPreset})" : "\n影プリセット: スキップ";
            }
            summary += createHipLightRig ? $"\nライトリグ: 作成 (MA Bone Proxy: {(usedBoneProxy ? "有" : "無")})" : "\nライトリグ: スキップ";
            if (createHipLightRig)
            {
                summary += $"\nライトプリセット: {lightPreset}";
                summary += $"\nPhysBone揺れ: {(enablePhysBoneSway && usedPhysBone ? "有効" : "無効")}";
                if (enablePhysBoneSway)
                {
                    summary += $"\n揺れスライダー: {(createSwaySlider ? "作成" : "なし")}";
                }
            }
            summary += createModularAvatarToggle ? "\nMAトグル: 作成" : "\nMAトグル: スキップ";
            summary += $"\n推定身長: {height:F2} m";

            EditorUtility.DisplayDialog("PCSS Hub (PC)", summary, "OK");
        }
    }
}
#endif
