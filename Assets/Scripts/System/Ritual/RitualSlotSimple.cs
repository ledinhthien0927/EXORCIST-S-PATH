using UnityEngine;

public class RitualSlotSimple : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform placePoint;
    [SerializeField] private RitualManagerSimple manager;

    private bool hasItem = false;

    public bool HasItem => hasItem;

    public string Prompt => hasItem ? "" : "Place Item";

    public bool CanInteract(PlayerInteractor interactor)
    {
        if (hasItem) return false;
        if (interactor == null || interactor.Inventory == null) return false;
        if (!interactor.Inventory.HasItem) return false;

        RitualItemSimple item = interactor.Inventory.GetHeldComponent<RitualItemSimple>();
        if (item == null) return false;

        return item.targetSlot == this;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (!CanInteract(interactor)) return;
        if (!interactor.Inventory.TryDrop(out GameObject obj)) return;

        Transform point = placePoint != null ? placePoint : transform;

        IHoldable holdable = obj.GetComponent<IHoldable>();
        holdable?.OnDrop(point.position);

        RitualItemSimple item = obj.GetComponent<RitualItemSimple>();
        if (item == null) return;

        item.Place(point);

        hasItem = true;

        if (manager != null)
            manager.CheckComplete();

        interactor.RefreshUIAfterPickup();
    }
}