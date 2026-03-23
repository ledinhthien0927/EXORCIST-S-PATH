using UnityEngine;
using ExorcistPath.Core.Managers;

[DisallowMultipleComponent]
public sealed class CleanableBloodInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private CleaningToolType requiredTool = CleaningToolType.Cloth;

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

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddPurifiedTarget(gameObject);
        }

        Destroy(gameObject);
    }
}