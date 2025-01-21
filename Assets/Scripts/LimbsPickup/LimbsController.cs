using UnityEditor;
using UnityEngine;
using HandScripts.Core;
using System;
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

        public void PickupLimb(Type type)
        {
            if (type == typeof(LegSystem))
            {
                Leg.enabled = true;
            }
            else if (type == typeof(HandSystem))
            {
                Hand.enabled = true;
            }
            else
            {
                Debug.LogWarning($"Unsupported limb type: {type}");
            }
        }

    }
}

