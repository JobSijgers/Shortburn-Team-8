using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AlphaLerper : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    public float _lerpSpeed;
    
    public void LerpAlpha(float targetAlpha)
    {
        StartCoroutine(LerpAlphaCoroutine(targetAlpha));
    }

    private IEnumerator LerpAlphaCoroutine(float targetAlpha)
    {
        float t = 0;
        float startAlpha = _canvasGroup.alpha;
        
        while (t < 1)
        {
            t += Time.deltaTime * _lerpSpeed;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        yield return new WaitForSeconds(5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);   
    }
}
