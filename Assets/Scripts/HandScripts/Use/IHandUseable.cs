using UnityEngine;
using UnityEngine.Events;

namespace HandScripts.Use
{
    public interface IHandUseable
    {
        void Use(UnityAction onComplete);
        bool HasBeenUsed();
    }
}