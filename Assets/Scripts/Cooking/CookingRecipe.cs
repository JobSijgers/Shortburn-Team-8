using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cooking
{
    [CreateAssetMenu(fileName = "Cooking Recipe", menuName = "Cooking Recipe", order = 0)]
    public class CookingRecipe : ScriptableObject
    {
        public string[] _ingredients;
        public GameObject _resultPrefab;
        
        public bool CanCook(ICookable[] ingredients)
        {
            if (ingredients.Length != _ingredients.Length) return false;
            
            foreach (var ingredient in _ingredients)
            {
                bool found = false;
                foreach (var i in ingredients)
                {
                    if (i == null)
                        continue;

                    if (i.GetCookingKey() != ingredient)
                        continue;
                    
                    found = true;
                    break;
                }

                if (!found) return false;
            }

            return true;
        }
    }
}