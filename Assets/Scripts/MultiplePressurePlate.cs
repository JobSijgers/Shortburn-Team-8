using UnityEngine;
using UnityEngine.Events;

public class MultiplePressurePlate : MonoBehaviour
{
    [SerializeField] private PressurePlate[] _pressurePlates;
    [SerializeField] private UnityEvent _onAllPressurePlate;
    
    private int _currentPressurePlateIndex;
    
    private void Start()
    {
        foreach (var pressurePlate in _pressurePlates)
        {
            pressurePlate.OnPressurePlate.AddListener(OnPressurePlate);
            pressurePlate.OnUnPressurePlate.AddListener(OnUnPressurePlate);
        }
    }

    private void OnPressurePlate()
    {
        _currentPressurePlateIndex++;
        if (_currentPressurePlateIndex >= _pressurePlates.Length)
        {
            _onAllPressurePlate.Invoke();
        }
    }

    private void OnUnPressurePlate()
    {
        _currentPressurePlateIndex--;
    }
}