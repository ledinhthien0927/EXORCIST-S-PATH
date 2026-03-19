using UnityEngine;

public class PlayerLookAround : MonoBehaviour
{
    public float sensitivity = 0.03f;
    public float minX = -80f;
    public float maxX = 80f;

    public FixedTouchField touchField;
    public Transform cameraPivot;

    private float xRotation = 0f;

    private void LateUpdate()
    {
        if (touchField == null || cameraPivot == null)
            return;

        Vector2 touchDelta = touchField.TouchDist;

        float lookX = touchDelta.x * sensitivity;
        float lookY = touchDelta.y * sensitivity;

        transform.Rotate(Vector3.up * lookX);

        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, minX, maxX);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}