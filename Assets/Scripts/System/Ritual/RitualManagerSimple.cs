using UnityEngine;

public class RitualManagerSimple : MonoBehaviour
{
    [SerializeField] private RitualSlotSimple[] slots;
    [SerializeField] private GameObject glowEffect;

    private bool isDone = false;

    public void CheckComplete()
    {
        if (isDone) return;

        foreach (var slot in slots)
        {
            if (slot == null || !slot.HasItem)
                return;
        }

        isDone = true;

        if (glowEffect != null)
            glowEffect.SetActive(true);

        Debug.Log("Ritual Completed ??");
    }
}