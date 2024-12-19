using HandScripts.Core;
using UnityEngine;
using UnityEngine.Events;

namespace HandScripts.Grab
{
    public class ItemDeposit : MonoBehaviour, IDeposit, IHandInteractable
    {
        [SerializeField] private string _depositKey;
        [SerializeField] private GrabPoint _heldPoint;
        [SerializeField] private UnityEvent _onDeposit;
        
        public void OnDeposit(IHandGrabable grabable) => _onDeposit.Invoke();
        public string GetDepositKey() => _depositKey;
        public GrabPoint GetGrabPoint() => _heldPoint;
        public EInteractType GetInteractType() => EInteractType.Deposit;
        public Transform GetObjectTransform() => transform;
    }
}