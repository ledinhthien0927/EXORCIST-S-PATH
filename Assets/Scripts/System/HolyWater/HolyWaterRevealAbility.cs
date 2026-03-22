using UnityEngine;
using UnityEngine.UI;

public class HolyWaterRevealAbility : MonoBehaviour
{
    [SerializeField] private float revealDuration = 3f;

    private GameObject revealButtonObject;
    private Button revealButton;

    private HolyMarkReveal[] allMarks;
    private bool isHeld = false;
    private HolyWaterSpawnButton spawnButtonOwner;
    private bool isInitialized = false;

    private void Awake()
    {
        allMarks = FindObjectsOfType<HolyMarkReveal>(true);

        revealButton = RevealButtonUI.Instance;

        if (revealButton != null)
        {
            revealButtonObject = revealButton.gameObject;

            revealButton.onClick.RemoveListener(OnRevealButtonClicked);
            revealButton.onClick.AddListener(OnRevealButtonClicked);

         
            revealButtonObject.SetActive(isHeld);
        }

        isInitialized = true;
    }

    public void SetHeld(bool value)
    {
        isHeld = value;

        if (isInitialized && revealButtonObject != null)
            revealButtonObject.SetActive(isHeld);
    }

    public void SetSpawnButtonOwner(HolyWaterSpawnButton owner)
    {
        spawnButtonOwner = owner;
    }

    private void OnRevealButtonClicked()
    {
        if (!isHeld) return;

        RevealAllMarks();

        if (revealButtonObject != null)
            revealButtonObject.SetActive(false);

        spawnButtonOwner?.ConsumeSpawnedHolyWater(gameObject);
    }

    public void ConsumeAfterUse()
    {
        if (revealButtonObject != null)
            revealButtonObject.SetActive(false);

        spawnButtonOwner?.ConsumeSpawnedHolyWater(gameObject);
    }

    private void RevealAllMarks()
    {
        if (allMarks == null || allMarks.Length == 0) return;

        foreach (var mark in allMarks)
        {
            if (mark != null)
                mark.Reveal(revealDuration);
        }
    }


}