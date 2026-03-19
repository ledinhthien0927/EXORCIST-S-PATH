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

    private Transform currentHoldPoint;
    private bool isHeld;

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

        transform.SetParent(holdPoint, true);
        transform.localPosition = localPosition;
        transform.localRotation = Quaternion.Euler(localRotation);
    }

    public void OnDrop(Vector3 worldPosition)
    {
        isHeld = false;
        currentHoldPoint = null;

        transform.SetParent(null, true);
        transform.position = worldPosition;

        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }

        SetColliders(true);
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