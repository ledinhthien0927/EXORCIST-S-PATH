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

        /// <summary>
        /// Gets the highest map level the player has unlocked. Default is 1.
        /// </summary>
        public int GetHighestUnlockedMap()
        {
            return PlayerPrefs.GetInt(HIGHEST_UNLOCKED_MAP_KEY, 1);
        }

        /// <summary>
        /// Gets the price to unlock a specific map.
        /// </summary>
        public int GetMapPrice(int mapLevel)
        {
            switch (mapLevel)
            {
                case 2: return 700000;
                case 3: return 1600000;
                case 4: return 3200000;
                default: return 0; // Map 1 is always unlocked/free
            }
        }

        /// <summary>
        /// Attempts to purchase a map by deducting coins.
        /// </summary>
        public bool TryPurchaseMap(int mapLevel)
        {
            int price = GetMapPrice(mapLevel);
            int currentCoins = GetTotalCoins();

            if (currentCoins >= price)
            {
                currentCoins -= price;
                PlayerPrefs.SetInt(TOTAL_COINS_KEY, currentCoins);
                
                int currentHighest = GetHighestUnlockedMap();
                if (mapLevel > currentHighest)
                {
                    PlayerPrefs.SetInt(HIGHEST_UNLOCKED_MAP_KEY, mapLevel);
                }
                
                PlayerPrefs.Save();
                OnCoinsChanged?.Invoke(currentCoins);
                
                Debug.Log($"[SaveManager] Purchased Map {mapLevel} for {price} coins. Remaining: {currentCoins}");
                return true;
            }
            return false;
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
