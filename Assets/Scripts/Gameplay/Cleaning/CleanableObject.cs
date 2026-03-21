using UnityEngine;
using ExorcistPath.Core.Managers;
// using ExorcistPath.Gameplay.Tools; // Uncomment when ToolSystem is created

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
            // Allowed to interact only if it hasn't been cleaned yet and the player has holy water
            return !isCleaned && HasHolyWater();
        }

        public void Interact(PlayerInteractor interactor)
        {
            if (!CanInteract(interactor)) return;

            Clean();
        }

        private void Clean()
        {
            isCleaned = true;

            // Optional: Consume holy water here if doing it via ToolSystem
            // ToolSystem.Instance.ConsumeHolyWater();

            // Reward purification progress
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddPurification(purificationValue);
            }
            else
            {
                Debug.LogWarning("[CleanableObject] GameManager instance not found. Cannot add purification.");
            }

            // Visual/Audio Feedback can be added here (e.g., UnityEvents)
            Debug.Log($"[{gameObject.name}] was cleaned. Added {purificationValue}% to purification.");

            // Hide or destroy the object after cleaning
            gameObject.SetActive(false);
        }

        private bool HasHolyWater()
        {
            // Placeholder: Replace with actual check to ToolSystem or PlayerInventory once ToolSystem is implemented
            // e.g., return ToolSystem.Instance.IsHolyWaterEquipped();

            return true; // Hardcoded to true temporarily to allow compilation and testing without ToolSystem
        }
    }
}
