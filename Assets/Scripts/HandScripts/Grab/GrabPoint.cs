using UnityEngine;
using System;
using PathCreation;

namespace HandScripts.Grab
{
    public class GrabPoint : MonoBehaviour
    {
        [SerializeField] private ProceduralAnimation.Finger[] _fingers;
        public PathCreator PathCreator;
        public Vector3 GetFingerPosition(string name) => Array.Find(_fingers, finger => finger.Name == name).Target.position; 
    }
}


