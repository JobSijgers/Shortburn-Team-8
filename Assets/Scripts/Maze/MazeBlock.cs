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
        
        public bool IsStraight()
        {
            if (_neighbours.Length == 1)
            {
                return true;
            }
            bool hasTwoNeighbours = _neighbours.Length == 2;
            if (!hasTwoNeighbours)
            {
                return false;
            }
            
            Vector3 firstNeighbour = _neighbours[0].transform.position;
            Vector3 secondNeighbour = _neighbours[1].transform.position;
            
            Vector3 direction = secondNeighbour - firstNeighbour;
            
            return direction.x == 0 || direction.z == 0;
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