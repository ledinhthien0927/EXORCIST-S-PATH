using UnityEngine;
using UnityEngine.UI;
using ExorcistPath.Core.Managers;
using TMPro;

namespace ExorcistPath.UI_UX.Gameplay
{
    /// <summary>
    /// UI that displays the Purification System Progress.
    /// Listens to GameManager.Instance.OnPurificationChanged.
    /// </summary>
    public class PurificationProgressBar : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("The Image component acting as the fill of the progress bar.")]
        [SerializeField] private Image progressBarFill;
        
        [Tooltip("Text component displaying percentage, e.g. 'Purification 50%'.")]
        [SerializeField] private TextMeshProUGUI percentageText;

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                // Subscribe to purification changes
                GameManager.Instance.OnPurificationChanged += UpdateProgressBar;
                
                // Initialize the UI with current value
                UpdateProgressBar(GameManager.Instance.GetPurificationPercentage());
            }
            else
            {
                Debug.LogWarning("[PurificationProgressBar] GameManager instance is missing!");
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPurificationChanged -= UpdateProgressBar;
            }
        }

        /// <summary>
        /// Updates the UI elements based on the current purification percentage.
        /// </summary>
        /// <param name="currentPercentage">The current percentage (0 to 100).</param>
        private void UpdateProgressBar(float currentPercentage)
        {
            if (progressBarFill != null)
            {
                // FillAmount is between 0 and 1
                progressBarFill.fillAmount = currentPercentage / 100f;
            }

            if (percentageText != null)
            {
                // Display format: "Purification XX%"
                percentageText.text = $"Purification {Mathf.RoundToInt(currentPercentage)}%";
            }
        }
    }
}
