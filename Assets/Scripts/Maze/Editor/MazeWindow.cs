using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Maze.Editor
{
    public class MazeWindow : EditorWindow
    {
        private class Point
        {
            public Vector3 _Position;
            public float _Height;
            public HashSet<Vector3> _Connections;

            public Point(Vector3 position, float height)
            {
                _Position = position;
                _Height = height;
                _Connections = new HashSet<Vector3>();
            }

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                Point other = (Point)obj;
                return _Position == other._Position && _Height == other._Height;
            }

            public override int GetHashCode()
            {
                return _Position.GetHashCode() ^ _Height.GetHashCode();
            }
        }

        private const int GridSize = 20;
        private const int CellSize = 30;
        private const int StartEndSize = 12;
        private const int HalfStartEndSize = StartEndSize / 2;
        private const int BorderThickness = 2;
        private const int HalfBorderThickness = BorderThickness / 2;

        private SerializedObject _serializedObject;
        private SerializedProperty _startHeightProperty;
        private SerializedProperty _endHeightProperty;
        private SerializedProperty _gradientProperty;
        private SerializedProperty _backgroundColorProperty;
        private SerializedProperty _lineColorProperty;
        private SerializedProperty _spawnPositionProperty;
        private SerializedProperty _straightProperty;
        private SerializedProperty _cornerProperty;
        private SerializedProperty _tJunctionProperty;
        private SerializedProperty _crossJunctionProperty;
        private SerializedProperty _mazeStartProperty;
        private SerializedProperty _mazeEndProperty;
        public Vector3Int _mazeStart;
        public Vector3Int _mazeEnd;
        private Vector2 _startPosition;
        public float _startHeight;
        public float _endHeight;
        private float _minHeight;
        private float _maxHeight;
        public Vector3 _spawnPosition;
        public Color _gridBackgroundColor = Color.grey;
        public Color _gridLineColor = Color.black;
        public Gradient _heightGradient = new Gradient();
        public GameObject _straight;
        public GameObject _corner;
        public GameObject _tJunction;
        public GameObject _crossJunction;

        private string _newName;
        private int _startIndex;

        private readonly HashSet<MazeLine> _mazeLines = new();
        private bool _isDrawingLine = false;

        private Texture2D _binkAppleTexture;

        private void OnGUI()
        {
            DrawGrid();
            DrawSettings();
            DrawLines();
            HandleMouseInput();
            DrawStart();
            DrawEnd();
        }

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(this);
            _startHeightProperty = _serializedObject.FindProperty("_startHeight");
            _endHeightProperty = _serializedObject.FindProperty("_endHeight");
            _gradientProperty = _serializedObject.FindProperty("_heightGradient");
            _backgroundColorProperty = _serializedObject.FindProperty("_gridBackgroundColor");
            _lineColorProperty = _serializedObject.FindProperty("_gridLineColor");
            _spawnPositionProperty = _serializedObject.FindProperty("_spawnPosition");
            _straightProperty = _serializedObject.FindProperty("_straight");
            _cornerProperty = _serializedObject.FindProperty("_corner");
            _tJunctionProperty = _serializedObject.FindProperty("_tJunction");
            _crossJunctionProperty = _serializedObject.FindProperty("_crossJunction");
            _mazeStartProperty = _serializedObject.FindProperty("_mazeStart");
            _mazeEndProperty = _serializedObject.FindProperty("_mazeEnd");
            _straight = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Maze/Straight.prefab");
            _corner = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Maze/Corner.prefab");
            _tJunction = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Maze/T Split.prefab");
            _crossJunction = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Maze/Split.prefab");
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
            EditorGUILayout.LabelField("Grid Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_backgroundColorProperty);
            EditorGUILayout.PropertyField(_lineColorProperty);
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Maze Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_mazeStartProperty);
            EditorGUILayout.PropertyField(_mazeEndProperty);
            EditorGUILayout.PropertyField(_spawnPositionProperty);
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Maze Prefabs", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_straightProperty);
            EditorGUILayout.PropertyField(_cornerProperty);
            EditorGUILayout.PropertyField(_tJunctionProperty);
            EditorGUILayout.PropertyField(_crossJunctionProperty);
            GUILayout.Space(10);


            GUILayout.Space(10);
            if (GUILayout.Button("Generate Maze", GUILayout.Height(40)))
            {
                GenerateMaze();
            }


            if (_mazeLines.Count > 0)
            {
                GUILayout.Space(10);
                if (GUILayout.Button("Clear Maze", GUILayout.Height(40)))
                {
                    _mazeLines.Clear();
                }
            }

            _serializedObject.ApplyModifiedProperties();

            _mazeStart = ClampVector3Int(_mazeStart, 0, GridSize - 1);
            _mazeEnd = ClampVector3Int(_mazeEnd, 0, GridSize - 1);
            GUILayout.EndArea();
        }

        private Vector3Int ClampVector3Int(Vector3Int vector, int min, int max)
        {
            if (vector.x < min)
            {
                vector.x = min;
            }

            if (vector.y < min)
            {
                vector.y = min;
            }

            if (vector.z < min)
            {
                vector.z = min;
            }

            if (vector.x > max)
            {
                vector.x = max;
            }

            if (vector.y > max)
            {
                vector.y = max;
            }

            if (vector.z > max)
            {
                vector.z = max;
            }

            return vector;
        }

        private void DrawLines()
        {
            foreach (MazeLine line in _mazeLines)
            {
                float normalizedStartHeight = Mathf.InverseLerp(_minHeight, _maxHeight, line.StartHeight);
                float normalizedEndHeight = Mathf.InverseLerp(_minHeight, _maxHeight, line.EndHeight);
                Color startColor = _heightGradient.Evaluate(normalizedStartHeight);
                Color endColor = _heightGradient.Evaluate(normalizedEndHeight);

                DrawGradientLine(line.StartPosition * CellSize, line.EndPosition * CellSize, startColor, endColor, 5);
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
                    int lineAmount = Mathf.RoundToInt(Vector2.Distance(_startPosition, gridPosition) / CellSize);

                    for (int i = 0; i < lineAmount; i++)
                    {
                        Vector2 startPosition = Vector2.Lerp(_startPosition / CellSize, gridPosition / CellSize,
                            i / (float)lineAmount);
                        Vector2 endPosition = Vector2.Lerp(_startPosition / CellSize, gridPosition / CellSize,
                            (i + 1) / (float)lineAmount);

                        MazeLine newLine = new MazeLine(startPosition, endPosition, _startHeight, _endHeight);

                        _mazeLines.Add(newLine);
                    }

                    _isDrawingLine = false;
                    UpdateMinAndMax(_startHeight, _endHeight);
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

                    int lineAmount = Mathf.RoundToInt(Vector2.Distance(_startPosition, gridPosition) / CellSize);

                    for (int i = 0; i < lineAmount; i++)
                    {
                        Vector2 startPosition = Vector2.Lerp(_startPosition / CellSize, gridPosition / CellSize,
                            i / (float)lineAmount);
                        Vector2 endPosition = Vector2.Lerp(_startPosition / CellSize, gridPosition / CellSize,
                            (i + 1) / (float)lineAmount);

                        MazeLine newLine = new MazeLine(startPosition, endPosition, _startHeight, _endHeight);
                        _mazeLines.Add(newLine);
                    }

                    _isDrawingLine = false;
                    UpdateMinAndMax(_startHeight, _endHeight);
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

            foreach (MazeLine mazeLine in _mazeLines)
            {
                if (IsPointNearLine(position, mazeLine.StartPosition, mazeLine.EndPosition, threshold))
                {
                    _mazeLines.Remove(mazeLine);
                    break;
                }
            }
        }

        private bool IsPointNearLine(Vector2 point, Vector2 lineStart, Vector2 lineEnd, float threshold)
        {
            float distance = HandleUtility.DistancePointLine(point, lineStart, lineEnd);
            return distance <= threshold;
        }

        private void UpdateMinAndMax(float minHeight, float maxHeight)
        {
            if (minHeight < _minHeight)
            {
                _minHeight = minHeight;
            }

            if (maxHeight < _minHeight)
            {
                _minHeight = maxHeight;
            }

            if (minHeight > _maxHeight)
            {
                _maxHeight = minHeight;
            }

            if (maxHeight > _maxHeight)
            {
                _maxHeight = maxHeight;
            }
        }


        private void GenerateMaze()
        {
            GenerateMazeObjects();
        }

        private void DrawStart()
        {
            EditorGUI.DrawRect(
                new Rect(_mazeStart.x * CellSize + HalfBorderThickness - HalfStartEndSize,
                    _mazeStart.z * CellSize + HalfBorderThickness - HalfStartEndSize, StartEndSize, StartEndSize),
                new Color(0, 1, 0, 0.5f));
        }

        private void DrawEnd()
        {
            EditorGUI.DrawRect(
                new Rect(_mazeEnd.x * CellSize + HalfBorderThickness - HalfStartEndSize,
                    _mazeEnd.z * CellSize + HalfBorderThickness - HalfStartEndSize, StartEndSize, StartEndSize),
                new Color(1, 0, 0, 0.5f));
        }

        public void GenerateMazeObjects()
        {
            GameObject maze = new GameObject("Maze");
            maze.transform.position = _spawnPosition;
            HashSet<Point> points = new();
            foreach (MazeLine mazeLine in _mazeLines)
            {
                Vector3 startPosition = new Vector3(mazeLine.StartPosition.x, mazeLine.StartHeight,
                    mazeLine.StartPosition.y);
                Vector3 endPosition = new Vector3(mazeLine.EndPosition.x, mazeLine.EndHeight, mazeLine.EndPosition.y);

                Point startPoint = new Point(startPosition, mazeLine.StartHeight);

                if (!points.Contains(startPoint))
                {
                    startPoint._Connections.Add(endPosition);
                    points.Add(startPoint);
                }
                else
                {
                    foreach (Point point in points)
                    {
                        if (point.Equals(startPoint))
                        {
                            Debug.Log("Point already exists, adding connection: " + endPosition);

                            point._Connections.Add(endPosition);
                        }
                    }
                }

                Point endPoint = new Point(endPosition, mazeLine.EndHeight);

                if (!points.Contains(endPoint))
                {
                    endPoint._Connections.Add(startPosition);
                    points.Add(endPoint);
                }
                else
                {
                    foreach (Point point in points)
                    {
                        if (point.Equals(endPoint))
                        {
                            Debug.Log("Point already exists, adding connection: " + startPosition);

                            point._Connections.Add(startPosition);
                        }
                    }
                }
            }

            foreach (Point point in points)
            {
                switch (point._Connections.Count)
                {
                    case 1:
                        SpawnStraight(point, maze.transform);
                        break;
                    case 2:
                        if (CheckIfStraight(point))
                        {
                            SpawnStraight(point, maze.transform);
                        }
                        else
                        {
                            SpawnCorner(point, maze.transform);
                        }

                        break;
                    case 3:
                        SpawnTJunction(point, maze.transform);
                        break;
                    case 4:
                        SpawnJunction(point, maze.transform);
                        break;
                }
            }
        }

        private bool CheckIfStraight(Point point)
        {
            bool forward = point._Connections.Contains(point._Position + Vector3.forward);
            bool back = point._Connections.Contains(point._Position + Vector3.back);
            bool left = point._Connections.Contains(point._Position + Vector3.left);
            bool right = point._Connections.Contains(point._Position + Vector3.right);
            return (forward && back) || (left && right);
        }


        private void SpawnJunction(Point point, Transform mazeTransform)
        {
            GameObject go = Instantiate(_crossJunction, point._Position, Quaternion.identity, mazeTransform);
        }

        private void SpawnTJunction(Point point, Transform mazeTransform)
        {
            GameObject go = Instantiate(_tJunction, point._Position, Quaternion.identity, mazeTransform);
            go.transform.rotation = Quaternion.LookRotation(GetTJunctionDirection(point));
        }

        private void SpawnCorner(Point point, Transform mazeTransform)
        {
            GameObject go = Instantiate(_corner, point._Position, Quaternion.identity, mazeTransform);
            go.transform.rotation = Quaternion.LookRotation(GetCornerDirection(point));
        }

        private void SpawnStraight(Point point, Transform parent)
        {
            GameObject go = Instantiate(_straight, point._Position, Quaternion.identity, parent);
            go.name = "Straight connecting to " + point._Connections.Count + " other points";

            foreach (Vector3 connection in point._Connections)
            {
                Vector3 direction = connection - point._Position;
                go.transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        private Vector3 GetCornerDirection(Point point)
        {
            bool forward = point._Connections.Contains(point._Position + Vector3.forward);
            bool back = point._Connections.Contains(point._Position + Vector3.back);
            bool left = point._Connections.Contains(point._Position + Vector3.left);
            bool right = point._Connections.Contains(point._Position + Vector3.right);
        
            if (forward && left)
            {
                return Vector3.left;
            }
        
            if (forward && right)
            {
                return Vector3.forward;
            }
        
            if (back && left)
            {
                return Vector3.back;
            }
        
            if (back && right)
            {
                return Vector3.right;
            }
        
            return Vector3.zero;
        }
        //
        private Vector3 GetTJunctionDirection(Point point)
        {
            bool forward = point._Connections.Contains(point._Position + Vector3.forward);
            bool back = point._Connections.Contains(point._Position + Vector3.back);
            bool left = point._Connections.Contains(point._Position + Vector3.left);
            bool right = point._Connections.Contains(point._Position + Vector3.right);
        
            if (forward && left && right)
            {
                return Vector3.forward;
            }
        
            if (back && left && right)
            {
                return Vector3.back;
            }
        
            if (forward && back && left)
            {
                return Vector3.left;
            }
        
            if (forward && back && right)
            {
                return Vector3.right;
            }
        
            return Vector3.zero;
        }
    }
}