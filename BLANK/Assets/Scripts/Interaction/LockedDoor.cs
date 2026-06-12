using UnityEngine;

public class LockedDoor : MonoBehaviour, IInteractable
{
    [Header("Door ID")]
    public string doorId = "door_1";

    [Header("Lock Settings")]
    public bool isLockedForever = false;
    public string lockedMessage = "Дверь заперта. Нужен ключ...";
    public string foreverLockedMessage = "Эта дверь не откроется...";

    [Header("Animation")]
    public Animator animator;
    public string openTrigger = "Open";

    [Header("Fallback rotation")]
    public Transform doorTransform;
    public Vector3 openRotation = new Vector3(0, 90, 0);
    public float openSpeed = 2f;
    private Vector3 startRotation;
    private bool isAnimating = false;

    private bool isOpen = false;

    private void Start()
    {
        if (doorTransform == null) doorTransform = transform;
        startRotation = doorTransform.eulerAngles;
        
        Debug.Log("[LockedDoor] Дверь создана! DoorId: " + doorId + ", isLockedForever: " + isLockedForever);
    }

    public void Interact()
    {
        Debug.Log("[LockedDoor] Interact() вызван!");
        
        if (isOpen) 
        {
            Debug.Log("[LockedDoor] Дверь уже открыта!");
            return;
        }

        if (isLockedForever)
        {
            Debug.Log("[LockedDoor] Дверь закрыта навсегда!");
            if (DialogueSystem.Instance != null)
                DialogueSystem.Instance.ShowThought(foreverLockedMessage, 2.5f);
            return;
        }

        Debug.Log("[LockedDoor] Проверяем SimpleInventory...");
        if (SimpleInventory.Instance == null)
        {
            Debug.LogError("[LockedDoor] SimpleInventory.Instance == NULL!");
            return;
        }
        
        Debug.Log("[LockedDoor] SimpleInventory найден! Проверяем ключ...");
        Debug.Log("[LockedDoor] doorId = " + doorId);
        Debug.Log("[LockedDoor] heldKey = " + (SimpleInventory.Instance.heldKey != null ? SimpleInventory.Instance.heldKey.displayName : "NULL"));
        if (SimpleInventory.Instance.heldKey != null)
        {
            Debug.Log("[LockedDoor] heldKey.targetDoorId = " + SimpleInventory.Instance.heldKey.targetDoorId);
        }
        
        if (SimpleInventory.Instance.HasKeyFor(doorId))
        {
            Debug.Log("[LockedDoor] Ключ подходит! Открываем дверь!");
            SimpleInventory.Instance.ConsumeKey();
            OpenDoor();
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySound("dooropen");
        }
        else
        {
            Debug.LogWarning("[LockedDoor] Ключ НЕ подходит или отсутствует!");
            if (DialogueSystem.Instance != null)
                DialogueSystem.Instance.ShowThought(lockedMessage, 2.5f);
        }
    }

    public void OpenDoor()
    {
        Debug.Log("[LockedDoor] OpenDoor() вызван!");
        isOpen = true;
        if (animator != null)
            animator.SetTrigger(openTrigger);
        else
            isAnimating = true;
    }

    private void Update()
    {
        if (isAnimating && doorTransform != null)
        {
            doorTransform.eulerAngles = Vector3.Lerp(
                doorTransform.eulerAngles,
                startRotation + openRotation,
                Time.deltaTime * openSpeed
            );
        }
    }

    public string GetInteractionPrompt()
    {
        if (isOpen) return "";
        if (isLockedForever) return "Закрыто навсегда";
        return "Открыть дверь";
    }

    public bool CanInteract()
    {
        return !isOpen;
    }
}