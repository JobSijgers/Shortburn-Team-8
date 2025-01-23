using System.Collections;
using System.Collections.Generic;
using Audio;
using Player;
using UnityEngine;

public class WalkingSounds : MonoBehaviour
{
    [SerializeField] private SoundClip[] _walkingSounds;
    private PlayerMovement _playerMovement;
}
