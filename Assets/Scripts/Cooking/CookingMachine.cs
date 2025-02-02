using System;
using System.Collections;
using HandScripts.Grab;
using UnityEngine;
using UnityEngine.Events;

namespace Cooking
{
    public class CookingMachine : MonoBehaviour
    {
        [SerializeField] private CookingRecipe[] _recipes;
        [SerializeField] private ItemDeposit[] _deposits;
        [SerializeField] private Transform _resultSpawnPoint;
        [SerializeField] private float _timeToCook;

        [SerializeField] private UnityEvent _onWrongIngredients;
        [SerializeField] private UnityEvent _onCorrectIngredients;
        
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

            foreach (ICookable ingredient in _ingredients)
            {
                if (ingredient == null)
                {
                    return;
                }
            }
            
            StartCoroutine(TryCook());
        }
            
        public IEnumerator TryCook()
        {
            yield return new WaitForSeconds(_timeToCook);

            foreach (CookingRecipe recipe in _recipes)
            {
                if (!recipe.CanCook(_ingredients)) 
                    continue;
                Instantiate(recipe._resultPrefab, _resultSpawnPoint.position, _resultSpawnPoint.rotation);
                // Clear ingredients
                foreach (ICookable cookable in _ingredients)
                {
                    cookable.Destroy();
                }
                _ingredients = new ICookable[_deposits.Length];
                _onCorrectIngredients.Invoke();
                yield break;
            }

            _onWrongIngredients.Invoke();
            yield return new WaitForSeconds(0.2f);
            for (int i = 0; i < _deposits.Length; i++)
            {
                _deposits[i]._onWithdraw.Invoke((IHandGrabable)_ingredients[i]);
                _ingredients[i] = null;
            }
        }
    }
}