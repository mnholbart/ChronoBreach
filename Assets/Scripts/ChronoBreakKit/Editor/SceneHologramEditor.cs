using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ChronoBreak
{
    [CustomEditor(typeof(SceneHologram), true)]
    public class SceneHologramEditor : Editor
    {

        SceneHologram hologram;

        public void OnEnable()
        {
            hologram = (SceneHologram)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            /*
            EditorGUILayout.LabelField("References", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            SerializedProperty hologramBaseReference = serializedObject.FindProperty("HologramBase");
            EditorGUILayout.ObjectField(hologramBaseReference);
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Clear Types"))
            {
                hologram.objectTypeNames.Clear();
                hologram.objectTypes.Clear();
            }

            if (System.Enum.GetNames(typeof(SceneHologram.ObjectTypes)).Length != hologram.objectTypeNames.Count)
            {
                foreach (string s in System.Enum.GetNames(typeof(SceneHologram.ObjectTypes)))
                {
                    if (!hologram.objectTypeNames.Contains(s))
                    {
                        hologram.objectTypeNames.Add(s);
                        hologram.objectTypes.Add(null);
                    }
                }
            }

            foreach (string s in hologram.objectTypeNames)
            {
                int i = hologram.objectTypeNames.IndexOf(s);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(s);
                if (i >= 2) //skip null and gameobject
                    hologram.objectTypes[i] = EditorGUILayout.ObjectField(hologram.objectTypes[i], typeof(Object));
                EditorGUILayout.EndHorizontal();
            } */
        }

    }
}
