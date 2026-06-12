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
    }

    public void Interact()
    {
        if (isOpen) return;

        if (isLockedForever)
        {
            if (DialogueSystem.Instance != null)
                DialogueSystem.Instance.ShowThought(foreverLockedMessage, 2.5f);
            return;
        }

        if (SimpleInventory.Instance == null) return;

        if (SimpleInventory.Instance.HasKeyFor(doorId))
        {
            SimpleInventory.Instance.ConsumeKey();
            OpenDoor();
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySound("dooropen"); // Обрати внимание: в твоём AudioManager звук называется "dooropen" (маленькие буквы)
        }
        else
        {
            if (DialogueSystem.Instance != null)
                DialogueSystem.Instance.ShowThought(lockedMessage, 2.5f);
        }
    }

    public void OpenDoor()
    {
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