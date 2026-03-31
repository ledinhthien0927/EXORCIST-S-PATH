using UnityEngine;

public class RitualItemSimple : MonoBehaviour
{
    public enum RitualItemType
    {
        Doll,
        RiceBowl,
        Candle,
        Incense
    }

    [Header("Item Type")]
    public RitualItemType itemType;

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