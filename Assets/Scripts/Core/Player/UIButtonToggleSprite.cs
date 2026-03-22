using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButtonHoldSprite : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite pressedSprite;

    private void Reset()
    {
        targetImage = GetComponent<Image>();
    }

    private void Awake()
    {
        if (targetImage != null && normalSprite != null)
            targetImage.sprite = normalSprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (targetImage != null && pressedSprite != null)
            targetImage.sprite = pressedSprite;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (targetImage != null && normalSprite != null)
            targetImage.sprite = normalSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetImage != null && normalSprite != null)
            targetImage.sprite = normalSprite;
    }
}