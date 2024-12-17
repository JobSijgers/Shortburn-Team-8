using System.Collections;
using HandScripts.Core;
using HandScripts.Grab;
using UnityEngine;
using UnityEngine.Events;

namespace HandScripts.Use
{
    public class HandUseableObject : MonoBehaviour, IHandUseable, IHandInteractable
    {
        [SerializeField] private GrabPoint _heldPoint;
        [SerializeField] private float _useDuration = 1f;
        public UnityEvent _onUseStart;
        public UnityEvent _onUseEnd;
        [SerializeField] private bool _canTriggerMultipleTimes;
        [SerializeField] private float _useCooldown;
        private bool _hasBeenUsed;
        
        
        public GrabPoint GetGrabPoint() => _heldPoint;
        public EInteractType GetInteractType() => EInteractType.Use;
        public Transform GetObjectTransform() => transform;
        public bool HasBeenUsed() => _hasBeenUsed;

        public void Use(UnityAction onComplete)
        {
            StartCoroutine(UseRoutine(onComplete));
            if (!_canTriggerMultipleTimes)
                _hasBeenUsed = true;
        }
        
        private IEnumerator UseRoutine(UnityAction onComplete)
        {
            _onUseStart?.Invoke();
            yield return new WaitForSeconds(_useDuration);
            _onUseEnd?.Invoke();
            onComplete?.Invoke();
            if (_canTriggerMultipleTimes)
            {
                _hasBeenUsed = true;
                yield return new WaitForSeconds(_useCooldown);
                _hasBeenUsed = false;
            }
        }
    }
}