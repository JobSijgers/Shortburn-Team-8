using UnityEngine;

namespace HandScripts.Grab
{
    public class StorageHand : MonoBehaviour
    {
        private IHandGrabable _storedObject;

        public void StoreObject(IHandGrabable grabable) => _storedObject = grabable;
        public IHandGrabable GetStoredObject() => _storedObject;
        public void ClearStoredObject() => _storedObject = null;
    }
}