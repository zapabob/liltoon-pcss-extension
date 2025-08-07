using UnityEditor;
using UnityEngine;

namespace lilToon.PCSS.Editor
{
    public class LilToonPCSSShaderGUI : ShaderGUI
    {
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            base.OnGUI(materialEditor, properties);

            Material targetMat = materialEditor.target as Material;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Flipbook Settings", EditorStyles.boldLabel);

            MaterialProperty useFlipbook = FindProperty("_UseFlipbook", properties);
            materialEditor.ShaderProperty(useFlipbook, "Use Flipbook");

            if (useFlipbook.floatValue != 0)
            {
                targetMat.EnableKeyword("_USEFLIPBOOK_ON");
                materialEditor.TextureProperty(FindProperty("_FlipbookTex", properties), "Flipbook Texture");
                materialEditor.ShaderProperty(FindProperty("_FlipbookDivisionsX", properties), "Divisions X");
                materialEditor.ShaderProperty(FindProperty("_FlipbookDivisionsY", properties), "Divisions Y");
                materialEditor.ShaderProperty(FindProperty("_FlipbookSpeed", properties), "Speed");
            }
            else
            {
                targetMat.DisableKeyword("_USEFLIPBOOK_ON");
            }
        }
    }
}
