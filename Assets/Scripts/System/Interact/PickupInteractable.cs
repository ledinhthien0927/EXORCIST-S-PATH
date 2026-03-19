using UnityEngine;

[DisallowMultipleComponent]
public sealed class PickupInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemName = "Item";

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

        IHoldable holdable = GetComponent<IHoldable>();
        holdable?.OnPick(interactor.HoldPoint);

        interactor.RefreshUIAfterPickup();
    }
}