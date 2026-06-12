using UnityEngine;

public class SlidingDoor : MonoBehaviour, IInteractable
{
    [Header("Doors")]
    public Transform leftDoor;
    public Transform rightDoor;

    [Header("Animation")]
    public float openDistance = 1.5f;      // Насколько далеко разъезжаются двери
    public float openSpeed = 2.5f;

    [Header("Interaction")]
    public string interactionPrompt = "Открыть лифт";

    private Vector3 leftClosedPos;
    private Vector3 rightClosedPos;
    private Vector3 leftOpenPos;
    private Vector3 rightOpenPos;

    private bool isOpen = false;
    private bool isMoving = false;

    private void Start()
    {
        if (leftDoor == null || rightDoor == null)
        {
            Debug.LogError("[SlidingDoor] Не назначены leftDoor и rightDoor!");
            return;
        }

        // Запоминаем закрытые позиции
        leftClosedPos = leftDoor.localPosition;
        rightClosedPos = rightDoor.localPosition;

        // Вычисляем открытые позиции
        leftOpenPos = leftClosedPos + new Vector3(-openDistance, 0, 0);
        rightOpenPos = rightClosedPos + new Vector3(openDistance, 0, 0);
    }

    public void Interact()
    {
        if (isMoving) return;

        isOpen = !isOpen;
        StartCoroutine(MoveDoors());
    }

    private System.Collections.IEnumerator MoveDoors()
    {
        Vector3 leftStart = leftDoor.localPosition;
        Vector3 rightStart = rightDoor.localPosition;

        isMoving = true;

        Vector3 leftTarget = isOpen ? leftOpenPos : leftClosedPos;
        Vector3 rightTarget = isOpen ? rightOpenPos : rightClosedPos;

        float elapsed = 0f;
        float duration = 1f / openSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            leftDoor.localPosition =
                Vector3.Lerp(leftStart, leftTarget, t);

            rightDoor.localPosition =
                Vector3.Lerp(rightStart, rightTarget, t);

            yield return null;
        }

        leftDoor.localPosition = leftTarget;
        rightDoor.localPosition = rightTarget;

        isMoving = false;
    }

    public string GetInteractionPrompt()
    {
        return interactionPrompt;
    }

    public bool CanInteract()
    {
        return !isMoving;
    }
}