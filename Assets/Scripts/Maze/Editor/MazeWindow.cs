using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Maze.Editor
{
    public class MazeWindow : EditorWindow
    {
        private const int GridSize = 20;
        private const int CellSize = 30;
        private const int StartEndSize = 12;
        private const int HalfStartEndSize = StartEndSize / 2;
        private const int BorderThickness = 2;
        private const int HalfBorderThickness = BorderThickness / 2;
        private const int GridWidth = GridSize * CellSize + BorderThickness + 300;
        private const int GridHeight = GridSize * CellSize + BorderThickness;
        private const int LineThickness = 4;

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
        public Vector2Int _mazeStart;
        public Vector2Int _mazeEnd;
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
        private readonly Dictionary<Point, MazeBlock> _mazeBlocks = new();


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
            InitializeSerializedProperties();
            LoadMazePrefabs();
        }

        [MenuItem("Window/Custom Grid")]
        public static void ShowWindow()
        {
            MazeWindow window = GetWindow<MazeWindow>("Custom Grid");
            SetWindowSizeConstraints(window);
        }

        private void InitializeSerializedProperties()
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
        }

        private void LoadMazePrefabs()
        {
            _straight = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Maze/P_railsstraight.prefab");
            _corner = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Maze/P_railscorners.prefab");
            _tJunction = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Maze/P_rails3way.prefab");
            _crossJunction = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Maze/P_rails4way.prefab");
        }

        private static void SetWindowSizeConstraints(MazeWindow window)
        {
            window.minSize = new Vector2(GridWidth, GridHeight);
            window.maxSize = new Vector2(GridWidth, GridHeight);
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
            DrawHeightSettings();
            DrawGridSettings();
            DrawMazeSettings();
            DrawMazePrefabs();
            DrawGenerateAndClearButtons();
            _serializedObject.ApplyModifiedProperties();
            ClampMazeStartEnd();
            GUILayout.EndArea();
        }

        private void DrawHeightSettings()
        {
            EditorGUILayout.LabelField("Height Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_startHeightProperty);
            EditorGUILayout.PropertyField(_endHeightProperty);
            EditorGUILayout.PropertyField(_gradientProperty);
            GUILayout.Space(10);
        }

        private void DrawGridSettings()
        {
            EditorGUILayout.LabelField("Grid Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_backgroundColorProperty);
            EditorGUILayout.PropertyField(_lineColorProperty);
            GUILayout.Space(10);
        }

        private void DrawMazeSettings()
        {
            EditorGUILayout.LabelField("Maze Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_mazeStartProperty);
            EditorGUILayout.PropertyField(_mazeEndProperty);
            EditorGUILayout.PropertyField(_spawnPositionProperty);
            GUILayout.Space(10);
        }

        private void DrawMazePrefabs()
        {
            EditorGUILayout.LabelField("Maze Prefabs", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_straightProperty);
            EditorGUILayout.PropertyField(_cornerProperty);
            EditorGUILayout.PropertyField(_tJunctionProperty);
            EditorGUILayout.PropertyField(_crossJunctionProperty);
            GUILayout.Space(10);
        }

        private void DrawGenerateAndClearButtons()
        {
            //if the maze start and end point fall on a line position, the generate maze button will be displayed
            bool isStartOnLine =
                _mazeLines.Any(line => line.StartPosition == _mazeStart || line.EndPosition == _mazeStart);
            bool isEndOnLine = _mazeLines.Any(line => line.StartPosition == _mazeEnd || line.EndPosition == _mazeEnd);
            if (!isStartOnLine || !isEndOnLine)
            {
                GUILayout.Label("Place the start and end positions on the grid to generate the maze",
                    EditorStyles.boldLabel);
            }
            else if (GUILayout.Button("Generate Maze", GUILayout.Height(40)))
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
        }

        private void ClampMazeStartEnd()
        {
            _mazeStart = ClampVector2Int(_mazeStart, 0, GridSize - 1);
            _mazeEnd = ClampVector2Int(_mazeEnd, 0, GridSize - 1);
        }

        private Vector2Int ClampVector2Int(Vector2Int vector, int min, int max)
        {
            vector.x = Mathf.Clamp(vector.x, min, max);
            vector.y = Mathf.Clamp(vector.y, min, max);
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
                Vector2 start = line.StartPosition * CellSize + new Vector2(HalfBorderThickness, HalfBorderThickness);
                Vector2 end = line.EndPosition * CellSize + new Vector2(HalfBorderThickness, HalfBorderThickness);
                DrawGradientLine(start, end, startColor, endColor, LineThickness);
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
                        float startHeight = Mathf.Lerp(_startHeight, _endHeight, i / (float)lineAmount);
                        float endHeight = Mathf.Lerp(_startHeight, _endHeight, (i + 1) / (float)lineAmount);

                        MazeLine newLine = new MazeLine(startPosition, endPosition, startHeight, endHeight);

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
            _minHeight = Mathf.Min(_minHeight, minHeight);
            _maxHeight = Mathf.Max(_maxHeight, maxHeight);
        }

        private void GenerateMaze()
        {
            GenerateMazeObjects();
        }

        private void DrawStart()
        {
            EditorGUI.DrawRect(
                new Rect(_mazeStart.x * CellSize + HalfBorderThickness - HalfStartEndSize,
                    _mazeStart.y * CellSize + HalfBorderThickness - HalfStartEndSize, StartEndSize, StartEndSize),
                new Color(0, 1, 0, 0.5f));
        }

        private void DrawEnd()
        {
            EditorGUI.DrawRect(
                new Rect(_mazeEnd.x * CellSize + HalfBorderThickness - HalfStartEndSize,
                    _mazeEnd.y * CellSize + HalfBorderThickness - HalfStartEndSize, StartEndSize, StartEndSize),
                new Color(1, 0, 0, 0.5f));
        }

        private Point AddConnection(HashSet<Point> points, Vector3 position, float height, Vector3 connection)
        {
            Point point = new Point(position, height);
            if (!points.Contains(point))
            {
                point._Connections.Add(connection);
                points.Add(point);
            }
            else
            {
                foreach (Point existingPoint in points)
                {
                    if (existingPoint.Equals(point))
                    {
                        existingPoint._Connections.Add(connection);
                        point = existingPoint;
                        break;
                    }
                }
            }

            return point;
        }

        private MazeBlock SpawnMazeObject(Point point, Transform mazeTransform)
        {
            MazeBlock spawnedBlock;
            switch (point._Connections.Count)
            {
                case 1:
                    return SpawnStraight(point, mazeTransform);
                case 2 when CheckIfStraight(point):
                {
                    return SpawnStraight(point, mazeTransform);
                }
                case 2:
                    return SpawnCorner(point, mazeTransform);
                case 3:
                    return SpawnTJunction(point, mazeTransform);
                case 4:
                    return SpawnJunction(point, mazeTransform);
            }

            return null;
        }

        private void GenerateMazeObjects()
        {
            _mazeBlocks.Clear();
            GameObject maze = new GameObject("Maze");
            maze.transform.position = _spawnPosition;
            maze.AddComponent<Maze>();
            HashSet<Point> points = new();

            Point startPoint = null;
            Point endPoint = null;
            MazeBlock startBlock = null;
            MazeBlock endBlock = null;

            foreach (MazeLine mazeLine in _mazeLines)
            {
                Vector3 startPosition = new Vector3(mazeLine.StartPosition.x, mazeLine.StartHeight,
                    mazeLine.StartPosition.y);
                Vector3 endPosition = new Vector3(mazeLine.EndPosition.x, mazeLine.EndHeight, mazeLine.EndPosition.y);

                if (mazeLine.StartPosition == _mazeStart)
                {
                    startPoint = AddConnection(points, startPosition, mazeLine.StartHeight, endPosition);
                }
                else if (mazeLine.StartPosition == _mazeEnd)
                {
                    endPoint = AddConnection(points, startPosition, mazeLine.StartHeight, endPosition);
                }
                else
                {
                    AddConnection(points, startPosition, mazeLine.StartHeight, endPosition);
                }

                if (mazeLine.EndPosition == _mazeStart)
                {
                    startPoint = AddConnection(points, endPosition, mazeLine.EndHeight, startPosition);
                }
                else if (mazeLine.EndPosition == _mazeEnd)
                {
                    endPoint = AddConnection(points, endPosition, mazeLine.EndHeight, startPosition);
                }
                else
                {
                    AddConnection(points, endPosition, mazeLine.EndHeight, startPosition);
                }
            }

            foreach (Point point in points)
            {
                if (point.Equals(startPoint))
                {
                    startBlock = SpawnMazeObject(point, maze.transform);
                }
                else if (point.Equals(endPoint))
                {
                    endBlock = SpawnMazeObject(point, maze.transform);
                }
                else
                {
                    SpawnMazeObject(point, maze.transform);
                }
            }

            foreach (KeyValuePair<Point, MazeBlock> mazeBlock in _mazeBlocks)
            {
                HashSet<MazeBlock> neighbours = new();
                foreach (Vector3 connection in mazeBlock.Key._Connections)
                {
                    foreach (KeyValuePair<Point, MazeBlock> otherMazeBlock in _mazeBlocks)
                    {
                        if (otherMazeBlock.Key._Position == connection)
                        {
                            neighbours.Add(otherMazeBlock.Value);
                        }
                    }
                }

                mazeBlock.Value.SetNeighbours(neighbours.ToArray());
            }
         
            maze.GetComponent<Maze>().SetCurrentBlock(startBlock);
            maze.GetComponent<Maze>().SetEndBlock(endBlock);
            
            _serializedObject.ApplyModifiedProperties();
        }

        private bool CheckIfStraight(Point point)
        {
            Vector3 forward = Vector3.forward;
            Vector3 back = Vector3.back;
            Vector3 left = Vector3.left;
            Vector3 right = Vector3.right;

            bool isForwardBack =
                point._Connections.Any(conn =>
                    Vector3.Normalize(new Vector3(conn.x - point._Position.x, 0, conn.z - point._Position.z)) ==
                    forward) && point._Connections.Any(conn =>
                    Vector3.Normalize(new Vector3(conn.x - point._Position.x, 0, conn.z - point._Position.z)) == back);

            bool isLeftRight =
                point._Connections.Any(conn =>
                    Vector3.Normalize(new Vector3(conn.x - point._Position.x, 0, conn.z - point._Position.z)) ==
                    left) && point._Connections.Any(conn =>
                    Vector3.Normalize(new Vector3(conn.x - point._Position.x, 0, conn.z - point._Position.z)) == right);

            return isForwardBack || isLeftRight;
        }


        private MazeBlock SpawnJunction(Point point, Transform mazeTransform)
        {
            GameObject go = Instantiate(_crossJunction, point._Position, Quaternion.identity, mazeTransform);
            MazeBlock block = go.AddComponent<MazeBlock>();
            _mazeBlocks.Add(point, block);
            return block;
        }

        private MazeBlock SpawnTJunction(Point point, Transform mazeTransform)
        {
            GameObject go = Instantiate(_tJunction, point._Position, Quaternion.identity, mazeTransform);
            go.transform.rotation = Quaternion.LookRotation(GetTJunctionDirection(point));
            MazeBlock block = go.AddComponent<MazeBlock>();
            _mazeBlocks.Add(point, block);
            return block;
        }

        private MazeBlock SpawnCorner(Point point, Transform mazeTransform)
        {
            GameObject go = Instantiate(_corner, point._Position, Quaternion.identity, mazeTransform);
            go.transform.rotation = Quaternion.LookRotation(GetCornerDirection(point));
            MazeBlock block = go.AddComponent<MazeBlock>();
            _mazeBlocks.Add(point, block);
            return block;
        }

        private MazeBlock SpawnStraight(Point point, Transform parent)
        {
            Vector3 connection = point._Connections.First();
            Vector3 direction = (connection - point._Position).normalized;

            float distance = Vector3.Distance(point._Position, connection);
            Vector3 midpoint = (point._Position + connection) / 2;

            bool hasHeightDifference = !Mathf.Approximately(connection.y, point._Position.y);

            GameObject go = Instantiate(_straight, midpoint, Quaternion.identity, parent);

            go.transform.rotation = Quaternion.LookRotation(direction);

            go.transform.localScale = new Vector3(go.transform.localScale.x, go.transform.localScale.y, distance);

            if (hasHeightDifference)
            {
                go.transform.position = new Vector3(midpoint.x, midpoint.y, midpoint.z);
                go.name = "Straight (Height Difference)";
                Quaternion oldRotation = go.transform.rotation;
                Vector3 rotation = go.transform.rotation.eulerAngles;
                rotation.x = 0;
                go.transform.rotation = Quaternion.Euler(rotation);
                go.transform.localPosition -= go.transform.forward / 2;
                go.transform.rotation = oldRotation;
            }
            else
            {
                go.transform.position -= go.transform.forward / 2;
            }

            MazeBlock block = go.AddComponent<MazeBlock>();
            _mazeBlocks.Add(point, block);
            return block;
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