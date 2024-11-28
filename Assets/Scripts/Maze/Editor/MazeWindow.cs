using System;
using UnityEditor;
using UnityEngine;

namespace Maze.Editor
{
    public class MazeWindow : EditorWindow
    {
        private struct Cell
        {
            public Vector2 StartPosition;
            public Vector2 EndPosition;
            public int StartHeight;
            public int EndHeight;
        }
        private const int GridSize = 20;
        private const int CellSize = 30;
        private const int BorderThickness = 2;

        private SerializedObject _serializedObject;
        private SerializedProperty _startHeightProperty;
        private SerializedProperty _endHeightProperty;
        private SerializedProperty _heightGradientProperty;
        public int _startHeight;
        public int _endHeight;
        public Gradient _heightGradient;
        
        private string _newName;
        private int _startIndex;
        
        private void OnGUI()
        {
            DrawGrid();
            HandleMouseInput();
            DrawSettings();
        }

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(this);
            _startHeightProperty = _serializedObject.FindProperty("_startHeight");
            _endHeightProperty = _serializedObject.FindProperty("_endHeight");
            _heightGradientProperty = _serializedObject.FindProperty("_heightGradient");
        }

        [MenuItem("Window/Custom Grid")]
        public static void ShowWindow()
        {
            MazeWindow window = GetWindow<MazeWindow>("Custom Grid");
            
            int gridWidth = GridSize * CellSize + BorderThickness + 300;
            int gridHeight = GridSize * CellSize + BorderThickness;

            // Set window size constraints
            window.minSize = new Vector2(gridWidth, gridHeight);
            window.maxSize = new Vector2(gridWidth, gridHeight);
        }

        private void DrawGrid()
        {
            Rect background = new(0, 0, GridSize * CellSize, GridSize * CellSize);
            EditorGUI.DrawRect(background, Color.grey);
            
            for (int x = 0; x < GridSize + 1; x++)
            {
                Rect xLine = new(x * CellSize, 0, BorderThickness, GridSize * CellSize);
                EditorGUI.DrawRect(xLine, Color.black);
            }
            for (int y = 0; y < GridSize + 1; y++)
            {
                Rect yLine = new(0, y * CellSize, GridSize * CellSize, BorderThickness);
                EditorGUI.DrawRect(yLine, Color.black);
            }
        }

        private void DrawSettings()
        {
            int offset = GridSize * CellSize + BorderThickness;
            Rect settingsRect = new Rect(offset, 0, position.width - offset, position.height);
            GUILayout.BeginArea(settingsRect);
            EditorGUILayout.PropertyField(_startHeightProperty);
            EditorGUILayout.PropertyField(_endHeightProperty);
            EditorGUILayout.PropertyField(_heightGradientProperty);
        }
        

        private void HandleMouseInput()
        {
            Event e = Event.current;

            if (e.type == EventType.MouseDown && e.button == 0) // Left-click
            {
                Vector2 mousePosition = e.mousePosition;

                int clickedX = Mathf.FloorToInt(mousePosition.x / CellSize);
                int clickedY = Mathf.FloorToInt(mousePosition.y / CellSize);

                if (clickedX >= 0 && clickedX < GridSize && clickedY >= 0 && clickedY < GridSize)
                {
                    Debug.Log($"Clicked on cell: {clickedX},{clickedY}");
                }

                e.Use(); // Mark the event as used
            }
        }
    }
}
