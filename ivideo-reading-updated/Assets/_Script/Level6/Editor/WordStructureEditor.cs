using UnityEditor;
using UnityEngine;

namespace Level6
{
    [CustomEditor(typeof(WordStructure))]
    [CanEditMultipleObjects]
    public class WordStructureEditor : Editor
    {
        private SerializedProperty _wordnameProperty;
        private SerializedProperty _wordAudioClipProperty;
        private SerializedProperty _wordPronouncedAudioClippProperty;

        private void OnEnable()
        {
            _wordnameProperty = serializedObject.FindProperty("WordName");
            _wordAudioClipProperty = serializedObject.FindProperty("WordAudioClip");
            _wordPronouncedAudioClippProperty = serializedObject.FindProperty("WordPronouncedAudioClip");
        }

        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            //color the skin.
            GUI.backgroundColor = Color.green;
            serializedObject.Update();
            EditorGUILayout.PropertyField(_wordnameProperty, new GUIContent("WordHolder : ", "Name of the word "));
            EditorGUILayout.PropertyField(_wordAudioClipProperty,
                new GUIContent("Sound Of the word : ", "Audio clip "));
            EditorGUILayout.PropertyField(_wordPronouncedAudioClippProperty,
                new GUIContent("WordHolder spelled : ", "Audio clip "));
            //Provide 3 Pixel space .
            GUILayout.Space(10f);
            //color the skin.
            GUI.backgroundColor = Color.red;
            var script = (WordStructure) target;
            GUILayout.BeginHorizontal("box");
            {
                GUILayout.FlexibleSpace();
                //color the skin.
                GUI.backgroundColor = Color.blue;
                //Add button.
                if (GUILayout.Button("ADD Alphabet", GUILayout.Width(100), GUILayout.Height(30))) script.AddAlphabet();

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
        }
    }
}