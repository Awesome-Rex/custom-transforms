using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using REXTools.REXCore;
using REXTools.EditorTools;

namespace REXTools.CustomTransforms
{
    [CustomEditor(typeof(CustomParent))]
    public class CustomParentEditor : EditorPRO<CustomParent>
    {
        protected override void DeclareProperties()
        {
            
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            OnInspectorGUIPRO(() =>
            {
                //foreach (KeyValuePair<CustomTransform<dynamic>, Transform> parent in target.parents)
                //{
                //    KeyValuePair(parent.Key, parent.Value);
                //}

                //EditorGUILayout.PropertyField(serializedObject.FindProperty("parents"));
            });
        }

        private void OnSceneGUI()
        {
            
        }
    }
}