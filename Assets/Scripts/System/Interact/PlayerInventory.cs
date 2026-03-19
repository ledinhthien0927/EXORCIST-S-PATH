using UnityEngine;

public sealed class PlayerInventory : MonoBehaviour
{
    public GameObject CurrentObject { get; private set; }
    public bool HasItem => CurrentObject != null;

    public bool TryPick(GameObject obj)
    {
        if (HasItem || obj == null) return false;

        CurrentObject = obj;
        return true;
    }

    public bool TryDrop(out GameObject obj)
    {
        if (!HasItem)
        {
            obj = null;
            return false;
        }

        obj = CurrentObject;
        CurrentObject = null;
        return true;
    }

    public T GetHeldComponent<T>() where T : Component
    {
        if (CurrentObject == null) return null;
        return CurrentObject.GetComponent<T>();
    }

    public bool IsHoldingTool(CleaningToolType toolType)
    {
        if (CurrentObject == null) return false;

        CleaningToolItem item = CurrentObject.GetComponent<CleaningToolItem>();
        if (item == null) return false;

        return item.ToolType == toolType;
    }
}