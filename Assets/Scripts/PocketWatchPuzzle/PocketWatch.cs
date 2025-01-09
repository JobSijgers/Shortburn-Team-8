using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace PocketWatchPuzzle
{
    public class PocketWatch : MonoBehaviour
    {
        [SerializeField] private WatchButton _hourButton;
        [SerializeField] private WatchButton _minuteButton;
        [SerializeField] private WatchButton _secondButton;
        [SerializeField] private Vector3 _startTime;
        [SerializeField] private Vector3 _correctTime;
        [SerializeField] private UnityEvent _onComplete;

        private int GetHour() => (int)_hourButton.GetHand().localEulerAngles.z / 30;
        private int GetMinute() => (int)_minuteButton.GetHand().localEulerAngles.z / 6;
        private int GetSecond() => (int)_secondButton.GetHand().localEulerAngles.z / 6;

        private void OnEnable()
        {
            _hourButton.OnTimeUpdated += OnTimeUpdated;
            _minuteButton.OnTimeUpdated += OnTimeUpdated;
            _secondButton.OnTimeUpdated += OnTimeUpdated;
        }

        private void Start()
        {
            SetTime(_startTime.x, _startTime.y, _startTime.z);
        }

        private void SetTime(float hours, float minutes, float seconds)
        {
            _hourButton.GetHand().localEulerAngles = new Vector3(0, 0, hours * 30);
            _minuteButton.GetHand().localEulerAngles = new Vector3(0, 0, minutes * 6);
            _secondButton.GetHand().localEulerAngles = new Vector3(0, 0, seconds * 6);
        }

        private void OnTimeUpdated()
        {
            if (GetHour() == _correctTime.x &&
                GetMinute() == _correctTime.y &&
                GetSecond() == _correctTime.z)
            {
                _onComplete.Invoke();
            }
        }
    }
}