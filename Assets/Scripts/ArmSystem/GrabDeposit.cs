using UnityEngine;
using UnityEngine.Events;

namespace ArmSystem
{
    public class GrabDeposit : MonoBehaviour, IDeposit
    {
        [SerializeField] private Transform _depositPoint;
        [SerializeField] private UnityEvent _onDeposit;
        [SerializeField] private string _key;

        public Transform GetDepositPoint() => _depositPoint;
        public void Deposited() => _onDeposit.Invoke();
        public string GetDepositKey() => _key;
    }
}