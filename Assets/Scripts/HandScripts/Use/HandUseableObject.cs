using System.Collections;
using HandScripts.Core;
using UnityEngine;
using UnityEngine.Events;

namespace HandScripts.Use
{
    public class HandUseableObject : MonoBehaviour, IHandUseable, IHandInteractable
    {
        [SerializeField] private Transform _heldPoint;
        [SerializeField] private float _useDuration = 1f;
        [SerializeField] private UnityEvent _onUseStart;
        [SerializeField] private UnityEvent _onUseEnd;

        private bool _hasBeenUsed;
        
        public Transform GetHeldPoint() => _heldPoint;
        public EInteractType GetInteractType() => EInteractType.Use;
        public Transform GetObjectTransform() => transform;
        public bool HasBeenUsed() => _hasBeenUsed;

        public void Use(UnityAction onComplete)
        {
            StartCoroutine(UseRoutine(onComplete));
            _hasBeenUsed = true;
        }


        private IEnumerator UseRoutine(UnityAction onComplete)
        {
            _onUseStart?.Invoke();
            yield return new WaitForSeconds(_useDuration);
            _onUseEnd?.Invoke();
            onComplete?.Invoke();
        }
        
    }
}