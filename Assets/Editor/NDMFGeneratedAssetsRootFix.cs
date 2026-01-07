using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace lilToon.PCSS.Editor
{
	/// <summary>
	/// NDMF(Modular Avatar) の生成アセット保存先が空でビルド失敗する問題に対処
	/// - 既定の生成先フォルダを強制作成: Assets/NDMFGenerated
	/// - 可能ならNDMFの設定に反映（リフレクション/EditorPrefsフォールバック）
	/// - 無効文字を含むアバター名を一括サニタイズ
	/// </summary>
	[InitializeOnLoad]
	public static class NDMFGeneratedAssetsRootFix
	{
		private const string DefaultGeneratedRoot = "Assets/NDMFGenerated";
		private static readonly string[] EditorPrefsKeys =
		{
			"nadena.dev.ndmf.generatedAssetsRoot",
			"ndmf.generatedAssetsRoot"
		};

		static NDMFGeneratedAssetsRootFix()
		{
			// 起動時にフォルダ/設定を整える
			EnsureGeneratedAssetsRootExists();
			TrySetNdmfGeneratedRoot(DefaultGeneratedRoot);
		}

		[MenuItem("Tools/NDMF/Set Generated Assets Root (Fix)")]
		private static void MenuSetGeneratedRoot()
		{
			EnsureGeneratedAssetsRootExists();
			TrySetNdmfGeneratedRoot(DefaultGeneratedRoot);
			EditorUtility.DisplayDialog("NDMF Generated Root",
				$"Generated assets root set to:\n{DefaultGeneratedRoot}", "OK");
		}

		[MenuItem("Tools/NDMF/Sanitize Avatar Names (Remove invalid chars)")]
		private static void MenuSanitizeAvatarNames()
		{
			var selected = Selection.gameObjects;
			if (selected == null || selected.Length == 0)
			{
				EditorUtility.DisplayDialog("Sanitize Avatar Names", "Select avatar GameObjects in Hierarchy first.", "OK");
				return;
			}

			int changed = 0;
			foreach (var go in selected)
			{
				string original = go.name;
				string sanitized = SanitizeName(original);
				if (sanitized != original)
				{
					Undo.RecordObject(go, "Sanitize Avatar Name");
					go.name = sanitized;
					changed++;
				}
			}

			EditorUtility.DisplayDialog("Sanitize Avatar Names",
				$"Processed: {selected.Length}\nRenamed: {changed}", "OK");
		}

		private static void EnsureGeneratedAssetsRootExists()
		{
			if (!AssetDatabase.IsValidFolder("Assets")) return;
			if (!AssetDatabase.IsValidFolder(DefaultGeneratedRoot))
			{
				string[] parts = DefaultGeneratedRoot.Split('/');
				string current = parts[0]; // "Assets"
				for (int i = 1; i < parts.Length; i++)
				{
					string next = parts[i];
					string combined = string.IsNullOrEmpty(current) ? next : current + "/" + next;
					if (!AssetDatabase.IsValidFolder(combined))
					{
						AssetDatabase.CreateFolder(current, next);
					}
					current = combined;
				}
			}
		}

		private static void TrySetNdmfGeneratedRoot(string path)
		{
			bool applied = false;

			// 1) リフレクションでそれっぽい設定に書き込む
			try
			{
				var assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (var asm in assemblies)
				{
					Type[] types;
					try { types = asm.GetTypes(); }
					catch (ReflectionTypeLoadException e) { types = e.Types.Where(t => t != null).ToArray(); }

					foreach (var t in types)
					{
						if (t.Namespace == null || !t.Namespace.Contains("nadena.dev.ndmf")) continue;

						// 静的プロパティ/フィールド候補
						var prop = t.GetProperty("GeneratedAssetsRoot", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
						if (prop != null && prop.PropertyType == typeof(string))
						{
							var cur = prop.GetValue(null) as string;
							if (string.IsNullOrEmpty(cur))
							{
								prop.SetValue(null, path);
								applied = true;
							}
						}

						var field = t.GetField("GeneratedAssetsRoot", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
						if (!applied && field != null && field.FieldType == typeof(string))
						{
							var cur = field.GetValue(null) as string;
							if (string.IsNullOrEmpty(cur))
							{
								field.SetValue(null, path);
								applied = true;
							}
						}

						var setMethod = t.GetMethod("SetGeneratedAssetsRoot", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(string) }, null);
						if (!applied && setMethod != null)
						{
							setMethod.Invoke(null, new object[] { path });
							applied = true;
						}
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning($"NDMF reflection apply failed: {e.Message}");
			}

			// 2) フォールバック: EditorPrefs キーに書き込み
			if (!applied)
			{
				foreach (var key in EditorPrefsKeys)
				{
					try { EditorPrefs.SetString(key, path); applied = true; }
					catch { /* ignore */ }
				}
			}

			if (applied)
			{
				Debug.Log($"[NDMF] Generated assets root ensured: {path}");
			}
			else
			{
				Debug.LogWarning("[NDMF] Could not set generated assets root via reflection/EditorPrefs. NDMF may manage it internally. Folder was created as fallback.");
			}
		}

		private static string SanitizeName(string name)
		{
			if (string.IsNullOrEmpty(name)) return "Avatar";
			// Unityのアセットパスで問題になりやすい文字を除去/置換
			char[] invalid = System.IO.Path.GetInvalidFileNameChars().Concat(new[] { '/', '\\' }).ToArray();
			var sanitized = new string(name.Where(c => !invalid.Contains(c)).ToArray());
			if (string.IsNullOrWhiteSpace(sanitized)) sanitized = "Avatar";
			return sanitized;
		}
	}
}


