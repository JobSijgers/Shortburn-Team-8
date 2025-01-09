using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightLerper : MonoBehaviour
{
    [SerializeField] private Light _light;

    [SerializeField] private float _minIntensity;
    [SerializeField] private float _maxIntensity;
    [SerializeField] private float _moveSpeed;

    private float _currentIntensity;
    private float _targetIntensity;

    private void Start()
    {
        _currentIntensity = _light.intensity;
        _targetIntensity = _currentIntensity;
    }

    public void UpdateLight(float t)
    {
        _targetIntensity = Mathf.Lerp(_minIntensity, _maxIntensity, t);
    }

    private void Update()
    {
        float increment = _moveSpeed * Time.deltaTime;
        _currentIntensity = Mathf.MoveTowards(_currentIntensity, _targetIntensity, increment);
        _currentIntensity = Mathf.Clamp(_currentIntensity, _minIntensity, _maxIntensity);
        _light.intensity = _currentIntensity;
    }
}