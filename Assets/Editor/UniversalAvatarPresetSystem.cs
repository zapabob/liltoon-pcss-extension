#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using VRC.SDK3.Avatars.Components;

// ↓ これをガードで囲う（今のCS0246の根本）
#if HAS_MODULAR_AVATAR
using nadena.dev.modular_avatar.core;
#endif

namespace lilToonPCSS.Editor
{
    /// <summary>
    /// 汎用アバター対応プリセットシステム
    /// Modular Avatarを使用してどんなアバターでも適用可能
    /// </summary>
    public class UniversalAvatarPresetSystem
    {
        [MenuItem(MenuConstants.PRESET_REALISTIC, false, MenuConstants.Priority.PRESET_REALISTIC)]
        public static void ApplyRealisticPreset()
        {
            ApplyUniversalPreset("Realistic", CreateRealisticPreset());
        }

        [MenuItem(MenuConstants.PRESET_ANIME, false, MenuConstants.Priority.PRESET_ANIME)]
        public static void ApplyAnimePreset()
        {
            ApplyUniversalPreset("Anime", CreateAnimePreset());
        }

        [MenuItem(MenuConstants.PRESET_CINEMATIC, false, MenuConstants.Priority.PRESET_CINEMATIC)]
        public static void ApplyCinematicPreset()
        {
            ApplyUniversalPreset("Cinematic", CreateCinematicPreset());
        }

        [MenuItem(MenuConstants.PRESET_PORTRAIT, false, MenuConstants.Priority.PRESET_PORTRAIT)]
        public static void ApplyPortraitPreset()
        {
            ApplyUniversalPreset("Portrait", CreatePortraitPreset());
        }

        [MenuItem(MenuConstants.PRESET_GAME, false, MenuConstants.Priority.PRESET_GAME)]
        public static void ApplyGamePreset()
        {
            ApplyUniversalPreset("Game", CreateGamePreset());
        }

        /// <summary>
        /// 汎用プリセットを適用
        /// </summary>
        private static void ApplyUniversalPreset(string presetName, PresetData presetData)
        {
            // Modular Avatarがインストールされているかチェック
            if (!IsModularAvatarInstalled())
            {
                EditorUtility.DisplayDialog("Modular Avatar Required", 
                    $"The {presetName} preset requires Modular Avatar to work with any avatar.\n\n" +
                    "Please install Modular Avatar from:\n" +
                    "https://github.com/bdunderscore/modular-avatar", "OK");
                return;
            }

            // 選択されたアバターをチェック
            GameObject selectedAvatar = Selection.activeGameObject;
            if (selectedAvatar == null)
            {
                EditorUtility.DisplayDialog("No Avatar Selected", 
                    "Please select an avatar to apply the preset to.", "OK");
                return;
            }

            // プリセットシステムを作成
            CreateUniversalPresetSystem(selectedAvatar, presetName, presetData);
        }

        /// <summary>
        /// Modular Avatarがインストールされているかチェック
        /// </summary>
        private static bool IsModularAvatarInstalled()
        {
            return System.Type.GetType("nadena.dev.modular_avatar.core.ModularAvatarMergeAnimator") != null;
        }

        /// <summary>
        /// 汎用プリセットシステムを作成
        /// </summary>
        private static void CreateUniversalPresetSystem(GameObject avatar, string presetName, PresetData presetData)
        {
            try
            {
                // プリセット用のオブジェクトを作成
                GameObject presetObject = new GameObject($"{presetName}Preset_MA");
                presetObject.transform.SetParent(avatar.transform);

                // Modular Avatarコンポーネントを追加
                AddModularAvatarComponents(presetObject, presetName);

                // プリセットデータを適用
                ApplyPresetData(presetObject, presetData);

                // アニメーターコントローラーを作成
                RuntimeAnimatorController animatorController = CreatePresetAnimator(presetName, presetData);
                var animator = presetObject.GetComponent<Animator>() ?? presetObject.AddComponent<Animator>();
                animator.runtimeAnimatorController = animatorController;

                // Modular Avatar Merge Animatorを設定
                #if HAS_MODULAR_AVATAR
                var mergeAnimator = presetObject.GetComponent<ModularAvatarMergeAnimator>();
                if (mergeAnimator != null)
                {
                    mergeAnimator.animator = animatorController;
                    mergeAnimator.layerType = VRCAvatarDescriptor.AnimLayerType.FX;
                    mergeAnimator.layerPriority = 200;
                    mergeAnimator.pathMode = ModularAvatarMergeAnimatorPathMode.Relative;
                    mergeAnimator.deleteAttachedAnimator = true;
                }
                #endif

                // 成功メッセージ
                EditorUtility.DisplayDialog($"{presetName} Preset Applied", 
                    $"The {presetName} preset has been applied to '{avatar.name}'.\n\n" +
                    "Features:\n" +
                    "• Works with any avatar (Modular Avatar compatible)\n" +
                    "• Automatic parameter setup\n" +
                    "• FX layer integration\n" +
                    "• Relative path mode for portability\n" +
                    $"• {presetData.description}", "OK");

                // 作成されたオブジェクトを選択
                Selection.activeGameObject = presetObject;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to apply {presetName} preset: {e.Message}");
                EditorUtility.DisplayDialog("Error", 
                    $"Failed to apply {presetName} preset:\n{e.Message}", "OK");
            }
        }

        /// <summary>
        /// Modular Avatarコンポーネントを追加
        /// </summary>
        private static void AddModularAvatarComponents(GameObject obj, string presetName)
        {
            // Modular Avatar Merge Animatorを追加
            #if HAS_MODULAR_AVATAR
            var ma = obj.AddComponent<ModularAvatarMergeAnimator>();
            var menuItem = obj.AddComponent<ModularAvatarMenuItem>();
            if (menuItem != null)
            {
                menuItem.control = new ModularAvatarMenuGroup
                {
                    name = $"{presetName} Preset",
                    targetObject = new AvatarObjectReference { reference = obj }
                };
            }
            #endif
        }

        /// <summary>
        /// プリセットデータを適用
        /// </summary>
        private static void ApplyPresetData(GameObject presetObject, PresetData presetData)
        {
            // ライトを作成
            if (presetData.lightSettings != null)
            {
                foreach (var lightSetting in presetData.lightSettings)
                {
                    GameObject lightObj = new GameObject(lightSetting.name);
                    lightObj.transform.SetParent(presetObject.transform);
                    
                    Light light = lightObj.AddComponent<Light>();
                    light.type = lightSetting.type;
                    light.color = lightSetting.color;
                    light.intensity = lightSetting.intensity;
                    light.range = lightSetting.range;
                    light.spotAngle = lightSetting.spotAngle;
                    
                    lightObj.SetActive(lightSetting.initialState);
                }
            }

            // エフェクトを作成
            if (presetData.effects != null)
            {
                foreach (var effect in presetData.effects)
                {
                    GameObject effectObj = new GameObject(effect.name);
                    effectObj.transform.SetParent(presetObject.transform);
                    
                    // エフェクトコンポーネントを追加（例：ParticleSystem）
                    if (effect.type == EffectType.Particle)
                    {
                        var particleSystem = effectObj.AddComponent<ParticleSystem>();
                        // パーティクルシステムの設定
                    }
                    
                    effectObj.SetActive(effect.initialState);
                }
            }
        }

        /// <summary>
        /// プリセット用アニメーターコントローラーを作成
        /// </summary>
        private static RuntimeAnimatorController CreatePresetAnimator(string presetName, PresetData presetData)
        {
            var controller = new AnimatorController();
            controller.name = $"{presetName}PresetController";
            
            // パラメータを追加
            controller.AddParameter($"{presetName}Toggle", AnimatorControllerParameterType.Bool);
            
            // レイヤーを作成
            var layer = new AnimatorControllerLayer();
            layer.name = $"{presetName}Preset";
            layer.defaultWeight = 1f;
            
            var stateMachine = new AnimatorStateMachine();
            layer.stateMachine = stateMachine;
            
            // オフ状態を作成
            var offState = stateMachine.AddState("Off");
            offState.motion = null;
            
            // オン状態を作成
            var onState = stateMachine.AddState("On");
            onState.motion = null;
            
            // トランジションを作成
            var offToOn = offState.AddTransition(onState);
            offToOn.hasExitTime = false;
            offToOn.duration = 0.1f;
            offToOn.AddCondition(AnimatorConditionMode.If, 0, $"{presetName}Toggle");
            
            var onToOff = onState.AddTransition(offState);
            onToOff.hasExitTime = false;
            onToOff.duration = 0.1f;
            onToOff.AddCondition(AnimatorConditionMode.IfNot, 0, $"{presetName}Toggle");
            
            controller.AddLayer(layer);
            
            return controller;
        }

        /// <summary>
        /// リアル風プリセットデータを作成
        /// </summary>
        private static PresetData CreateRealisticPreset()
        {
            return new PresetData
            {
                description = "Realistic lighting with soft shadows and natural colors",
                lightSettings = new List<LightSetting>
                {
                    new LightSetting
                    {
                        name = "MainLight",
                        type = LightType.Directional,
                        color = new Color(1f, 0.95f, 0.8f), // 暖かい白
                        intensity = 1.2f,
                        range = 10f,
                        spotAngle = 30f,
                        initialState = true
                    },
                    new LightSetting
                    {
                        name = "FillLight",
                        type = LightType.Point,
                        color = new Color(0.8f, 0.9f, 1f), // 冷たい白
                        intensity = 0.8f,
                        range = 5f,
                        spotAngle = 30f,
                        initialState = true
                    }
                },
                effects = new List<EffectSetting>
                {
                    new EffectSetting
                    {
                        name = "Atmosphere",
                        type = EffectType.Particle,
                        initialState = true
                    }
                }
            };
        }

        /// <summary>
        /// アニメ風プリセットデータを作成
        /// </summary>
        private static PresetData CreateAnimePreset()
        {
            return new PresetData
            {
                description = "Anime-style lighting with vibrant colors and sharp contrasts",
                lightSettings = new List<LightSetting>
                {
                    new LightSetting
                    {
                        name = "AnimeMain",
                        type = LightType.Spot,
                        color = new Color(1f, 0.7f, 0.9f), // ピンク
                        intensity = 1.5f,
                        range = 8f,
                        spotAngle = 45f,
                        initialState = true
                    },
                    new LightSetting
                    {
                        name = "AnimeAccent",
                        type = LightType.Point,
                        color = new Color(0.7f, 0.9f, 1f), // シアン
                        intensity = 1.0f,
                        range = 4f,
                        spotAngle = 30f,
                        initialState = true
                    }
                }
            };
        }

        /// <summary>
        /// 映画風プリセットデータを作成
        /// </summary>
        private static PresetData CreateCinematicPreset()
        {
            return new PresetData
            {
                description = "Cinematic lighting with dramatic shadows and moody atmosphere",
                lightSettings = new List<LightSetting>
                {
                    new LightSetting
                    {
                        name = "CinematicKey",
                        type = LightType.Spot,
                        color = new Color(1f, 0.9f, 0.7f), // 暖かい白
                        intensity = 2.0f,
                        range = 12f,
                        spotAngle = 30f,
                        initialState = true
                    },
                    new LightSetting
                    {
                        name = "CinematicRim",
                        type = LightType.Spot,
                        color = new Color(0.6f, 0.7f, 1f), // 冷たい白
                        intensity = 1.5f,
                        range = 10f,
                        spotAngle = 60f,
                        initialState = true
                    }
                }
            };
        }

        /// <summary>
        /// ポートレート風プリセットデータを作成
        /// </summary>
        private static PresetData CreatePortraitPreset()
        {
            return new PresetData
            {
                description = "Portrait lighting optimized for character photography",
                lightSettings = new List<LightSetting>
                {
                    new LightSetting
                    {
                        name = "PortraitMain",
                        type = LightType.Area,
                        color = Color.white,
                        intensity = 1.8f,
                        range = 6f,
                        spotAngle = 30f,
                        initialState = true
                    },
                    new LightSetting
                    {
                        name = "PortraitFill",
                        type = LightType.Point,
                        color = new Color(0.9f, 0.9f, 1f),
                        intensity = 0.6f,
                        range = 4f,
                        spotAngle = 30f,
                        initialState = true
                    }
                }
            };
        }

        /// <summary>
        /// ゲーム風プリセットデータを作成
        /// </summary>
        private static PresetData CreateGamePreset()
        {
            return new PresetData
            {
                description = "Game-style lighting with dynamic effects and vibrant colors",
                lightSettings = new List<LightSetting>
                {
                    new LightSetting
                    {
                        name = "GameMain",
                        type = LightType.Point,
                        color = new Color(1f, 0.8f, 0.6f), // オレンジ
                        intensity = 1.2f,
                        range = 7f,
                        spotAngle = 30f,
                        initialState = true
                    },
                    new LightSetting
                    {
                        name = "GameAccent",
                        type = LightType.Spot,
                        color = new Color(0.6f, 1f, 0.8f), // 緑
                        intensity = 0.8f,
                        range = 5f,
                        spotAngle = 40f,
                        initialState = true
                    }
                },
                effects = new List<EffectSetting>
                {
                    new EffectSetting
                    {
                        name = "GameEffect",
                        type = EffectType.Particle,
                        initialState = true
                    }
                }
            };
        }

        /// <summary>
        /// プリセットデータ構造体
        /// </summary>
        public class PresetData
        {
            public string description;
            public List<LightSetting> lightSettings;
            public List<EffectSetting> effects;
        }

        /// <summary>
        /// ライト設定構造体
        /// </summary>
        public class LightSetting
        {
            public string name;
            public LightType type;
            public Color color;
            public float intensity;
            public float range;
            public float spotAngle;
            public bool initialState;
        }

        /// <summary>
        /// エフェクト設定構造体
        /// </summary>
        public class EffectSetting
        {
            public string name;
            public EffectType type;
            public bool initialState;
        }

        /// <summary>
        /// エフェクトタイプ列挙
        /// </summary>
        public enum EffectType
        {
            Particle,
            Trail,
            Glow
        }
    }
}
#endif 