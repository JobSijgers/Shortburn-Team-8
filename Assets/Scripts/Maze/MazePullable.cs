using HandScripts.Core;
using HandScripts.Pull;
using UnityEngine;
using UnityEngine.Events;

namespace Maze
{
    public class MazePullable : MonoBehaviour, IHandInteractable, IHandPullable
    {
        [SerializeField] private Transform _handHoldPoint;
        [SerializeField] private float _pullSpeed;
        [SerializeField] private Maze _maze;

        public Transform GetHeldPoint() => _handHoldPoint;
        public EInteractType GetInteractType() => EInteractType.Pull;
        public Transform GetObjectTransform() => transform;
        public bool CanPull(Vector3 playerPosition) => true;
        public bool HasBeenPulled() => false;

        public void Pull(UnityAction onComplete)
        {
            throw new System.NotImplementedException();
        }
    }
}