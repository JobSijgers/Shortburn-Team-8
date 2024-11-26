using UnityEngine;
using UnityEngine.Serialization;

public enum SoundType
{
    Music,
    SFX
}
[CreateAssetMenu(fileName = "New AudioClip", menuName = "Audio/AudioClip")]
public class SoundClip : ScriptableObject
{
    public string Name;
    public AudioClip Clip;
    public SoundType SoundType;
    public bool Loop;
    public bool PlayOnAwake;
    public float Volume;
    public float Pitch;
    public AudioSource Source;
}
