using UnityEngine;

namespace Audio
{
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] private SoundClip _clip;
        
        public void Play()
        {
            AudioManager.instance.PlaySound(_clip._name);
        }
    }
}