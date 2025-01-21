using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tools
{
    public class MassRename : EditorWindow
    {
        public GameObject[] Objects;
        private SerializedObject _serializedObject;
        private SerializedProperty _objectsToRename;
        private string _newName;
        private int _startIndex;

        [MenuItem("Window/Mass Rename")]
        public static void ShowWindow()
        {
            GetWindow(typeof(MassRename));
        }

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(this);
            _objectsToRename = _serializedObject.FindProperty("Objects");
        }

        private void OnGUI()
        {
            _serializedObject.Update();
            //show all field in the inspector
            EditorGUILayout.PropertyField(_objectsToRename);
            _newName = EditorGUILayout.TextField("New Name", _newName);
            _startIndex = EditorGUILayout.IntField("Start Index", _startIndex);
            if (Objects != null && Objects.Length > 0)
            {
                if (GUILayout.Button("Rename"))
                {
                    Rename();
                }
            }
            
            _serializedObject.ApplyModifiedProperties();
        }

        //renames all objects in the array
        private void Rename()
        {
            //check if the objects array is empty
            if (Objects == null || Objects.Length == 0)
            {
                Debug.LogWarning("No objects to rename");
                return;
            }
            for (int i = 0; i < Objects.Length; i++)
            {
                if (Objects[i] == null)
                {
                    Debug.LogWarning("Object at index " + i + " is null");
                    return;
                }
                Objects[i].name = _newName + " " + (i + _startIndex);
            }
        }
    }
}