using HandScripts.ProceduralAnimation;
using HandScripts.Grab;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace HandScripts.Editor
{
    [InitializeOnLoad][ExecuteAlways]
    public static class HandVisualizer
    {
        private static GameObject _previewInstance;
        private static GameObject _prefab;
        private static GameObject _previousSelection;
        private static ProceduralHandAnimation _handAnimation;
        private static GrabPoint _grabPoint;

        static HandVisualizer()
        {
            // Subscribe to SceneView callback
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (_prefab == null)
            {
                _prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Arminteraction/Ms_Arm.prefab");
            }
            
            if (_prefab == null)
                return;

            GameObject selectedObject = Selection.activeGameObject;
            bool isChildOfGrabPoint = selectedObject != null && _grabPoint != null && selectedObject.transform.IsChildOf(_grabPoint.transform);
            if (Selection.activeGameObject != null && Selection.activeGameObject.name == "GrabPoint" || isChildOfGrabPoint)
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
                if (_grabPoint == null)
                    _grabPoint = grabPoint.GetComponent<GrabPoint>();

                if (_previewInstance == null)
                {
                    _previewInstance = CreatePreviewInstance(_grabPoint.gameObject);
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
            
            // update fingers
            if (_previewInstance != null && _grabPoint != null)
            {
                _handAnimation.SetFingerPosition("Thumb", _grabPoint.GetFingerPosition("Thumb"));
                _handAnimation.SetFingerPosition("Index", _grabPoint.GetFingerPosition("Index"));
                _handAnimation.SetFingerPosition("Middle", _grabPoint.GetFingerPosition("Middle"));
                _handAnimation.SetFingerPosition("Ring", _grabPoint.GetFingerPosition("Ring"));
                _handAnimation.SetFingerPosition("Pink", _grabPoint.GetFingerPosition("Pink"));
            }
        }

        private static GameObject CreatePreviewInstance(GameObject parent)
        {
            GameObject preview = Object.Instantiate(_prefab, parent.transform, true);
            preview.transform.localPosition = Vector3.zero;
            preview.transform.localRotation = Quaternion.identity;
            _handAnimation = preview.GetComponentInChildren<ProceduralHandAnimation>();

                _handAnimation.GetComponent<RigBuilder>().Build();

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