using UnityEngine;

namespace HandScripts.Grab
{
    public interface IHandGrabable
    {
        void SetParent(Transform newParent);
        void ResetPosition();
        void Grabbed();
        string GetDepositKey();
    }
}