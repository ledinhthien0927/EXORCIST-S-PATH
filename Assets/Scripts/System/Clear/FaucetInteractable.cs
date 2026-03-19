using UnityEngine;

public class FaucetInteractable : MonoBehaviour, IInteractable
{
    public string Prompt => "Fill Bucket";

    public bool CanInteract(PlayerInteractor interactor)
    {
        if (interactor == null || interactor.Inventory == null)
        {
            Debug.Log("Faucet: interactor or inventory is null");
            return false;
        }

        bool holdingBucket = interactor.Inventory.IsHoldingTool(CleaningToolType.Bucket);
        Debug.Log("Faucet: holding bucket = " + holdingBucket);

        return holdingBucket;
    }

    public void Interact(PlayerInteractor interactor)
    {
        Debug.Log("Faucet interact pressed");

        if (!CanInteract(interactor)) return;

        WaterBucket heldBucket = interactor.Inventory.GetHeldComponent<WaterBucket>();
        if (heldBucket == null)
        {
            Debug.Log("Faucet: held item has no WaterBucket component");
            return;
        }

        heldBucket.FillWithNormalWater();
        Debug.Log("Bucket filled with normal water.");
    }
}