using System;
using UnityEngine;

namespace ExorcistPath.Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        // Singleton Instance
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        [SerializeField, Range(0f, 100f)]
        private float currentPurificationPercentage = 0f;

        // Event triggered when purification changes. UI systems should subscribe to this.
        public event Action<float> OnPurificationChanged;

        private void Awake()
        {
            // Standard Singleton setup
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            // Optional: Keep GameManager alive across scene loads
            // DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Adds a specific amount to the total purification percentage.
        /// Clamps the result between 0 and 100.
        /// </summary>
        /// <param name="amount">The percentage amount to add.</param>
        public void AddPurification(float amount)
        {
            if (currentPurificationPercentage >= 100f) return;

            // Add and clamp the value
            currentPurificationPercentage += amount;
            currentPurificationPercentage = Mathf.Clamp(currentPurificationPercentage, 0f, 100f);

            // Notify subscribers (like UI) that the value changed
            OnPurificationChanged?.Invoke(currentPurificationPercentage);

            Debug.Log($"[GameManager] Purification Updated: {currentPurificationPercentage}%");

            if (currentPurificationPercentage >= 100f)
            {
                Debug.Log("Game Win");
            }
        }

        /// <summary>
        /// Gets the current purification percentage.
        /// </summary>
        public float GetPurificationPercentage()
        {
            return currentPurificationPercentage;
        }
    }
}
