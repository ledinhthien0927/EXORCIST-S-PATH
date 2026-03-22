using UnityEngine;

public class HolyWaterSpawnButton : MonoBehaviour
{
    [SerializeField] private float cooldownTime = 30f;
    [SerializeField] private PlayerInteractor playerInteractor;
    [SerializeField] private GameObject holyWaterPrefab;

    private float cooldownTimer = 0f;
    private bool isCoolingDown = false;
    private GameObject currentHolyWater;

    private void Update()
    {
        if (!isCoolingDown) return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            cooldownTimer = 0f;
            isCoolingDown = false;
        }
    }

    public void SpawnHolyWater()
    {
        if (isCoolingDown) return; 
        if (playerInteractor == null || holyWaterPrefab == null) return;
        if (playerInteractor.Inventory == null) return;
        if (playerInteractor.Inventory.HasItem) return;
        if (currentHolyWater != null) return;

        Transform holdPoint = playerInteractor.HoldPoint;
        if (holdPoint == null) return;

        GameObject obj = Instantiate(holyWaterPrefab, holdPoint.position, holdPoint.rotation);
        currentHolyWater = obj;

        if (!playerInteractor.Inventory.TryPick(obj))
        {
            Destroy(obj);
            currentHolyWater = null;
            return;
        }

        IHoldable holdable = obj.GetComponent<IHoldable>();
        holdable?.OnPick(holdPoint);

        obj.transform.SetParent(holdPoint, false);

        HolyWaterRevealAbility ability = obj.GetComponent<HolyWaterRevealAbility>();
        if (ability != null)
        {
            ability.SetSpawnButtonOwner(this);
            ability.SetHeld(true);
        }

        playerInteractor.RefreshUIAfterPickup();
    }

    public void ConsumeSpawnedHolyWater(GameObject usedHolyWater)
    {
        if (usedHolyWater == null) return;
        if (currentHolyWater != usedHolyWater) return;

        if (playerInteractor != null && playerInteractor.Inventory != null && playerInteractor.Inventory.HasItem)
        {
            playerInteractor.Inventory.TryDrop(out GameObject droppedObj);

            if (droppedObj != null)
                Destroy(droppedObj);
        }
        else
        {
            Destroy(usedHolyWater);
        }

        currentHolyWater = null;

        StartCooldown();

        if (playerInteractor != null)
            playerInteractor.RefreshUIAfterPickup();
    }

    private void StartCooldown()
    {
        isCoolingDown = true;
        cooldownTimer = cooldownTime;
    }
}