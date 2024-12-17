using System;
using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
    public class MazeBlock : MonoBehaviour
    {
        [SerializeReference] private MazeBlock[] _neighbours;
        
        public void SetNeighbours(MazeBlock[] neighbours)
        {
            _neighbours = neighbours;
            gameObject.name = $"MazeBlock with ({_neighbours.Length}) neighbours";
        }
        

        public MazeBlock GetNextMazeBlock(Vector3 playerPosition)
        {
            float minDistance = float.MaxValue;
            MazeBlock nextBlock = null;
            
            foreach (var neighbour in _neighbours)
            {
                float distance = Vector3.Distance(neighbour.transform.position, playerPosition);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nextBlock = neighbour;
                }
            }
            
            return nextBlock;
        }
    }
}