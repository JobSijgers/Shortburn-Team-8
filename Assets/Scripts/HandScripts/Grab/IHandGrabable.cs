using UnityEngine;

namespace HandScripts.Grab
{
    public interface IHandGrabable
    {
        void SetParent(Transform newParent);
        void ResetPosition(Quaternion rotation);
        void Grabbed();
        void Released();
        string GetDepositKey();
    }
}