using UnityEngine;

public sealed class CleaningToolItem : MonoBehaviour
{
    [SerializeField] private CleaningToolType toolType = CleaningToolType.None;
    [SerializeField] private int maxUses = 0;

    private int currentUses = 0;

    public CleaningToolType ToolType => toolType;
    public bool HasUses => currentUses > 0;
    public int CurrentUses => currentUses;
    public int MaxUses => maxUses;

    public void AddUses(int amount)
    {
        if (maxUses <= 0) return;
        currentUses = Mathf.Clamp(currentUses + amount, 0, maxUses);
    }

    public void FillToMax()
    {
        if (maxUses <= 0) return;
        currentUses = maxUses;
    }

    public bool UseOnce()
    {
        if (currentUses <= 0) return false;
        currentUses--;
        return true;
    }

    public void ClearUses()
    {
        currentUses = 0;
    }
}