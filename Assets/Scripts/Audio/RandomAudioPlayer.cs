using UnityEngine;

namespace Audio
{
    public class RandomAudioPlayer : MonoBehaviour
    {
        [SerializeField] private SoundClip[] _soundClips;
        [SerializeField] private float _minTimeBetweenPlays = 40f;
        [SerializeField] private float _maxTimeBetweenPlays = 60f;
        
        private float _timeToNextPlay;
        
        private void Start()
        {
            _timeToNextPlay = Random.Range(_minTimeBetweenPlays, _maxTimeBetweenPlays);
        }
        
        private void Update()
        {
            _timeToNextPlay -= Time.deltaTime;
            if (_timeToNextPlay <= 0)
            {
                PlayRandomSound();
                _timeToNextPlay = Random.Range(_minTimeBetweenPlays, _maxTimeBetweenPlays);
            }
        }

        private void PlayRandomSound()
        {
            SoundClip randomSound = _soundClips[Random.Range(0, _soundClips.Length)];
            
            if (randomSound._clip == null)
            {
                Debug.LogWarning($"Sound clip: {randomSound._name} is null");
                return;
            }
            
            AudioManager.instance.PlaySound(randomSound._name);
        }
    }
}