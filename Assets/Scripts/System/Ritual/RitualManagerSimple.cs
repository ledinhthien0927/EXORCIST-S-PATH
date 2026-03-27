using UnityEngine;
using ExorcistPath.Core.Managers;
using ExorcistPath.Gameplay;

public class RitualManagerSimple : MonoBehaviour
{
    [SerializeField] private RitualSlotSimple[] slots;
    [SerializeField] private GameObject glowEffect;

    [Header("End Sequence")]
    [SerializeField] private CustomerWinSequence winSequence;

    public enum CryType
    {
        None,
        Male,
        Female
    }

    [Header("Crying Settings")]
    [SerializeField] private CryType cryType = CryType.None;
    [SerializeField] private bool playCryingOnStart = true;

    private bool isDone = false;
    public bool IsDone => isDone;

    private void Start()
    {
        if (!playCryingOnStart) return;
        if (AudioManager.Instance == null) return;

        if (cryType == CryType.Male)
            AudioManager.Instance.PlayCryingMale();
        else if (cryType == CryType.Female)
            AudioManager.Instance.PlayCryingFemale();
    }

    public void CheckComplete()
    {
        if (isDone) return;

        foreach (var slot in slots)
        {
            if (slot == null || !slot.HasItem)
                return;
        }

        isDone = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopCrying();
        }

        if (glowEffect != null)
            glowEffect.SetActive(true);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMagic();
        }

        Debug.Log("Ritual Completed ??");

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

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopCrying();
        }

        if (glowEffect != null)
            glowEffect.SetActive(true);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMagic();
        }

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