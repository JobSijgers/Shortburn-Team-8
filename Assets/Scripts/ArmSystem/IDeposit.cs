using UnityEngine;

namespace ArmSystem
{
    public interface IDeposit
    {
        public Transform GetDepositPoint();
        public void Deposited();
        public string GetDepositKey();
    }
}