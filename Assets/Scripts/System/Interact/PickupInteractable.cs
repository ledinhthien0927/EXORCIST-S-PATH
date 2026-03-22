using UnityEngine;
using ExorcistPath.Core.Managers;

[DisallowMultipleComponent]
public sealed class PickupInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemName = "Item";

    [Header("Purification")]
    [Tooltip("Amount of purification % awarded when this is picked up (for cursed items).")]
    [SerializeField] private float purificationAward = 0f;
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

        if (!hasAwarded && GameManager.Instance != null && purificationAward > 0f)
        {
            GameManager.Instance.AddPurification(purificationAward);
            hasAwarded = true;
        }

        IHoldable holdable = GetComponent<IHoldable>();
        holdable?.OnPick(interactor.HoldPoint);

        interactor.RefreshUIAfterPickup();
    }
}