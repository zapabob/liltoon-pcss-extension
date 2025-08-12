using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class StartupLogAnnouncer
{
    static StartupLogAnnouncer()
    {
        try
        {
            // ルート/_docs または Assets/_docs のどちらでも検索
            var candidateFolders = new[] { "_docs", "Assets/_docs" }
                .Where(AssetDatabase.IsValidFolder)
                .ToArray();
            if (candidateFolders.Length == 0) return;

            var mdGuids = candidateFolders
                .SelectMany(f => AssetDatabase.FindAssets("t:TextAsset", new[] { f }))
                .ToArray();
            if (mdGuids.Length == 0) return;

            var latest = mdGuids
                .Select(g => AssetDatabase.GUIDToAssetPath(g))
                .Select(p => new { path = p, time = File.GetLastWriteTime(p) })
                .OrderByDescending(x => x.time)
                .First();

            var ta = AssetDatabase.LoadAssetAtPath<TextAsset>(latest.path);
            if (ta != null)
            {
                var preview = ta.text;
                if (preview.Length > 400) preview = preview.Substring(0, 400) + "...";
                Debug.Log($"【なんJ】最新の実装ログ読むで→ {latest.path}\n---\n{preview}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[StartupLogAnnouncer] {ex.Message}");
        }
    }
}


