using UnityEngine;
using UnityEditor;
using System.Linq;
using lilToon.PCSS.Runtime;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// アバターの身長から動的に最適な位置に外部ライトを配置する機能
    /// </summary>
    public static class AutoLightPlacement
    {
        [MenuItem("Tools/lilToon PCSS/外部ライト自動配置", false, 25)]
        public static void AutoPlaceExternalLights()
        {
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject == null)
            {
                EditorUtility.DisplayDialog("エラー", "アバターを選択してください。", "OK");
                return;
            }

            // アバターの身長を計算
            float avatarHeight = CalculateAvatarHeight(selectedObject);
            if (avatarHeight <= 0)
            {
                EditorUtility.DisplayDialog("エラー", "アバターの身長を計算できませんでした。", "OK");
                return;
            }

            Debug.Log($"アバター身長: {avatarHeight:F2}m");

            // 最適なライト位置を計算
            Vector3[] optimalPositions = CalculateOptimalLightPositions(avatarHeight);
            
            // ライトを配置
            CreateExternalLights(selectedObject, optimalPositions);
            
            EditorUtility.DisplayDialog("完了", $"アバター身長 {avatarHeight:F2}m に基づいて外部ライトを配置しました。", "OK");
        }

        [MenuItem("Tools/lilToon PCSS/外部ライト自動配置", true)]
        public static bool ValidateAutoPlaceExternalLights()
        {
            return Selection.activeGameObject != null;
        }

        private static float CalculateAvatarHeight(GameObject avatar)
        {
            Animator animator = avatar.GetComponent<Animator>();
            if (animator == null || !animator.isHuman)
            {
                return 0f;
            }

            // 頭と足の位置から身長を計算
            Transform headBone = animator.GetBoneTransform(HumanBodyBones.Head);
            Transform leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            Transform rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);

            if (headBone == null || leftFoot == null || rightFoot == null)
            {
                return 0f;
            }

            // 足の中心位置を計算
            Vector3 footCenter = (leftFoot.position + rightFoot.position) * 0.5f;
            
            // 身長を計算（Y軸の差）
            float height = headBone.position.y - footCenter.y;
            
            return Mathf.Abs(height);
        }

        private static Vector3[] CalculateOptimalLightPositions(float avatarHeight)
        {
            // アバター身長に基づいて最適なライト位置を計算
            float baseDistance = avatarHeight * 2.5f; // 身長の2.5倍の距離
            float heightOffset = avatarHeight * 0.8f; // 身長の80%の高さ
            
            Vector3[] positions = new Vector3[4];
            
            // 前面ライト（メインライト）
            positions[0] = new Vector3(0, heightOffset, baseDistance);
            
            // 背面ライト（リムライト）
            positions[1] = new Vector3(0, heightOffset, -baseDistance * 0.8f);
            
            // 左側ライト（フィルライト）
            positions[2] = new Vector3(-baseDistance * 0.7f, heightOffset, 0);
            
            // 右側ライト（フィルライト）
            positions[3] = new Vector3(baseDistance * 0.7f, heightOffset, 0);
            
            return positions;
        }

        private static void CreateExternalLights(GameObject avatar, Vector3[] positions)
        {
            string[] lightNames = { "Main Light", "Rim Light", "Fill Light Left", "Fill Light Right" };
            Color[] lightColors = { Color.white, new Color(0.8f, 0.8f, 1f), new Color(1f, 0.9f, 0.8f), new Color(0.8f, 1f, 0.9f) };
            float[] intensities = { 1.2f, 0.8f, 0.6f, 0.6f };
            LightType[] lightTypes = { LightType.Spot, LightType.Spot, LightType.Spot, LightType.Spot };

            for (int i = 0; i < positions.Length; i++)
            {
                // ライトオブジェクトを作成
                GameObject lightObject = new GameObject($"PCSS External {lightNames[i]}");
                Undo.RegisterCreatedObjectUndo(lightObject, $"Create {lightNames[i]}");
                
                // アバターの子オブジェクトとして配置
                lightObject.transform.SetParent(avatar.transform, false);
                lightObject.transform.localPosition = positions[i];
                
                // アバターを向くように回転
                lightObject.transform.LookAt(avatar.transform.position + Vector3.up * positions[i].y);
                
                // Lightコンポーネントを追加
                Light light = lightObject.AddComponent<Light>();
                light.type = lightTypes[i];
                light.color = lightColors[i];
                light.intensity = intensities[i];
                light.range = avatar.transform.localScale.magnitude * 5f;
                light.spotAngle = 45f;
                light.shadows = LightShadows.Soft;
                light.shadowStrength = 0.8f;
                light.shadowNormalBias = 0.1f;
                light.cullingMask = 1; // Default layer only
                
                // PCSS用の設定（アセンブリ参照の問題のため一時的にコメントアウト）
                // var pcssController = lightObject.AddComponent<ModularAvatarPCSSController>();
                // if (pcssController != null)
                // {
                //     pcssController.RealtimeQuality = intensities[i];
                //     pcssController.AutoLightManagement = true;
                //     pcssController.EnableLOD = true;
                // }
                
                Debug.Log($"外部ライト '{lightNames[i]}' を配置しました: {positions[i]}");
            }
            
            // 作成したライトを選択
            Selection.activeGameObject = avatar;
        }
    }
}
