using System.Collections.Generic;
using UnityEngine;


namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;
        [SerializeField] private SoundClip[] _soundClips;

        private void Awake() => instance = this;
        private readonly Dictionary<string, SoundClip> soundDictionary = new();

        private void Start()
        {
            //play sounds that are set to play on awake
            foreach (SoundClip sound in _soundClips)
            {
                if (sound._clip == null)
                {
                    Debug.LogWarning($"Sound clip: {sound.name} is null");
                    continue;
                }

                soundDictionary.Add(sound._name, sound);
                Debug.Log(sound._name);
                CreateAudioSource(sound);
            }

            _soundClips = null;
        }

        //find sound and play it
        public void PlaySound(string soundName)
        {
            if (!soundDictionary.TryGetValue(soundName, out SoundClip sound))
            {
                return;
            }

            if (!sound._source.isPlaying)
            {
                sound._source.Play();
                return;
            }

            if (sound._disableCutoff && sound._source.time > sound._clip.length * sound._cutoffRange)
            {
                sound._source.Play();
            }
        }

        public void StopSound(string soundName)
        {
            if (!soundDictionary.TryGetValue(soundName, out SoundClip sound))
            {
                return;
            }

            sound._source.Stop();
        }

        private void CreateAudioSource(SoundClip sound)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = sound._clip;
            audioSource.volume = sound._volume;
            audioSource.pitch = sound._pitch;
            audioSource.loop = sound._loop;
            audioSource.outputAudioMixerGroup = sound._mixerGroup;
            sound._source = audioSource;
            if (sound._playOnAwake)
            {
                audioSource.Play();
            }
        }
    }
}