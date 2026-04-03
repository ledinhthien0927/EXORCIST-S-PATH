using UnityEngine;

/// <summary>
/// Gắn vào HandRoot (con của Main Camera).
/// Khi tường ở gần, script kéo tay lùi lại để tránh xuyên tường.
/// </summary>
public class HandWallAvoidance : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("Khoảng cách bắt đầu kéo tay lùi khi detect tường")]
    [SerializeField] private float pullBackStartDistance = 0.8f;

    [Tooltip("Bán kính SphereCast")]
    [SerializeField] private float sphereRadius = 0.05f;

    [Tooltip("Layer nào được coi là tường (chọn Default nếu tường dùng Default layer)")]
    [SerializeField] private LayerMask wallMask = ~0;

    [Header("Pull Back")]
    [Tooltip("Khoảng cách kéo lùi tối đa (mét)")]
    [SerializeField] private float maxPullBack = 0.3f;

    [Tooltip("Thời gian smooth transition (giây)")]
    [SerializeField] private float smoothTime = 0.08f;

    [Header("Debug")]
    [SerializeField] private bool showDebugRay = false;

    private Camera mainCamera;
    private Vector3 originalLocalPosition;
    private float currentPullBack;
    private float pullBackVelocity;

    private void Awake()
    {
        // Lưu vị trí local gốc của HandRoot
        originalLocalPosition = transform.localPosition;

        // Tìm camera từ parent
        mainCamera = GetComponentInParent<Camera>();

        if (mainCamera == null)
        {
            Debug.LogWarning("[HandWallAvoidance] Không tìm thấy Camera trong parent. Script sẽ bị tắt.");
            enabled = false;
        }
    }

    private void LateUpdate()
    {
        if (mainCamera == null) return;

        float targetPullBack = 0f;

        // Bắn ray từ camera về phía trước
        Vector3 origin = mainCamera.transform.position;
        Vector3 direction = mainCamera.transform.forward;

        if (Physics.SphereCast(origin, sphereRadius, direction, out RaycastHit hit,
            pullBackStartDistance, wallMask, QueryTriggerInteraction.Ignore))
        {
            // Tường ở gần → tính tỷ lệ kéo lùi
            float t = 1f - (hit.distance / pullBackStartDistance);
            targetPullBack = t * maxPullBack;

            if (showDebugRay)
                Debug.DrawLine(origin, hit.point, Color.red);
        }
        else
        {
            if (showDebugRay)
                Debug.DrawRay(origin, direction * pullBackStartDistance, Color.green);
        }

        // Smooth transition
        currentPullBack = Mathf.SmoothDamp(currentPullBack, targetPullBack, ref pullBackVelocity, smoothTime);

        // Áp dụng vị trí mới: kéo HandRoot lùi về phía sau camera (local -Z)
        transform.localPosition = originalLocalPosition - Vector3.forward * currentPullBack;
    }
}
