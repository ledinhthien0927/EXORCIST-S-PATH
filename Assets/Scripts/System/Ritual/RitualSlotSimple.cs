using UnityEngine;

public class RitualSlotSimple : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform placePoint;
    [SerializeField] private RitualManagerSimple manager;
    [SerializeField] private RitualItemSimple.RitualItemType acceptedItemType;

    private bool hasItem = false;
    private RitualItemSimple.RitualItemType placedItemType;

    public bool HasItem => hasItem;
    public RitualItemSimple.RitualItemType PlacedItemType => placedItemType;

    public string Prompt => hasItem ? "" : "Place Item";

    public bool CanInteract(PlayerInteractor interactor)
    {
        if (hasItem) return false;
        if (manager == null || manager.IsDone) return false;
        if (interactor == null || interactor.Inventory == null) return false;
        if (!interactor.Inventory.HasItem) return false;

        RitualItemSimple item = interactor.Inventory.GetHeldComponent<RitualItemSimple>();
        if (item == null) return false;

        if (item.IsPlaced) return false;
        if (item.itemType != acceptedItemType) return false;
        if (manager.HasPlacedType(item.itemType)) return false;

        return true;
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
        placedItemType = item.itemType;

        if (manager != null)
            manager.NotifyItemPlaced();

        interactor.RefreshUIAfterPickup();
    }
}