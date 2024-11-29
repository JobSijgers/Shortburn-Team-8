using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Maze.Editor
{
    public class MazeWindow : EditorWindow
    {
        private struct Line
        {
            public Line(Vector2 startPosition, Vector2 endPosition, int startHeight, int endHeight)
            {
                StartPosition = startPosition;
                EndPosition = endPosition;
                StartHeight = startHeight;
                EndHeight = endHeight;
            }

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
        private SerializedProperty _gradientProperty;
        private SerializedProperty _backgroundColorProperty;
        private SerializedProperty _lineColorProperty;
        private Vector2 _startPosition;
        public int _startHeight;
        public int _endHeight;
        private int _minHeight;
        private int _maxHeight;
        public Color _gridBackgroundColor = Color.grey;
        public Color _gridLineColor = Color.black;
        public Gradient _heightGradient = new Gradient();

        private string _newName;
        private int _startIndex;

        private readonly List<Line> _linePoints = new List<Line>();
        private bool _isDrawingLine = false;

        private Texture2D _binkAppleTexture;

        private void OnGUI()
        {
            DrawGrid();
            DrawSettings();
            DrawLines();
            HandleMouseInput();
            DrawTexture();
        }

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(this);
            _startHeightProperty = _serializedObject.FindProperty("_startHeight");
            _endHeightProperty = _serializedObject.FindProperty("_endHeight");
            _gradientProperty = _serializedObject.FindProperty("_heightGradient");
            _backgroundColorProperty = _serializedObject.FindProperty("_gridBackgroundColor");
            _lineColorProperty = _serializedObject.FindProperty("_gridLineColor");

            _binkAppleTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Food/Binkapple.jpg");
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
            EditorGUI.DrawRect(background, _gridBackgroundColor);

            for (int x = 0; x < GridSize + 1; x++)
            {
                Rect xLine = new(x * CellSize, 0, BorderThickness, GridSize * CellSize);
                EditorGUI.DrawRect(xLine, _gridLineColor);
            }

            for (int y = 0; y < GridSize + 1; y++)
            {
                Rect yLine = new(0, y * CellSize, GridSize * CellSize, BorderThickness);
                EditorGUI.DrawRect(yLine, _gridLineColor);
            }
        }

        private void DrawSettings()
        {
            int offset = GridSize * CellSize + BorderThickness;
            Rect settingsRect = new Rect(offset, 0, position.width - offset, position.height);
            GUILayout.BeginArea(settingsRect);
            _serializedObject.Update();
            EditorGUILayout.LabelField("Height Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_startHeightProperty);
            EditorGUILayout.PropertyField(_endHeightProperty);
            EditorGUILayout.PropertyField(_gradientProperty);
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Grid Setting", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_backgroundColorProperty);
            EditorGUILayout.PropertyField(_lineColorProperty);
            _serializedObject.ApplyModifiedProperties();
            GUILayout.EndArea();
        }

        private void DrawLines()
        {
            foreach (Line line in _linePoints)
            {
                float normalizedStartHeight = Mathf.InverseLerp(_minHeight, _maxHeight, line.StartHeight);
                float normalizedEndHeight = Mathf.InverseLerp(_minHeight, _maxHeight, line.EndHeight);
                Color startColor = _heightGradient.Evaluate(normalizedStartHeight);
                Color endColor = _heightGradient.Evaluate(normalizedEndHeight);

                DrawGradientLine(line.StartPosition, line.EndPosition, startColor, endColor, 5);
            }
        }

        private void DrawGradientLine(Vector2 start, Vector2 end, Color startColor, Color endColor, float width)
        {
            // Set up the material to use GL for custom drawing
            Material lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);

            // Apply the material
            lineMaterial.SetPass(0);

            GL.PushMatrix();
            GL.Begin(GL.QUADS);

            Vector2 dir = (end - start).normalized;
            Vector2 perp = new Vector2(-dir.y, dir.x) * width * 0.5f;

            // Define gradient vertices
            Vector2 v1 = start - perp;
            Vector2 v2 = start + perp;
            Vector2 v3 = end + perp;
            Vector2 v4 = end - perp;

            // Start color
            GL.Color(startColor);
            GL.Vertex(v1);
            GL.Vertex(v2);

            // End color
            GL.Color(endColor);
            GL.Vertex(v3);
            GL.Vertex(v4);

                GL.End();
                GL.PopMatrix();
        }

        private void HandleMouseInput()
        {
            Event e = Event.current;

            if (e.type != EventType.MouseDown) 
                return;
            Vector2 mousePosition = e.mousePosition;

            int clickedX = Mathf.RoundToInt(mousePosition.x / CellSize);
            int clickedY = Mathf.RoundToInt(mousePosition.y / CellSize);

            if (clickedX < 0 || clickedX >= GridSize || clickedY < 0 || clickedY >= GridSize) 
                return;
            Vector2 gridPosition = new Vector2(clickedX * CellSize, clickedY * CellSize);

            switch (e.button)
            {
                // Left-click
                case 0 when !_isDrawingLine:
                    _startPosition = gridPosition;
                    _isDrawingLine = true;
                    break;
                case 0 when _startPosition == gridPosition:
                    return;
                // Make the line straight
                case 0 when Mathf.Approximately(_startPosition.x, gridPosition.x) ||
                            Mathf.Approximately(_startPosition.y, gridPosition.y):
                {
                    Line newLine = new Line(_startPosition, gridPosition, _startHeight, _endHeight);
                    _linePoints.Add(newLine);
                    _isDrawingLine = false;
                    UpdateMinAndMax(newLine);
                    break;
                }
                case 0:
                {
                    int xDifference = Mathf.RoundToInt(gridPosition.x - _startPosition.x);
                    int yDifference = Mathf.RoundToInt(gridPosition.y - _startPosition.y);

                    if (Mathf.Abs(xDifference) > Mathf.Abs(yDifference))
                    {
                        gridPosition.y = _startPosition.y;
                    }
                    else
                    {
                        gridPosition.x = _startPosition.x;
                    }

                    Line newLine = new Line(_startPosition, gridPosition, _startHeight, _endHeight);
                    _linePoints.Add(newLine);
                    _isDrawingLine = false;
                    UpdateMinAndMax(newLine);
                    break;
                }
                // Right-click
                case 1:
                    RemoveLineAtPosition(gridPosition);
                    break;
            }

            e.Use();
        }

        private void RemoveLineAtPosition(Vector2 position)
        {
            const float threshold = 5.0f; // Adjust this value as needed

            for (int i = _linePoints.Count - 1; i >= 0; i--)
            {
                Line line = _linePoints[i];
                if (IsPointNearLine(position, line.StartPosition, line.EndPosition, threshold))
                {
                    _linePoints.RemoveAt(i);
                    break;
                }
            }
        }

        private bool IsPointNearLine(Vector2 point, Vector2 lineStart, Vector2 lineEnd, float threshold)
        {
            float distance = HandleUtility.DistancePointLine(point, lineStart, lineEnd);
            return distance <= threshold;
        }

        private void UpdateMinAndMax(Line newLine)
        {
            if (newLine.StartHeight < _minHeight)
            {
                _minHeight = newLine.StartHeight;
            }

            if (newLine.EndHeight < _minHeight)
            {
                _minHeight = newLine.EndHeight;
            }

            if (newLine.StartHeight > _maxHeight)
            {
                _maxHeight = newLine.StartHeight;
            }

            if (newLine.EndHeight > _maxHeight)
            {
                _maxHeight = newLine.EndHeight;
            }
        }

        private void DrawTexture()
        {
            if (_binkAppleTexture != null)
            {
                float textureWidth = 200;
                float textureHeight = 200;
                float xPosition = position.width - textureWidth - 10;
                float yPosition = position.height - textureHeight - 10;

                Rect textureRect = new Rect(xPosition, yPosition, textureWidth, textureHeight);
                GUI.DrawTexture(textureRect, _binkAppleTexture);
            }
        }
    }
}