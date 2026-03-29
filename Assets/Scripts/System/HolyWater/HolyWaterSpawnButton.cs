using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HolyWaterSpawnButton : MonoBehaviour
{
    [SerializeField] private float cooldownTime = 30f;
    [SerializeField] private PlayerInteractor playerInteractor;
    [SerializeField] private GameObject holyWaterPrefab;

    [Header("UI")]
    [SerializeField] private Button spawnButton;
    [SerializeField] private TMP_Text cooldownText;

    private float cooldownTimer = 0f;
    private bool isCoolingDown = false;
    private GameObject currentHolyWater;

    private void Start()
    {
        if (cooldownText != null)
            cooldownText.text = "";
    }

    private void Update()
    {
        if (!isCoolingDown)
            return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownText != null)
            cooldownText.text = Mathf.CeilToInt(cooldownTimer).ToString();

        if (cooldownTimer <= 0f)
        {
            cooldownTimer = 0f;
            isCoolingDown = false;

            if (cooldownText != null)
                cooldownText.text = "";

            if (spawnButton != null)
                spawnButton.interactable = true;
        }
    }

    public void SpawnHolyWater()
    {
        if (isCoolingDown) return;
        if (playerInteractor == null || holyWaterPrefab == null) return;
        if (playerInteractor.Inventory == null) return;

        if (currentHolyWater != null)
        {
            CancelSpawnedHolyWater();
            return;
        }

        if (playerInteractor.Inventory.HasItem) return;

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

        HolyWaterRevealAbility ability = currentHolyWater.GetComponent<HolyWaterRevealAbility>();
        if (ability != null)
            ability.SetHeld(false);

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

    private void CancelSpawnedHolyWater()
    {
        if (currentHolyWater == null) return;

        HolyWaterRevealAbility ability = currentHolyWater.GetComponent<HolyWaterRevealAbility>();
        if (ability != null)
            ability.SetHeld(false);

        if (playerInteractor != null && playerInteractor.Inventory != null && playerInteractor.Inventory.HasItem)
        {
            playerInteractor.Inventory.TryDrop(out GameObject droppedObj);

            if (droppedObj != null)
                Destroy(droppedObj);
        }
        else
        {
            Destroy(currentHolyWater);
        }

        currentHolyWater = null;

        if (playerInteractor != null)
            playerInteractor.RefreshUIAfterPickup();
    }

    private void StartCooldown()
    {
        isCoolingDown = true;
        cooldownTimer = cooldownTime;

        if (spawnButton != null)
            spawnButton.interactable = false;

        if (cooldownText != null)
            cooldownText.text = Mathf.CeilToInt(cooldownTimer).ToString();
    }
}