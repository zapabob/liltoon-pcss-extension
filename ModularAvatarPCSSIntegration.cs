using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if UNITY_EDITOR
namespace lilToon.PCSS
{
    /// <summary>
    /// ModularAvatarとの深い統合を提供するクラス
    /// </summary>
    public class ModularAvatarPCSSIntegration
    {
        private static System.Type modularAvatarMenuInstallerType;
        private static System.Type modularAvatarParametersType;
        private static System.Type modularAvatarMergeAnimatorType;
        
        static ModularAvatarPCSSIntegration()
        {
            // ModularAvatarの型を動的に取得
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                try
                {
                    modularAvatarMenuInstallerType = assembly.GetType("nadena.dev.modular_avatar.core.ModularAvatarMenuInstaller");
                    modularAvatarParametersType = assembly.GetType("nadena.dev.modular_avatar.core.ModularAvatarParameters");
                    modularAvatarMergeAnimatorType = assembly.GetType("nadena.dev.modular_avatar.core.ModularAvatarMergeAnimator");
                    
                    if (modularAvatarMenuInstallerType != null) break;
                }
                catch { }
            }
        }
        
        /// <summary>
        /// ModularAvatarが利用可能かチェック
        /// </summary>
        public static bool IsModularAvatarAvailable()
        {
            return modularAvatarMenuInstallerType != null;
        }
        
        /// <summary>
        /// アバターにPCSS制御メニューを追加
        /// </summary>
        public static void AddPCSSControlMenu(GameObject avatar)
        {
            if (!IsModularAvatarAvailable())
            {
                Debug.LogWarning("ModularAvatarが見つかりません。通常のアニメーター設定を使用します。");
                return;
            }
            
            try
            {
                // PCSS制御用のGameObjectを作成
                var pcssControlObject = new GameObject("PCSS_Control");
                pcssControlObject.transform.SetParent(avatar.transform);
                
                // ModularAvatarMenuInstallerを追加
                var menuInstaller = pcssControlObject.AddComponent(modularAvatarMenuInstallerType);
                
                // メニューアセットを作成
                var menu = CreatePCSSControlMenu();
                
                // MenuInstallerにメニューを設定
                var menuToAppendField = modularAvatarMenuInstallerType.GetField("menuToAppend");
                if (menuToAppendField != null)
                {
                    menuToAppendField.SetValue(menuInstaller, menu);
                }
                
                // パラメータを追加
                AddPCSSParameters(pcssControlObject);
                
                // アニメーターを追加
                AddPCSSAnimator(pcssControlObject, avatar);
                
                Debug.Log("PCSS制御メニューがModularAvatarに追加されました。");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"ModularAvatar統合エラー: {e.Message}");
            }
        }
        
        private static Object CreatePCSSControlMenu()
        {
            // VRCExpressionsMenuを作成
            var menuType = System.Type.GetType("VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu, VRCSDKBase");
            if (menuType == null)
            {
                Debug.LogError("VRCSDKが見つかりません。");
                return null;
            }
            
            var menu = ScriptableObject.CreateInstance(menuType);
            menu.name = "PCSS_Control_Menu";
            
            // メニューコントロールを追加
            var controlsField = menuType.GetField("controls");
            if (controlsField != null)
            {
                var controlType = System.Type.GetType("VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu+Control, VRCSDKBase");
                var parameterType = System.Type.GetType("VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu+Control+Parameter, VRCSDKBase");
                
                if (controlType != null && parameterType != null)
                {
                    var controlsList = System.Activator.CreateInstance(typeof(List<>).MakeGenericType(controlType));
                    var controlsListType = controlsList.GetType();
                    
                    // PCSS On/Off制御
                    var pcssToggleControl = System.Activator.CreateInstance(controlType);
                    SetControlProperty(pcssToggleControl, "name", "PCSS効果");
                    SetControlProperty(pcssToggleControl, "type", 2); // Toggle
                    
                    var parameter = System.Activator.CreateInstance(parameterType);
                    SetControlProperty(parameter, "name", "PCSS_Enable");
                    
                    var parametersArray = System.Array.CreateInstance(parameterType, 1);
                    parametersArray.SetValue(parameter, 0);
                    SetControlProperty(pcssToggleControl, "parameters", parametersArray);
                    
                    controlsListType.GetMethod("Add").Invoke(controlsList, new[] { pcssToggleControl });
                    
                    // 品質設定制御
                    var qualityControl = System.Activator.CreateInstance(controlType);
                    SetControlProperty(qualityControl, "name", "PCSS品質");
                    SetControlProperty(qualityControl, "type", 4); // Four Axis Puppet
                    
                    var qualityParameter = System.Activator.CreateInstance(parameterType);
                    SetControlProperty(qualityParameter, "name", "PCSS_Quality");
                    
                    var qualityParametersArray = System.Array.CreateInstance(parameterType, 1);
                    qualityParametersArray.SetValue(qualityParameter, 0);
                    SetControlProperty(qualityControl, "parameters", qualityParametersArray);
                    
                    controlsListType.GetMethod("Add").Invoke(controlsList, new[] { qualityControl });
                    
                    controlsField.SetValue(menu, controlsList);
                }
            }
            
            // アセットとして保存
            string path = "Assets/PCSS/PCSS_Control_Menu.asset";
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            AssetDatabase.CreateAsset(menu, path);
            
            return menu;
        }
        
        private static void SetControlProperty(object control, string propertyName, object value)
        {
            var field = control.GetType().GetField(propertyName);
            if (field != null)
            {
                field.SetValue(control, value);
            }
        }
        
        private static void AddPCSSParameters(GameObject pcssControlObject)
        {
            if (modularAvatarParametersType == null) return;
            
            var parametersComponent = pcssControlObject.AddComponent(modularAvatarParametersType);
            
            // パラメータリストを作成
            var parameterType = System.Type.GetType("nadena.dev.modular_avatar.core.ParameterConfig, nadena.dev.modular-avatar.core");
            if (parameterType != null)
            {
                var parametersList = new List<object>();
                
                // PCSS_Enable パラメータ
                var pcssEnableParam = System.Activator.CreateInstance(parameterType);
                SetParameterProperty(pcssEnableParam, "nameOnAvatar", "PCSS_Enable");
                SetParameterProperty(pcssEnableParam, "internalParameter", false);
                SetParameterProperty(pcssEnableParam, "isPrefix", false);
                SetParameterProperty(pcssEnableParam, "syncType", 0); // Bool
                SetParameterProperty(pcssEnableParam, "defaultValue", 1.0f);
                SetParameterProperty(pcssEnableParam, "saved", true);
                
                parametersList.Add(pcssEnableParam);
                
                // PCSS_Quality パラメータ
                var pcssQualityParam = System.Activator.CreateInstance(parameterType);
                SetParameterProperty(pcssQualityParam, "nameOnAvatar", "PCSS_Quality");
                SetParameterProperty(pcssQualityParam, "internalParameter", false);
                SetParameterProperty(pcssQualityParam, "isPrefix", false);
                SetParameterProperty(pcssQualityParam, "syncType", 1); // Float
                SetParameterProperty(pcssQualityParam, "defaultValue", 1.0f);
                SetParameterProperty(pcssQualityParam, "saved", true);
                
                parametersList.Add(pcssQualityParam);
                
                // パラメータリストを設定
                var parametersField = modularAvatarParametersType.GetField("parameters");
                if (parametersField != null)
                {
                    parametersField.SetValue(parametersComponent, parametersList);
                }
            }
        }
        
        private static void SetParameterProperty(object parameter, string propertyName, object value)
        {
            var field = parameter.GetType().GetField(propertyName);
            if (field != null)
            {
                field.SetValue(parameter, value);
            }
        }
        
        private static void AddPCSSAnimator(GameObject pcssControlObject, GameObject avatar)
        {
            if (modularAvatarMergeAnimatorType == null) return;
            
            var mergeAnimator = pcssControlObject.AddComponent(modularAvatarMergeAnimatorType);
            
            // アニメーターコントローラーを作成
            var animatorController = CreatePCSSAnimatorController(avatar);
            
            // MergeAnimatorに設定
            var animatorToMergeField = modularAvatarMergeAnimatorType.GetField("animator");
            if (animatorToMergeField != null)
            {
                animatorToMergeField.SetValue(mergeAnimator, animatorController);
            }
            
            var layerTypeField = modularAvatarMergeAnimatorType.GetField("layerType");
            if (layerTypeField != null)
            {
                // FX Layer = 0
                layerTypeField.SetValue(mergeAnimator, 0);
            }
        }
        
        private static RuntimeAnimatorController CreatePCSSAnimatorController(GameObject avatar)
        {
            var controller = new UnityEditor.Animations.AnimatorController();
            controller.name = "PCSS_Controller";
            
            // パラメータを追加
            controller.AddParameter("PCSS_Enable", AnimatorControllerParameterType.Bool);
            controller.AddParameter("PCSS_Quality", AnimatorControllerParameterType.Float);
            
            // レイヤーを作成
            var layer = new UnityEditor.Animations.AnimatorControllerLayer();
            layer.name = "PCSS Control";
            layer.defaultWeight = 1.0f;
            layer.stateMachine = new UnityEditor.Animations.AnimatorStateMachine();
            
            // ステートを作成
            var idleState = layer.stateMachine.AddState("Idle");
            var pcssOnState = layer.stateMachine.AddState("PCSS On");
            var pcssOffState = layer.stateMachine.AddState("PCSS Off");
            
            // トランジションを作成
            var toOnTransition = idleState.AddTransition(pcssOnState);
            toOnTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "PCSS_Enable");
            toOnTransition.duration = 0;
            
            var toOffTransition = idleState.AddTransition(pcssOffState);
            toOffTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, "PCSS_Enable");
            toOffTransition.duration = 0;
            
            // アニメーションクリップを作成
            CreatePCSSAnimationClips(avatar, pcssOnState, pcssOffState);
            
            controller.layers = new UnityEditor.Animations.AnimatorControllerLayer[] { layer };
            
            // アセットとして保存
            string path = "Assets/PCSS/PCSS_Controller.controller";
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            AssetDatabase.CreateAsset(controller, path);
            
            return controller;
        }
        
        private static void CreatePCSSAnimationClips(GameObject avatar, UnityEditor.Animations.AnimatorState onState, UnityEditor.Animations.AnimatorState offState)
        {
            var renderers = avatar.GetComponentsInChildren<Renderer>();
            
            // PCSS On アニメーション
            var pcssOnClip = new AnimationClip();
            pcssOnClip.name = "PCSS_On";
            
            // PCSS Off アニメーション
            var pcssOffClip = new AnimationClip();
            pcssOffClip.name = "PCSS_Off";
            
            foreach (var renderer in renderers)
            {
                var materials = renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    var material = materials[i];
                    if (material != null && material.name.Contains("_PCSS"))
                    {
                        string path = GetGameObjectPath(renderer.gameObject, avatar);
                        
                        // PCSS有効のキーフレーム
                        var onCurve = new AnimationCurve();
                        onCurve.AddKey(0, 1.0f);
                        pcssOnClip.SetCurve(path, typeof(Renderer), $"material._UsePCSS", onCurve);
                        
                        // PCSS無効のキーフレーム
                        var offCurve = new AnimationCurve();
                        offCurve.AddKey(0, 0.0f);
                        pcssOffClip.SetCurve(path, typeof(Renderer), $"material._UsePCSS", offCurve);
                    }
                }
            }
            
            // アニメーションクリップを保存
            AssetDatabase.CreateAsset(pcssOnClip, "Assets/PCSS/PCSS_On.anim");
            AssetDatabase.CreateAsset(pcssOffClip, "Assets/PCSS/PCSS_Off.anim");
            
            // ステートにクリップを設定
            onState.motion = pcssOnClip;
            offState.motion = pcssOffClip;
        }
        
        private static string GetGameObjectPath(GameObject obj, GameObject root)
        {
            string path = obj.name;
            Transform parent = obj.transform.parent;
            
            while (parent != null && parent.gameObject != root)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            
            return path;
        }
        
        /// <summary>
        /// PCSS制御メニューを削除
        /// </summary>
        public static void RemovePCSSControlMenu(GameObject avatar)
        {
            var pcssControlObject = avatar.transform.Find("PCSS_Control");
            if (pcssControlObject != null)
            {
                Object.DestroyImmediate(pcssControlObject.gameObject);
                Debug.Log("PCSS制御メニューが削除されました。");
            }
        }
    }
}
#endif 