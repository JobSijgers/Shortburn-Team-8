using System;
using UnityEngine;
using UnityEngine.Events;

namespace HandScripts.Grab
{
    public class ItemDepositManager : MonoBehaviour
    {
        [SerializeField] private ItemDeposit[] _itemDeposits;
        [SerializeField] private UnityEvent _onAllDepositComplete;
        private int _currentDepositIndex;

        private void Start()
        {
            foreach (ItemDeposit itemDeposit in _itemDeposits)
            {
                itemDeposit._onDeposit.AddListener(ctx => DepositItem());
            }
        }

        public void DepositItem()
        {
            _currentDepositIndex++;
            if (_currentDepositIndex >= _itemDeposits.Length)
            {
                _onAllDepositComplete.Invoke();
            }
        }
    }
}