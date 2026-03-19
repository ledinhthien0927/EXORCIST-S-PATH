using UnityEngine;

public class BucketInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private WaterBucket bucket;

    public string Prompt
    {
        get
        {
            PlayerInteractor player = FindFirstObjectByType<PlayerInteractor>();
            if (player == null || player.Inventory == null)
                return "Use Bucket";

            CleaningToolItem heldItem = player.Inventory.GetHeldComponent<CleaningToolItem>();
            if (heldItem == null)
                return "Use Bucket";

            return heldItem.ToolType switch
            {
                CleaningToolType.HolyWaterBottle => "Pour Holy Water",
                CleaningToolType.Cloth => "Dip Cloth",
                CleaningToolType.Mop => "Dip Mop",
                _ => "Use Bucket"
            };
        }
    }

    public bool CanInteract(PlayerInteractor interactor)
    {
        if (bucket == null) return false;
        if (interactor == null || interactor.Inventory == null) return false;

        CleaningToolItem heldItem = interactor.Inventory.GetHeldComponent<CleaningToolItem>();
        if (heldItem == null) return false;

        if (heldItem.ToolType == CleaningToolType.HolyWaterBottle)
        {
            return bucket.HasWater && !bucket.IsHolyWater;
        }

        if (heldItem.ToolType == CleaningToolType.Cloth || heldItem.ToolType == CleaningToolType.Mop)
        {
            return bucket.CanDip();
        }

        return false;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (!CanInteract(interactor)) return;

        CleaningToolItem heldItem = interactor.Inventory.GetHeldComponent<CleaningToolItem>();
        if (heldItem == null) return;

        if (heldItem.ToolType == CleaningToolType.HolyWaterBottle)
        {
            bool success = bucket.BlessWater();
            Debug.Log("Trying to pour holy water -> " + success);
            return;
        }

        if (heldItem.ToolType == CleaningToolType.Cloth)
        {
            if (!bucket.UseDip()) return;
            heldItem.FillToMax();
            Debug.Log("Cloth dipped");
            return;
        }

        if (heldItem.ToolType == CleaningToolType.Mop)
        {
            if (!bucket.UseDip()) return;
            heldItem.FillToMax();
            Debug.Log("Mop dipped");
            return;
        }
    }
}