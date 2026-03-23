using UnityEngine;
using ExorcistPath.Core.Managers;
using ExorcistPath.Gameplay;

public class RitualManagerSimple : MonoBehaviour
{
    [SerializeField] private RitualSlotSimple[] slots;
    [SerializeField] private GameObject glowEffect;
    
    [Header("End Sequence")]
    [Tooltip("The sequence to play when this specific ritual is completed.")]
    [SerializeField] private CustomerWinSequence winSequence;

    private bool isDone = false;
    public bool IsDone => isDone;

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

        // Play the customer fly sequence. This sequence will tell GameManager
        // that a ritual is done, and GameManager will decide if it's the last one.
        if (winSequence != null)
        {
            winSequence.PlaySequence();
        }
        else
        {
            Debug.LogWarning("[RitualManagerSimple] No CustomerWinSequence assigned!");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CompleteRitual();
            }
        }
    }

    public void ForceComplete()
    {
        if (isDone) return;
        isDone = true;

        if (glowEffect != null)
            glowEffect.SetActive(true);

        Debug.Log("[RitualManagerSimple] Ritual Force Completed via Debug!");

        if (winSequence != null)
        {
            winSequence.PlaySequence();
        }
        else
        {
            Debug.LogWarning("[RitualManagerSimple] No CustomerWinSequence assigned!");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CompleteRitual();
            }
        }
    }
}