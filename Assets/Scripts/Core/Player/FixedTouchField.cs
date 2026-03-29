using UnityEngine;
using UnityEngine.EventSystems;

public class FixedTouchField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Vector2 TouchDist { get; private set; }

    [SerializeField] private float dragThreshold = 10f;

    private Vector2 pointerOld;
    private int pointerId;
    private bool pressed;
    private bool isDragging;
    private bool gotDragThisFrame;

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
        isDragging = false;
        pointerId = eventData.pointerId;
        pointerOld = eventData.position;
        TouchDist = Vector2.zero;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != pointerId)
            return;

        pressed = false;
        isDragging = false;
        gotDragThisFrame = false;
        TouchDist = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!pressed || eventData.pointerId != pointerId)
            return;

        Vector2 pointerNew = eventData.position;
        Vector2 delta = pointerNew - pointerOld;

        if (!isDragging)
        {
            if (delta.magnitude < dragThreshold)
            {
                TouchDist = Vector2.zero;
                return;
            }

            isDragging = true;
        }

        TouchDist = delta;
        pointerOld = pointerNew;
        gotDragThisFrame = true;
    }

    private void LateUpdate()
    {
        if (!pressed)
        {
            TouchDist = Vector2.zero;
            return;
        }

        if (!gotDragThisFrame)
            TouchDist = Vector2.zero;

        gotDragThisFrame = false;
    }
}