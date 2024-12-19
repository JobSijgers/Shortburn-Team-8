using HandScripts.Core;
using UnityEngine;
using HandScripts.Pull;

namespace Maze
{
    public class Maze : MonoBehaviour
    {
        [SerializeField] private GameObject _mazeFollowObject;
        [SerializeField] private Transform _player;
        [SerializeReference] private MazeBlock _currentBlock;
        [SerializeReference] private MazeBlock _endBlock;

        private bool _isPastMidpoint = false;
        
        public void Pull(Vector3 currentLocation)
        {
            bool currentBlockIsStraight = _currentBlock.IsStraight();
            MazeBlock target = _currentBlock.GetNextMazeBlock(currentLocation);
            
            
        }

        public void SetCurrentBlock(MazeBlock block)
        {
            _currentBlock = block;
        }

        public void SetEndBlock(MazeBlock block)
        {
            _endBlock = block;
        }
    }
}