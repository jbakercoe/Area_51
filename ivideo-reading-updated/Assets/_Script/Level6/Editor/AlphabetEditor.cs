using UnityEditor;
using UnityEngine;

namespace Level6
{
    [CustomEditor(typeof(Alphabet))]
    [CanEditMultipleObjects]
    public class AlphabetEditor : Editor
    {
        private bool _isDefaultInspectorWillbeDrawn;
        private bool _foldAlphabetDetails;
        private SerializedProperty _alphabetNameProperty;
        private SerializedProperty _alphabetGameObject;
        private SerializedProperty _alphabetRectTransForm;
        private SerializedProperty _alphabetTransitionTimeToMove;

        private void OnEnable()
        {
            _alphabetNameProperty = serializedObject.FindProperty("NameOfTheAlphabet");
            _alphabetGameObject = serializedObject.FindProperty("AlphabetObject");
            _alphabetRectTransForm = serializedObject.FindProperty("AlphabetRectTransform");
            _alphabetTransitionTimeToMove = serializedObject.FindProperty("TransitionTime");
        }


        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update();
            //Provide a free space
            GUILayout.Space(10f);
            //Name the Alphabet Editor
            GUILayout.Label("Alphabet Class Custom Editor", EditorStyles.boldLabel);
            //Provide 5 Pixel space
            GUILayout.Space(5f);
            //Set Version
            EditorGUILayout.LabelField("Version", "1.0", EditorStyles.centeredGreyMiniLabel);
            //Provide 10 Pixel space
            GUILayout.Space(10f);
            //Check default inspector will be drawn.
            _isDefaultInspectorWillbeDrawn =
                EditorGUILayout.Toggle("Default Inspector", _isDefaultInspectorWillbeDrawn);
            //Draw The Default Editor
            if (_isDefaultInspectorWillbeDrawn)
            {
                DrawDefaultInspector();
            }
            else
            {
                //Provide 5 Pixel space
                GUILayout.Space(5f);
                //Set up target
                var alphabet = (Alphabet) target;
                //Provide 10 Pixel space
                GUILayout.Space(5f);
                //Provide a group 
                _foldAlphabetDetails = EditorGUILayout.Foldout(_foldAlphabetDetails, "Alphabet Details");
                if (_foldAlphabetDetails)
                {
                    var level = EditorGUI.indentLevel;
                    EditorGUI.indentLevel++;
                    //Alphabet name
                    EditorGUILayout.PropertyField(_alphabetNameProperty,
                        new GUIContent("Alphabet Name : ", "Name of the current Alphabet"));
                    //Provide 3 Pixel space
                    GUILayout.Space(3f);
                    //Alphabet Gamobject
                    EditorGUILayout.PropertyField(_alphabetGameObject,
                        new GUIContent("Alphabet GameObject : ", "Text Field Of the GameObject"));
                    //Provide 3 Pixel space
                    GUILayout.Space(3f);
                    //Alphabet RectTransForm
                    EditorGUILayout.PropertyField(_alphabetRectTransForm,
                        new GUIContent("Alphabet's RectTransform : ", "Rect TransForm Of the Text Filed"));
                    //Provide 3 Pixel space
                    GUILayout.Space(3f);
                    //Transition Time
                    EditorGUILayout.PropertyField(_alphabetTransitionTimeToMove,
                        new GUIContent("Alphabet's Moving speed : ",
                            "Amount Of time required to move the alphabet in world"));

                    EditorGUI.indentLevel = level;
                }
            }

            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
        }
    }
}