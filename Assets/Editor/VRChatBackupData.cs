using System;
using System.Collections.Generic;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// VRChatバックアップデータの共通構造
    /// なんｊ風に言うと「これで完璧な共通バックアップ構造が完成したぜ！」💪🔥
    /// </summary>
    [System.Serializable]
    public class VRChatMaterialBackupData
    {
        public string avatarName;
        public string backupTime;
        public string version;
        public string uploadType;
        public List<MaterialBackupEntry> materials;
    }

    /// <summary>
    /// VRChat自動バックアップデータ構造
    /// なんｊ風に言うと「これで完璧な自動バックアップデータ構造が完成したぜ！」💪🔥
    /// </summary>
    [System.Serializable]
    public class VRChatAutoBackupData
    {
        public string avatarName;
        public string backupTime;
        public string version;
        public string uploadType;
        public List<MaterialBackupEntry> materials;
    }

    /// <summary>
    /// マテリアルバックアップエントリ構造
    /// なんｊ風に言うと「これで完璧なマテリアルバックアップ構造が完成したぜ！」💪🔥
    /// </summary>
    [System.Serializable]
    public class MaterialBackupEntry
    {
        public string materialName;
        public string materialGUID;
        public string materialPath;
        public string shaderName;
        public string shaderGUID;
        public string rendererName;
        public string rendererPath;
        public int materialIndex;
        public Dictionary<string, string> properties;
        public Dictionary<string, string> textureGUIDs;
    }
}