using UnityEngine;
using System;
namespace HandScripts.Grab
{
    public class GrabPoint : MonoBehaviour
    {
        [SerializeField] private ProceduralAnimation.Finger[] _fingers;
        public Vector3 GetFingerPosition(string name) => Array.Find(_fingers, finger => finger.Name == name).Target.position; 
    }
}


