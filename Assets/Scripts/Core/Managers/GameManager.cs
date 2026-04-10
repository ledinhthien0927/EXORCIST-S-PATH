using System;
using System.Collections.Generic;
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

        [Header("Level Settings")]
        [Tooltip("The map level currently being played (1 to 4).")]
        [SerializeField] private int currentMapLevel = 1;
        public int CurrentMapLevel => currentMapLevel;

        // Event triggered when purification changes. UI systems should subscribe to this.
        public event Action<float> OnPurificationChanged;

        // Event triggered when the game is won. Useful for cutscenes and win sequences.
        public event Action OnGameWon;

        [Header("Purification Progress")]
        [Tooltip("Number of rituals required to beat this map.")]
        [SerializeField] private int totalRitualsToWin = 1;
        private int completedRitualsCount = 0;

        [Tooltip("Drag and drop all stains / cursed objects of this Map here.")]
        [SerializeField] private GameObject[] purificationTargets;
        private int totalPurificationTargets = 0;
        private int cleanedTargetsCount = 0;

        private void Awake()
        {
            // Standard Singleton setup
            if (Instance != null && Instance != this)
            {
                // If a new GameManager exists in the scene, check if we should copy its settings
                // before destroying it (useful for setting Level-specific values in Inspector)
                Instance.SyncFromOther(this);

                Destroy(gameObject);
                return;
            }

            Instance = this;
            // Keep GameManager alive across scene loads
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // If no targets were manually assigned, try to find all cleanables/pickups in the scene automatically
            if (totalPurificationTargets == 0)
            {
                AutoCollectTargets();
            }
        }

        /// <summary>
        /// Automatically finds all objects in the scene that should count towards purification.
        /// </summary>
        private void AutoCollectTargets()
        {
            var bloods = FindObjectsByType<CleanableBloodInteractable>(FindObjectsSortMode.None);
            var pickups = FindObjectsByType<PickupInteractable>(FindObjectsSortMode.None);

            // Use a HashSet to ensure each GameObject is only counted once
            HashSet<GameObject> uniqueTargets = new HashSet<GameObject>();
            
            foreach (var b in bloods)
            {
                if (b != null) uniqueTargets.Add(b.gameObject);
            }
            
            foreach (var p in pickups)
            {
                if (p != null && p.isPurificationTarget)
                {
                    uniqueTargets.Add(p.gameObject);
                }
            }

            if (uniqueTargets.Count == 0) return;

            purificationTargets = new GameObject[uniqueTargets.Count];
            uniqueTargets.CopyTo(purificationTargets);
            
            totalPurificationTargets = purificationTargets.Length;
            cleanedTargetsCount = 0;
            currentPurificationPercentage = 0f;

            Debug.Log($"[GameManager] Auto-Collected {totalPurificationTargets} UNIQUE purification targets in Scene.");
        }

        /// <summary>
        /// Copies important level-specific settings from another GameManager.
        /// This allows you to have a GameManager object in each scene with local settings
        /// that get passed to the main persistent Instance.
        /// </summary>
        private void SyncFromOther(GameManager other)
        {
            InitializeLevel(other.currentMapLevel, other.totalRitualsToWin, other.purificationTargets);
            
            // If the scene-local GameManager has no targets, trigger auto-collect on the instance
            if (totalPurificationTargets == 0)
            {
                AutoCollectTargets();
            }
        }

        /// <summary>
        /// Resets the game state and sets up goals for a new map.
        /// Call this when a level starts.
        /// </summary>
        public void InitializeLevel(int mapLevel, int ritualsRequired, GameObject[] targets)
        {
            currentMapLevel = mapLevel;
            totalRitualsToWin = ritualsRequired;
            
            // Setup Array Targets
            if (targets != null)
            {
                List<GameObject> uniqueTargets = new List<GameObject>();
                foreach (var t in targets)
                {
                    if (t != null && !uniqueTargets.Contains(t))
                    {
                        uniqueTargets.Add(t);
                    }
                }
                purificationTargets = uniqueTargets.ToArray();
            }
            else
            {
                purificationTargets = null;
            }

            totalPurificationTargets = purificationTargets != null ? purificationTargets.Length : 0;
            cleanedTargetsCount = 0;

            // Reset Progress
            currentPurificationPercentage = 0f;
            completedRitualsCount = 0;

            Debug.Log($"[GameManager] Level Initialized: Map {mapLevel}, Rituals: {ritualsRequired}, Targets: {totalPurificationTargets}");
            
            // Notify UI to reset
            OnPurificationChanged?.Invoke(currentPurificationPercentage);
        }

        /// <summary>
        /// Adds a purified target (like blood or cursed item) to the count.
        /// It will calculate the total percentage based on the array size.
        /// </summary>
        public void AddPurifiedTarget(GameObject targetObj)
        {
            if (purificationTargets == null || totalPurificationTargets == 0)
            {
                Debug.LogWarning($"[GameManager] AddPurifiedTarget called for {targetObj.name}, but purificationTargets array is EMPTY or NULL!");
                return;
            }

            bool found = false;
            for (int i = 0; i < purificationTargets.Length; i++)
            {
                if (purificationTargets[i] == targetObj)
                {
                    purificationTargets[i] = null; // Remove it so it can't be counted twice
                    found = true;
                    break;
                }
            }

            if (found)
            {
                cleanedTargetsCount++;
                currentPurificationPercentage = ((float)cleanedTargetsCount / totalPurificationTargets) * 100f;
                currentPurificationPercentage = Mathf.Clamp(currentPurificationPercentage, 0f, 100f);
                
                OnPurificationChanged?.Invoke(currentPurificationPercentage);
                Debug.Log($"[GameManager] Target Cleaned: {targetObj.name}. Progress: {cleanedTargetsCount}/{totalPurificationTargets} ({currentPurificationPercentage}%)");
            }
            else
            {
                Debug.LogWarning($"[GameManager] Target {targetObj.name} was NOT found in the purificationTargets array! (Did you drag a Prefab instead of a Scene Instance?)");
            }
        }

        /// <summary>
        /// Gets the current purification percentage.
        /// </summary>
        public float GetPurificationPercentage()
        {
            return currentPurificationPercentage;
        }

        /// <summary>
        /// Registers a completed ritual. Returns true if all rituals for the level are completed.
        /// </summary>
        public bool CompleteRitual()
        {
            completedRitualsCount++;
            Debug.Log($"[GameManager] Ritual completed: {completedRitualsCount}/{totalRitualsToWin}");

            if (completedRitualsCount >= totalRitualsToWin)
            {
                // End the game
                WinGame();
                return true;
            }
            return false;
        }

        public int LastCoinReward { get; private set; }

        public string GetRankString(float percentage)
        {
            int roundedPercent = Mathf.RoundToInt(percentage);
            if (roundedPercent >= 100) return "Perfect";
            if (roundedPercent >= 80) return "Clean";
            if (roundedPercent >= 60) return "Acceptable";
            return "Poor";
        }

        /// <summary>
        /// API for Dev B to call when the player successfully completes the level
        /// (e.g., finishes all purification rituals).
        /// </summary>
        public void WinGame()
        {
            Debug.Log($"[GameManager] WinGame called! Purification: {currentPurificationPercentage}%");

            if (SaveManager.Instance != null)
            {
                // Calculate and add coin reward
                int reward = CalculateCoinReward(currentMapLevel, currentPurificationPercentage);
                LastCoinReward = reward;
                SaveManager.Instance.AddCoins(reward);
                
                // Map unlocking is now handled by MapSelectionManager using purchased coins
                Debug.Log($"[GameManager] Win processed. Coins awarded: {reward}");
            }
            else
            {
                Debug.LogWarning("[GameManager] SaveManager is missing. Progress and Coins not saved.");
            }

            // Implement other post-game logic here (e.g., show victory UI)
            OnGameWon?.Invoke();
        }

        /// <summary>
        /// Calculates the final coin reward from base map reward based on purification percentage.
        /// </summary>
        private int CalculateCoinReward(int mapLevel, float percentage)
        {
            int baseReward = 0;
            switch (mapLevel)
            {
                case 1: baseReward = 300000; break;
                case 2: baseReward = 650000; break;
                case 3: baseReward = 1200000; break;
                case 4: baseReward = 2200000; break;
                default: baseReward = 300000; break;
            }

            int roundedPercent = Mathf.RoundToInt(percentage);
            if (roundedPercent >= 100) return Mathf.RoundToInt(baseReward * 1.45f); // Hoàn hảo
            if (roundedPercent >= 80) return Mathf.RoundToInt(baseReward * 1.25f);  // Sạch
            if (roundedPercent >= 60) return Mathf.RoundToInt(baseReward * 1.10f);  // Chấp nhận
            return baseReward; // Kém (< 60%)
        }
    }
}
