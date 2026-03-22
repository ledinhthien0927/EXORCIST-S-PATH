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

        private void Start()
        {
            UpdateMapButtons();
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

                if (mapLevel <= highestUnlockedMap)
                {
                    // Map is unlocked
                    mapButtons[i].interactable = true;
                    SetButtonAlpha(mapButtons[i], 1f);

                    // Add click listener to load the scene
                    mapButtons[i].onClick.RemoveAllListeners();
                    mapButtons[i].onClick.AddListener(() => LoadMapScene(mapLevel));
                }
                else
                {
                    // Map is locked
                    mapButtons[i].interactable = false;
                    SetButtonAlpha(mapButtons[i], lockedAlpha);
                    mapButtons[i].onClick.RemoveAllListeners();
                }
            }
        }

        /// <summary>
        /// Loads the scene corresponding to the map level.
        /// Convention: Map_01, Map_02, etc.
        /// </summary>
        private void LoadMapScene(int mapLevel)
        {
            string sceneName = $"Map_{mapLevel:D2}"; // Formats 1 to "01", 2 to "02"
            Debug.Log($"[MapSelectionManager] Loading scene: {sceneName}");
            SceneManager.LoadScene(sceneName);
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
