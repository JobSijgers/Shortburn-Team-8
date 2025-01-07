using System;
using HandScripts.Grab;
using UnityEngine;

namespace Cooking
{
    public class CookingMachine : MonoBehaviour
    {
        [SerializeField] private CookingRecipe[] _recipes;
        [SerializeField] private ItemDeposit[] _deposits;
        [SerializeField] private Transform _resultSpawnPoint;

        private ICookable[] _ingredients;

        private void Start()
        {
            _ingredients = new ICookable[_deposits.Length];

            for (int i = 0; i < _deposits.Length; i++)
            {
                int index = i;
                _deposits[i]._onDeposit.AddListener(c => OnDeposit(c, index));
            }
        }

        private void OnDeposit(IHandGrabable grabable, int index)
        {
            if (grabable is not ICookable cookable) return;
            
            _ingredients[index] = cookable;
        }
        
        public void TryCook()
        {
            foreach (CookingRecipe recipe in _recipes)
            {
                if (!recipe.CanCook(_ingredients)) 
                    continue;
                Instantiate(recipe._resultPrefab, _resultSpawnPoint.position, _resultSpawnPoint.rotation);
                break;
            }
        }
    }
}