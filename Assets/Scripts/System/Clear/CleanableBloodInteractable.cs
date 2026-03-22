using UnityEngine;
using ExorcistPath.Core.Managers;

[DisallowMultipleComponent]
public sealed class CleanableBloodInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private CleaningToolType requiredTool = CleaningToolType.Cloth;
    
    [Header("Purification")]
    [Tooltip("Amount of purification % awarded when this is cleaned.")]
    [SerializeField] private float purificationAward = 4f;

    public string Prompt
    {
        get
        {
            return requiredTool == CleaningToolType.Cloth ? "Wipe Blood" : "Mop Blood";
        }
    }

    public bool CanInteract(PlayerInteractor interactor)
    {
        if (interactor == null || interactor.Inventory == null) return false;
        if (!interactor.Inventory.IsHoldingTool(requiredTool)) return false;

        CleaningToolItem heldTool = interactor.Inventory.GetHeldComponent<CleaningToolItem>();
        if (heldTool == null) return false;

        return heldTool.HasUses;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (!CanInteract(interactor)) return;

        CleaningToolItem heldTool = interactor.Inventory.GetHeldComponent<CleaningToolItem>();
        if (heldTool == null) return;

        if (!heldTool.UseOnce()) return;

        if (GameManager.Instance != null && purificationAward > 0f)
        {
            GameManager.Instance.AddPurification(purificationAward);
        }

        Destroy(gameObject);
    }
}