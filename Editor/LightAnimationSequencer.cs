#define MODULAR_AVATAR_EXISTS
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
// #if MODULAR_AVATAR_EXISTS
// using nadena.dev.modular_avatar.core;
// #endif
using System.IO;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// ライトアニメーションシーケンサー
    /// 時間ベースのライトアニメーションを作成・管理するシステム
    /// </summary>
    public class LightAnimationSequencer : EditorWindow
    {
        private GameObject avatarRoot;
        private LightAnimationSequence currentSequence;
        private List<LightAnimationSequence> sequences = new List<LightAnimationSequence>();
        private Vector2 scrollPos;
        private int selectedKeyframeIndex = -1;
        private float currentTime = 0f;
        private bool isPlaying = false;
        private double lastFrameTime;

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Light Animation Sequencer")]
        public static void ShowWindow()
        {
            var window = GetWindow<LightAnimationSequencer>("Light Animation Sequencer");
            window.minSize = new Vector2(600, 700);
            window.LoadSequences();
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Light Animation Sequencer", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("時間ベースのライトアニメーションを作成・管理できます。", MessageType.Info);

            EditorGUILayout.Space(10);

            // アバター設定
            avatarRoot = (GameObject)EditorGUILayout.ObjectField("Avatar Root", avatarRoot, typeof(GameObject), true);

            EditorGUILayout.Space(20);

            // シーケンス管理
            EditorGUILayout.LabelField("アニメーションシーケンス", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("New Sequence"))
            {
                CreateNewSequence();
            }

            if (currentSequence != null && GUILayout.Button("Save Sequence"))
            {
                SaveSequences();
            }

            if (currentSequence != null && GUILayout.Button("Export Animation"))
            {
                ExportAsAnimationClip();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // シーケンスリスト
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(100));

            for (int i = 0; i < sequences.Count; i++)
            {
                var sequence = sequences[i];
                EditorGUILayout.BeginHorizontal();

                GUI.color = currentSequence == sequence ? Color.cyan : Color.white;
                if (GUILayout.Button(sequence.name, EditorStyles.toolbarButton))
                {
                    currentSequence = sequence;
                    selectedKeyframeIndex = -1;
                }
                GUI.color = Color.white;

                if (GUILayout.Button("Play", EditorStyles.miniButton, GUILayout.Width(40)))
                {
                    PlaySequence(sequence);
                }

                if (GUILayout.Button("Stop", EditorStyles.miniButton, GUILayout.Width(40)))
                {
                    StopSequence();
                }

                if (GUILayout.Button("Delete", EditorStyles.miniButton, GUILayout.Width(50)))
                {
                    if (EditorUtility.DisplayDialog("Delete Sequence",
                        $"Delete sequence '{sequence.name}'?", "Delete", "Cancel"))
                    {
                        sequences.RemoveAt(i);
                        if (currentSequence == sequence) currentSequence = null;
                        SaveSequences();
                        i--;
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(20);

            if (currentSequence != null)
            {
                // シーケンス設定
                EditorGUILayout.LabelField("シーケンス設定", EditorStyles.boldLabel);

                EditorGUI.BeginChangeCheck();

                currentSequence.name = EditorGUILayout.TextField("Name", currentSequence.name);
                currentSequence.duration = EditorGUILayout.FloatField("Duration (seconds)", currentSequence.duration);
                currentSequence.loop = EditorGUILayout.Toggle("Loop", currentSequence.loop);

                if (EditorGUI.EndChangeCheck())
                {
                    SaveSequences();
                }

                EditorGUILayout.Space(10);

                // タイムライン
                DrawTimeline();

                EditorGUILayout.Space(10);

                // キーフレーム管理
                DrawKeyframeEditor();

                EditorGUILayout.Space(10);

                // プレビュータイムライン
                DrawPreviewTimeline();
            }

            EditorGUILayout.Space(10);

            // プレビュー再生
            if (isPlaying)
            {
                EditorGUILayout.LabelField("🔴 Playing...", EditorStyles.boldLabel);
                if (GUILayout.Button("Stop Preview"))
                {
                    StopSequence();
                }
            }
            else if (currentSequence != null)
            {
                if (GUILayout.Button("Play Preview"))
                {
                    PlaySequence(currentSequence);
                }
            }
        }

        private void CreateNewSequence()
        {
            var sequence = new LightAnimationSequence
            {
                name = "New Light Sequence",
                duration = 5.0f,
                loop = false,
                keyframes = new List<LightKeyframe>
                {
                    new LightKeyframe { time = 0.0f, color = Color.white, intensity = 1.0f },
                    new LightKeyframe { time = 2.5f, color = Color.red, intensity = 2.0f },
                    new LightKeyframe { time = 5.0f, color = Color.blue, intensity = 0.5f }
                }
            };

            sequences.Add(sequence);
            currentSequence = sequence;
            SaveSequences();
        }

        private void DrawTimeline()
        {
            EditorGUILayout.LabelField("タイムライン", EditorStyles.boldLabel);

            Rect timelineRect = EditorGUILayout.GetControlRect(false, 60);
            timelineRect = EditorGUI.IndentedRect(timelineRect);

            // 背景
            EditorGUI.DrawRect(timelineRect, new Color(0.2f, 0.2f, 0.2f));

            // 時間目盛
            float pixelsPerSecond = timelineRect.width / currentSequence.duration;
            int numMarkers = Mathf.CeilToInt(currentSequence.duration) + 1;

            for (int i = 0; i < numMarkers; i++)
            {
                float time = i;
                float x = timelineRect.x + time * pixelsPerSecond;

                if (x > timelineRect.xMax) break;

                // 目盛線
                EditorGUI.DrawRect(new Rect(x, timelineRect.y, 1, timelineRect.height), Color.gray);

                // 時間ラベル
                if (i % 5 == 0 || currentSequence.duration <= 10)
                {
                    GUI.Label(new Rect(x + 2, timelineRect.y, 50, 20), time.ToString("0"), EditorStyles.miniLabel);
                }
            }

            // キーフレーム
            for (int i = 0; i < currentSequence.keyframes.Count; i++)
            {
                var keyframe = currentSequence.keyframes[i];
                float x = timelineRect.x + keyframe.time * pixelsPerSecond;
                float y = timelineRect.y + timelineRect.height / 2 - 5;

                Rect keyframeRect = new Rect(x - 5, y, 10, 10);

                // キーフレーム矩形
                Color keyframeColor = selectedKeyframeIndex == i ? Color.yellow : Color.cyan;
                EditorGUI.DrawRect(keyframeRect, keyframeColor);

                // クリック検出
                if (Event.current.type == EventType.MouseDown && keyframeRect.Contains(Event.current.mousePosition))
                {
                    selectedKeyframeIndex = i;
                    Event.current.Use();
                    Repaint();
                }
            }

            // 現在の再生位置
            if (isPlaying || currentTime > 0)
            {
                float playheadX = timelineRect.x + currentTime * pixelsPerSecond;
                EditorGUI.DrawRect(new Rect(playheadX, timelineRect.y, 2, timelineRect.height), Color.red);
            }
        }

        private void DrawKeyframeEditor()
        {
            EditorGUILayout.LabelField("キーフレームエディター", EditorStyles.boldLabel);

            if (selectedKeyframeIndex >= 0 && selectedKeyframeIndex < currentSequence.keyframes.Count)
            {
                var keyframe = currentSequence.keyframes[selectedKeyframeIndex];

                EditorGUI.BeginChangeCheck();

                keyframe.time = EditorGUILayout.Slider("Time", keyframe.time, 0f, currentSequence.duration);
                keyframe.color = EditorGUILayout.ColorField("Color", keyframe.color);
                keyframe.intensity = EditorGUILayout.Slider("Intensity", keyframe.intensity, 0f, 5f);
                keyframe.range = EditorGUILayout.FloatField("Range", keyframe.range);
                keyframe.spotAngle = EditorGUILayout.Slider("Spot Angle", keyframe.spotAngle, 1f, 179f);

                if (EditorGUI.EndChangeCheck())
                {
                    SaveSequences();
                }

                EditorGUILayout.Space(5);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Add Keyframe"))
                {
                    AddKeyframe();
                }

                if (GUILayout.Button("Remove Keyframe"))
                {
                    RemoveKeyframe(selectedKeyframeIndex);
                }

                if (GUILayout.Button("Duplicate Keyframe"))
                {
                    DuplicateKeyframe(selectedKeyframeIndex);
                }

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("タイムラインからキーフレームを選択してください。", MessageType.Info);

                if (GUILayout.Button("Add Keyframe"))
                {
                    AddKeyframe();
                }
            }
        }

        private void DrawPreviewTimeline()
        {
            EditorGUILayout.LabelField("プレビュータイムライン", EditorStyles.boldLabel);

            currentTime = EditorGUILayout.Slider("Current Time", currentTime, 0f, currentSequence.duration);

            if (avatarRoot != null && currentSequence != null)
            {
                var lights = avatarRoot.GetComponentsInChildren<Light>(true);

                if (lights.Length > 0)
                {
                    var interpolated = currentSequence.Evaluate(currentTime);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ColorField("Preview Color", interpolated.color);
                    EditorGUILayout.FloatField("Preview Intensity", interpolated.intensity);
                    EditorGUILayout.EndHorizontal();

                    if (GUILayout.Button("Apply to Lights"))
                    {
                        foreach (var light in lights)
                        {
                            Undo.RecordObject(light, "Apply Animation Frame");
                            light.color = interpolated.color;
                            light.intensity = interpolated.intensity;
                            light.range = interpolated.range;
                            light.spotAngle = interpolated.spotAngle;
                        }

                        EditorUtility.SetDirty(avatarRoot);
                    }
                }
            }
        }

        private void AddKeyframe()
        {
            float newTime = currentSequence.keyframes.Count > 0 ?
                currentSequence.keyframes.Last().time + 1.0f : 0.0f;

            newTime = Mathf.Clamp(newTime, 0f, currentSequence.duration);

            var newKeyframe = new LightKeyframe
            {
                time = newTime,
                color = Color.white,
                intensity = 1.0f,
                range = 5.0f,
                spotAngle = 30.0f
            };

            currentSequence.keyframes.Add(newKeyframe);
            selectedKeyframeIndex = currentSequence.keyframes.Count - 1;
            SaveSequences();
        }

        private void RemoveKeyframe(int index)
        {
            if (index >= 0 && index < currentSequence.keyframes.Count)
            {
                currentSequence.keyframes.RemoveAt(index);
                selectedKeyframeIndex = Mathf.Clamp(selectedKeyframeIndex, 0, currentSequence.keyframes.Count - 1);
                if (currentSequence.keyframes.Count == 0) selectedKeyframeIndex = -1;
                SaveSequences();
            }
        }

        private void DuplicateKeyframe(int index)
        {
            if (index >= 0 && index < currentSequence.keyframes.Count)
            {
                var original = currentSequence.keyframes[index];
                var duplicate = new LightKeyframe
                {
                    time = Mathf.Clamp(original.time + 0.5f, 0f, currentSequence.duration),
                    color = original.color,
                    intensity = original.intensity,
                    range = original.range,
                    spotAngle = original.spotAngle
                };

                currentSequence.keyframes.Insert(index + 1, duplicate);
                selectedKeyframeIndex = index + 1;
                SaveSequences();
            }
        }

        private void PlaySequence(LightAnimationSequence sequence)
        {
            if (avatarRoot == null) return;

            StopSequence();

            currentSequence = sequence;
            isPlaying = true;
            currentTime = 0f;
            lastFrameTime = EditorApplication.timeSinceStartup;

            EditorApplication.update += UpdateAnimation;
        }

        private void StopSequence()
        {
            isPlaying = false;
            EditorApplication.update -= UpdateAnimation;
        }

        private void UpdateAnimation()
        {
            if (!isPlaying || currentSequence == null || avatarRoot == null) return;

            double currentFrameTime = EditorApplication.timeSinceStartup;
            float deltaTime = (float)(currentFrameTime - lastFrameTime);
            lastFrameTime = currentFrameTime;

            currentTime += deltaTime;

            if (currentTime >= currentSequence.duration)
            {
                if (currentSequence.loop)
                {
                    currentTime = currentTime % currentSequence.duration;
                }
                else
                {
                    StopSequence();
                    return;
                }
            }

            var interpolated = currentSequence.Evaluate(currentTime);
            var lights = avatarRoot.GetComponentsInChildren<Light>(true);

            foreach (var light in lights)
            {
                light.color = interpolated.color;
                light.intensity = interpolated.intensity;
                light.range = interpolated.range;
                light.spotAngle = interpolated.spotAngle;
            }

            Repaint();
        }

        private void ExportAsAnimationClip()
        {
            if (currentSequence == null || avatarRoot == null) return;

            var lights = avatarRoot.GetComponentsInChildren<Light>(true);
            if (lights.Length == 0) return;

            var clip = new AnimationClip();
            clip.name = currentSequence.name;

            // カーブの作成
            foreach (var light in lights)
            {
                string path = GetRelativePath(avatarRoot.transform, light.transform);

                // 色のカーブ
                clip.SetCurve(path, typeof(Light), "m_Color.r", CreateColorCurve(0));
                clip.SetCurve(path, typeof(Light), "m_Color.g", CreateColorCurve(1));
                clip.SetCurve(path, typeof(Light), "m_Color.b", CreateColorCurve(2));
                clip.SetCurve(path, typeof(Light), "m_Color.a", CreateColorCurve(3));

                // 強度のカーブ
                clip.SetCurve(path, typeof(Light), "m_Intensity", CreateIntensityCurve());

                // 範囲のカーブ
                clip.SetCurve(path, typeof(Light), "m_Range", CreateRangeCurve());

                // スポット角度のカーブ
                if (light.type == LightType.Spot)
                {
                    clip.SetCurve(path, typeof(Light), "m_SpotAngle", CreateSpotAngleCurve());
                }
            }

            // アニメーションクリップの保存
            string savePath = $"Assets/_Generated/{currentSequence.name}.anim";
            EnsureDirectoryExists(savePath);
            AssetDatabase.CreateAsset(clip, savePath);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("Export Complete",
                $"Animation clip exported to:\n{savePath}", "OK");
        }

        private AnimationCurve CreateColorCurve(int channel)
        {
            var curve = new AnimationCurve();

            foreach (var keyframe in currentSequence.keyframes)
            {
                float value = channel switch
                {
                    0 => keyframe.color.r,
                    1 => keyframe.color.g,
                    2 => keyframe.color.b,
                    3 => keyframe.color.a,
                    _ => 1.0f
                };

                curve.AddKey(new Keyframe(keyframe.time, value));
            }

            return curve;
        }

        private AnimationCurve CreateIntensityCurve()
        {
            var curve = new AnimationCurve();

            foreach (var keyframe in currentSequence.keyframes)
            {
                curve.AddKey(new Keyframe(keyframe.time, keyframe.intensity));
            }

            return curve;
        }

        private AnimationCurve CreateRangeCurve()
        {
            var curve = new AnimationCurve();

            foreach (var keyframe in currentSequence.keyframes)
            {
                curve.AddKey(new Keyframe(keyframe.time, keyframe.range));
            }

            return curve;
        }

        private AnimationCurve CreateSpotAngleCurve()
        {
            var curve = new AnimationCurve();

            foreach (var keyframe in currentSequence.keyframes)
            {
                curve.AddKey(new Keyframe(keyframe.time, keyframe.spotAngle));
            }

            return curve;
        }

        private string GetRelativePath(Transform root, Transform target)
        {
            if (target == root) return "";

            var path = target.name;
            var current = target.parent;

            while (current != null && current != root)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }

            return path;
        }

        private void SaveSequences()
        {
            var saveData = new LightAnimationSaveData
            {
                sequences = sequences.ToArray()
            };

            string json = JsonUtility.ToJson(saveData, true);
            string savePath = "Assets/_Generated/LightAnimationSequences.json";

            EnsureDirectoryExists(savePath);
            File.WriteAllText(savePath, json);
            AssetDatabase.Refresh();
        }

        private void LoadSequences()
        {
            string loadPath = "Assets/_Generated/LightAnimationSequences.json";

            if (File.Exists(loadPath))
            {
                string json = File.ReadAllText(loadPath);
                var saveData = JsonUtility.FromJson<LightAnimationSaveData>(json);

                if (saveData != null && saveData.sequences != null)
                {
                    sequences = saveData.sequences.ToList();
                }
            }
        }

        private void EnsureDirectoryExists(string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private void OnEnable()
        {
            LoadSequences();
        }

        private void OnDisable()
        {
            SaveSequences();
        }
    }

    [System.Serializable]
    public class LightKeyframe
    {
        public float time;
        public Color color;
        public float intensity;
        public float range;
        public float spotAngle;
    }

    [System.Serializable]
    public class LightAnimationSequence
    {
        public string name;
        public float duration;
        public bool loop;
        public List<LightKeyframe> keyframes;

        public LightKeyframe Evaluate(float time)
        {
            if (keyframes == null || keyframes.Count == 0)
            {
                return new LightKeyframe { color = Color.white, intensity = 1.0f, range = 5.0f, spotAngle = 30.0f };
            }

            if (keyframes.Count == 1)
            {
                return keyframes[0];
            }

            // 時間範囲内に収める
            if (loop)
            {
                time = time % duration;
            }
            else
            {
                time = Mathf.Clamp(time, 0f, duration);
            }

            // キーフレームを探す
            LightKeyframe start = keyframes[0];
            LightKeyframe end = keyframes[keyframes.Count - 1];

            for (int i = 0; i < keyframes.Count - 1; i++)
            {
                if (time >= keyframes[i].time && time <= keyframes[i + 1].time)
                {
                    start = keyframes[i];
                    end = keyframes[i + 1];
                    break;
                }
            }

            // 補間
            float t = (time - start.time) / (end.time - start.time);
            t = Mathf.Clamp01(t);

            return new LightKeyframe
            {
                color = Color.Lerp(start.color, end.color, t),
                intensity = Mathf.Lerp(start.intensity, end.intensity, t),
                range = Mathf.Lerp(start.range, end.range, t),
                spotAngle = Mathf.Lerp(start.spotAngle, end.spotAngle, t)
            };
        }
    }

    [System.Serializable]
    public class LightAnimationSaveData
    {
        public LightAnimationSequence[] sequences;
    }
}
