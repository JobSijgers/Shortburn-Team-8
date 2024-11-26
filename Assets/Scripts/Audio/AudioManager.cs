using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] private AudioMixer _mixer;
    private SoundClip[] _audioClips;

    private void Awake()
    {
        Instance = this;
        
        // get all audio clips
        _audioClips = Resources.LoadAll<SoundClip>("AudioClips");
        foreach (var audioClip in _audioClips)
        {
            audioClip.Source = gameObject.AddComponent<AudioSource>();
            audioClip.Source.clip = audioClip.Clip;
            audioClip.Source.volume = audioClip.Volume;
            audioClip.Source.pitch = audioClip.Pitch;
            audioClip.Source.loop = audioClip.Loop;
            audioClip.Source.outputAudioMixerGroup = _mixer.FindMatchingGroups(audioClip.SoundType.ToString())[0];
        }
    }
    public void PlayClip(string clipName)
    {
        SoundClip clip = GetClip(clipName);
        if (clip == null) return;
        clip.Source.Play();
    }
    public void StopClip(string clipName)
    {
        SoundClip clip = GetClip(clipName);
        if (clip == null) return;
        clip.Source.Stop();
    }
    private SoundClip GetClip(string clipName)
    {
        SoundClip clip = Array.Find(_audioClips, sound => sound.Name == clipName);
        if (clip == null)
        {
            Debug.LogError($"Sound clip with name {clipName} not found!");
            return null;
        }
        return clip;
    }
}
