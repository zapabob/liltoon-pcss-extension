#if UNITY_EDITOR
using UnityEngine;

namespace lilToonPCSS.Editor
{
    /// <summary>
    /// 製品版用メニュー定数
    /// ユーザーフレンドリーで整理されたメニュー構造
    /// </summary>
    public static class MenuConstants
    {
        // メインルート
        public const string MENU_ROOT = "Tools/lilToon PCSS Extension/";
        
        // 主要機能
        public const string MATERIALS_MENU = MENU_ROOT + "Materials/";
        public const string VRChat_MENU = MENU_ROOT + "VRChat/";
        public const string PRESETS_MENU = MENU_ROOT + "Presets/";
        public const string UTILITIES_MENU = MENU_ROOT + "Utilities/";
        public const string HELP_MENU = MENU_ROOT + "Help/";
        
        // サブメニュー
        public const string MATERIAL_UPGRADE = MATERIALS_MENU + "Upgrade Materials";
        public const string MATERIAL_BACKUP = MATERIALS_MENU + "Backup Materials";
        public const string MATERIAL_RESTORE = MATERIALS_MENU + "Restore Materials";
        
        public const string VRC_EXPRESSION_MENU = VRChat_MENU + "Create Expression Menu";
        public const string VRC_LIGHT_VOLUMES = VRChat_MENU + "VRC Light Volumes/";
        public const string VRC_OPTIMIZATION = VRChat_MENU + "Optimization Settings";
        
        public const string PRESET_REALISTIC = PRESETS_MENU + "Realistic Shadows";
        public const string PRESET_ANIME = PRESETS_MENU + "Anime Style";
        public const string PRESET_CINEMATIC = PRESETS_MENU + "Cinematic Style";
        public const string PRESET_PORTRAIT = PRESETS_MENU + "Portrait Style";
        public const string PRESET_GAME = PRESETS_MENU + "Game Style";
        
        public const string UTILITY_AVATAR_SELECTOR = UTILITIES_MENU + "Avatar Selector";
        public const string UTILITY_LIGHT_TOGGLE = UTILITIES_MENU + "Add Light Toggle";
        public const string UTILITY_COMPETITOR_SETUP = UTILITIES_MENU + "Competitor Setup";
        
        public const string HELP_ABOUT = HELP_MENU + "About";
        public const string HELP_DOCUMENTATION = HELP_MENU + "Documentation";
        public const string HELP_INSTALLATION = HELP_MENU + "Check Installation";
        public const string HELP_FIX_MATERIALS = HELP_MENU + "Fix Materials";
        
        // 優先度設定（数値が小さいほど上に表示）
        public static class Priority
        {
            public const int MATERIALS = 100;
            public const int VRChat = 200;
            public const int PRESETS = 300;
            public const int UTILITIES = 400;
            public const int HELP = 500;
            
            // サブメニュー優先度
            public const int MATERIAL_UPGRADE = 101;
            public const int MATERIAL_BACKUP = 102;
            public const int MATERIAL_RESTORE = 103;
            
            public const int VRC_EXPRESSION = 201;
            public const int VRC_LIGHT_VOLUMES = 202;
            public const int VRC_OPTIMIZATION = 203;
            
            public const int PRESET_REALISTIC = 301;
            public const int PRESET_ANIME = 302;
            public const int PRESET_CINEMATIC = 303;
            public const int PRESET_PORTRAIT = 304;
            public const int PRESET_GAME = 305;
            
            public const int UTILITY_AVATAR_SELECTOR = 401;
            public const int UTILITY_LIGHT_TOGGLE = 402;
            public const int UTILITY_COMPETITOR_SETUP = 403;
            
            public const int HELP_ABOUT = 501;
            public const int HELP_DOCUMENTATION = 502;
            public const int HELP_INSTALLATION = 503;
            public const int HELP_FIX_MATERIALS = 504;
        }
    }
}
#endif 