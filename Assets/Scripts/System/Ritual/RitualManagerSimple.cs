using UnityEngine;
using ExorcistPath.Core.Managers;

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

        if (GameManager.Instance != null)
        {
            // The GDD requirement states that completing the ritual gives the rest of the purification.
            // Add 100f to ensure it reaches 100% (GameManager clamps it to 100).
            GameManager.Instance.AddPurification(100f);
            
            // Winning the game
            GameManager.Instance.WinGame();
        }
    }
}