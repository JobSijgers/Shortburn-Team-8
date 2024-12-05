using UnityEngine;
using UnityEngine.Events;

namespace HandScripts.Grab
{
    public class ItemDepositManager : MonoBehaviour
    {
        [SerializeField] private ItemDeposit[] _itemDeposits;
        [SerializeField] private UnityEvent _onAllDepositComplete;
        private int _currentDepositIndex;

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