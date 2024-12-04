using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
    public class MazeBlock : MonoBehaviour
    {
        [SerializeField] private HashSet<MazeBlock> _neighbours;

        public void SetNeighbours(HashSet<MazeBlock> neighbours)
        {
            _neighbours = neighbours;
            gameObject.name = $"MazeBlock with ({_neighbours.Count}) neighbours";
        }
        
        
    }
}