using HandScripts.Core;
using HandScripts.Grab;
using HandScripts.Pull;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Maze
{
    public class MazePullable : MonoBehaviour, IHandInteractable, IHandPullable
    {
        [SerializeField] private GrabPoint _handHoldPoint;
        [SerializeField] private float _pullStep;
        [SerializeField] private Maze _maze;
        [SerializeField] private bool _currentlyInteractable = true;

        public GrabPoint GetGrabPoint() => _handHoldPoint;
        public EInteractType GetInteractType() => EInteractType.Pull;
        public Transform GetObjectTransform() => transform;
        public float GetPullSpeed() => _pullStep;
        public bool CanPull(Vector3 playerPosition) => true;
        public bool HasBeenPulled() => false;
        public bool CurrentlyInteractable() => _currentlyInteractable;

        public void Pull(UnityAction onComplete)
        {
            transform.position = _maze.GetNextPosition();
        }
    }
}