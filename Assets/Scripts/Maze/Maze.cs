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

        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_currentBlock == _endBlock)
                {
                    Debug.Log("You won!");
                }
                else
                {
                    MazeBlock nextBlock = _currentBlock.GetNextMazeBlock(_player.position);
                    if (nextBlock != null)
                    {
                        _currentBlock = nextBlock;
                    }
                    _mazeFollowObject.transform.position = _currentBlock.transform.position;
                }
            }
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