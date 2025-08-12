#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class MenuNameRefactorTool
{
    private const string MenuRoot = "Tools/lilToon-PCSS-Extension/";

    [MenuItem("Tools/lilToon-PCSS-Extension/Utilities/Refactor Menu Names", priority = 9999)]
    public static void RefactorMenuNames()
    {
        string[] files = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
        int count = 0;
        foreach (var file in files)
        {
            string text = File.ReadAllText(file);
            string original = text;

            // 旧メニュー名・日本語・表記揺れを一括置換
            // 例: Tools/lilToon PCSS/ → Tools/lilToon-PCSS-Extension/
            text = Regex.Replace(text, @"MenuItem\(""Tools/lilToon-PCSS-Extension/", $"MenuItem(\"{MenuRoot}");
            text = Regex.Replace(text, @"MenuItem\(""Tools/lilToon-PCSS-Extension/", $"MenuItem(\"{MenuRoot}");
            text = Regex.Replace(text, @"MenuItem\(""Tools/lilToon-PCSS-Extension/", $"MenuItem(\"{MenuRoot}");
            text = Regex.Replace(text, @"MenuItem\(""Tools/lilToon-PCSS-Extension/", $"MenuItem(\"{MenuRoot}");
            text = Regex.Replace(text, @"MenuItem\(""Tools/lilToon-PCSS-Extension/", $"MenuItem(\"{MenuRoot}");
            text = Regex.Replace(text, @"MenuItem\(""Tools/lilToon-PCSS-Extension/", $"MenuItem(\"{MenuRoot}");
            text = Regex.Replace(text, @"MenuItem\(""Tools/lilToon-PCSS-Extension/", $"MenuItem(\"{MenuRoot}");
            // 単独メニュー名
            text = Regex.Replace(text, @"MenuItem\(""Tools/Material Backup""\)", $"MenuItem(\"{MenuRoot}Material Backup\")");

            // 文字列リテラルとしてのルートも置換（MenuItem以外に定義されたメニューパス定数類）
            text = Regex.Replace(text, @"""Tools/lilToon-PCSS-Extension/", $"\"{MenuRoot}");
            text = Regex.Replace(text, @"""Tools/lilToon-PCSS-Extension/", $"\"{MenuRoot}");
            text = Regex.Replace(text, @"""Tools/lilToon-PCSS-Extension/", $"\"{MenuRoot}");
            text = Regex.Replace(text, @"""Tools/lilToon-PCSS-Extension/", $"\"{MenuRoot}");
            text = Regex.Replace(text, @"""Tools/lilToon-PCSS-Extension/", $"\"{MenuRoot}");
            text = Regex.Replace(text, @"""Tools/lilToon-PCSS-Extension/", $"\"{MenuRoot}");
            text = Regex.Replace(text, @"""Tools/lilToon-PCSS-Extension/", $"\"{MenuRoot}");

            // 二重ネストの解消
            text = Regex.Replace(
                text,
                Regex.Escape(MenuRoot) + @"(?:lilToon(?: |-|)PCSS(?: ?Extension)?/)+",
                MenuRoot
            );
            // 日本語メニュー名を英語に統一
            text = Regex.Replace(text, @"Competitor Setup Wizard", "Competitor Setup Wizard");
            text = Regex.Replace(text, @"Add Light Toggle", "Add Light Toggle");
            text = Regex.Replace(text, @"Create Expression Menu", "Create Expression Menu");
            text = Regex.Replace(text, @"Upgrade Materials", "Upgrade Materials");
            text = Regex.Replace(text, @"Material Backup/Restore", "Material Backup/Restore");
            text = Regex.Replace(text, @"Fix Missing Materials", "Fix Missing Materials");
            // その他の日本語→英語変換もここに追加

            if (text != original)
            {
                File.WriteAllText(file, text);
                Debug.Log($"[MenuNameRefactorTool] MenuItemパスを修正: {file}");
                count++;
            }
        }
        EditorUtility.DisplayDialog("Menu名リファクタリング完了", $"{count}ファイルを修正しました。", "OK");
    }
}
#endif 