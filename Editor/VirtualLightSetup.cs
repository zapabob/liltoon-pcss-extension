using UnityEditor;
using UnityEngine;

public static class VirtualLightSetup
{
    [MenuItem("Tools/lilToon/PCSS影システムセットアップ")]
    public static void SetupPCSSShadowSystem()
    {
        var selected = Selection.activeGameObject;
        if (selected == null)
        {
            Debug.LogWarning("アバターを選択してください");
            return;
        }
        // Headボーン探索
        var head = FindChildRecursive(selected.transform, "Head");
        if (head == null)
        {
            Debug.LogWarning("Headボーンが見つかりません");
            return;
        }
        // VirtualLight生成または既存取得
        Transform vlight = head.Find("VirtualLight");
        if (vlight == null)
        {
            vlight = new GameObject("VirtualLight").transform;
            vlight.parent = head;
            vlight.localPosition = new Vector3(0, 0, 0.2f);
            vlight.localRotation = Quaternion.identity;
            Debug.Log("VirtualLightをHead直下に生成しました");
        }
        else
        {
            Debug.Log("既にVirtualLightが存在します");
        }

        // PhysBone自動アタッチ（既にある場合はスキップ）
        if (vlight.GetComponent("VRCPhysBone") == null)
        {
            var physBoneType = System.Type.GetType("VRC.Dynamics.VRCPhysBone, Assembly-CSharp");
            if (physBoneType != null)
            {
                vlight.gameObject.AddComponent(physBoneType);
                Debug.Log("VirtualLightにPhysBoneを自動アタッチしました");
            }
            else
            {
                Debug.LogWarning("VRCPhysBoneが見つかりません。手動で追加してください。");
            }
        }

        // 拡張ポイント: ここに今後の自動セットアップ処理（ExpressionMenu/Animator/影マスク等）を追加可能
        // 例: SetupExpressionMenu(selected);
        // 例: SetupShadowMask(selected);

        Debug.Log("PCSS影システムセットアップ完了");
    }

    private static Transform FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            var found = FindChildRecursive(child, name);
            if (found != null) return found;
        }
        return null;
    }
} 