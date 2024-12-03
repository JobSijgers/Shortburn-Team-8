using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Xml.Schema;
using Maze.Editor;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Maze
{
    public class Maze : MonoBehaviour
    {
        private readonly Dictionary<Vector3, GameObject> _tiles = new();
        private readonly Dictionary<GameObject, int> _intersections = new();

        public void GenerateMaze(Line[] linePoints, GameObject maze, GameObject straight, GameObject corner,
            GameObject tJunction, GameObject crossJunction)
        {
            foreach (Line line in linePoints)
            {
                for (int i = 0; i < Vector2.Distance(line.StartPosition, line.EndPosition) + 1; i++)
                {
                    Debug.Log("Creating tile");
                    Vector2 position = Vector2.Lerp(line.StartPosition, line.EndPosition,
                        i / Vector2.Distance(line.StartPosition, line.EndPosition));
                    Vector3 position3D = new(position.x, 0, position.y);
                    if (_tiles.ContainsKey(position3D))
                    {
                        Debug.Log("Tile already exists");
                        _tiles.TryGetValue(position3D, out GameObject intersection);
                        if (intersection != null)
                        {
                            intersection.name = "Intersection: " + position3D;
                            
                            _intersections.Add(intersection, 0);
                        }

                        continue;
                    }

                    GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    tile.name = "Tile: " + position3D;
                    tile.transform.position = position3D;
                    tile.transform.SetParent(maze.transform);
                    _tiles.Add(tile.transform.position, tile);
                }
            }

            List<KeyValuePair<GameObject, int>> updates = new();

            foreach (KeyValuePair<GameObject, int> intersection in _intersections)
            {
                int neighbours = GetIntersectType(intersection.Key.transform.position);
                intersection.Key.gameObject.name = "Intersection: " + intersection.Key.transform.position +
                                                   " Neighbours: " + neighbours;
                updates.Add(new KeyValuePair<GameObject, int>(intersection.Key, neighbours));
            }

            foreach (KeyValuePair<GameObject, int> update in updates)
            {
                _intersections[update.Key] = update.Value;
            }

            List<GameObject> totalMaze;
            foreach (KeyValuePair<Vector3, GameObject> tile in _tiles)
            {
                Vector3 position = tile.Key;
                GameObject tileObject = tile.Value;
                GameObject newObject = null;

                if (_intersections.ContainsKey(tileObject))
                {
                    switch (_intersections[tileObject])
                    {
                        case 2:
                            newObject = Instantiate(corner, position,
                                Quaternion.LookRotation(GetCornerDirection(tile.Key)));
                            break;
                        case 3:
                            newObject = Instantiate(tJunction, position,
                                Quaternion.LookRotation(GetTJunctionDirection(tile.Key)));
                            break;
                        case 4:
                            newObject = Instantiate(crossJunction, position, Quaternion.identity);
                            break;
                    }
                }
                else
                {
                    newObject = Instantiate(straight, position,
                        Quaternion.LookRotation(GetStraightDirection(tile.Key)));
                }

            }


            List<Vector3> keys = new List<Vector3>(_tiles.Keys);

            foreach (Vector3 key in keys)
            {
                GameObject tile = _tiles[key];
                if (tile != null)
                {
                    DestroyImmediate(tile);
                }
            }


            _tiles.Clear();
            _intersections.Clear();
        }

        private Vector3 GetCornerDirection(Vector3 position)
        {
            bool forward = _tiles.ContainsKey(position + Vector3.forward);
            bool back = _tiles.ContainsKey(position + Vector3.back);
            bool left = _tiles.ContainsKey(position + Vector3.left);
            bool right = _tiles.ContainsKey(position + Vector3.right);

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

        private Vector3 GetTJunctionDirection(Vector3 position)
        {
            bool forward = _tiles.ContainsKey(position + Vector3.forward);
            bool back = _tiles.ContainsKey(position + Vector3.back);
            bool left = _tiles.ContainsKey(position + Vector3.left);
            bool right = _tiles.ContainsKey(position + Vector3.right);

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

        private Vector3 GetStraightDirection(Vector3 position)
        {
            bool forward = _tiles.ContainsKey(position + Vector3.forward);
            bool back = _tiles.ContainsKey(position + Vector3.back);
            bool left = _tiles.ContainsKey(position + Vector3.left);
            bool right = _tiles.ContainsKey(position + Vector3.right);

            if (forward || back)
            {
                return Vector3.forward;
            }

            if (left || right)
            {
                return Vector3.right;
            }

            return Vector3.zero;
        }

        private int GetIntersectType(Vector3 position)
        {
            int total = 0;
            bool forward = _tiles.ContainsKey(position + Vector3.forward);
            bool back = _tiles.ContainsKey(position + Vector3.back);
            bool left = _tiles.ContainsKey(position + Vector3.left);
            bool right = _tiles.ContainsKey(position + Vector3.right);

            if (forward) total++;
            if (back) total++;
            if (left) total++;
            if (right) total++;

            return total;
        }
    }
}