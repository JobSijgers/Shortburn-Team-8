using System.Collections;
using HandScripts.Grab;
using UnityEngine;
using UnityEngine.Android;

public class EmissionEnabler : MonoBehaviour
{
    private static readonly int EmissiveColor = Shader.PropertyToID("_EmissionColor");
    
    public void EnableEmission(IHandGrabable grabable)
    {
        Material newMaterial = new Material(grabable.GetMaterial());
        newMaterial.EnableKeyword("_EMISSION");
        grabable.GetMeshRenderer().material = newMaterial;
        StartCoroutine(LerpColor(newMaterial, 2));
    }

    public IEnumerator LerpColor(Material material, float time)
    {
        Color targetColor = material.GetColor(EmissiveColor);
        material.SetColor(EmissiveColor, Color.black);
        
        float t = 0;
        
        while (t < 1)
        {
            t += Time.deltaTime / time;
            material.SetColor(EmissiveColor, Color.Lerp(Color.black, targetColor, t));
            yield return null;
        }
    }
    
}