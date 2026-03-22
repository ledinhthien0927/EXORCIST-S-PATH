using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using ExorcistPath.Core.Managers;

namespace ExorcistPath.UI_UX.Gameplay
{
    /// <summary>
    /// Manages the gameplay UI, including pausing, stat updates, and navigation.
    /// </summary>
    public class GameplayUIManager : MonoBehaviour
    {
        [Header("Pause Panel")]
        [SerializeField] private GameObject panelPause;
        [SerializeField] private Button buttonPause;
        [SerializeField] private Button buttonContinue;
        [SerializeField] private Button buttonHome;

        [Header("Stats UI")]
        [SerializeField] private TextMeshProUGUI textMoney;
        [SerializeField] private TextMeshProUGUI textPurification;

        [Header("Fade Settings")]
        [SerializeField] private Image fadeOverlay;
        [SerializeField] private float fadeOutDuration = 0.6f;

        private bool isPaused = false;
        private bool isTransitioning = false;

        private void Start()
        {
            // Initial UI Setup
            if (panelPause != null) panelPause.SetActive(false);
            
            UpdateMoneyText(SaveManager.Instance != null ? SaveManager.Instance.GetTotalCoins() : 0);
            UpdatePurificationText(GameManager.Instance != null ? GameManager.Instance.GetPurificationPercentage() : 0f);

            // Subscribe to events
            if (SaveManager.Instance != null)
                SaveManager.Instance.OnCoinsChanged += UpdateMoneyText;

            if (GameManager.Instance != null)
                GameManager.Instance.OnPurificationChanged += UpdatePurificationText;

            // Setup button listeners
            if (buttonPause != null) buttonPause.onClick.AddListener(() => TogglePause(true));
            if (buttonContinue != null) buttonContinue.onClick.AddListener(() => TogglePause(false));
            if (buttonHome != null) buttonHome.onClick.AddListener(OnHomeClicked);
            
            // Ensure fade overlay is transparent at start
            if (fadeOverlay != null) SetFadeAlpha(0f);
        }

        private void OnDestroy()
        {
            // Unsubscribe to avoid memory leaks
            if (SaveManager.Instance != null)
                SaveManager.Instance.OnCoinsChanged -= UpdateMoneyText;

            if (GameManager.Instance != null)
                GameManager.Instance.OnPurificationChanged -= UpdatePurificationText;
        }

        private void Update()
        {
            // Optional: Handle Escape/Back key for pausing on mobile/desktop
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause(!isPaused);
            }
        }

        /// <summary>
        /// Toggles the game's pause state.
        /// </summary>
        public void TogglePause(bool pause)
        {
            if (isTransitioning) return;

            isPaused = pause;
            Time.timeScale = isPaused ? 0f : 1f;

            if (panelPause != null)
            {
                panelPause.SetActive(isPaused);
            }

            Debug.Log($"[GameplayUIManager] Game {(isPaused ? "Paused" : "Resumed")}");
        }

        /// <summary>
        /// Called when the Home button is clicked. Returns to Main Menu with a fade.
        /// </summary>
        private void OnHomeClicked()
        {
            if (isTransitioning) return;
            StartCoroutine(LoadHomeRoutine());
        }

        private IEnumerator LoadHomeRoutine()
        {
            isTransitioning = true;
            Time.timeScale = 1f; // Ensure time flows for coroutines if paused

            if (fadeOverlay != null)
            {
                yield return FadeToBlack(1f, fadeOutDuration);
            }

            SceneManager.LoadScene("MainMenu");
        }

        private void UpdateMoneyText(int totalCoins)
        {
            if (textMoney != null)
            {
                textMoney.text = totalCoins.ToString("N0");
            }
        }

        private void UpdatePurificationText(float percentage)
        {
            if (textPurification != null)
            {
                textPurification.text = $"{Mathf.RoundToInt(percentage)}%";
            }
        }

        private IEnumerator FadeToBlack(float targetAlpha, float duration)
        {
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
            fadeOverlay.raycastTarget = color.a > 0.01f;
        }
    }
}
