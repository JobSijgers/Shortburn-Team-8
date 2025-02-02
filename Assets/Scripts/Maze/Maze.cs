using System.Collections;
using HandScripts.Core;
using UnityEngine;
using HandScripts.Pull;

namespace Maze
{
    public class Maze : MonoBehaviour
    {
        [SerializeField] private MazePullable _mazeFollowObject;
        [SerializeField] private Transform _player;
        [SerializeReference] private MazeBlock _currentBlock;
        [SerializeReference] private MazeBlock _endBlock;
        public MazeBlock GetEndBlock() => _endBlock;

        private bool _isPastMidpoint = false;
        public Vector3 GetNextPosition()
        {
            bool currentBlockIsStraight = _currentBlock.IsStraight();
            MazeBlock target = _currentBlock.GetNextMazeBlock(_player.position);
            if (_currentBlock == _endBlock) return _endBlock.transform.position;
            
            Vector3 currentPos = _mazeFollowObject.transform.position;
            Vector3 targetPos = target.transform.position;
            Vector3 direction = targetPos - currentPos;
            Vector3 nextPos = currentPos + direction.normalized * _mazeFollowObject.GetPullSpeed();
            
            if (Vector3.Distance(nextPos, target.transform.position) < 0.05f)
            {
                _currentBlock = target;
            }
            // rotatefollow boject in direction of movement smoothly
            _mazeFollowObject.transform.rotation = Quaternion.Slerp(_mazeFollowObject.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10f);

            return nextPos;
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