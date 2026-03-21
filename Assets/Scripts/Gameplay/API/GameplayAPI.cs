using UnityEngine;
using ExorcistPath.Core.Managers;
using ExorcistPath.Gameplay.Tools;

namespace ExorcistPath.Gameplay.API
{
    /// <summary>
    /// Serves as the primary bridge between the Player Layer and Gameplay Layer.
    /// Dev B (Player) should call these methods instead of referencing systems directly.
    /// </summary>
    public static class GameplayAPI
    {
        /// <summary>
        /// Attempts to add purification to the game state.
        /// </summary>
        /// <param name="amount">The percentage amount to add.</param>
        public static void AddPurification(float amount)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddPurification(amount);
            }
            else
            {
                Debug.LogWarning("[GameplayAPI] AddPurification failed: GameManager instance is missing.");
            }
        }

        /// <summary>
        /// Checks if the player meets the requirements to clean an object (e.g., has Holy Water).
        /// </summary>
        /// <returns>True if the player can clean, false otherwise.</returns>
        public static bool CanClean()
        {
            if (ToolSystem.Instance != null)
            {
                return ToolSystem.Instance.HasHolyWater();
            }
            
            Debug.LogWarning("[GameplayAPI] ToolSystem missing! CanClean defaulting to false.");
            return false;
        }

        /// <summary>
        /// Consumes water/tool resources if applicable. Call this AFTER confirming CanClean().
        /// </summary>
        public static void UseWater()
        {
            if (ToolSystem.Instance != null)
            {
                ToolSystem.Instance.UseWater();
            }
            else
            {
                Debug.LogWarning("[GameplayAPI] UseWater failed: ToolSystem missing.");
            }
        }
    }
}
