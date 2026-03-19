using UnityEngine;
using UnityEngine.EventSystems;

public class FixedTouchField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Vector2 TouchDist { get; private set; }

    private Vector2 pointerOld;
    private int pointerId;
    private bool pressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
        pointerId = eventData.pointerId;
        pointerOld = eventData.position;
        TouchDist = Vector2.zero;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
        TouchDist = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!pressed || eventData.pointerId != pointerId)
            return;

        Vector2 pointerNew = eventData.position;
        TouchDist = pointerNew - pointerOld;
        pointerOld = pointerNew;
    }

    private void LateUpdate()
    {
        if (!pressed)
            TouchDist = Vector2.zero;
    }
}