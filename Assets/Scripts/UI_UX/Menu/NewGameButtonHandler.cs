using System.Collections;
using ExorcistPath.Core.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ExorcistPath.UI_UX.Menu
{
    /// <summary>
    /// Handles the "New Game" button logic: resets progress and starts from the beginning with a fade effect.
    /// </summary>
    public class NewGameButtonHandler : MonoBehaviour
    {
        [Header("Scene Transitions")]
        [Tooltip("The scene to load after resetting progress (e.g., 'Story' or 'Map_01').")]
        [SerializeField] private string firstSceneName = "Story";

        [Header("Fade Settings")]
        [Tooltip("Reference to a full screen black Image used as fade overlay.")]
        [SerializeField] private Image fadeOverlay;
        [Tooltip("Duration (in seconds) for fading to black.")]
        [SerializeField] private float fadeOutDuration = 0.6f;
        [Tooltip("Prevents multiple clicks while loading.")]
        [SerializeField] private bool disableButtonWhileLoading = true;

        private bool isBusy;

        /// <summary>
        /// Resets the game data and starts the loading routine with fade.
        /// Call this from the Button's OnClick() event.
        /// </summary>
        public void OnNewGameClicked()
        {
            if (isBusy) return;
            StartCoroutine(LoadRoutine());
        }

        private IEnumerator LoadRoutine()
        {
            isBusy = true;
            Debug.Log("[NewGameButtonHandler] Resetting progress and starting new game...");

            // 1. Disable button to prevent double clicking
            Button btn = GetComponent<Button>();
            if (disableButtonWhileLoading && btn != null)
            {
                btn.interactable = false;
            }

            // 2. Reset all saved data via SaveManager
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.ResetProgress();
            }
            else
            {
                PlayerPrefs.DeleteKey("HighestUnlockedMap");
                PlayerPrefs.DeleteKey("TotalCoins");
                PlayerPrefs.Save();
                Debug.LogWarning("[NewGameButtonHandler] SaveManager.Instance not found. Resetting via direct PlayerPrefs.");
            }

            // 3. Fade screen to black before loading scene
            if (fadeOverlay != null)
            {
                yield return FadeToBlack(1f, fadeOutDuration);
            }

            // 4. Load the first scene
            SceneManager.LoadScene(firstSceneName);
        }

        private IEnumerator FadeToBlack(float targetAlpha, float duration)
        {
            if (fadeOverlay == null) yield break;

            float startAlpha = fadeOverlay.color.a;
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
            
            // Block clicks when overlay is visible
            fadeOverlay.raycastTarget = color.a > 0.01f;
        }
    }
}
