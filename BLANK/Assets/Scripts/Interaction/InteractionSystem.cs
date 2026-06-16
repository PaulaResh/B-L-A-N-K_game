using UnityEngine;
using UnityEngine.UI; // если будешь использовать Text

public class InteractionSystem : MonoBehaviour
{
    [Header("Settings")]
    public float interactionDistance = 3f;
    public LayerMask interactableLayer = ~0; // Everything по умолчанию

    [Header("UI (опционально)")]
    public Text interactionPromptText; // можешь оставить пустым, если нет

    private Camera playerCamera;
    private IInteractable currentInteractable;

    private void Start()
    {
        playerCamera = Camera.main;
    }

    private void Update()
    {
        CheckForInteractable();

        if (Input.GetKeyDown(KeyCode.F) && currentInteractable != null && currentInteractable.CanInteract())
        {
            currentInteractable.Interact();
            HidePrompt();
        }
    }

    private void CheckForInteractable()
    {
        currentInteractable = null;

        if (playerCamera == null) return;

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null && interactable.CanInteract())
            {
                currentInteractable = interactable;
                ShowPrompt(interactable.GetInteractionPrompt());
                return;
            }
        }

        HidePrompt();
    }

    private void ShowPrompt(string prompt)
    {
        if (interactionPromptText != null)
            interactionPromptText.text = prompt;
        else
            Debug.Log($"[Interaction] Можно взаимодействовать: {prompt}"); // временно в консоль
    }

    private void HidePrompt()
    {
        if (interactionPromptText != null)
            interactionPromptText.text = "";
    }
}