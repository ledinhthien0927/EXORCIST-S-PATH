using UnityEngine;
using System;

[DisallowMultipleComponent]
public sealed class DoorInteractable : MonoBehaviour, IInteractable
{
    [Header("Door Setup")]
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float speed = 180f;

    [Header("Door Blocking")]
    [SerializeField] private Collider doorBlocker;

    [Header("UI Text")]
    [SerializeField] private string openText = "Open Door";
    [SerializeField] private string closeText = "Close Door";

    [Header("Lock (Optional)")]
    [SerializeField] private bool locked = false;
    [SerializeField] private string lockedText = "Locked";

    [Header("Audio")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    private bool isOpen;
    private float currentAngle;
    private float targetAngle;

    public event Action<bool> OnDoorStateChanged;

    public string Prompt
    {
        get
        {
            if (locked) return lockedText;
            return isOpen ? closeText : openText;
        }
    }

    private void Awake()
    {
        if (doorPivot == null)
            doorPivot = transform;

        currentAngle = 0f;
        targetAngle = 0f;

        ApplyRotation();
        SetBlockerState(isOpen);
    }

    public bool CanInteract(PlayerInteractor interactor)
    {
        if (doorPivot == null) return false;
        if (locked) return false;
        return true;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (!CanInteract(interactor)) return;

        isOpen = !isOpen;
        targetAngle = isOpen ? openAngle : 0f;

        SetBlockerState(isOpen);

     

        OnDoorStateChanged?.Invoke(isOpen);
    }

    private void Update()
    {
        if (Mathf.Approximately(currentAngle, targetAngle))
            return;

        currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, speed * Time.deltaTime);
        ApplyRotation();
    }

    private void ApplyRotation()
    {
        if (doorPivot != null)
            doorPivot.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
    }

    private void SetBlockerState(bool open)
    {
        if (doorBlocker != null)
            doorBlocker.enabled = !open;
    }

    public void SetLocked(bool value)
    {
        locked = value;

        if (locked)
        {
            isOpen = false;
            targetAngle = 0f;
            SetBlockerState(false);
        }
    }

    public bool IsOpen => isOpen;
    public bool IsLocked => locked;
}