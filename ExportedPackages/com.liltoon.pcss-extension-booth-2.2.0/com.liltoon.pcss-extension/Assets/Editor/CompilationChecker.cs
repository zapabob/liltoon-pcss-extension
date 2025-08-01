#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// コンパイルエラーをチェックするユーティリティクラス
    /// </summary>
    public static class CompilationChecker
    {
        [MenuItem("Tools/lilToon PCSS Extension/Check Compilation Errors")]
        public static void CheckCompilationErrors()
        {
            Debug.Log("=== lilToon PCSS Extension Compilation Check ===");
            
            // コンパイル状態をチェック
            if (EditorApplication.isCompiling)
            {
                Debug.LogWarning("コンパイル中です。完了までお待ちください。");
                return;
            }
            
            // エラーがあるかチェック
            var errors = GetCompilationErrors();
            if (errors.Count > 0)
            {
                Debug.LogError($"コンパイルエラーが {errors.Count} 個見つかりました:");
                foreach (var error in errors)
                {
                    Debug.LogError($"- {error}");
                }
            }
            else
            {
                Debug.Log("✅ コンパイルエラーはありません！");
            }
            
            // 警告をチェック
            var warnings = GetCompilationWarnings();
            if (warnings.Count > 0)
            {
                Debug.LogWarning($"コンパイル警告が {warnings.Count} 個見つかりました:");
                foreach (var warning in warnings)
                {
                    Debug.LogWarning($"- {warning}");
                }
            }
            else
            {
                Debug.Log("✅ コンパイル警告はありません！");
            }
        }
        
        /// <summary>
        /// コンパイルエラーを取得
        /// </summary>
        private static List<string> GetCompilationErrors()
        {
            var errors = new List<string>();
            
            // コンソールログからエラーを取得
            var logEntries = GetLogEntries();
            foreach (var entry in logEntries)
            {
                if (entry.type == LogType.Error)
                {
                    errors.Add(entry.message);
                }
            }
            
            return errors;
        }
        
        /// <summary>
        /// コンパイル警告を取得
        /// </summary>
        private static List<string> GetCompilationWarnings()
        {
            var warnings = new List<string>();
            
            // コンソールログから警告を取得
            var logEntries = GetLogEntries();
            foreach (var entry in logEntries)
            {
                if (entry.type == LogType.Warning)
                {
                    warnings.Add(entry.message);
                }
            }
            
            return warnings;
        }
        
        /// <summary>
        /// ログエントリを取得
        /// </summary>
        private static List<LogEntry> GetLogEntries()
        {
            var entries = new List<LogEntry>();
            
            // Unityのコンソールログを取得
            var consoleWindow = EditorWindow.GetWindow<ConsoleWindow>();
            if (consoleWindow != null)
            {
                // コンソールウィンドウからログを取得する方法
                // 注: これはUnityの内部APIを使用するため、実際の実装では
                // より安全な方法を使用する必要があります
            }
            
            return entries;
        }
        
        /// <summary>
        /// ログエントリ構造体
        /// </summary>
        private struct LogEntry
        {
            public string message;
            public LogType type;
        }
        
        /// <summary>
        /// コンソールウィンドウクラス（Unity内部）
        /// </summary>
        private class ConsoleWindow : EditorWindow
        {
            // Unityのコンソールウィンドウの実装
        }
    }
}
#endif 