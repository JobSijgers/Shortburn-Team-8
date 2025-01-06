using System.Collections.Generic;
using UnityEngine;

namespace Maze.Editor
{
    public class Point
    {
        public readonly Vector3 _Position;
        public readonly float _Height;
        public readonly HashSet<Vector3> _Connections;

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
            return _Position == other._Position && Mathf.Approximately(_Height, other._Height);
        }

        public override int GetHashCode()
        {
            return _Position.GetHashCode() ^ _Height.GetHashCode();
        }
    }
}