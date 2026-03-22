using UnityEngine;

namespace ExorcistPath.Core.Managers
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        public delegate void CoinsChangedHandler(int newTotal);
        public event CoinsChangedHandler OnCoinsChanged;

        private const string HIGHEST_UNLOCKED_MAP_KEY = "HighestUnlockedMap";
        private const string TOTAL_COINS_KEY = "TotalCoins";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // Check for unlocks initially in case coins were added offline or via debug
            CheckAutoUnlock();
        }

        /// <summary>
        /// Gets the highest map level the player has unlocked. Default is 1.
        /// </summary>
        public int GetHighestUnlockedMap()
        {
            return PlayerPrefs.GetInt(HIGHEST_UNLOCKED_MAP_KEY, 1);
        }

        /// <summary>
        /// Unlocks the next map if the provided completed map was the highest unlocked one.
        /// </summary>
        /// <param name="completedMapIndex">The map level that was just completed (1 to 4).</param>
        public void UnlockNextMap(int completedMapIndex)
        {
            int maxMaps = 4; // Based on GDD
            int currentHighest = GetHighestUnlockedMap();

            // If we completed the highest map we have access to, and it's not the final map
            if (completedMapIndex == currentHighest && currentHighest < maxMaps)
            {
                int nextMap = currentHighest + 1;
                PlayerPrefs.SetInt(HIGHEST_UNLOCKED_MAP_KEY, nextMap);
                PlayerPrefs.Save();
                Debug.Log($"[SaveManager] Unlocked Map {nextMap}!");
            }
            else
            {
                Debug.Log($"[SaveManager] Map {completedMapIndex} completed. No new maps unlocked (Highest is {currentHighest}).");
            }
        }

        /// <summary>
        /// Gets the total amount of coins the player has accumulated.
        /// </summary>
        public int GetTotalCoins()
        {
            return PlayerPrefs.GetInt(TOTAL_COINS_KEY, 0); // Default to 0 initially
        }

        /// <summary>
        /// Adds coins to the player's total.
        /// </summary>
        public void AddCoins(int amount)
        {
            int currentCoins = GetTotalCoins();
            currentCoins += amount;
            PlayerPrefs.SetInt(TOTAL_COINS_KEY, currentCoins);
            PlayerPrefs.Save();
            Debug.Log($"[SaveManager] Added {amount} coins. New Total: {currentCoins}");

            OnCoinsChanged?.Invoke(currentCoins);

            // Automatically check if this new balance unlocks a map
            CheckAutoUnlock();
        }

        /// <summary>
        /// Automatically unlocks maps if the player has reached the coin threshold.
        /// No coins are deducted.
        /// </summary>
        public void CheckAutoUnlock()
        {
            int totalCoins = GetTotalCoins();
            int currentHighest = GetHighestUnlockedMap();
            int newHighest = currentHighest;

            // Thresholds based on GDD
            // Map 2: 700,000
            // Map 3: 1,600,000
            // Map 4: 3,200,000

            if (totalCoins >= 3200000) newHighest = Mathf.Max(newHighest, 4);
            else if (totalCoins >= 1600000) newHighest = Mathf.Max(newHighest, 3);
            else if (totalCoins >= 700000) newHighest = Mathf.Max(newHighest, 2);

            if (newHighest > currentHighest)
            {
                PlayerPrefs.SetInt(HIGHEST_UNLOCKED_MAP_KEY, newHighest);
                PlayerPrefs.Save();
                Debug.Log($"[SaveManager] AUTO-UNLOCK: New Highest Map is {newHighest} due to coin balance ({totalCoins})");
            }
        }

        /// <summary>
        /// Resets all save progress, including unlocked maps and coins.
        /// Used for starting a New Game.
        /// </summary>
        public void ResetProgress()
        {
            PlayerPrefs.DeleteKey(HIGHEST_UNLOCKED_MAP_KEY);
            PlayerPrefs.DeleteKey(TOTAL_COINS_KEY);
            PlayerPrefs.Save();
            Debug.Log("[SaveManager] Game progress and coins have been reset.");

            OnCoinsChanged?.Invoke(0);
        }
    }
}
