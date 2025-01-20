using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        [Serializable]
        public class Sound
        {
            public string name;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume = 0.7f;
            [Range(0.5f, 1.5f)] public float pitch = 1f;
            public bool loop;
            public bool playOnAwake;
            public bool disableCutoff;
            [Range(0f, 1f)] public float cutoffRange;
            public AudioMixerGroup mixerGroup;
            [HideInInspector] public AudioSource source;
        }

        public static AudioManager instance;
        [SerializeField] private Sound[] sounds;

        private void Awake() => instance = this;
        private readonly Dictionary<string, Sound> soundDictionary = new();

        private void Start()
        {
            //play sounds that are set to play on awake
            foreach (Sound sound in sounds)
            {
                if (sound.clip == null)
                {
                    Debug.LogWarning($"Sound clip: {sound.name} is null");
                    continue;
                }

                soundDictionary.Add(sound.name, sound);
                CreateAudioSource(sound);
            }

            sounds = null;
        }

        //find sound and play it
        public void PlaySound(string soundName)
        {
            if (!soundDictionary.TryGetValue(soundName, out Sound sound))
            {
                return;
            }

            if (!sound.source.isPlaying)
            {
                sound.source.Play();
                return;
            }

            if (sound.disableCutoff && sound.source.time > sound.clip.length * sound.cutoffRange)
            {
                sound.source.Play();
            }
        }

        public void StopSound(string soundName)
        {
            if (!soundDictionary.TryGetValue(soundName, out Sound sound))
            {
                return;
            }

            sound.source.Stop();
        }

        private void CreateAudioSource(Sound sound)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = sound.clip;
            audioSource.volume = sound.volume;
            audioSource.pitch = sound.pitch;
            audioSource.loop = sound.loop;
            audioSource.outputAudioMixerGroup = sound.mixerGroup;
            sound.source = audioSource;
            if (sound.playOnAwake)
            {
                audioSource.Play();
            }
        }
    }
}