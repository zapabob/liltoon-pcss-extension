using UnityEditor;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// パッケージエクスポーター実行クラス
    /// なんｊ風に言うと「これで完璧なエクスポート実行システムが完成したぜ！」💪🔥
    /// </summary>
    public class RunExporter
    {
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Package/Run Package Exporter")]
        public static void Run()
        {
            PackageExporter.ExportAndGenerateReleaseNotes();
        }
    }
}