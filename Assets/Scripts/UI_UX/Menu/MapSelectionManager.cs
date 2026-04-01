using System.Collections;
using ExorcistPath.Core.Managers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace ExorcistPath.UI_UX.Menu
{
    public class MapSelectionManager : MonoBehaviour
    {
        [Header("Map Buttons (Order: Map 1 to Map 4)")]
        [Tooltip("Assign the Map Buttons in order here, starting from Map 1.")]
        [SerializeField] private Button[] mapButtons;

        [Header("Visual Settings")]
        [Tooltip("Alpha value applied to locked map buttons to make them appear dim.")]
        [SerializeField, Range(0f, 1f)] private float lockedAlpha = 0.5f;

        [Header("Coin UI")]
        [Tooltip("The text component displaying the player's total coins.")]
        [SerializeField] private TextMeshProUGUI totalCoinText;

        [Header("Navigation Buttons")]
        [Tooltip("Reference to the Back button to Main Menu.")]
        [SerializeField] private Button backButton;
        [Tooltip("Optional: Reference to the Hack button to be disabled during transitions.")]
        [SerializeField] private Button hackButton;

        [Header("Fade Settings")]
        [Tooltip("Reference to a full screen black Image used as fade overlay.")]
        [SerializeField] private Image fadeOverlay;
        [Tooltip("Duration (in seconds) for fading to black before scene load.")]
        [SerializeField] private float fadeOutDuration = 0.6f;

        private bool isBusy;

        private void Start()
        {
            UpdateMapButtons();

            // Fade in from black at the start of the scene
            if (fadeOverlay != null)
            {
                SetFadeAlpha(1f);
                StartCoroutine(FadeToBlack(0f, fadeOutDuration));
            }
        }

        /// <summary>
        /// Updates the interactability and visual dimming of the map buttons based on saved progress.
        /// </summary>
        public void UpdateMapButtons()
        {
            // Update Coin Text
            if (totalCoinText != null)
            {
                int totalCoins = SaveManager.Instance != null ? SaveManager.Instance.GetTotalCoins() : PlayerPrefs.GetInt("TotalCoins", 0);
                totalCoinText.text = totalCoins.ToString("N0"); // Formats as 2,000,000
            }

            // Default to 1 if SaveManager doesn't exist yet, to prevent breaking
            int highestUnlockedMap = 1;
            
            if (SaveManager.Instance != null)
            {
                highestUnlockedMap = SaveManager.Instance.GetHighestUnlockedMap();
            }
            else
            {
                highestUnlockedMap = PlayerPrefs.GetInt("HighestUnlockedMap", 1);
                Debug.LogWarning("[MapSelectionManager] SaveManager Instance not found. Falling back to direct PlayerPrefs.");
            }

            for (int i = 0; i < mapButtons.Length; i++)
            {
                if (mapButtons[i] == null) continue;

                int mapLevel = i + 1; // Maps are 1-indexed

                // 1. Map is already unlocked
                if (mapLevel <= highestUnlockedMap)
                {
                    mapButtons[i].interactable = true;
                    SetButtonAlpha(mapButtons[i], 1f);

                    // Add click listener to load the scene
                    mapButtons[i].onClick.RemoveAllListeners();
                    mapButtons[i].onClick.AddListener(() => OnMapButtonClicked(mapLevel));
                }
                // 2. Map is the NEXT one to unlock
                else if (mapLevel == highestUnlockedMap + 1)
                {
                    int price = SaveManager.Instance != null ? SaveManager.Instance.GetMapPrice(mapLevel) : 9999999;
                    int totalCoins = SaveManager.Instance != null ? SaveManager.Instance.GetTotalCoins() : PlayerPrefs.GetInt("TotalCoins", 0);

                    if (totalCoins >= price)
                    {
                        // Can afford to buy
                        mapButtons[i].interactable = true;
                        SetButtonAlpha(mapButtons[i], 1f); // Maybe slightly different visual to indicate purchasable?
                        
                        mapButtons[i].onClick.RemoveAllListeners();
                        mapButtons[i].onClick.AddListener(() => OnPurchaseMapClicked(mapLevel));
                    }
                    else
                    {
                        // Cannot afford
                        mapButtons[i].interactable = false;
                        SetButtonAlpha(mapButtons[i], lockedAlpha);
                        mapButtons[i].onClick.RemoveAllListeners();
                    }
                }
                // 3. Map is locked and not next in line
                else
                {
                    mapButtons[i].interactable = false;
                    SetButtonAlpha(mapButtons[i], lockedAlpha);
                    mapButtons[i].onClick.RemoveAllListeners();
                }
            }
        }

        private void OnPurchaseMapClicked(int mapLevel)
        {
            if (isBusy) return;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayClick();

            if (SaveManager.Instance != null && SaveManager.Instance.TryPurchaseMap(mapLevel))
            {
                Debug.Log($"[MapSelectionManager] Map {mapLevel} purchased successfully!");
                // Refresh buttons (it will now fall into the 'unlocked' category)
                UpdateMapButtons();
            }
            else
            {
                Debug.LogWarning($"[MapSelectionManager] Cannot purchase Map {mapLevel}. Not enough coins or error.");
            }
        }

        /// <summary>
        /// Cheat function to add 999,999 coins.
        /// Suggested to be called from a Button's OnClick event in the MapSelection scene.
        /// </summary>
        public void AddHackMoney()
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayClick();

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.AddCoins(999999);
                
                // Refresh the UI to reflect the new coin count and update button interactability
                UpdateMapButtons();
                
                Debug.Log("[MapSelectionManager] HackMoney added: 999,999 coins.");
            }
            else
            {
                Debug.LogError("[MapSelectionManager] SaveManager Instance not found. Cannot add hack money.");
            }
        }

        public void BackToMainMenu()
        {
            if (isBusy) return;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayClick();

            StartCoroutine(LoadSceneRoutine("MainMenu"));
        }

        private void OnMapButtonClicked(int mapLevel)
        {
            if (isBusy) return;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayClick();

            string sceneName = $"Map_{mapLevel:D2}";
            StartCoroutine(LoadSceneRoutine(sceneName));
        }

        private IEnumerator LoadSceneRoutine(string sceneName)
        {
            isBusy = true;
            
            // Disable all interactive buttons during fade
            foreach (var btn in mapButtons) if (btn != null) btn.interactable = false;
            if (backButton != null) backButton.interactable = false;
            if (hackButton != null) hackButton.interactable = false;

            // Fade screen to black
            if (fadeOverlay != null)
            {
                yield return FadeToBlack(1f, fadeOutDuration);
            }

            // Load the map scene
            Debug.Log($"[MapSelectionManager] Loading scene: {sceneName}");
            SceneManager.LoadScene(sceneName);
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
            fadeOverlay.raycastTarget = color.a > 0.01f;
        }

        /// <summary>
        /// Sets the alpha component for the Button's CanvasGroup. 
        /// If no CanvasGroup exists, it adjusts the target graphic's color.
        /// </summary>
        private void SetButtonAlpha(Button button, float alpha)
        {
            CanvasGroup cg = button.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = alpha;
            }
            else if (button.targetGraphic != null)
            {
                Color color = button.targetGraphic.color;
                color.a = alpha;
                button.targetGraphic.color = color;
            }
        }
    }
}
