using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
#if VRCHAT_SDK_AVAILABLE
using VRC.SDK3.Avatars.Components;
#endif

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// アバター選択メニューシステム
    /// シーン内のアバターを自動検出して選択・導入できるメニュー
    /// </summary>
    public class AvatarSelectorMenu : EditorWindow
    {
        private Vector2 scrollPosition;
        private List<AvatarInfo> availableAvatars = new List<AvatarInfo>();
        private AvatarInfo selectedAvatar;
        private SetupMode selectedSetupMode = SetupMode.CompetitorCompatible;
        private bool showAdvancedOptions = false;
        
        // UI スタイル
        private GUIStyle headerStyle;
        private GUIStyle avatarButtonStyle;
        private GUIStyle selectedAvatarStyle;
        private GUIStyle setupButtonStyle;
        
        public enum SetupMode
        {
            CompetitorCompatible,
            ToonShadows,
            RealisticShadows,
            CustomSetup
        }
        
        [System.Serializable]
        public class AvatarInfo
        {
            public GameObject gameObject;
            public string name;
            public string path;
            public bool hasVRCAvatarDescriptor;
            public int rendererCount;
            public int materialCount;
            public Texture2D preview;
            
            public AvatarInfo(GameObject obj)
            {
                gameObject = obj;
                name = obj.name;
                path = GetGameObjectPath(obj);
                
#if VRCHAT_SDK_AVAILABLE
                hasVRCAvatarDescriptor = obj.GetComponent<VRCAvatarDescriptor>() != null;
#else
                hasVRCAvatarDescriptor = false;
#endif
                
                var renderers = obj.GetComponentsInChildren<Renderer>();
                rendererCount = renderers.Length;
                materialCount = renderers.SelectMany(r => r.sharedMaterials).Where(m => m != null).Distinct().Count();
            }
            
            private string GetGameObjectPath(GameObject obj)
            {
                string path = obj.name;
                Transform parent = obj.transform.parent;
                while (parent != null)
                {
                    path = parent.name + "/" + path;
                    parent = parent.parent;
                }
                return path;
            }
        }
        
        [MenuItem("Window/lilToon PCSS Extension/🎯 Avatar Selector")]
        public static void ShowWindow()
        {
            var window = GetWindow<AvatarSelectorMenu>("Avatar Selector");
            window.titleContent = new GUIContent("🎯 Avatar Selector", "アバター選択メニュー");
            window.minSize = new Vector2(500, 400);
            window.Show();
        }
        
        [MenuItem("lilToon PCSS Extension/🎯 Avatar Selector", false, 1)]
        public static void ShowWindowFromMenu()
        {
            ShowWindow();
        }
        
        private void OnEnable()
        {
            RefreshAvatarList();
            InitializeStyles();
        }
        
        private void OnFocus()
        {
            RefreshAvatarList();
        }
        
        private void InitializeStyles()
        {
            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.8f, 0.8f, 1f) }
            };
            
            avatarButtonStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 11,
                fixedHeight = 60,
                padding = new RectOffset(10, 10, 5, 5)
            };
            
            selectedAvatarStyle = new GUIStyle(avatarButtonStyle)
            {
                normal = { background = MakeTexture(2, 2, new Color(0.3f, 0.6f, 1f, 0.3f)) }
            };
            
            setupButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                fixedHeight = 35,
                fontStyle = FontStyle.Bold
            };
        }
        
        private Texture2D MakeTexture(int width, int height, Color color)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = color;
            
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
        
        private void OnGUI()
        {
            if (headerStyle == null) InitializeStyles();
            
            DrawHeader();
            DrawAvatarList();
            DrawSetupOptions();
            DrawActionButtons();
        }
        
        private void DrawHeader()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("🎯 lilToon PCSS Extension", headerStyle);
            EditorGUILayout.LabelField("Avatar Selector & Setup System", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space(10);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("🔄 Refresh Avatar List", GUILayout.Height(25)))
            {
                RefreshAvatarList();
            }
            if (GUILayout.Button("📖 Help", GUILayout.Height(25), GUILayout.Width(60)))
            {
                Application.OpenURL("https://zapabob.github.io/liltoon-pcss-extension/documentation.html");
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
        }
        
        private void DrawAvatarList()
        {
            EditorGUILayout.LabelField($"Available Avatars ({availableAvatars.Count})", EditorStyles.boldLabel);
            
            if (availableAvatars.Count == 0)
            {
                EditorGUILayout.HelpBox("No avatars found in the scene. Please add VRChat avatars to the scene.", MessageType.Info);
                return;
            }
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
            
            foreach (var avatar in availableAvatars)
            {
                DrawAvatarButton(avatar);
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        private void DrawAvatarButton(AvatarInfo avatar)
        {
            var style = (selectedAvatar == avatar) ? selectedAvatarStyle : avatarButtonStyle;
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("", style, GUILayout.Height(60)))
            {
                selectedAvatar = avatar;
                Selection.activeGameObject = avatar.gameObject;
                SceneView.FrameLastActiveSceneView();
            }
            
            var buttonRect = GUILayoutUtility.GetLastRect();
            
            // アバター情報を描画
            var contentRect = new Rect(buttonRect.x + 10, buttonRect.y + 5, buttonRect.width - 20, buttonRect.height - 10);
            
            GUI.Label(new Rect(contentRect.x, contentRect.y, contentRect.width, 16), 
                $"🎭 {avatar.name}", EditorStyles.boldLabel);
            
            GUI.Label(new Rect(contentRect.x, contentRect.y + 18, contentRect.width, 14), 
                $"Path: {avatar.path}", EditorStyles.miniLabel);
            
            var statusText = avatar.hasVRCAvatarDescriptor ? "✅ VRChat Avatar" : "⚠️ No VRCAvatarDescriptor";
            var statusColor = avatar.hasVRCAvatarDescriptor ? Color.green : Color.yellow;
            
            var oldColor = GUI.color;
            GUI.color = statusColor;
            GUI.Label(new Rect(contentRect.x, contentRect.y + 32, contentRect.width / 2, 14), 
                statusText, EditorStyles.miniLabel);
            GUI.color = oldColor;
            
            GUI.Label(new Rect(contentRect.x + contentRect.width / 2, contentRect.y + 32, contentRect.width / 2, 14), 
                $"Renderers: {avatar.rendererCount} | Materials: {avatar.materialCount}", EditorStyles.miniLabel);
            
            // 選択ボタン
            if (GUI.Button(new Rect(buttonRect.x + buttonRect.width - 80, buttonRect.y + 5, 70, 20), "Select"))
            {
                selectedAvatar = avatar;
                Selection.activeGameObject = avatar.gameObject;
                SceneView.FrameLastActiveSceneView();
            }
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(2);
        }
        
        private void DrawSetupOptions()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Setup Options", EditorStyles.boldLabel);
            
            selectedSetupMode = (SetupMode)EditorGUILayout.EnumPopup("Setup Mode", selectedSetupMode);
            
            // セットアップモードの説明
            string description = GetSetupModeDescription(selectedSetupMode);
            EditorGUILayout.HelpBox(description, MessageType.Info);
            
            showAdvancedOptions = EditorGUILayout.Foldout(showAdvancedOptions, "Advanced Options");
            if (showAdvancedOptions)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Advanced options will be available in future updates.", EditorStyles.miniLabel);
                EditorGUI.indentLevel--;
            }
        }
        
        private string GetSetupModeDescription(SetupMode mode)
        {
            switch (mode)
            {
                case SetupMode.CompetitorCompatible:
                    return "🎯 競合製品互換モード: nHaruka等の競合製品と同等の機能を提供します。PhysBone連動、10m距離制御、シャドウマスク機能を含みます。";
                case SetupMode.ToonShadows:
                    return "🎭 トゥーン影モード: アニメ調の美しい影境界線を作成します。VTuberアバターに最適化された設定です。";
                case SetupMode.RealisticShadows:
                    return "📸 リアル影モード: 写実的な影表現を提供します。リアルなアバターや写真撮影に適しています。";
                case SetupMode.CustomSetup:
                    return "⚙️ カスタムセットアップ: 詳細設定ウィザードを開いて、個別にカスタマイズできます。";
                default:
                    return "";
            }
        }
        
        private void DrawActionButtons()
        {
            EditorGUILayout.Space(10);
            
            GUI.enabled = selectedAvatar != null;
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button($"🚀 Setup Selected Avatar", setupButtonStyle))
            {
                ExecuteSetup();
            }
            
            if (GUILayout.Button("🔧 Open Setup Wizard", GUILayout.Height(35)))
            {
                if (selectedSetupMode == SetupMode.CustomSetup)
                {
                    CompetitorCompatibleSetupWizard.ShowWindow();
                }
                else
                {
                    VCCSetupWizard.ShowWindow();
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            GUI.enabled = true;
            
            if (selectedAvatar == null)
            {
                EditorGUILayout.HelpBox("Please select an avatar to proceed with setup.", MessageType.Warning);
            }
            else if (!selectedAvatar.hasVRCAvatarDescriptor)
            {
                EditorGUILayout.HelpBox("Selected object is not a VRChat avatar. VRCAvatarDescriptor component is required.", MessageType.Warning);
            }
        }
        
        private void ExecuteSetup()
        {
            if (selectedAvatar == null) return;
            
            // 選択されたアバターをUnityのSelectionに設定
            Selection.activeGameObject = selectedAvatar.gameObject;
            
            switch (selectedSetupMode)
            {
                case SetupMode.CompetitorCompatible:
                    OneClickSetupMenu.QuickSetupCompetitorCompatible(selectedAvatar.gameObject);
                    break;
                case SetupMode.ToonShadows:
                    OneClickSetupMenu.QuickSetupToonShadows(selectedAvatar.gameObject);
                    break;
                case SetupMode.RealisticShadows:
                    OneClickSetupMenu.QuickSetupRealisticShadows(selectedAvatar.gameObject);
                    break;
                case SetupMode.CustomSetup:
                    OneClickSetupMenu.OpenCustomSetupWizard();
                    break;
            }
        }
        
        private void RefreshAvatarList()
        {
            availableAvatars.Clear();
            
            // シーン内のすべてのGameObjectを検索
            var allObjects = FindObjectsOfType<GameObject>();
            
            foreach (var obj in allObjects)
            {
                // ルートオブジェクトのみを対象とする
                if (obj.transform.parent == null)
                {
                    // VRCAvatarDescriptorを持つオブジェクトを優先
#if VRCHAT_SDK_AVAILABLE
                    if (obj.GetComponent<VRCAvatarDescriptor>() != null)
                    {
                        availableAvatars.Add(new AvatarInfo(obj));
                        continue;
                    }
#endif
                    
                    // 子オブジェクトにVRCAvatarDescriptorがある場合も含める
#if VRCHAT_SDK_AVAILABLE
                    if (obj.GetComponentInChildren<VRCAvatarDescriptor>() != null)
                    {
                        availableAvatars.Add(new AvatarInfo(obj));
                        continue;
                    }
#endif
                    
                    // Rendererを持つオブジェクトも候補として追加（アバターの可能性）
                    if (obj.GetComponentsInChildren<Renderer>().Length > 0)
                    {
                        availableAvatars.Add(new AvatarInfo(obj));
                    }
                }
            }
            
            // VRCAvatarDescriptorを持つものを優先してソート
            availableAvatars = availableAvatars.OrderByDescending(a => a.hasVRCAvatarDescriptor)
                                             .ThenBy(a => a.name)
                                             .ToList();
        }
    }
} 