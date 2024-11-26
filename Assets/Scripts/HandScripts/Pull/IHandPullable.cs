using UnityEngine;
using UnityEngine.Events;

namespace HandScripts.Pull
{
    public interface IHandPullable
    {
        void Pull(UnityAction onComplete);
        bool CanPull(Vector3 playerPosition);
    }
}