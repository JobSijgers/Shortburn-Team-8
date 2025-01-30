using Audio;
using UnityEngine;
public class RandomAudioClipPlayer : MonoBehaviour
{
    [SerializeField] private SoundClip[] _clips;
    private AudioManager _audioManager;

    private void Start()
    {
        _audioManager = AudioManager.instance;
    }

    public void Play()
    {
        SoundClip clip = _clips[Random.Range(0, _clips.Length)];
        _audioManager.PlaySound(clip._name);
    }
}