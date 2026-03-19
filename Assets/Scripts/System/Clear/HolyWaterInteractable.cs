using UnityEngine;

public class HolyWaterInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private WaterBucket bucket;

    public string Prompt => "Pour Holy Water";

    public bool CanInteract(PlayerInteractor interactor)
    {
        return bucket != null;
    }

    public void Interact(PlayerInteractor interactor)
    {
        bucket.BlessWater();
    }
}