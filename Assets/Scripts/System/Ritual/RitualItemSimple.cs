using UnityEngine;

public class RitualItemSimple : MonoBehaviour
{
    [Header("Slot Target")]
    public RitualSlotSimple targetSlot;

    public bool IsPlaced { get; private set; }

    public void Place(Transform point)
    {
        IsPlaced = true;

        transform.SetParent(point);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider[] cols = GetComponentsInChildren<Collider>();
        foreach (var c in cols)
            c.enabled = false;
    }
}