using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ExorcistPath.Core.Managers;

namespace ExorcistPath.Gameplay
{
    /// <summary>
    /// Handles the visual sequence when the game is won based on the GDD.
    /// Spawns the customer, plays animations, and transitions to the Map Selection screen.
    /// </summary>
    public class CustomerWinSequence : MonoBehaviour
    {
        [Header("Customer Settings")]
        [Tooltip("The customer prefab to spawn.")]
        [SerializeField] private GameObject customerPrefab;
        [Tooltip("Where the customer should spawn.")]
        [SerializeField] private Transform customerSpawnPoint;
        [Tooltip("Where the customer flies towards.")]
        [SerializeField] private Transform customerTargetPoint;
        [Tooltip("Speed at which the customer flies upwards.")]
        [SerializeField] private float flySpeed = 2f;

        [Header("Animation States")]
        [Tooltip("Name of the Idle animation state in the Animator.")]
        [SerializeField] private string idleStateName = "Idle";
        [Tooltip("Name of the Fly animation state in the Animator.")]
        [SerializeField] private string flyStateName = "Fly";
        [Tooltip("Time to wait in Idle before flying.")]
        [SerializeField] private float idleDuration = 2f;

        [Header("Scene Transition Settings")]
        [Tooltip("Scene to load after sequence finishes.")]
        [SerializeField] private string nextSceneName = "MapSelection";
        [Tooltip("Fade overlay image (black screen).")]
        [SerializeField] private Image fadeOverlay;
        [Tooltip("Duration of the fade-to-black effect.")]
        [SerializeField] private float fadeDuration = 1f;

        private void Start()
        {
            if (fadeOverlay != null)
                SetFadeAlpha(0f); // Ensure screen is transparent initially

            // Subscribe to the Win event
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameWon += StartWinSequence;
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe to avoid memory leaks
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameWon -= StartWinSequence;
            }
        }

        private void StartWinSequence()
        {
            Debug.Log("[CustomerWinSequence] Triggered!");
            StartCoroutine(WinRoutine());
        }

        private IEnumerator WinRoutine()
        {
            // Optional: Disable player input or pause game time here if needed
            // Time.timeScale = 1f;

            // 1. Spawn Customer
            GameObject customer = null;
            Animator anim = null;
            
            if (customerPrefab != null && customerSpawnPoint != null)
            {
                customer = Instantiate(customerPrefab, customerSpawnPoint.position, customerSpawnPoint.rotation);
                anim = customer.GetComponentInChildren<Animator>();
                
                // 2. Play Idle Animation
                if (anim != null) anim.CrossFade(idleStateName, 0.1f);
            }

            // Wait during Idle
            yield return new WaitForSeconds(idleDuration);

            // 3. Play Fly Animation
            if (anim != null) anim.CrossFade(flyStateName, 0.1f);

            // 4. Fly towards target
            if (customer != null && customerTargetPoint != null)
            {
                while (Vector3.Distance(customer.transform.position, customerTargetPoint.position) > 0.1f)
                {
                    // Move vertically/towards target
                    customer.transform.position = Vector3.MoveTowards(
                        customer.transform.position, 
                        customerTargetPoint.position, 
                        flySpeed * Time.deltaTime
                    );
                    yield return null;
                }
            }
            else
            {
                // Fallback if no target set, just wait a bit
                yield return new WaitForSeconds(2f);
            }

            // 5. Fade to Black
            if (fadeOverlay != null)
            {
                yield return FadeToBlack(1f, fadeDuration);
            }

            // 6. Transition to Next Scene
            SceneManager.LoadScene(nextSceneName);
        }

        private IEnumerator FadeToBlack(float targetAlpha, float duration)
        {
            float startAlpha = fadeOverlay != null ? fadeOverlay.color.a : 0f;
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
                SetFadeAlpha(alpha);
                yield return null;
            }

            SetFadeAlpha(targetAlpha);
        }

        private void SetFadeAlpha(float alpha)
        {
            if (fadeOverlay == null) return;
            Color color = fadeOverlay.color;
            color.a = Mathf.Clamp01(alpha);
            fadeOverlay.color = color;
            fadeOverlay.raycastTarget = color.a > 0.01f;
        }
    }
}
