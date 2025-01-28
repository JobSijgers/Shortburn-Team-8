using System.Collections;
using Audio;
using Player;
using UnityEngine;

public class WalkingSounds : MonoBehaviour
{
    [SerializeField] private SoundClip[] _walkingSounds;
    [SerializeField] private SoundClip[] _creakSound;
    [SerializeField] private float _walkInterval;
    [SerializeField] private float _runInterval;
    [SerializeField] private float _hopInterval;
    [Range(0, 100)][SerializeField] private int _creakChance;
    private LegSystem.LegSystem _legSystem;
    private PlayerMovement _playerMovement;
    private float _currentInterval;
    private Coroutine _soundCoroutine;
    private AudioManager _audioManager;

    private void Start()
    {
        _legSystem = LegSystem.LegSystem.Instance;
        _playerMovement = PlayerMovement.Instance;
        _audioManager = AudioManager.instance;
    }

    private void Update()
    {
        Debug.Log(_playerMovement.PlayerMovementState);
        if (_playerMovement.PlayerMovementState != EPlayerMovementState.Idle && _soundCoroutine == null)
        {
            _soundCoroutine = StartCoroutine(PlaySound());
        }
        
        bool hasLeg = !_legSystem.enabled ? false : _legSystem.Leg == null;
        if (!hasLeg)
        {
            _currentInterval = _hopInterval;
            return;
        }

        switch (_playerMovement.PlayerMovementState)
        {
            case EPlayerMovementState.Walking:
                _currentInterval = _walkInterval;
                break;
            case EPlayerMovementState.Running:
                _currentInterval = _runInterval;
                break;
        }
    }

    private IEnumerator PlaySound()
    {
        while (true)
        {
            yield return new WaitForSeconds(_currentInterval);
            if (_playerMovement.PlayerMovementState == EPlayerMovementState.Idle)
            {
                _soundCoroutine = null;
                yield break;
            }
            SoundClip sound = _walkingSounds[Random.Range(0, _walkingSounds.Length)];
            _audioManager.PlaySound(sound._name);
            if (Random.Range(0, 100) < _creakChance)
            {
                SoundClip creak = _creakSound[Random.Range(0, _creakSound.Length)];
                _audioManager.PlaySound(creak._name);
            }
        }
    }
}