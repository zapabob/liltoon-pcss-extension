using UnityEditor;

public class RunExporter
{
    [MenuItem("Tools/PCSS/Internal/Run Package Exporter")]
    public static void Run()
    {
        PackageExporter.ExportAndGenerateReleaseNotes();
    }
}
