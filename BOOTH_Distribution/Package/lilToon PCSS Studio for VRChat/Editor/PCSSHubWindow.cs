#if UNITY_EDITOR
using System;
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
        private bool enableVrcLightVolumes = false;
        private bool createHipLightRig = false;
        private bool createModularAvatarToggle = false;
        private bool createIntensitySlider = true;
        private bool includeNonStandardLilToon = false;
        private bool performanceSafeMode = true;
        private PCSSHubSetup.LightPreset lightPreset = PCSSHubSetup.LightPreset.PCVRPerformance;
        private bool applyShadowPreset = true;
        private bool enablePhysBoneSway = false;
        private bool addHandColliders = false;
        private bool createSwaySlider = true;
        private bool removeNonPCSSLights = true;
        private bool disperseLightPositions = true;
        private bool avatarOnlyLightMask = true;
        private bool hideLightHelpersInSceneView = true;

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
            try
            {
                EditorGUILayout.Space(8);
                EditorGUILayout.LabelField("PCSS Hub (PC)", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("PCVR向けの標準導入は 0 Avatar Lights です。PCSSの影とツヤはマテリアル側で強調し、VRChat/AAOの負荷を増やすリアルライトは任意のプレビュー機能として扱います。", MessageType.Info);

                DrawAvatarSection();
                DrawMaterialSection();
                DrawPerformanceSafeSection();
                DrawLightRigSection();
                DrawModularAvatarSection();

                EditorGUILayout.Space(16);
                if (GUILayout.Button("PC向けセットアップ実行", GUILayout.Height(36)))
                {
                    RunSetupSafely();
                }
            }
            finally
            {
                EditorGUILayout.EndScrollView();
            }
        }

        private void RunSetupSafely()
        {
            try
            {
                RunSetup();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                EditorUtility.DisplayDialog("PCSS Hub", $"セットアップ中にエラーが発生しました。\n{ex.Message}", "OK");
            }
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
            if (avatarRoot != null && animator != null && animator.avatar != null && animator.isHuman)
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
            lightPreset = (PCSSHubSetup.LightPreset)EditorGUILayout.EnumPopup("PCSS / 艶プリセット", lightPreset);
            applyShadowPreset = EditorGUILayout.Toggle("影プリセットも適用", applyShadowPreset);
        }

        private void DrawPerformanceSafeSection()
        {
            EditorGUILayout.Space(12);
            EditorGUILayout.LabelField("AAO / VRChat Performance", EditorStyles.boldLabel);
            performanceSafeMode = EditorGUILayout.Toggle("PCVR推奨: 0 Avatar Lights", performanceSafeMode);
            if (performanceSafeMode)
            {
                removeNonPCSSLights = true;
                createHipLightRig = false;
                createModularAvatarToggle = false;
                EditorGUILayout.HelpBox("生成済みPCSSライトとアバター配下の既存Lightを除去します。PCSSとツヤ表現はマテリアル側に残すため、AAOの最適化を邪魔しにくく、Very Poorの原因になりやすいLight数を0に寄せます。", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("リアルタイムLightはVRChatで非常に重く、PCランクも悪化します。意図して高負荷な見た目を確認する場合だけ使ってください。", MessageType.Warning);
            }
        }

        private void DrawLightRigSection()
        {
            EditorGUILayout.Space(12);
            EditorGUILayout.LabelField("3. Optional PCSS Light Rig", EditorStyles.boldLabel);
            using (new EditorGUI.DisabledScope(performanceSafeMode))
            {
                removeNonPCSSLights = EditorGUILayout.Toggle("このプラグイン以外のLightを除去", removeNonPCSSLights);
            }
            disperseLightPositions = EditorGUILayout.Toggle("PCSSライト位置を分散", disperseLightPositions);
            avatarOnlyLightMask = EditorGUILayout.Toggle("アバターのRendererレイヤーだけ照射", avatarOnlyLightMask);
            hideLightHelpersInSceneView = EditorGUILayout.Toggle("生成Lightヘルパーを隠す", hideLightHelpersInSceneView);
            if (performanceSafeMode)
            {
                EditorGUILayout.HelpBox("安全モードではLight Rigを作成しません。既存Lightは強制的に取り除き、0-Lightのアップロード導線にします。", MessageType.Info);
                return;
            }
            createHipLightRig = EditorGUILayout.Toggle("リアルLight Rigを作成（高負荷）", createHipLightRig);
            using (new EditorGUI.DisabledScope(!createHipLightRig))
            {
                lightPreset = (PCSSHubSetup.LightPreset)EditorGUILayout.EnumPopup("任意Lightプリセット", lightPreset);
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
            if (performanceSafeMode)
            {
                EditorGUILayout.HelpBox("0-Light標準導入では、Light操作メニューは追加しません。表情や既存Animatorを壊さないため、マテリアル設定だけを導入します。", MessageType.Info);
                return;
            }
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
            if (!PCSSAvatarDescriptorGuard.TryEnsureDescriptor(avatarRoot, useUndo: true, out PCSSAvatarDescriptorGuard.RepairReport descriptorReport))
            {
                EditorUtility.DisplayDialog("PCSS Hub", descriptorReport.ToDialogString(), "OK");
                return;
            }

            avatarRoot = descriptorReport.AvatarRoot ?? avatarRoot;

            if (!PCSSHubSetup.TryGetHumanoidAnimator(avatarRoot, out Animator animator, out string error))
            {
                EditorUtility.DisplayDialog("PCSS Hub", error, "OK");
                return;
            }

            if (performanceSafeMode)
            {
                removeNonPCSSLights = true;
                createHipLightRig = false;
                createModularAvatarToggle = false;
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
            bool shouldCreateHipLightRig = !performanceSafeMode && createHipLightRig;
            bool shouldCreateModularAvatarToggle = !performanceSafeMode && createModularAvatarToggle && shouldCreateHipLightRig;
            int removedNonPcssLights = 0;
            if (removeNonPCSSLights || performanceSafeMode)
            {
                removedNonPcssLights = PCSSAvatarPerformanceGuard.RemoveNonPCSSAvatarLightComponents(avatarRoot, useUndo: false);
            }
            if (shouldCreateHipLightRig)
            {
                if (!PCSSHubSetup.TryCreateHipLightRig(avatarRoot, animator, height, lightPreset, enablePhysBoneSway, addHandColliders, disperseLightPositions, avatarOnlyLightMask, hideLightHelpersInSceneView, out createdLights, out usedBoneProxy, out usedPhysBone, out error))
                {
                    EditorUtility.DisplayDialog("PCSS Hub", error, "OK");
                    return;
                }
            }

            if (shouldCreateModularAvatarToggle)
            {
                bool includeSway = enablePhysBoneSway && createSwaySlider;
                if (!ModularAvatarLightToggleBuilder.BuildForAvatar(avatarRoot, createdLights, createIntensitySlider, includeSway, out error))
                {
                    EditorUtility.DisplayDialog("PCSS Hub", error, "OK");
                    return;
                }
            }

            PCSSAvatarPerformanceGuard.SetAllowAvatarLightsMarker(avatarRoot, allow: !performanceSafeMode && shouldCreateHipLightRig, useUndo: false);

            PCSSAvatarPerformanceGuard.PerformanceSummary performanceSummary = default;
            if (performanceSafeMode)
            {
                performanceSummary = PCSSAvatarPerformanceGuard.Apply(avatarRoot, useUndo: false, removeGeneratedLightControls: true, tuneMaterials: true);
            }

            PCSSNDMFAAOFixUtility.RepairSummary repairSummary = PCSSNDMFAAOFixUtility.RepairAvatar(avatarRoot);
            PCSSPCVRUploadAudit.AuditReport auditReport = PCSSPCVRUploadAudit.AuditAvatar(avatarRoot);

            EditorUtility.SetDirty(avatarRoot);
            AssetDatabase.SaveAssets();

            if (performanceSafeMode)
            {
                createHipLightRig = false;
                createModularAvatarToggle = false;
            }

            string summary = "完了しました。";
            if (applyPcssToLilToon)
            {
                summary += $"\nPCSS材質: {updated} 更新 / {alreadyPcss} 済み (対象 {total})";
                summary += applyShadowPreset ? $"\n影プリセット: 適用 ({lightPreset})" : "\n影プリセット: スキップ";
            }
            summary += shouldCreateHipLightRig ? $"\nライトリグ: 作成 (MA Bone Proxy: {(usedBoneProxy ? "有" : "無")})" : "\nライトリグ: なし（0-Light標準）";
            if (shouldCreateHipLightRig)
            {
                summary += $"\nライトプリセット: {lightPreset}";
                summary += $"\nPhysBone揺れ: {(enablePhysBoneSway && usedPhysBone ? "有効" : "無効")}";
                if (enablePhysBoneSway)
                {
                    summary += $"\n揺れスライダー: {(createSwaySlider ? "作成" : "なし")}";
                }
            }
            summary += shouldCreateModularAvatarToggle ? "\nMAトグル: 作成" : "\nMAトグル: なし";
            if (performanceSafeMode)
            {
                summary += "\nPCVR/AAO: 0 Avatar Lights";
                summary += $"\nPCSSライト削除: {performanceSummary.RemovedLightObjects}";
                summary += $"\n他Light削除: {removedNonPcssLights + performanceSummary.RemovedOtherLightComponents}";
                summary += $"\nPCSS材質調整: {performanceSummary.TunedMaterials}";
                summary += $"\n残存Light: {performanceSummary.RemainingLights}";
            }
            int repaired = repairSummary.BoneProxyFixed + repairSummary.MenuInstallerFixed + repairSummary.EmptyInstallerRemoved;
            summary += $"\nAvatarDescriptor: {descriptorReport.StatusText}";
            summary += $"\nNDMF/AAO自動修復: {repaired} 件";
            summary += $"\nPCVR監査: {(auditReport.Passed ? "合格" : "要確認")} (Light {auditReport.TotalLights}, 重い材質 {auditReport.HeavyMaterialIssues}, Animator {auditReport.AnimatorIssues})";
            summary += $"\n推定身長: {height:F2} m";

            EditorUtility.DisplayDialog("PCSS Hub (PC)", summary, "OK");
        }
    }
}
#endif
