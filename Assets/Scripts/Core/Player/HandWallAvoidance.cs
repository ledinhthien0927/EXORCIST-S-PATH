using UnityEngine;

/// <summary>
/// Gắn vào HandRoot (con của Main Camera).
/// Khi tường ở gần:
///   1. Kéo tay lùi lại (hand pull-back)
///   2. Đẩy player ra xa tường (player push-back)
///   3. Tự động tăng khoảng cách detect khi đang cầm vật phẩm lớn
/// </summary>
public class HandWallAvoidance : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("Khoảng cách cơ bản bắt đầu kéo tay lùi (khi tay không)")]
    [SerializeField] private float pullBackStartDistance = 0.8f;

    [Tooltip("Bán kính SphereCast")]
    [SerializeField] private float sphereRadius = 0.05f;

    [Tooltip("Layer nào được coi là tường")]
    [SerializeField] private LayerMask wallMask = ~0;

    [Header("Held Item Detection")]
    [Tooltip("Reference đến PlayerInventory để biết đang cầm gì")]
    [SerializeField] private PlayerInventory inventory;

    [Tooltip("Khoảng cách detect thêm khi đang cầm vật phẩm (tự động tính từ bounds)")]
    [SerializeField] private float heldItemExtraDistance = 0f;

    [Tooltip("Nếu true, tự động tính extra distance từ renderer bounds của vật phẩm")]
    [SerializeField] private bool autoDetectItemSize = true;

    [Header("Hand Pull Back")]
    [Tooltip("Khoảng cách kéo tay lùi tối đa (mét)")]
    [SerializeField] private float maxHandPullBack = 0.3f;

    [Tooltip("Thời gian smooth transition tay (giây)")]
    [SerializeField] private float handSmoothTime = 0.08f;

    [Header("Player Push Back")]
    [Tooltip("Bật/tắt đẩy player ra khỏi tường")]
    [SerializeField] private bool enablePlayerPush = true;

    [Tooltip("Khoảng cách tường bắt đầu đẩy player")]
    [SerializeField] private float playerPushStartDistance = 0.5f;

    [Tooltip("Tốc độ đẩy player tối đa (m/s)")]
    [SerializeField] private float maxPushSpeed = 2f;

    [Tooltip("Thời gian smooth transition đẩy player (giây)")]
    [SerializeField] private float playerSmoothTime = 0.1f;

    [Header("Debug")]
    [SerializeField] private bool showDebugRay = false;
    [SerializeField] private float debugEffectiveDistance;
    [SerializeField] private string debugHeldItem = "None";

    private Camera mainCamera;
    private CharacterController characterController;
    private Vector3 originalLocalPosition;
    private float currentHandPullBack;
    private float handPullBackVelocity;
    private float currentPushSpeed;
    private float pushSpeedVelocity;

    // Cache để không tính bounds mỗi frame
    private GameObject cachedHeldObject;
    private float cachedExtraDistance;

    private void Awake()
    {
        originalLocalPosition = transform.localPosition;
        mainCamera = GetComponentInParent<Camera>();

        if (mainCamera != null)
            characterController = mainCamera.GetComponentInParent<CharacterController>();

        // Tự tìm PlayerInventory nếu chưa gán
        if (inventory == null && mainCamera != null)
            inventory = mainCamera.GetComponentInParent<PlayerInventory>();

        if (mainCamera == null)
        {
            Debug.LogWarning("[HandWallAvoidance] Không tìm thấy Camera. Script bị tắt.");
            enabled = false;
        }
    }

    private void LateUpdate()
    {
        if (mainCamera == null) return;

        // Tính effective distance dựa vào vật phẩm đang cầm
        float effectiveDistance = pullBackStartDistance + GetHeldItemExtraDistance();
        debugEffectiveDistance = effectiveDistance;

        Vector3 origin = mainCamera.transform.position;
        Vector3 direction = mainCamera.transform.forward;

        float targetHandPullBack = 0f;
        float targetPushSpeed = 0f;
        Vector3 pushDirection = Vector3.zero;

        if (Physics.SphereCast(origin, sphereRadius, direction, out RaycastHit hit,
            effectiveDistance, wallMask, QueryTriggerInteraction.Ignore))
        {
            // === Hand pull-back ===
            float handT = 1f - (hit.distance / effectiveDistance);
            targetHandPullBack = handT * maxHandPullBack;

            // === Player push-back ===
            if (enablePlayerPush && characterController != null && hit.distance < playerPushStartDistance)
            {
                float pushT = 1f - (hit.distance / playerPushStartDistance);
                targetPushSpeed = pushT * maxPushSpeed;

                pushDirection = hit.normal;
                pushDirection.y = 0f;
                pushDirection.Normalize();
            }

            if (showDebugRay)
            {
                Debug.DrawLine(origin, hit.point, Color.red);
                if (pushDirection != Vector3.zero && characterController != null)
                    Debug.DrawRay(characterController.transform.position, pushDirection * 0.5f, Color.yellow);
            }
        }
        else
        {
            if (showDebugRay)
                Debug.DrawRay(origin, direction * effectiveDistance, Color.green);
        }

        // === Áp dụng hand pull-back ===
        currentHandPullBack = Mathf.SmoothDamp(currentHandPullBack, targetHandPullBack,
            ref handPullBackVelocity, handSmoothTime);
        transform.localPosition = originalLocalPosition - Vector3.forward * currentHandPullBack;

        // === Áp dụng player push-back ===
        if (enablePlayerPush && characterController != null)
        {
            currentPushSpeed = Mathf.SmoothDamp(currentPushSpeed, targetPushSpeed,
                ref pushSpeedVelocity, playerSmoothTime);

            if (currentPushSpeed > 0.001f && pushDirection != Vector3.zero)
            {
                characterController.Move(pushDirection * currentPushSpeed * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// Tính khoảng cách thêm dựa vào kích thước vật phẩm đang cầm.
    /// Dùng cache để tránh tính lại mỗi frame.
    /// </summary>
    private float GetHeldItemExtraDistance()
    {
        if (inventory == null || !inventory.HasItem)
        {
            cachedHeldObject = null;
            cachedExtraDistance = 0f;
            debugHeldItem = "None";
            return 0f;
        }

        GameObject heldObj = inventory.CurrentObject;

        // Nếu vẫn đang cầm cùng vật → dùng cache
        if (heldObj == cachedHeldObject)
            return cachedExtraDistance;

        // Vật phẩm mới → tính lại
        cachedHeldObject = heldObj;
        debugHeldItem = heldObj.name;

        if (!autoDetectItemSize)
        {
            cachedExtraDistance = heldItemExtraDistance;
            return cachedExtraDistance;
        }

        // Tính bounds từ tất cả Renderer con
        Renderer[] renderers = heldObj.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0)
        {
            cachedExtraDistance = heldItemExtraDistance;
            return cachedExtraDistance;
        }

        Bounds combinedBounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            combinedBounds.Encapsulate(renderers[i].bounds);
        }

        // Lấy kích thước lớn nhất (forward extent)
        // Dùng max của extents vì vật phẩm có thể xoay
        float maxExtent = Mathf.Max(combinedBounds.extents.x, combinedBounds.extents.y, combinedBounds.extents.z);
        cachedExtraDistance = maxExtent;

        return cachedExtraDistance;
    }
}
