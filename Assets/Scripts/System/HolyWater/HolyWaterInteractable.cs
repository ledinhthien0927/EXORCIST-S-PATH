using UnityEngine;

public class HolyWaterInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private WaterBucket bucket;

    public string Prompt => "Pour Holy Water";

    public bool CanInteract(PlayerInteractor interactor)
    {
        if (bucket == null) return false;
        if (interactor == null || interactor.Inventory == null) return false;
        if (!interactor.Inventory.HasItem) return false;

        HolyWaterRevealAbility holyWater =
            interactor.Inventory.GetHeldComponent<HolyWaterRevealAbility>();

        return holyWater != null;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (!CanInteract(interactor)) return;

        bucket.BlessWater();

        HolyWaterRevealAbility holyWater =
            interactor.Inventory.GetHeldComponent<HolyWaterRevealAbility>();

        if (holyWater != null)
        {
            holyWater.ConsumeAfterUse();
        }
    }
}