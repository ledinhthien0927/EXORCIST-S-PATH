using UnityEngine;

/// <summary>
/// Gắn vào HandRoot (con của Main Camera).
/// Khi tường ở gần:
///   1. Kéo tay lùi lại (hand pull-back)
///   2. Đẩy player ra xa tường (player push-back)
/// </summary>
public class HandWallAvoidance : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("Khoảng cách bắt đầu kéo tay lùi khi detect tường")]
    [SerializeField] private float pullBackStartDistance = 0.8f;

    [Tooltip("Bán kính SphereCast")]
    [SerializeField] private float sphereRadius = 0.05f;

    [Tooltip("Layer nào được coi là tường")]
    [SerializeField] private LayerMask wallMask = ~0;

    [Header("Hand Pull Back")]
    [Tooltip("Khoảng cách kéo tay lùi tối đa (mét)")]
    [SerializeField] private float maxHandPullBack = 0.3f;

    [Tooltip("Thời gian smooth transition tay (giây)")]
    [SerializeField] private float handSmoothTime = 0.08f;

    [Header("Player Push Back")]
    [Tooltip("Bật/tắt đẩy player ra khỏi tường")]
    [SerializeField] private bool enablePlayerPush = true;

    [Tooltip("Khoảng cách tường bắt đầu đẩy player (nhỏ hơn pullBackStartDistance)")]
    [SerializeField] private float playerPushStartDistance = 0.5f;

    [Tooltip("Tốc độ đẩy player tối đa (m/s)")]
    [SerializeField] private float maxPushSpeed = 2f;

    [Tooltip("Thời gian smooth transition đẩy player (giây)")]
    [SerializeField] private float playerSmoothTime = 0.1f;

    [Header("Debug")]
    [SerializeField] private bool showDebugRay = false;

    private Camera mainCamera;
    private CharacterController characterController;
    private Vector3 originalLocalPosition;
    private float currentHandPullBack;
    private float handPullBackVelocity;
    private float currentPushSpeed;
    private float pushSpeedVelocity;

    private void Awake()
    {
        originalLocalPosition = transform.localPosition;

        mainCamera = GetComponentInParent<Camera>();

        // Tìm CharacterController trên Player root
        if (mainCamera != null)
            characterController = mainCamera.GetComponentInParent<CharacterController>();

        if (mainCamera == null)
        {
            Debug.LogWarning("[HandWallAvoidance] Không tìm thấy Camera trong parent. Script sẽ bị tắt.");
            enabled = false;
        }

        if (enablePlayerPush && characterController == null)
        {
            Debug.LogWarning("[HandWallAvoidance] Không tìm thấy CharacterController. Player push-back sẽ bị tắt.");
        }
    }

    private void LateUpdate()
    {
        if (mainCamera == null) return;

        Vector3 origin = mainCamera.transform.position;
        Vector3 direction = mainCamera.transform.forward;

        float targetHandPullBack = 0f;
        float targetPushSpeed = 0f;
        Vector3 pushDirection = Vector3.zero;

        if (Physics.SphereCast(origin, sphereRadius, direction, out RaycastHit hit,
            pullBackStartDistance, wallMask, QueryTriggerInteraction.Ignore))
        {
            // === Hand pull-back ===
            float handT = 1f - (hit.distance / pullBackStartDistance);
            targetHandPullBack = handT * maxHandPullBack;

            // === Player push-back ===
            if (enablePlayerPush && characterController != null && hit.distance < playerPushStartDistance)
            {
                float pushT = 1f - (hit.distance / playerPushStartDistance);
                targetPushSpeed = pushT * maxPushSpeed;

                // Đẩy player theo hướng ngược lại normal của tường (chỉ trên mặt phẳng XZ)
                pushDirection = hit.normal;
                pushDirection.y = 0f;
                pushDirection.Normalize();
            }

            if (showDebugRay)
            {
                Debug.DrawLine(origin, hit.point, Color.red);
                if (pushDirection != Vector3.zero)
                    Debug.DrawRay(characterController.transform.position, pushDirection * 0.5f, Color.yellow);
            }
        }
        else
        {
            if (showDebugRay)
                Debug.DrawRay(origin, direction * pullBackStartDistance, Color.green);
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
}
