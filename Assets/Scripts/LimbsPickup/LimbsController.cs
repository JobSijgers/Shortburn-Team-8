using UnityEditor;
using UnityEngine;
using HandScripts.Core;
namespace LimbsPickup
{
    using LegSystem;
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

        public bool LegState => Leg.enabled;
        public bool HandState => Hand.enabled;

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

