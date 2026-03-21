using System.Collections;
using UnityEngine;

public class HolyMarkReveal : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Material glowMaterial;

    private Material originalMaterial;
    private Coroutine routine;

    private void Awake()
    {
        if (targetRenderer != null)
            originalMaterial = targetRenderer.material;
    }

    public void Reveal(float duration)
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(RevealRoutine(duration));
    }

    private IEnumerator RevealRoutine(float duration)
    {
        if (targetRenderer != null && glowMaterial != null)
            targetRenderer.material = glowMaterial;

        yield return new WaitForSeconds(duration);

        if (targetRenderer != null && originalMaterial != null)
            targetRenderer.material = originalMaterial;

        routine = null;
    }
}