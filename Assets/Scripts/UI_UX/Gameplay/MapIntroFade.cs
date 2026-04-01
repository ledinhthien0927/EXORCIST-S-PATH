using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ExorcistPath.UI_UX.Gameplay
{
    /// <summary>
    /// Handles the fade-in and fade-out effect for a UI element (like the Map Intro image)
    /// when the scene starts or the object is enabled.
    /// </summary>
    public class MapIntroFade : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The CanvasGroup to fade. If null, will try to find one on this object.")]
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Delay Settings")]
        [SerializeField] private float initialDelay = 0.5f;    // Wait before starting fade-in

        [Header("Fade Durations")]
        [SerializeField] private float fadeInDuration = 1.0f;  // Time to reach full opacity
        [SerializeField] private float holdDuration = 2.0f;    // Time to stay fully opaque
        [SerializeField] private float fadeOutDuration = 1.0f; // Time to reach full transparency

        [Header("Post-Fade Options")]
        [SerializeField] private bool disableOnFinish = true;  // Disable the GameObject after fade-out

        private void Awake()
        {
            // Auto-assign CanvasGroup if not set
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                
                // If still null, add one automatically
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }
        }

        private void OnEnable()
        {
            // Reset alpha and start the sequence
            canvasGroup.alpha = 0f;
            StartCoroutine(FadeSequenceRoutine());
        }

        private IEnumerator FadeSequenceRoutine()
        {
            // 1. Initial Delay
            if (initialDelay > 0)
            {
                yield return new WaitForSeconds(initialDelay);
            }

            // 2. Fade In
            yield return Fade(0, 1, fadeInDuration);

            // 3. Hold
            if (holdDuration > 0)
            {
                yield return new WaitForSeconds(holdDuration);
            }

            // 4. Fade Out
            yield return Fade(1, 0, fadeOutDuration);

            // 5. Cleanup
            if (disableOnFinish)
            {
                gameObject.SetActive(false);
            }
        }

        private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
        {
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float normalizedTime = Mathf.Clamp01(elapsed / duration);
                
                // Use smoothstep for a more natural feel
                float easedAlpha = Mathf.SmoothStep(startAlpha, endAlpha, normalizedTime);
                canvasGroup.alpha = easedAlpha;
                
                yield return null;
            }

            canvasGroup.alpha = endAlpha;
        }
    }
}
