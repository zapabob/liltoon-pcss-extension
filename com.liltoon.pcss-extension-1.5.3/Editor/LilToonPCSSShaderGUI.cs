using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using lilToon;

namespace lilToon.PCSS
{
    // liltoon本体のShaderGUIを継承することで、互換性を確保しマテリアル消失を防ぐ
    public class LilToonPCSSShaderGUI : lilToon.lilToonShaderGUI
    {
        // PCSS用のプロパティを保持
        private MaterialProperty _PCSS_ON;
        private MaterialProperty _PCSSFilterRadius;
        private MaterialProperty _PCSSLightSize;
        private MaterialProperty _PCSSSamples;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            // まずliltoon本体のGUIを描画
            base.OnGUI(materialEditor, properties);

            // PCSS拡張機能のUIを描画
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("PCSS Extension Settings", EditorStyles.boldLabel);
            
            // プロパティを取得
            _PCSS_ON = FindProperty("_PCSS_ON", properties, false);
            _PCSSFilterRadius = FindProperty("_PCSSFilterRadius", properties, false);
            _PCSSLightSize = FindProperty("_PCSSLightSize", properties, false);
            _PCSSSamples = FindProperty("_PCSSSamples", properties, false);

            if (_PCSS_ON != null)
            {
                materialEditor.ShaderProperty(_PCSS_ON, "Enable PCSS");

                if (_PCSS_ON.floatValue > 0.5f)
                {
                    EditorGUI.indentLevel++;
                    materialEditor.ShaderProperty(_PCSSFilterRadius, "Filter Radius");
                    materialEditor.ShaderProperty(_PCSSLightSize, "Light Size");
                    materialEditor.ShaderProperty(_PCSSSamples, "Sample Count");
                    EditorGUI.indentLevel--;
                }
            }
        }
    }
} 