using UnityEngine;
using ExorcistPath.Core.Managers;
using ExorcistPath.Gameplay.API;

namespace ExorcistPath.Gameplay.Cleaning
{
    public class CleanableObject : MonoBehaviour, IInteractable
    {
        [Header("Cleaning Settings")]
        [SerializeField, Tooltip("Amount of purification added when this is cleaned.")]
        private float purificationValue = 5f;

        [Header("UI Prompts")]
        [SerializeField] private string interactPrompt = "Clean Sign (Holy Water)";
        [SerializeField] private string missingToolPrompt = "Requires Holy Water!";

        [Header("Tool Requirement")]
        [Tooltip("What tool the player must hold to clean this object (e.g. Mop for floor, Cloth for wall)")]
        // Requires Dev B's CleaningToolType enum
        [SerializeField] private CleaningToolType requiredTool;

        [Header("State")]
        [SerializeField, Tooltip("Is this object already cleaned?")]
        private bool isCleaned = false;

        // Properties from IInteractable
        public string Prompt 
        {
            get 
            {
                if (isCleaned) return string.Empty;
                return HasHolyWater() ? interactPrompt : missingToolPrompt;
            }
        }

        public bool CanInteract(PlayerInteractor interactor)
        {
            // Allowed to interact only if:
            // 1. Not cleaned yet
            // 2. Player has Holy Water in the bucket (Gameplay Layer check via API)
            // 3. Player is holding the required tool (Player Layer check via Inventory)
            if (isCleaned) return false;
            if (!HasHolyWater()) return false;
            
            // Check Dev B's inventory system
            if (interactor != null && interactor.Inventory != null)
            {
                return interactor.Inventory.IsHoldingTool(requiredTool);
            }

            return false;
        }

        public void Interact(PlayerInteractor interactor)
        {
            if (!CanInteract(interactor)) return;

            Clean();
        }

        private void Clean()
        {
            isCleaned = true;

            // Consume holy water using the API
            GameplayAPI.UseWater();

            // Reward purification progress
            GameplayAPI.AddPurification(purificationValue);

            // Visual/Audio Feedback can be added here (e.g., UnityEvents)
            Debug.Log($"[{gameObject.name}] was cleaned. Added {purificationValue}% to purification.");

            // Hide or destroy the object after cleaning
            gameObject.SetActive(false);
        }

        private bool HasHolyWater()
        {
            return GameplayAPI.CanClean();
        }
    }
}
