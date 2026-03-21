using UnityEngine;
using System;

namespace ExorcistPath.Gameplay.Tools
{
    public class ToolSystem : MonoBehaviour
    {
        public static ToolSystem Instance { get; private set; }

        [Header("Bucket Settings")]
        [SerializeField, Tooltip("Maximum amount of water the bucket can hold.")]
        private int maxWaterAmount = 5;

        [Header("Current State")]
        [SerializeField, Tooltip("Current amount of water in the bucket.")]
        private int currentWaterAmount = 0;

        [SerializeField, Tooltip("Is the current water blessed?")]
        private bool isHoly = false;

        // Events for UI/Audio/VFX to listen to (Dev B will use these)
        public event Action<int, bool> OnWaterStateChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        // --- PUBLIC API ---

        /// <summary>
        /// Fills the bucket with regular water.
        /// </summary>
        public void FillWater()
        {
            currentWaterAmount = maxWaterAmount;
            isHoly = false; // Filling replaces water, removing holy status
            NotifyStateChanged();
            Debug.Log($"[ToolSystem] Bucket filled with regular water. Amount: {currentWaterAmount}/{maxWaterAmount}");
        }

        /// <summary>
        /// Converts the current regular water into holy water using an item.
        /// </summary>
        public void BlessWater()
        {
            if (currentWaterAmount > 0)
            {
                isHoly = true;
                NotifyStateChanged();
                Debug.Log($"[ToolSystem] Water has been blessed! It is now Holy Water.");
            }
            else
            {
                Debug.LogWarning("[ToolSystem] Cannot bless an empty bucket!");
            }
        }

        /// <summary>
        /// Checks if the bucket currently holds holy water.
        /// </summary>
        public bool HasHolyWater()
        {
            return currentWaterAmount > 0 && isHoly;
        }

        /// <summary>
        /// Consumes one charge of holy water.
        /// </summary>
        public void UseWater()
        {
            if (HasHolyWater())
            {
                currentWaterAmount--;
                Debug.Log($"[ToolSystem] Holy water consumed. Remaining: {currentWaterAmount}/{maxWaterAmount}");
                
                if (currentWaterAmount <= 0)
                {
                    isHoly = false; // Resets when empty
                }

                NotifyStateChanged();
            }
            else
            {
                Debug.LogWarning("[ToolSystem] Cannot use holy water. Either empty or not blessed.");
            }
        }

        private void NotifyStateChanged()
        {
            OnWaterStateChanged?.Invoke(currentWaterAmount, isHoly);
        }
    }
}
