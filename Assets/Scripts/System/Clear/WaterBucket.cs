using UnityEngine;

public class WaterBucket : MonoBehaviour
{
    [Header("Bucket Water State")]
    [SerializeField] private int maxDipUses = 3;

    [Header("Water Visual")]
    [SerializeField] private GameObject waterSurface;
    [SerializeField] private float fullWaterY = 0.12f;
    [SerializeField] private float lowWaterY = 0.03f;

    [Header("Spill Settings")]
    [SerializeField] private bool spillWhenTilted = true;
    [SerializeField] private float spillAngle = 60f;
    [SerializeField] private bool onlySpillWhenNotHeld = true;

    [Header("Water Color")]
    [SerializeField] private Renderer waterRenderer;

    [SerializeField] private Color normalWaterColor = new Color(0.3f, 0.7f, 1f, 0.5f);
    [SerializeField] private Color holyWaterColor = new Color(0.6f, 1f, 1f, 0.8f);

    private int currentDipUses = 0;
    private bool hasWater = false;
    private bool isHolyWater = false;

    private Vector3 waterLocalPos;
    private Holdable holdable;

    public bool HasWater => hasWater;
    public bool IsHolyWater => isHolyWater;
    public int CurrentDipUses => currentDipUses;
    public int MaxDipUses => maxDipUses;

    private void Awake()
    {
        if (waterSurface != null)
            waterLocalPos = waterSurface.transform.localPosition;

        holdable = GetComponent<Holdable>();

        RefreshWaterVisual();
    }

    private void Update()
    {
        CheckSpillByTilt();
    }

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

        RefreshWaterVisual();
        return true;
    }

    public void FillWithNormalWater()
    {
        hasWater = true;
        isHolyWater = false;
        currentDipUses = maxDipUses;

        RefreshWaterVisual();
    }

    public bool BlessWater()
    {
        if (!hasWater || currentDipUses <= 0)
            return false;

        isHolyWater = true;
        RefreshWaterVisual();
        return true;
    }

    public void SpillAllWater()
    {
        hasWater = false;
        isHolyWater = false;
        currentDipUses = 0;

        RefreshWaterVisual();
        Debug.Log("Bucket spilled all water.");
    }

    private void CheckSpillByTilt()
    {
        if (!spillWhenTilted) return;
        if (!hasWater || currentDipUses <= 0) return;

        if (onlySpillWhenNotHeld && holdable != null && holdable.IsHeld)
            return;

        float angleFromUp = Vector3.Angle(transform.up, Vector3.up);

        if (angleFromUp >= spillAngle)
        {
            SpillAllWater();
        }
    }

    private void RefreshWaterVisual()
    {
        if (waterSurface == null) return;

        if (!hasWater || currentDipUses <= 0)
        {
            waterSurface.SetActive(false);
            return;
        }

        waterSurface.SetActive(true);

        float ratio = (float)currentDipUses / maxDipUses;
        float y = Mathf.Lerp(lowWaterY, fullWaterY, ratio);

        Vector3 pos = waterLocalPos;
        pos.y = y;
        waterSurface.transform.localPosition = pos;
        if (waterRenderer != null)
        {
            Material mat = waterRenderer.material;
            mat.color = isHolyWater ? holyWaterColor : normalWaterColor;
        }
    }
}