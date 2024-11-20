using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelReset : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;
    private InputAction _resetAction;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _resetAction = _playerInputActions.Player.Reset;
        _resetAction.Enable();
        _resetAction.performed += ctx => ResetLevel();
    }

    private void OnDisable()
    {
        _resetAction.performed -= ctx => ResetLevel();
        _resetAction.Disable();
    }

    private void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}