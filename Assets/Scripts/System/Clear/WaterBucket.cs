using UnityEngine;

public class WaterBucket : MonoBehaviour
{
    [Header("Bucket Water State")]
    [SerializeField] private int maxDipUses = 3;

    private int currentDipUses = 0;
    private bool hasWater = false;
    private bool isHolyWater = false;

    public bool HasWater => hasWater;
    public bool IsHolyWater => isHolyWater;
    public int CurrentDipUses => currentDipUses;
    public int MaxDipUses => maxDipUses;

    public bool CanDip()
    {
        return hasWater && isHolyWater && currentDipUses > 0;
    }

    public bool UseDip()
    {
        if (!CanDip()) return false;

        currentDipUses--;

        if (currentDipUses <= 0)
        {
            currentDipUses = 0;
            hasWater = false;
            isHolyWater = false;
        }

        Debug.Log($"Bucket used. Uses left = {currentDipUses}, HasWater = {hasWater}, IsHoly = {isHolyWater}");
        return true;
    }

    public void FillWithNormalWater()
    {
        hasWater = true;
        isHolyWater = false;
        currentDipUses = maxDipUses;

        Debug.Log($"Bucket filled. Uses = {currentDipUses}/{maxDipUses}, Holy = {isHolyWater}");
    }

    public bool BlessWater()
    {
        if (!hasWater || currentDipUses <= 0)
        {
            Debug.Log("Cannot bless water: no water in bucket");
            return false;
        }

        isHolyWater = true;
        Debug.Log("Bucket is now holy water");
        return true;
    }
}