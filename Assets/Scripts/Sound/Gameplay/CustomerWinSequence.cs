using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ExorcistPath.Core.Managers;

namespace ExorcistPath.Gameplay
{
    /// <summary>
    /// Handles the visual sequence when a ritual is completed.
    /// Spawns the customer, plays animations, and if it's the last ritual, 
    /// transitions to the Map Selection screen.
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

        [Header("Animation Settings")]
        [Tooltip("Name of the boolean parameter in the Animator to trigger the win sequence.")]
        [SerializeField] private string winParameterName = "IsWingame";
        [Tooltip("Wait time before starting the flight movement (fallback if animator not found).")]
        [SerializeField] private float flightDelay = 1.5f;

        [Header("Scene Transition Settings")]
        [Tooltip("Scene to load after sequence finishes.")]
        [SerializeField] private string nextSceneName = "MapSelection";
        [Tooltip("Fade overlay image (black screen).")]
        [SerializeField] private Image fadeOverlay;
        [Tooltip("Duration of the fade-to-black effect.")]
        [SerializeField] private float fadeDuration = 1f;

        private GameObject spawnedCustomer;

        private void Start()
        {
            if (fadeOverlay != null)
                SetFadeAlpha(0f); // Ensure screen is transparent initially

            // 1. Spawn the customer immediately at the start of the level
            if (customerPrefab != null && customerSpawnPoint != null)
            {
                spawnedCustomer = Instantiate(customerPrefab, customerSpawnPoint.position, customerSpawnPoint.rotation);
            }
        }

        public void PlaySequence()
        {
            Debug.Log("[CustomerWinSequence] Triggered!");
            StartCoroutine(WinRoutine());
        }

        private IEnumerator WinRoutine()
        {
            Animator anim = null;

            if (spawnedCustomer != null)
            {
                anim = spawnedCustomer.GetComponentInChildren<Animator>();
                
                // 2. Trigger Win Animation via parameter
                if (anim != null && !string.IsNullOrEmpty(winParameterName))
                {
                    anim.SetBool(winParameterName, true);
                    
                    // 3. Wait for the Animator to transition into the "Fly" clip
                    // (This ensures KneeToFly has finished playing)
                    while (!anim.GetCurrentAnimatorStateInfo(0).IsName("Fly"))
                    {
                        yield return null;
                    }
                }
                else
                {
                    // Fallback if no animator found
                    yield return new WaitForSeconds(flightDelay);
                }
            }
            else
            {
                // Fallback: If for some reason it wasn't spawned, spawn it now or skip
                Debug.LogWarning("[CustomerWinSequence] No spawned customer found in WinRoutine!");
                yield return new WaitForSeconds(1f);
            }

            // 4. Fly towards target
            if (spawnedCustomer != null && customerTargetPoint != null)
            {
                while (Vector3.Distance(spawnedCustomer.transform.position, customerTargetPoint.position) > 0.1f)
                {
                    // Move vertically/towards target
                    spawnedCustomer.transform.position = Vector3.MoveTowards(
                        spawnedCustomer.transform.position, 
                        customerTargetPoint.position, 
                        flySpeed * Time.deltaTime
                    );
                    yield return null;
                }
            }

            // 5. Check if we should end the game
            if (GameManager.Instance != null)
            {
                bool isGameWon = GameManager.Instance.CompleteRitual();
                
                // Dọn dẹp khách hàng sau khi bay xong
                if (spawnedCustomer != null) Destroy(spawnedCustomer);
                
                // Việc hiển thị UI và chuyển cảnh đã được giao lại cho GameplayUIManager (sự kiện OnGameWon)
            }
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
            
            if (!fadeOverlay.gameObject.activeSelf && alpha > 0f)
            {
                fadeOverlay.gameObject.SetActive(true);
            }

            Color color = fadeOverlay.color;
            color.a = Mathf.Clamp01(alpha);
            fadeOverlay.color = color;
            fadeOverlay.raycastTarget = color.a > 0.01f;
        }
    }
}
