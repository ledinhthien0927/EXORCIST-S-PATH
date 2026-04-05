using UnityEngine;
using TMPro;

public sealed class PlayerInteractor : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float distance = 3f;
    [SerializeField] private float sphereRadius = 0.08f;
    [SerializeField] private float graceTime = 0.12f;
    [SerializeField] private float enterRange = 2.9f;
    [SerializeField] private float exitRange = 3.1f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask interactMask = ~0;

    [Header("Inventory / Hold")]
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private Transform holdPoint;

    [Header("UI")]
    [SerializeField] private GameObject pickupButtonObject;
    [SerializeField] private GameObject dropButtonObject;
    [SerializeField] private TextMeshProUGUI pickupButtonText;

    private IInteractable currentTarget;
    private float lastSeenTime;
    private float lastSeenDistance;
    private Collider lastHitCollider; // Optimization: Cache for the hit collider
    private IInteractable lastHitTarget; // Optimization: Cache for the interactable result

    public Transform HoldPoint => holdPoint;
    public PlayerInventory Inventory => inventory;

    private void Start()
    {
        if (pickupButtonObject != null)
            pickupButtonObject.SetActive(true);

        UpdateDropButton();
        RefreshPickupUI();
    }

    private void Update()
    {
        UpdateTarget();
        UpdateDropButton();
    }

    private void UpdateTarget()
    {
        IInteractable hitTarget = null;
        float hitDistance = float.PositiveInfinity;

        if (playerCamera == null)
        {
            currentTarget = null;
            RefreshPickupUI();
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.SphereCast(ray, sphereRadius, out RaycastHit hit, distance, interactMask, QueryTriggerInteraction.Ignore))
        {
            hitDistance = hit.distance;
            
            // Optimization: Only search for the interactable if the collider has changed
            if (hit.collider != lastHitCollider)
            {
                lastHitCollider = hit.collider;
                lastHitTarget = FindBestInteractable(hit.collider);
            }
            hitTarget = lastHitTarget;
        }
        else
        {
            lastHitCollider = null;
            lastHitTarget = null;
        }

        if (hitTarget != null)
        {
            lastSeenTime = Time.time;
            lastSeenDistance = hitDistance;
        }

        bool lostTooLong = (Time.time - lastSeenTime) > graceTime;
        bool withinEnterRange = (hitTarget != null && hitDistance <= enterRange);
        bool beyondExitRange = (currentTarget != null && lastSeenDistance >= exitRange);

        IInteractable newTarget = currentTarget;

        if (currentTarget == null)
        {
            if (withinEnterRange)
                newTarget = hitTarget;
        }
        else
        {
            if (hitTarget == currentTarget)
            {
                newTarget = currentTarget;
            }
            else
            {
                if (lostTooLong || beyondExitRange)
                    newTarget = null;

                if (withinEnterRange)
                    newTarget = hitTarget;
            }
        }

        currentTarget = newTarget;
        RefreshPickupUI();
    }

    private IInteractable FindBestInteractable(Collider hitCollider)
    {
        if (hitCollider == null) return null;

        MonoBehaviour[] behaviours = hitCollider.GetComponentsInParent<MonoBehaviour>(true);

        IInteractable fallback = null;

        for (int i = 0; i < behaviours.Length; i++)
        {
            if (behaviours[i] is not IInteractable interactable)
                continue;

            if (fallback == null)
                fallback = interactable;

            if (interactable.CanInteract(this))
                return interactable;
        }

        return fallback;
    }

    private void RefreshPickupUI()
    {
        if (pickupButtonObject != null)
            pickupButtonObject.SetActive(true);

        if (pickupButtonText == null)
            return;

        if (currentTarget != null && currentTarget.CanInteract(this))
            pickupButtonText.text = currentTarget.Prompt;
        else
            pickupButtonText.text = "";
    }

    public void OnPickupButton()
    {
        if (currentTarget != null && currentTarget.CanInteract(this))
            currentTarget.Interact(this);

        RefreshPickupUI();
        UpdateDropButton();
    }

    [Header("Drop Settings")]
    [Tooltip("Khoảng cách drop mặc định phía trước camera")]
    [SerializeField] private float dropDistance = 1.2f;

    [Tooltip("Khoảng cách tối thiểu từ tường khi drop")]
    [SerializeField] private float dropWallOffset = 0.15f;

    [Tooltip("Layer tường để kiểm tra khi drop")]
    [SerializeField] private LayerMask dropWallMask = ~0;

    public void OnDropButton()
    {
        if (inventory == null) return;
        if (!inventory.TryDrop(out GameObject obj)) return;

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 dropPos = transform.position + forward * 1.2f + Vector3.up * 1f;

        IHoldable holdable = obj.GetComponent<IHoldable>();
        holdable?.OnDrop(dropPos);

        RefreshPickupUI();
        UpdateDropButton();
    }

    /// <summary>
    /// Tính vị trí drop an toàn: raycast về phía trước, nếu có tường thì đặt item
    /// trước tường thay vì xuyên qua.
    /// </summary>
    private Vector3 CalculateSafeDropPosition()
    {
        Transform cam = playerCamera != null ? playerCamera.transform : transform;
        Vector3 origin = cam.position;
        Vector3 forward = cam.forward;

        float safeDist = dropDistance;

        // Raycast để kiểm tra tường phía trước
        if (Physics.Raycast(origin, forward, out RaycastHit hit, dropDistance, dropWallMask, QueryTriggerInteraction.Ignore))
        {
            // Đặt item trước tường, không xuyên qua
            safeDist = Mathf.Max(hit.distance - dropWallOffset, 0.3f);
        }

        Vector3 dropPos = origin + forward * safeDist;

        // Hạ xuống một chút để item không lơ lửng giữa không trung
        dropPos.y -= 0.3f;

        return dropPos;
    }

    public void RefreshUIAfterPickup()
    {
        RefreshPickupUI();
        UpdateDropButton();
    }

    private void UpdateDropButton()
    {
        if (dropButtonObject != null)
            dropButtonObject.SetActive(inventory != null && inventory.HasItem);
    }
}