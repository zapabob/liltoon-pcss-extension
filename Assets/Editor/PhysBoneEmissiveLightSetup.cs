using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

#if VRCHAT_SDK_AVAILABLE
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Dynamics.PhysBone.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
#endif

namespace lilToon.PCSS.Editor
{
    public static class PhysBoneEmissiveLightSetup
    {
        private const string MenuPath = "Tools/lilToon-PCSS-Extension/ライト/PhysBoneエミッシブライト作成";
        private const string ControllerDir = "Assets/PCSS/Controllers";
        private const string ControllerPath = "Assets/PCSS/Controllers/PB_EmissiveLight.controller";
        // PhysBoneの base parameter 名（出力は PB_Light_Angle / _Stretch / _Squish / _IsGrabbed / _IsPosed）
        private const string PhysBoneBaseParam = "PB_Light";
        private const string PhysBoneAngleParam = "PB_Light_Angle";

        [MenuItem(MenuPath, false, 26)]
        public static void CreatePhysBoneEmissiveLight()
        {
            var avatar = Selection.activeGameObject;
            if (avatar == null)
            {
                EditorUtility.DisplayDialog("PCSS", "アバターを選択してください。", "OK");
                return;
            }

            var animator = avatar.GetComponent<Animator>();
            if (animator == null)
            {
                animator = avatar.AddComponent<Animator>();
            }

            // ライト見た目用のエミッシブ球を作成
            var lightObj = new GameObject("PB_EmissiveLight");
            Undo.RegisterCreatedObjectUndo(lightObj, "Create PB_EmissiveLight");
            // 頭に追従させる（あれば）
            var head = FindHeadTransform(avatar);
            lightObj.transform.SetParent(head != null ? head : avatar.transform, false);
            lightObj.transform.localPosition = head != null ? new Vector3(0, 0.15f, 0.1f) : new Vector3(0, 1.6f, 0.2f);

            // PhysBoneで動く先端用のTipを用意
            var tip = new GameObject("PB_Tip");
            Undo.RegisterCreatedObjectUndo(tip, "Create PB_Tip");
            tip.transform.SetParent(lightObj.transform, false);
            tip.transform.localPosition = new Vector3(0, 0.05f, 0.15f);

            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "PB_EmissiveSphere";
            Undo.RegisterCreatedObjectUndo(sphere, "Create PB_EmissiveSphere");
            sphere.transform.SetParent(tip.transform, false);
            sphere.transform.localScale = Vector3.one * 0.08f;
            var mr = sphere.GetComponent<MeshRenderer>();
            var mat = new Material(Shader.Find("Standard"));
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", Color.white * 2.0f);
            mr.sharedMaterial = mat;

            // PhysBone 付与（SDK があれば）
#if VRCHAT_SDK_AVAILABLE
            if (avatar.GetComponent<VRCAvatarDescriptor>() != null)
            {
                var pb = lightObj.AddComponent<VRCPhysBone>();
                pb.rootTransform = lightObj.transform;
                pb.pull = 0.8f;
                pb.pullCurve = AnimationCurve.Linear(0, 0.8f, 1, 0.8f);
                pb.spring = 0.2f;
                pb.immobile = 0.0f;
                pb.stretchMotion = 0.1f;
                // PhysBone の出力を Expression Parameters に書き出すベース名
                pb.parameter = PhysBoneBaseParam;
                // 手で掴んで動かせるようにする
                pb.allowGrabbing = true;
                pb.allowPosing = true;

                // 手のコライダー自動作成（Sphere）して割り当て
                var leftHand = FindHumanoidBone(avatar, HumanBodyBones.LeftHand);
                var rightHand = FindHumanoidBone(avatar, HumanBodyBones.RightHand);
                var colliders = new System.Collections.Generic.List<VRCPhysBoneCollider>();
                if (leftHand != null)
                {
                    var lc = leftHand.gameObject.GetComponent<VRCPhysBoneCollider>();
                    if (lc == null) lc = leftHand.gameObject.AddComponent<VRCPhysBoneCollider>();
                    lc.shapeType = VRCPhysBoneCollider.ShapeType.Sphere;
                    lc.radius = 0.06f;
                    colliders.Add(lc);
                }
                if (rightHand != null)
                {
                    var rc = rightHand.gameObject.GetComponent<VRCPhysBoneCollider>();
                    if (rc == null) rc = rightHand.gameObject.AddComponent<VRCPhysBoneCollider>();
                    rc.shapeType = VRCPhysBoneCollider.ShapeType.Sphere;
                    rc.radius = 0.06f;
                    colliders.Add(rc);
                }
                if (colliders.Count > 0)
                {
                    pb.colliders = colliders;
                }

                // Expression Parameters にベースパラメータを追加（未設定なら生成）
                EnsureExpressionParameter(avatar.GetComponent<VRCAvatarDescriptor>(), PhysBoneBaseParam, VRCExpressionParameters.ValueType.Float, saved: true);
            }
#endif

            // Animator Controller 準備（パラメータに応じてエミッションをブレンド）
            EnsureDirectories();
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            if (controller == null)
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            }

            // ブレンドは PhysBone の Angle 出力で駆動
            EnsureParameter(controller, PhysBoneAngleParam, AnimatorControllerParameterType.Float);

            // 2つのクリップを生成（Emission Low / High）
            var clipLow = CreateEmissionClip("Assets/PCSS/Controllers/PB_Emission_Low.anim", sphere, 0.2f);
            var clipHigh = CreateEmissionClip("Assets/PCSS/Controllers/PB_Emission_High.anim", sphere, 3.0f);

            // レイヤーとブレンドツリー作成
            var layerName = "PB_EmissiveLight";
            var layer = GetOrCreateLayer(controller, layerName);
            var sm = layer.stateMachine;

            var state = sm.AddState("PB_EmissiveBlend");
            var blendTree = new BlendTree { name = "PB_EmissionBT", blendParameter = PhysBoneAngleParam, hideFlags = HideFlags.HideInHierarchy }; 
            AssetDatabase.AddObjectToAsset(blendTree, ControllerPath);
            blendTree.useAutomaticThresholds = false;
            blendTree.blendType = BlendTreeType.Simple1D;
            blendTree.AddChild(clipLow, 0f);
            blendTree.AddChild(clipHigh, 1f);
            state.motion = blendTree;
            sm.defaultState = state;

            animator.runtimeAnimatorController = controller;
            EditorUtility.SetDirty(controller);
            EditorUtility.SetDirty(animator);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorGUIUtility.PingObject(lightObj);
            Selection.activeGameObject = lightObj;
            EditorUtility.DisplayDialog("PCSS", "PhysBoneエミッシブライトを作成しました。VRChatではエミッシブで光表現を行います。", "OK");
        }

#if VRCHAT_SDK_AVAILABLE
        private static void EnsureExpressionParameter(VRCAvatarDescriptor avatarDesc, string name, VRCExpressionParameters.ValueType type, bool saved)
        {
            if (avatarDesc == null) return;
            var ep = avatarDesc.expressionParameters;
            if (ep == null)
            {
                // 新規作成
                EnsureDirectories();
                string epPath = "Assets/PCSS/Controllers/PB_ExpressionParameters.asset";
                ep = ScriptableObject.CreateInstance<VRCExpressionParameters>();
                AssetDatabase.CreateAsset(ep, epPath);
                avatarDesc.expressionParameters = ep;
                EditorUtility.SetDirty(avatarDesc);
            }

            var list = new System.Collections.Generic.List<VRCExpressionParameters.Parameter>();
            if (ep.parameters != null) list.AddRange(ep.parameters);
            if (!list.Exists(p => p != null && p.name == name))
            {
                list.Add(new VRCExpressionParameters.Parameter
                {
                    name = name,
                    valueType = type,
                    saved = saved
                });
                ep.parameters = list.ToArray();
                EditorUtility.SetDirty(ep);
                AssetDatabase.SaveAssets();
            }
        }
#endif

        private static void EnsureDirectories()
        {
            if (!AssetDatabase.IsValidFolder("Assets/PCSS")) AssetDatabase.CreateFolder("Assets", "PCSS");
            if (!AssetDatabase.IsValidFolder(ControllerDir)) AssetDatabase.CreateFolder("Assets/PCSS", "Controllers");
        }

        private static void EnsureParameter(AnimatorController controller, string name, AnimatorControllerParameterType type)
        {
            if (controller.parameters != null && System.Array.Exists(controller.parameters, p => p.name == name)) return;
            controller.AddParameter(name, type);
        }

        private static AnimatorControllerLayer GetOrCreateLayer(AnimatorController controller, string name)
        {
            foreach (var l in controller.layers)
            {
                if (l.name == name) return l;
            }
            var newLayer = new AnimatorControllerLayer
            {
                name = name,
                defaultWeight = 1f,
                stateMachine = new AnimatorStateMachine { name = name + "_SM" }
            };
            AssetDatabase.AddObjectToAsset(newLayer.stateMachine, ControllerPath);
            controller.AddLayer(newLayer);
            return newLayer;
        }

        private static AnimationClip CreateEmissionClip(string path, GameObject target, float intensity)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null)
            {
                clip = new AnimationClip { name = Path.GetFileNameWithoutExtension(path) };
                AssetDatabase.CreateAsset(clip, path);
            }

            // マテリアルのEmissionColor(カラー)をアニメーション。HDRカラーはRGBをスカラーで近似。
            var binding = new EditorCurveBinding
            {
                type = typeof(Renderer),
                path = AnimationUtility.CalculateTransformPath(target.transform, target.transform.root),
                propertyName = "material._EmissionColor.a" // Alpha成分を強度近似に利用
            };
            var curve = AnimationCurve.Linear(0f, intensity, 1f, intensity);
            AnimationUtility.SetEditorCurve(clip, binding, curve);

            return clip;
        }

        private static Transform FindHeadTransform(GameObject avatar)
        {
            var anim = avatar.GetComponent<Animator>();
            if (anim != null)
            {
                try
                {
                    var head = anim.GetBoneTransform(HumanBodyBones.Head);
                    if (head != null) return head;
                }
                catch { }
            }
            // フォールバック: よくあるボーン名で探索
            var t = avatar.transform;
            var candidates = new[] { "Head", "head", "J_Head", "J_Neck", "Neck" };
            foreach (var name in candidates)
            {
                var found = t.Find(name);
                if (found != null) return found;
            }
            return null;
        }
    }
}


