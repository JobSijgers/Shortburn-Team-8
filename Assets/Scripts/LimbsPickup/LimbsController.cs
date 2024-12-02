using UnityEditor;
using UnityEngine;
namespace LimbsPickup
{
    using LegSystem;
    using HandScripts.Core;
    public class LimbsController : MonoBehaviour
    {
        public static LimbsController Instance;
        public LegSystem Leg;
        public HandSystem Hand;
        private void Awake()
        {
            Instance = this;
            Leg.enabled = false;
            Hand.enabled = false;
        }

        public void PickupLimb(MonoScript type)
        {
            
            if (type.GetClass() == typeof (LegSystem))
            {
                Leg.enabled = true;
            }
            else if (type.GetClass() == typeof (HandSystem))
            {
                Hand.enabled = true;
            }
        }
    }
}

