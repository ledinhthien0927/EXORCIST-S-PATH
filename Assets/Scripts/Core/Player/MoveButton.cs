using UnityEngine;
using UnityEngine.EventSystems;

public class MoveButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public enum MoveType
    {
        Up,
        Down,
        Left,
        Right
    }

    public MoveType moveType;
    public PlayerMovementMobile playerMovement;

    public void OnPointerDown(PointerEventData eventData)
    {
        SetState(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetState(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetState(false);
    }

    private void SetState(bool value)
    {
        if (playerMovement == null) return;

        switch (moveType)
        {
            case MoveType.Up:
                playerMovement.SetUp(value);
                break;
            case MoveType.Down:
                playerMovement.SetDown(value);
                break;
            case MoveType.Left:
                playerMovement.SetLeft(value);
                break;
            case MoveType.Right:
                playerMovement.SetRight(value);
                break;
        }
    }
}