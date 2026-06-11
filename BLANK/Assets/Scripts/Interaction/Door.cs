using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public bool isOpen = false;
    public float openAngle = 90f;
    public float openSpeed = 2.5f;

    [Header("Interaction")]
    public string interactionPromptOpen = "Открыть";
    public string interactionPromptClose = "Закрыть";

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isMoving = false;

    private void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));

        // Случайное начальное состояние (можно убрать)
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

        Quaternion startRot = transform.rotation;
        Quaternion targetRot = isOpen ? openRotation : closedRotation;

        float elapsed = 0f;
        float duration = 1f / openSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        transform.rotation = targetRot;
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