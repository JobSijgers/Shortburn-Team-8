using HandScripts.Use;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class MultipleEventTrigger : MonoBehaviour
{
    [SerializeField] private PressurePlate[] _pressurePlates;
    [SerializeField] private HandUseableObject[] _useableObjects;
    [FormerlySerializedAs("_onAllPressurePlate")] [SerializeField] private UnityEvent _onAllEvents;
    [FormerlySerializedAs("_onUnPressurePlate")] [SerializeField] private UnityEvent _onUnEvents;
    private int _currentEventIndex;
    
    private void Start()
    {
        foreach (var pressurePlate in _pressurePlates)
        {
            pressurePlate.OnPressurePlate.AddListener(OnEvent);
            pressurePlate.OnUnPressurePlate.AddListener(OnEventEnd);
        }

        foreach (var useableObject in _useableObjects)
        {
            useableObject._onUseStart.AddListener(OnEvent);
            useableObject._onUseEnd.AddListener(OnEventEnd);
        }
    }

    private void OnEvent()
    {
        _currentEventIndex++;
        if (_currentEventIndex >= _pressurePlates.Length + _useableObjects.Length)
        {
            _onAllEvents.Invoke();
        }
        Debug.Log(_currentEventIndex);
    }

    private void OnEventEnd()
    {
        if (_currentEventIndex == _pressurePlates.Length + _useableObjects.Length)
        {
            _onUnEvents.Invoke();
        }
        _currentEventIndex--;
    }
}