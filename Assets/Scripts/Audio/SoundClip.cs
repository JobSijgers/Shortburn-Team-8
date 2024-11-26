using UnityEngine;

namespace Audio
{
    public enum SoundType
    {
        Music,
        Sfx
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
}