using UnityEngine;

[DisallowMultipleComponent]
public sealed class Holdable : MonoBehaviour, IHoldable
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider[] colliders;

    [Header("Hold Offset")]
    [SerializeField] private Vector3 localPosition = Vector3.zero;
    [SerializeField] private Vector3 localRotation = Vector3.zero;

    [Header("Hand Pose")]
    [SerializeField] private HandPoseType handPose = HandPoseType.Default;
    [SerializeField] private HandPoseManager handPoseManager;

    private Transform currentHoldPoint;
    private bool isHeld;
    private Vector3 originalLocalScale;

    public bool IsHeld => isHeld;

    private void Awake()
    {
        originalLocalScale = transform.localScale;
    }

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>(true);
    }

    private void LateUpdate()
    {
        if (!isHeld || currentHoldPoint == null) return;

        transform.position = currentHoldPoint.position;
        transform.rotation = currentHoldPoint.rotation;

        transform.localPosition += localPosition;
        transform.localRotation *= Quaternion.Euler(localRotation);
        transform.localScale = originalLocalScale;
    }

    public void OnPick(Transform holdPoint)
    {
        if (holdPoint == null) return;

        currentHoldPoint = holdPoint;
        isHeld = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        SetColliders(false);

        transform.SetParent(holdPoint, false);
        transform.localPosition = localPosition;
        transform.localRotation = Quaternion.Euler(localRotation);
        transform.localScale = originalLocalScale;

        if (handPoseManager != null)
            handPoseManager.SetPose(handPose);

        GetComponent<HolyWaterRevealAbility>()?.SetHeld(true);
    }

    public void OnDrop(Vector3 worldPosition)
    {
        isHeld = false;
        currentHoldPoint = null;

        transform.SetParent(null, true);
        transform.position = worldPosition;
        transform.localScale = originalLocalScale;

        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }

        SetColliders(true);

        if (handPoseManager != null)
            handPoseManager.ResetToDefault();

        GetComponent<HolyWaterRevealAbility>()?.SetHeld(false);
    }

    private void SetColliders(bool enabled)
    {
        if (colliders == null) return;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
                colliders[i].enabled = enabled;
        }
    }
}