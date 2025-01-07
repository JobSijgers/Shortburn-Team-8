using HandScripts.Grab;
using UnityEngine;

namespace Cooking
{
    public class CookingObject : GrabableObject, ICookable
    {
        [SerializeField] private string _cookingKey;
        public string GetCookingKey() => _cookingKey;
    }
}