using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public bool isOpen = false;
    public float openAngle = 90f;
    public float openSpeed = 2f;
    public string interactionPromptOpen = "Открыть";
    public string interactionPromptClose = "Закрыть";

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isMoving = false;

    private void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
        
        // Random initial state (optional)
        if (Random.value > 0.7f)
        {
            isOpen = true;
            transform.rotation = openRotation;
        }
    }

    public void Interact()
    {
        if (isMoving) return;

        isOpen = !isOpen;
        StartCoroutine(MoveDoor());
    }

    private System.Collections.IEnumerator MoveDoor()
    {
        isMoving = true;
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);
            yield return null;
        }

        transform.rotation = targetRotation;
        isMoving = false;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound(isOpen ? "DoorOpen" : "DoorClose");
    }

    public string GetInteractionPrompt()
    {
        return isOpen ? interactionPromptClose : interactionPromptOpen;
    }

    public bool CanInteract()
    {
        return !isMoving;
    }
}