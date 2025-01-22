using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace Audio
{
    [CreateAssetMenu(fileName = "New AudioClip", menuName = "Audio/AudioClip")]
    public class SoundClip : ScriptableObject
    {
        public string _name;
        public AudioClip _clip;
        [Range(0f, 1f)] public float _volume = 0.7f;
        [Range(0.5f, 1.5f)] public float _pitch = 1f;
        public bool _loop;
        public bool _playOnAwake;
        public bool _disableCutoff;
        [Range(0f, 1f)] public float _cutoffRange;
        public AudioMixerGroup _mixerGroup;
        [HideInInspector] public AudioSource _source;
    }
}