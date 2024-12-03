using UnityEngine;

namespace Maze.Editor
{
    public struct Line
    {
        public Line(Vector2 startPosition, Vector2 endPosition, float startHeight, float endHeight)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
            StartHeight = startHeight;
            EndHeight = endHeight;
        }

        public Vector2 StartPosition;
        public Vector2 EndPosition;
        public float StartHeight;
        public float EndHeight;
    }
}