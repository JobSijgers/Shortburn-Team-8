using HandScripts.Core;
using UnityEngine;
using UnityEngine.Events;

namespace HandScripts.Grab
{
    public class ItemDeposit : MonoBehaviour, IDeposit, IHandInteractable
    {
        [SerializeField] private string _depositKey;
        [SerializeField] private GrabPoint _heldPoint;
        [SerializeField] private bool _currentlyInteractable = true;
        
        public UnityEvent<IHandGrabable> _onDeposit;
        public UnityEvent<IHandGrabable> _onWithdraw;
        public void OnDeposit(IHandGrabable grabable) => _onDeposit.Invoke(grabable);
        public string GetDepositKey() => _depositKey;
        public GrabPoint GetGrabPoint() => _heldPoint;
        public EInteractType GetInteractType() => EInteractType.Deposit;
        public Transform GetObjectTransform() => transform;
        public bool CurrentlyInteractable() => _currentlyInteractable;
        public void SetInteractable(bool interactable) => _currentlyInteractable = interactable;
    }
}