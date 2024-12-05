using HandScripts.ProceduralAnimation;
using HandScripts.Grab;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace HandScripts.Editor
{
    [InitializeOnLoad] [ExecuteAlways]
    public static class HandVisualizer
    {
        private static GameObject _previewInstance;
        private static GameObject _prefab;
        private static GameObject _previousSelection;
        private static ProceduralHandAnimation _handAnimation;

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

            if (selectedObject == null)
                return;

            bool isArmLocation = selectedObject.name == "ArmLocation";
            bool isChildOfArmLocation = false;
            if (selectedObject.transform.parent!=null)
            {
                isChildOfArmLocation = selectedObject.transform.parent.name == "ArmLocation";
            }
            bool shouldVisualize = isArmLocation || isChildOfArmLocation;

            if (!shouldVisualize)
            {
                if (_previewInstance != null)
                {
                    Object.DestroyImmediate(_previewInstance);
                    _previewInstance = null;
                }

                return;
            }

            if (_previewInstance == null)
            {
                if (isArmLocation)
                {
                    _previewInstance = CreatePreviewInstance(selectedObject);
                }
                else
                {
                    _previewInstance = CreatePreviewInstance(selectedObject.transform.parent.gameObject);
                }
            }

            _previousSelection = Selection.activeGameObject;


            GrabPoint grabPoint = _previewInstance.GetComponentInParent<GrabPoint>();

            if (_previewInstance != null && grabPoint != null)
            {
                _handAnimation.SetFingerPosition("Thumb", grabPoint.GetFingerPosition("Thumb"));
                _handAnimation.SetFingerPosition("Index", grabPoint.GetFingerPosition("Index"));
                _handAnimation.SetFingerPosition("Middle", grabPoint.GetFingerPosition("Middle"));
                _handAnimation.SetFingerPosition("Ring", grabPoint.GetFingerPosition("Ring"));
                _handAnimation.SetFingerPosition("Pink", grabPoint.GetFingerPosition("Pink"));
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
    }
}