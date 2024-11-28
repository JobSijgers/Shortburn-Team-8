using UnityEditor;
using UnityEngine;

namespace HandScripts.Editor
{
    [InitializeOnLoad]
    public static class HandVisualizer
    {
        private static GameObject _previewInstance;
        private static GameObject _prefab;
        private static GameObject _previousSelection;

        static HandVisualizer()
        {
            _prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Arminteraction/Hand.prefab");
            // Subscribe to SceneView callback
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (_prefab == null)
            {
                _prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Arminteraction/Hand.prefab");
            }
            if (_prefab == null)
                return;
            
            if (Selection.activeGameObject != null && Selection.activeGameObject.name == "GrabPoint")
            {
                if (_previousSelection != Selection.activeGameObject)
                {
                    if (_previewInstance != null)
                    {
                        Object.DestroyImmediate(_previewInstance);
                        _previewInstance = null;
                    }
                }

                _previousSelection = Selection.activeGameObject;
                GameObject grabPoint = Selection.activeGameObject;

                if (_previewInstance == null)
                {
                    _previewInstance = CreatePreviewInstance(grabPoint);
                }

                ApplyWorldScale(_previewInstance, Vector3.one);
            }
            else
            {
                // Destroy preview instance if deselected
                if (_previewInstance != null)
                {
                    Object.DestroyImmediate(_previewInstance);
                    _previewInstance = null;
                }
            }
        }

        private static GameObject CreatePreviewInstance(GameObject parent)
        {
            GameObject preview = Object.Instantiate(_prefab, parent.transform, true);
            preview.transform.localPosition = Vector3.zero;
            preview.transform.localRotation = Quaternion.identity;

            return preview;
        }

        private static void ApplyWorldScale(GameObject obj, Vector3 targetWorldScale)
        {
            if (obj.transform.parent != null)
            {
                Vector3 parentScale = obj.transform.parent.lossyScale;
                obj.transform.localScale = new Vector3(
                    targetWorldScale.x / parentScale.x,
                    targetWorldScale.y / parentScale.y,
                    targetWorldScale.z / parentScale.z
                );
            }
            else
            {
                obj.transform.localScale = targetWorldScale; // If no parent, use directly
            }
        }
    }
}