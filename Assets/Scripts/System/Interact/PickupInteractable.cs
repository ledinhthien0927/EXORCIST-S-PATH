using UnityEngine;
using ExorcistPath.Core.Managers;

[DisallowMultipleComponent]
public sealed class PickupInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemName = "Item";

    // Track if it has been picked up so we don't count it twice if dropped
    private bool hasAwarded = false;

    public string Prompt => $"Pick Up {itemName}";

    public bool CanInteract(PlayerInteractor interactor)
    {
        return interactor != null &&
               interactor.Inventory != null &&
               !interactor.Inventory.HasItem;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (!CanInteract(interactor)) return;
        if (!interactor.Inventory.TryPick(gameObject)) return;

        if (!hasAwarded && GameManager.Instance != null)
        {
            GameManager.Instance.AddPurifiedTarget(gameObject);
            hasAwarded = true;
        }

        IHoldable holdable = GetComponent<IHoldable>();
        holdable?.OnPick(interactor.HoldPoint);

        interactor.RefreshUIAfterPickup();
    }
}