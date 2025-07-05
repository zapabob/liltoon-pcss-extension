using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// lilToon PCSS Extension 一括セットアップウィンドウ
    /// </summary>
    public class PCSSBatchSetupWindow : EditorWindow
    {
        private Vector2 scroll;
        private List<GameObject> avatars = new List<GameObject>();
        private List<GameObject> worlds = new List<GameObject>();
        private List<GameObject> selectedAvatars = new List<GameObject>();
        private List<GameObject> selectedWorlds = new List<GameObject>();
        private string log = "";
        private bool showAvatars = true;
        private bool showWorlds = false;
        private bool showLog = false;
        private bool enableExpressionMenu = true;
        private bool enableShadowMask = true;

        [MenuItem("lilToon/PCSS Extension/一括セットアップウィンドウ")]
        public static void ShowWindow()
        {
            var win = GetWindow<PCSSBatchSetupWindow>("PCSS一括セットアップ");
            win.minSize = new Vector2(500, 600);
            win.RefreshTargets();
            win.Show();
        }

        private void OnEnable()
        {
            RefreshTargets();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("lilToon PCSS Extension 一括セットアップ", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            if (GUILayout.Button("🔄 対象を再検出", GUILayout.Height(24))) RefreshTargets();
            EditorGUILayout.Space();

            // 新規: オプション
            EditorGUILayout.LabelField("追加オプション", EditorStyles.boldLabel);
            enableExpressionMenu = EditorGUILayout.ToggleLeft("ExpressionMenu/Animator自動生成（ON/OFF, 色, 強度）", enableExpressionMenu);
            enableShadowMask = EditorGUILayout.ToggleLeft("影マスクテクスチャ自動設定", enableShadowMask);
            EditorGUILayout.Space();

            scroll = EditorGUILayout.BeginScrollView(scroll);
            showAvatars = EditorGUILayout.Foldout(showAvatars, $"アバター一覧 ({avatars.Count})", true);
            if (showAvatars)
            {
                EditorGUI.indentLevel++;
                foreach (var go in avatars)
                {
                    bool sel = selectedAvatars.Contains(go);
                    bool newSel = EditorGUILayout.ToggleLeft(go.name, sel);
                    if (newSel && !sel) selectedAvatars.Add(go);
                    if (!newSel && sel) selectedAvatars.Remove(go);
                }
                EditorGUI.indentLevel--;
            }
            showWorlds = EditorGUILayout.Foldout(showWorlds, $"ワールド一覧 ({worlds.Count})", true);
            if (showWorlds)
            {
                EditorGUI.indentLevel++;
                foreach (var go in worlds)
                {
                    bool sel = selectedWorlds.Contains(go);
                    bool newSel = EditorGUILayout.ToggleLeft(go.name, sel);
                    if (newSel && !sel) selectedWorlds.Add(go);
                    if (!newSel && sel) selectedWorlds.Remove(go);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();
            if (GUILayout.Button("🚀 一括セットアップを実行", GUILayout.Height(32)))
            {
                RunBatchSetup();
            }
            EditorGUILayout.Space();
            showLog = EditorGUILayout.Foldout(showLog, "適用ログ", true);
            if (showLog)
            {
                EditorGUILayout.TextArea(log, GUILayout.MinHeight(120));
            }
        }

        private void RefreshTargets()
        {
            avatars.Clear();
            worlds.Clear();
            selectedAvatars.Clear();
            selectedWorlds.Clear();
            // VRCAvatarDescriptorを持つGameObjectをアバターとみなす
            avatars.AddRange(FindObjectsOfType<GameObject>(true)
                .Where(go => go.GetComponent<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>() != null));
            // ワールド判定: Light, ReflectionProbe, PostProcess等を持つルートをワールド候補とみなす
            worlds.AddRange(FindObjectsOfType<GameObject>(true)
                .Where(go => go.scene.IsValid() && go.transform.parent == null &&
                    (go.GetComponentInChildren<Light>() != null || go.GetComponentInChildren<ReflectionProbe>() != null)));
        }

        private void RunBatchSetup()
        {
            log = "";
            int avatarCount = 0, worldCount = 0;
            foreach (var avatar in selectedAvatars)
            {
                Undo.RecordObject(avatar, "PCSS一括セットアップ(アバター)");
                int matCount = lilToon.PCSS.Runtime.PCSSUtilities.ApplyPresetToAvatar(avatar, lilToon.PCSS.Runtime.PCSSUtilities.PCSSPreset.Realistic);
                SetupVirtualLight(avatar);
                if (enableExpressionMenu) SetupExpressionMenu(avatar);
                if (enableShadowMask) SetupShadowMask(avatar);
                log += $"[Avatar] {avatar.name}: {matCount}マテリアルにPCSS適用\n";
                avatarCount++;
            }
            foreach (var world in selectedWorlds)
            {
                Undo.RecordObject(world, "PCSS一括セットアップ(ワールド)");
                int matCount = lilToon.PCSS.Runtime.PCSSUtilities.ApplyPresetToAvatar(world, lilToon.PCSS.Runtime.PCSSUtilities.PCSSPreset.Realistic);
                SetupLightVolumes(world);
                if (enableShadowMask) SetupShadowMask(world);
                log += $"[World] {world.name}: {matCount}マテリアルにPCSS適用\n";
                worldCount++;
            }
            log += $"\n✅ アバター: {avatarCount}体, ワールド: {worldCount}件に一括適用完了\n";
            EditorUtility.DisplayDialog("PCSS一括セットアップ", "一括適用が完了しました。\n詳細はログを参照してください。", "OK");
        }

        private void SetupVirtualLight(GameObject avatar)
        {
            // Headボーン探索
            var animator = avatar.GetComponent<Animator>();
            if (animator == null) return;
            var head = animator.GetBoneTransform(HumanBodyBones.Head);
            if (head == null) return;
            var vlight = head.Find("VirtualLight");
            if (vlight == null)
            {
                vlight = new GameObject("VirtualLight").transform;
                vlight.parent = head;
                vlight.localPosition = new Vector3(0, 0.1f, 0.15f);
                vlight.localRotation = Quaternion.identity;
            }
            // VirtualLightBinder自動アタッチ
            var binder = avatar.GetComponent<VirtualLightBinder>();
            if (binder == null) binder = avatar.AddComponent<VirtualLightBinder>();
            binder.virtualLight = vlight;
        }

        private void SetupLightVolumes(GameObject world)
        {
            // VRCLightVolumesIntegration自動アタッチ
            var integration = world.GetComponent<lilToon.PCSS.VRCLightVolumesIntegration>();
            if (integration == null) integration = world.AddComponent<lilToon.PCSS.VRCLightVolumesIntegration>();
            integration.SetVRCLightVolumesEnabled(true);
        }

        // ExpressionMenu/Animator自動生成
        private void SetupExpressionMenu(GameObject avatar)
        {
            var mats = lilToon.PCSS.Runtime.PCSSUtilities.FindPCSSMaterials(avatar);
            foreach (var mat in mats)
            {
                lilToon.PCSS.VRChatExpressionMenuCreator.CreatePCSSExpressionMenu(mat, true, "PCSS");
            }
            log += $"  └ ExpressionMenu/Animator自動生成\n";
        }

        // 影マスク自動設定
        private void SetupShadowMask(GameObject target)
        {
            var mats = lilToon.PCSS.Runtime.PCSSUtilities.FindPCSSMaterials(target);
            foreach (var mat in mats)
            {
                if (mat.HasProperty("_ShadowColorTex"))
                {
                    // 白黒の影マスクテクスチャを自動生成（例: 全部白）
                    var tex = new Texture2D(2, 2);
                    tex.SetPixels(new[] { Color.white, Color.white, Color.white, Color.white });
                    tex.Apply();
                    mat.SetTexture("_ShadowColorTex", tex);
                }
            }
            log += $"  └ 影マスク自動設定\n";
        }
    }
} 