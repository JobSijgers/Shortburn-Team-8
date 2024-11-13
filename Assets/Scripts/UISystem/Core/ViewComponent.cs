using UnityEngine;

namespace UISystem.Core
{
    public abstract class ViewComponent : MonoBehaviour
    {
        public virtual void Initialize()
        {
        }

        public virtual void Show()
        {
        }

        public virtual void Hide()
        {
        }
    }
}