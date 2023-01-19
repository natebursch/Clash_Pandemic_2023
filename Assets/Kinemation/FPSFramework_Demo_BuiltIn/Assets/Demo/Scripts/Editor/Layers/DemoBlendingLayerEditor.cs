// Designed by Kinemation, 2022

using Demo.Scripts.Runtime.Layers;
using UnityEditor;
using UnityEngine;

namespace Demo.Scripts.Editor.Layers
{
    [CustomEditor(typeof(DemoBlendingLayer))]
    public class DemoBlendingLayerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var layer = (DemoBlendingLayer) target;
            
            if (GUILayout.Button("To Mesh Space Rot"))
            {
                layer.EvaluateSpineMS();
            }
        }
    }
}
